import json
import torch
from datasets import Dataset
from transformers import (
    AutoTokenizer,
    AutoModelForCausalLM,
    BitsAndBytesConfig,
)
from peft import LoraConfig, get_peft_model
from trl import SFTTrainer, SFTConfig

# ── CONFIG ────────────────────────────────────────────────────────────────────
MODEL_NAME    = "Qwen/Qwen2.5-1.5B-Instruct"
DATASET_PATH  = "faq_merged_final.json"
OUTPUT_DIR    = "gymbot-output"
MAX_SEQ_LEN   = 1024
EPOCHS        = 3
BATCH_SIZE    = 2
GRAD_ACCUM    = 4
LEARNING_RATE = 2e-4
# ─────────────────────────────────────────────────────────────────────────────

def format_sample(item):
    return {
        "text": (
            "<|im_start|>system\n"
            "You are an AI Gym Personal Trainer. "
            "Answer all fitness, gym, nutrition and workout questions "
            "clearly, accurately and helpfully in the same language as the user.\n"
            "<|im_end|>\n"
            "<|im_start|>user\n"
            f"{item['question']}\n"
            "<|im_end|>\n"
            "<|im_start|>assistant\n"
            f"{item['answer']}\n"
            "<|im_end|>"
        )
    }

def main():
    # 1. Load dataset
    print(f"[1/5] Loading dataset from {DATASET_PATH} ...")
    with open(DATASET_PATH, "r", encoding="utf-8-sig") as f:
        raw = json.load(f)
    dataset = Dataset.from_list([format_sample(x) for x in raw])
    print(f"      → {len(dataset)} FAQ entries loaded.")

    # 2. 4-bit quantization
    print("[2/5] Configuring 4-bit quantization ...")
    bnb_config = BitsAndBytesConfig(
        load_in_4bit=True,
        bnb_4bit_quant_type="nf4",
        bnb_4bit_compute_dtype=torch.float16,
        bnb_4bit_use_double_quant=True,
    )

    # 3. Load model + tokenizer
    print(f"[3/5] Loading {MODEL_NAME} ...")
    tokenizer = AutoTokenizer.from_pretrained(
        MODEL_NAME, trust_remote_code=True
    )
    tokenizer.pad_token = tokenizer.eos_token
    tokenizer.padding_side = "right"

    model = AutoModelForCausalLM.from_pretrained(
        MODEL_NAME,
        quantization_config=bnb_config,
        device_map="auto",
        trust_remote_code=True,
    )
    model.config.use_cache = False

    # 4. LoRA
    print("[4/5] Applying LoRA ...")
    lora_config = LoraConfig(
        r=16,
        lora_alpha=32,
        target_modules=[
            "q_proj", "k_proj", "v_proj", "o_proj",
            "gate_proj", "up_proj", "down_proj"
        ],
        lora_dropout=0.05,
        bias="none",
        task_type="CAUSAL_LM",
    )
    model = get_peft_model(model, lora_config)
    model.print_trainable_parameters()

    # 5. Train — dùng SFTConfig thay TrainingArguments (TRL mới)
    print("[5/5] Starting training ...")
    sft_config = SFTConfig(
        output_dir=OUTPUT_DIR,
        num_train_epochs=EPOCHS,
        per_device_train_batch_size=BATCH_SIZE,
        gradient_accumulation_steps=GRAD_ACCUM,
        learning_rate=LEARNING_RATE,
        fp16=True,
        logging_steps=10,
        save_steps=100,
        save_total_limit=2,
        warmup_ratio=0.03,
        lr_scheduler_type="cosine",
        report_to="none",
        optim="paged_adamw_8bit",
        max_seq_length=MAX_SEQ_LEN,
        dataset_text_field="text",
    )

    trainer = SFTTrainer(
        model=model,
        processing_class=tokenizer,
        train_dataset=dataset,
        args=sft_config,
    )

    trainer.train()

    print(f"\n✅ Training done! Saving to {OUTPUT_DIR}/ ...")
    trainer.save_model(OUTPUT_DIR)
    tokenizer.save_pretrained(OUTPUT_DIR)
    print("✅ Done! Tiếp theo chạy export_gguf.py")

if __name__ == "__main__":
    main()