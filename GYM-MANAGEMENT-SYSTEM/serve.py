"""
serve.py — AI Gym Chatbot Inference Server
Chạy: python serve.py
API:  POST http://localhost:5000/chat
"""
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import torch
from transformers import AutoTokenizer, AutoModelForCausalLM, BitsAndBytesConfig
from peft import PeftModel
import uvicorn

# ── CONFIG ────────────────────────────────────────────────────────────────────
BASE_MODEL   = "Qwen/Qwen2.5-1.5B-Instruct"
ADAPTER_DIR  = "gymbot-output"
PORT         = 5000
MAX_NEW_TOKENS = 512
TEMPERATURE    = 0.7
# ─────────────────────────────────────────────────────────────────────────────

app = FastAPI(title="Gym AI Chatbot")
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

# Global model (load 1 lần khi khởi động)
tokenizer = None
model     = None

class ChatRequest(BaseModel):
    message: str
    history: list = []   # [{"role": "user"|"assistant", "content": "..."}]

class ChatResponse(BaseModel):
    reply: str

@app.on_event("startup")
async def load_model():
    global tokenizer, model
    print("Loading tokenizer...")
    tokenizer = AutoTokenizer.from_pretrained(ADAPTER_DIR, trust_remote_code=True)
    tokenizer.pad_token = tokenizer.eos_token

    print("Loading base model (4-bit)...")
    bnb_config = BitsAndBytesConfig(
        load_in_4bit=True,
        bnb_4bit_quant_type="nf4",
        bnb_4bit_compute_dtype=torch.float16,
        bnb_4bit_use_double_quant=True,
    )
    base = AutoModelForCausalLM.from_pretrained(
        BASE_MODEL,
        quantization_config=bnb_config,
        device_map="auto",
        trust_remote_code=True,
    )

    print("Loading LoRA adapter...")
    model = PeftModel.from_pretrained(base, ADAPTER_DIR)
    model.eval()
    print("✅ Model ready! Listening on http://localhost:5000")

@app.post("/chat", response_model=ChatResponse)
async def chat(req: ChatRequest):
    # Build messages
    messages = [
        {
            "role": "system",
            "content": (
                "You are an AI Gym Personal Trainer. "
                "Answer all fitness, gym, nutrition and workout questions "
                "clearly, accurately and helpfully. "
                "Reply in the same language as the user."
            )
        }
    ]
    for h in req.history[-6:]:   # giữ tối đa 6 lượt gần nhất
        messages.append(h)
    messages.append({"role": "user", "content": req.message})

    # Tokenize
    text = tokenizer.apply_chat_template(
        messages,
        tokenize=False,
        add_generation_prompt=True
    )
    inputs = tokenizer(text, return_tensors="pt").to(model.device)

    # Generate
    with torch.no_grad():
        output_ids = model.generate(
            **inputs,
            max_new_tokens=MAX_NEW_TOKENS,
            temperature=TEMPERATURE,
            do_sample=True,
            top_p=0.9,
            repetition_penalty=1.1,
            pad_token_id=tokenizer.eos_token_id,
        )

    # Decode (chỉ lấy phần reply, bỏ phần input)
    new_tokens = output_ids[0][inputs["input_ids"].shape[1]:]
    reply = tokenizer.decode(new_tokens, skip_special_tokens=True).strip()

    return ChatResponse(reply=reply)

@app.get("/health")
async def health():
    return {"status": "ok", "model": ADAPTER_DIR}

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=PORT)