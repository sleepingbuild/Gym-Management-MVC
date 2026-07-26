"""
serve.py — AI Gym Chatbot Inference Server
Chạy: python serve.py
API:  POST http://localhost:5000/chat/stream
      POST http://localhost:5000/embed
"""
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import StreamingResponse
from pydantic import BaseModel
from transformers import AutoTokenizer, AutoModelForCausalLM, AutoModel, TextIteratorStreamer
import torch
import torch.nn.functional as F
from peft import PeftModel
from threading import Thread
import uvicorn

# ── CONFIG ────────────────────────────────────────────────────────────────────
BASE_MODEL     = "Qwen/Qwen2.5-1.5B-Instruct"
ADAPTER_DIR    = "gymbot-output"
EMBED_MODEL    = "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"
PORT           = 5000
MAX_NEW_TOKENS = 200
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
tokenizer       = None
model           = None
embed_tokenizer = None
embed_model     = None


class ChatRequest(BaseModel):
    message: str
    history: list = []   # [{"role": "user"|"assistant", "content": "..."}]


class ChatResponse(BaseModel):
    reply: str


class EmbedRequest(BaseModel):
    text: str


class EmbedResponse(BaseModel):
    vector: list[float]


def mean_pooling(model_output, attention_mask):
    token_embeddings = model_output[0]
    mask = attention_mask.unsqueeze(-1).expand(token_embeddings.size()).float()
    return torch.sum(token_embeddings * mask, 1) / torch.clamp(mask.sum(1), min=1e-9)


@app.on_event("startup")
async def load_model():
    global tokenizer, model, embed_tokenizer, embed_model

    print("Loading tokenizer...")
    tokenizer = AutoTokenizer.from_pretrained(ADAPTER_DIR, trust_remote_code=True)
    tokenizer.pad_token = tokenizer.eos_token

    print("Loading base model (fp16)...")
    base = AutoModelForCausalLM.from_pretrained(
        BASE_MODEL,
        torch_dtype=torch.float16,
        device_map="cuda:0",
        trust_remote_code=True,
    )

    print("Loading LoRA adapter...")
    model = PeftModel.from_pretrained(base, ADAPTER_DIR)
    model.eval()
    print("✅ Chat model ready!")

    print("Loading embedding model...")
    embed_tokenizer = AutoTokenizer.from_pretrained(EMBED_MODEL)
    embed_model = AutoModel.from_pretrained(EMBED_MODEL).to("cuda:0")
    embed_model.eval()
    print("✅ Embedding model ready!")

    print(f"✅ All models ready! Listening on http://localhost:{PORT}")


@app.post("/chat/stream")
async def chat_stream(req: ChatRequest):
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
    for h in req.history[-6:]:
        messages.append(h)
    messages.append({"role": "user", "content": req.message})

    text = tokenizer.apply_chat_template(
        messages, tokenize=False, add_generation_prompt=True
    )
    inputs = tokenizer(text, return_tensors="pt").to(model.device)

    streamer = TextIteratorStreamer(
        tokenizer, skip_prompt=True, skip_special_tokens=True
    )

    generation_kwargs = dict(
        **inputs,
        max_new_tokens=MAX_NEW_TOKENS,
        do_sample=False,
        repetition_penalty=1.1,
        pad_token_id=tokenizer.eos_token_id,
        streamer=streamer,
    )

    thread = Thread(target=model.generate, kwargs=generation_kwargs)
    thread.start()

    def token_stream():
        for new_text in streamer:
            yield new_text

    return StreamingResponse(token_stream(), media_type="text/plain")


@app.post("/embed", response_model=EmbedResponse)
async def embed(req: EmbedRequest):
    encoded = embed_tokenizer(
        [req.text], padding=True, truncation=True, return_tensors="pt"
    ).to(embed_model.device)

    with torch.no_grad():
        output = embed_model(**encoded)

    vec = mean_pooling(output, encoded["attention_mask"])
    vec = F.normalize(vec, p=2, dim=1)
    return EmbedResponse(vector=vec[0].cpu().tolist())


@app.get("/health")
async def health():
    return {"status": "ok", "model": ADAPTER_DIR}


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=PORT)