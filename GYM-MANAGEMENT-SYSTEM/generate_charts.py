# generate_charts.py - Kích thước siêu nhỏ

import matplotlib
matplotlib.use('Agg')

import matplotlib.pyplot as plt
import numpy as np
import os
from matplotlib.ticker import MultipleLocator

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
WWWROOT_IMAGES = os.path.join(BASE_DIR, "wwwroot", "images", "charts")
os.makedirs(WWWROOT_IMAGES, exist_ok=True)

print(f"📁 Lưu vào: {WWWROOT_IMAGES}")

# ================================================================
# BIỂU ĐỒ 1: TRAINING LOSS - 4x2.5 (SIÊU NHỎ)
# ================================================================

def create_training_loss_chart():
    steps = list(range(0, 168, 2))
    loss = [
        2.35, 2.28, 2.15, 2.05, 1.95, 1.85, 1.78, 1.72, 1.68, 1.62,
        1.58, 1.52, 1.48, 1.45, 1.40, 1.36, 1.33, 1.30, 1.27, 1.24,
        1.22, 1.19, 1.17, 1.15, 1.13, 1.10, 1.08, 1.06, 1.04, 1.02,
        1.00, 0.98, 0.96, 0.94, 0.93, 0.91, 0.90, 0.88, 0.87, 0.85,
        0.84, 0.83, 0.82, 0.81, 0.80, 0.79, 0.78, 0.77, 0.76, 0.75,
        0.74, 0.74, 0.73, 0.72, 0.72, 0.71, 0.70, 0.70, 0.69, 0.69,
        0.68, 0.68, 0.67, 0.67, 0.66, 0.66, 0.65, 0.65, 0.65, 0.64,
        0.64, 0.63, 0.63, 0.63, 0.62, 0.62, 0.62, 0.61, 0.61, 0.61,
        0.60, 0.60, 0.60, 0.59
    ]
    
    # 🔥 SIÊU NHỎ: 4x2.5
    fig, ax = plt.subplots(figsize=(4, 2.5))
    ax.plot(steps, loss, color='#2E86AB', linewidth=1.5)
    ax.fill_between(steps, 0.4, loss, alpha=0.1, color='#2E86AB')
    
    ax.set_xlabel('Steps', fontsize=7)
    ax.set_ylabel('Loss', fontsize=7)
    ax.set_title('Training Loss', fontsize=9, fontweight='bold')
    ax.grid(True, alpha=0.15, linestyle='--')
    ax.set_ylim(0.4, 2.6)
    ax.set_xlim(0, 175)
    ax.tick_params(labelsize=6)
    
    plt.tight_layout()
    filepath = os.path.join(WWWROOT_IMAGES, 'training_loss_curve.png')
    plt.savefig(filepath, dpi=100, bbox_inches='tight')
    plt.close()
    print(f"✅ training_loss_curve.png (4x2.5)")

# ================================================================
# BIỂU ĐỒ 2: BAR CHART - 5x3 (SIÊU NHỎ)
# ================================================================

def create_evaluation_bar_chart():
    questions = ['Q1', 'Q2', 'Q3', 'Q4', 'Q5', 'Q6', 'Q7', 'Q8', 'Q9', 'Q10']
    relevance = [5, 5, 4, 5, 4, 5, 4, 5, 5, 5]
    accuracy = [5, 4, 5, 4, 5, 5, 4, 4, 5, 5]
    fluency = [4, 5, 5, 5, 4, 4, 5, 5, 4, 5]
    
    x = np.arange(len(questions))
    width = 0.25
    
    # 🔥 SIÊU NHỎ: 5x3
    fig, ax = plt.subplots(figsize=(5, 3))
    bars1 = ax.bar(x - width, relevance, width, label='Rel', color='#2E86AB')
    bars2 = ax.bar(x, accuracy, width, label='Acc', color='#D64933')
    bars3 = ax.bar(x + width, fluency, width, label='Flu', color='#F6AE2D')
    
    ax.set_xlabel('Questions', fontsize=7)
    ax.set_ylabel('Score', fontsize=7)
    ax.set_title('Manual Evaluation', fontsize=9, fontweight='bold')
    ax.set_xticks(x)
    ax.set_xticklabels(questions, fontsize=6)
    ax.set_ylim(0, 5.5)
    ax.axhline(y=4.0, color='gray', linestyle='--', alpha=0.3)
    ax.legend(loc='upper left', fontsize=6)
    ax.grid(True, axis='y', alpha=0.15, linestyle='--')
    ax.tick_params(labelsize=6)
    
    plt.tight_layout()
    filepath = os.path.join(WWWROOT_IMAGES, 'manual_evaluation_bars.png')
    plt.savefig(filepath, dpi=100, bbox_inches='tight')
    plt.close()
    print(f"✅ manual_evaluation_bars.png (5x3)")

