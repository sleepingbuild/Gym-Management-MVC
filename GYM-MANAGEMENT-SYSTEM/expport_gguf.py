"""
export_gguf.py
Chạy SAU khi train.py hoàn thành.
Merge LoRA adapter vào base model và export GGUF để dùng với Ollama.
"""
import subprocess
import sys
import os
from pathlib import Path

ADAPTER_DIR  = "gymbot-output"         # output từ train.py
MERGED_DIR   = "gymbot-merged"         # model sau khi merge
GGUF_DIR     = "gymbot-gguf"           # output GGUF cuối cùng
MODEL_NAME   = "Qwen/Qwen2.5-1.5B-Instruct"
GGUF_Q       = "q4_k_m"               # quantization tốt nhất cho 6GB VRAM

def merge_lora():
    print("[1/3] Merging LoRA adapter into base model ...")
    from transformers import AutoTokenizer, AutoModelForCausalLM
    from peft import PeftModel
    import torch

    tokenizer = AutoTokenizer.from_pretrained(ADAPTER_DIR, trust_remote_code=True)
    base_model = AutoModelForCausalLM.from_pretrained(
        MODEL_NAME,
        torch_dtype=torch.float16,
        device_map="cpu",           # merge trên CPU để không bị OOM
        trust_remote_code=True,
    )
    model = PeftModel.from_pretrained(base_model, ADAPTER_DIR)
    model = model.merge_and_unload()

    os.makedirs(MERGED_DIR, exist_ok=True)
    model.save_pretrained(MERGED_DIR)
    tokenizer.save_pretrained(MERGED_DIR)
    print(f"      → Merged model saved to {MERGED_DIR}/")

def clone_llamacpp():
    print("[2/3] Setting up llama.cpp for GGUF conversion ...")
    if not Path("llama.cpp").exists():
        subprocess.run(
            ["git", "clone", "https://github.com/ggerganov/llama.cpp.git"],
            check=True
        )
    subprocess.run(
        [sys.executable, "-m", "pip", "install", "-r", "llama.cpp/requirements.txt"],
        check=True
    )

def convert_to_gguf():
    print(f"[3/3] Converting to GGUF ({GGUF_Q}) ...")
    os.makedirs(GGUF_DIR, exist_ok=True)
    subprocess.run([
        sys.executable,
        "llama.cpp/convert_hf_to_gguf.py",
        MERGED_DIR,
        "--outtype", "f16",
        "--outfile", f"{GGUF_DIR}/gymbot-f16.gguf",
    ], check=True)

    # Quantize
    subprocess.run([
        "llama.cpp/build/bin/llama-quantize",
        f"{GGUF_DIR}/gymbot-f16.gguf",
        f"{GGUF_DIR}/gymbot.{GGUF_Q}.gguf",
        GGUF_Q,
    ], check=True)

    print(f"\n✅ GGUF ready: {GGUF_DIR}/gymbot.{GGUF_Q}.gguf")
    print("Tiếp theo chạy: ollama create gymbot -f Modelfile")

if __name__ == "__main__":
    merge_lora()
    clone_llamacpp()
    convert_to_gguf()