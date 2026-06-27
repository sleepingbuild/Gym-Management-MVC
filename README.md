# 🏋️ Gym Management System

A comprehensive Gym Management Platform built with ASP.NET Core MVC, supporting membership management, trainer scheduling, workout tracking, online payments, and an AI-powered Fitness Assistant.

---

## 📌 Project Overview

Gym Management System is a web-based application designed to support gym operations and improve member experience.

The system allows:
- Member management
- Membership package management
- Trainer management
- Training schedule booking
- Workout progress tracking
- Online payment processing
- AI-based fitness consultation

---

## 🎯 Project Objectives

- Digitalize gym management processes
- Improve operational efficiency
- Provide personalized fitness consultation
- Track workout progress effectively
- Reduce manual administrative workload
- Apply AI technology to fitness management

---

## 👥 Team Members

| Name        | Role                                |
| ----------- | ----------------------------------- |
| Phi         | Team Leader, Frontend Lead, AI Lead |
| Quang Trung | Backend Developer                   |
| Kiệt        | Backend Developer                   |
| Văn Quang   | AI Support, Documentation           |
| Hoàng Long  | Tester, Documentation Support       |

---

## 🚀 Main Features

### 1. User Management
- User Registration
- User Login
- Profile Management
- Password Management
- Role-based Access Control

**Roles:**
- Admin
- Trainer
- Member

### 2. Membership Management
- View Membership Packages
- Register Membership
- Renew Membership
- Membership Status Tracking

**Admin Features:**
- Create Package
- Update Package
- Disable Package

### 3. Trainer Management
- Trainer Profile Management
- Trainer Availability Management
- Trainer Assignment

### 4. Booking Management
- Book Training Sessions
- Cancel Bookings
- Booking History
- Schedule Tracking

### 5. Workout Progress Tracking
Track:
- Weight
- Height
- BMI
- Body Fat Percentage
- Workout Performance

### 6. Online Payment
**Supported:** VNPay

**Functions:**
- Membership Payment
- Membership Renewal Payment
- Transaction History
- Payment Status Tracking

### 7. AI Fitness Assistant
The AI Assistant supports:
- Fitness consultation
- Workout recommendations
- Nutrition suggestions
- Gym-related Q&A
- Fitness goal guidance

---

## 🤖 AI Architecture

**Hybrid AI Architecture:**
User Question
↓
Knowledge Base Retrieval
↓
Fine-Tuned Gym Model (Qwen 2.5 + LoRA)
↓
Confidence Evaluation
↓
If confidence is high → Return Response
Else → Gemini API → Return Response
↓
Save Chat History

text

---

## 🏗️ System Architecture

**Architecture Pattern:** MVC + Service Layer + Repository Pattern

### Layers
- **Presentation Layer:** ASP.NET Core MVC, Razor Views, Bootstrap 5
- **Business Layer:** Services, Business Logic, AI Integration
- **Data Layer:** Repository Pattern, Entity Framework Core, SQL Server
- **External Services:** Gemini API, VNPay

---

## 💻 Technology Stack

### Frontend
- ASP.NET Core MVC
- Razor View
- Bootstrap 5
- HTML5, CSS3, JavaScript

### Backend
- ASP.NET Core 8
- C#
- Entity Framework Core

### Database
- SQL Server 2022

### Authentication
- ASP.NET Core Identity
- Cookie Authentication

### AI
- Qwen 2.5 Instruct
- LoRA Fine-Tuning
- RAG (Retrieval-Augmented Generation)
- Gemini API

### Payment
- VNPay

### Logging
- Serilog

---

## 🗄️ Core Database Entities

- ApplicationUser (Identity)
- UserProfile
- MembershipPackage
- Membership
- Trainer
- Booking
- Payment
- WorkoutProgress
- ChatHistory
- FAQ

---

## 📅 Development Roadmap

**Duration:** 6 Weeks

| Week | Milestone | Status |
|------|-----------|--------|
| Week 1 | Foundation & Setup | ✅ Completed |
| Week 2 | Membership Management | 🔄 In Progress |
| Week 3 | Trainer & Booking Management | ⏳ Pending |
| Week 4 | Workout Progress & Payment | ⏳ Pending |
| Week 5 | AI Fitness Assistant | ⏳ Pending |
| Week 6 | Dashboard & Final Release | ⏳ Pending |

---

## 📂 Project Structure
GYM-MANAGEMENT-SYSTEM/
│
├── Controllers/
│ ├── AccountController.cs
│ ├── HomeController.cs
│ └── ...
├── Models/
│ ├── ApplicationUser.cs
│ ├── UserProfile.cs
│ └── ...
├── ViewModels/
│ ├── LoginViewModel.cs
│ ├── RegisterViewModel.cs
│ └── ...
├── Services/
│ ├── AuthService.cs
│ └── ...
├── Data/
│ └── ApplicationDbContext.cs
├── Views/
│ ├── Account/
│ ├── Home/
│ └── Shared/
├── wwwroot/
│ └── css/
├── AI/
│ ├── Services/
│ ├── Models/
│ └── Datasets/
└── Migrations/

text

---

## 🔒 Security Features

- ASP.NET Identity
- Password Hashing
- Role-based Authorization
- Session Management
- Secure Payment Verification

---

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server 2022 (or LocalDB)
- Visual Studio 2022 (recommended)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/sleepingbuild/Gym-Management-MVC.git
Navigate to project directory:

bash
cd GYM-MANAGEMENT-SYSTEM
Restore packages:

bash
dotnet restore
Update database:

bash
dotnet ef database update
Run the application:

bash
dotnet run
Access the application:

text
http://localhost:5225
📈 Future Enhancements
Personalized Workout Recommendation Engine

Nutrition Recommendation System

Voice-Based AI Assistant

Mobile Application

Wearable Device Integration

Advanced Analytics Dashboard

📜 License
This project is developed for educational purposes as a university software engineering project.

❤️ Acknowledgements
ASP.NET Core Community

Microsoft Learn

Qwen Team

Google Gemini

VNPay

Open Source Contributors