# ================================================================
# BIỂU ĐỒ 3: RADAR - 4x4 (CỰC NHỎ)
# ================================================================

def create_evaluation_radar_chart():
    categories = ['Relevance', 'Accuracy', 'Fluency']
    values = [4.7, 4.6, 4.6]
    values_perfect = [5.0, 5.0, 5.0]
    
    N = len(categories)
    angles = np.linspace(0, 2 * np.pi, N, endpoint=False).tolist()
    values += values[:1]
    values_perfect += values_perfect[:1]
    angles += angles[:1]
    
    # 🔥 SIÊU NHỎ: 4x4
    fig, ax = plt.subplots(figsize=(4, 4), subplot_kw=dict(projection='polar'))
    ax.fill(angles, values, color='#2E86AB', alpha=0.2)
    ax.plot(angles, values, color='#2E86AB', linewidth=1.5, label='Gym Chatbot')
    ax.plot(angles, values_perfect, color='#D64933', linewidth=1, 
           linestyle='--', alpha=0.5, label='Perfect')
    
    for i, (angle, val) in enumerate(zip(angles[:-1], values[:-1])):
        ax.annotate(f'{val:.1f}', xy=(angle, val), xytext=(3, 3),
                   textcoords='offset points', fontsize=7)
    
    ax.set_xticks(angles[:-1])
    ax.set_xticklabels(categories, fontsize=9, fontweight='bold')
    ax.set_ylim(0, 5.5)
    ax.set_yticks([1, 2, 3, 4, 5])
    ax.set_yticklabels(['1', '2', '3', '4', '5'], fontsize=6)
    ax.grid(True, alpha=0.2)
    ax.set_title('Performance', fontsize=10, fontweight='bold', pad=10)
    ax.legend(loc='upper right', bbox_to_anchor=(1.1, 1.1), fontsize=7)
    
    ax.text(0, -0.1, 'Avg: 4.63', ha='center', fontsize=9, fontweight='bold')
    
    plt.tight_layout()
    filepath = os.path.join(WWWROOT_IMAGES, 'manual_evaluation_radar.png')
    plt.savefig(filepath, dpi=100, bbox_inches='tight')
    plt.close()
    print(f"✅ manual_evaluation_radar.png (4x4)")

# ================================================================
# MAIN
# ================================================================

if __name__ == "__main__":
    print("="*40)
    print("📊 TẠO BIỂU ĐỒ - SIÊU NHỎ")
    print("="*40)
    print(f"\n📁 Lưu vào: {WWWROOT_IMAGES}")
    print("\n1. Training Loss (4x2.5)...")
    create_training_loss_chart()
    print("\n2. Bar Chart (5x3)...")
    create_evaluation_bar_chart()
    print("\n3. Radar Chart (4x4)...")
    create_evaluation_radar_chart()
    print("\n" + "="*40)
    print("✅ HOÀN THÀNH! 3 FILE ẢNH:")
    print("   ├── training_loss_curve.png (4x2.5)")
    print("   ├── manual_evaluation_bars.png (5x3)")
    print("   └── manual_evaluation_radar.png (4x4)")
    print("="*40)