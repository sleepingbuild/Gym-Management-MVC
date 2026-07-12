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
- Online payment processing (VNPay)
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
- Role-based Access Control (Admin, Trainer, Member)

### 2. Membership Management
- View Membership Packages
- Register Membership
- Renew Membership
- Membership Status Tracking
- Admin CRUD for Packages

### 3. Trainer Management
- Trainer Profile Management
- Trainer Availability Management
- Trainer Schedule Management
- Trainer Assignment

### 4. Booking Management
- Book Training Sessions
- Cancel Bookings
- Booking History
- Schedule Tracking
- Calendar View

### 5. Workout Progress Tracking
- Weight Tracking
- Height Tracking
- BMI Calculation
- Body Fat Percentage
- Muscle Mass Tracking
- Workout Performance

### 6. Online Payment
- VNPay Integration
- Membership Payment
- Membership Renewal Payment
- Transaction History
- Payment Status Tracking

### 7. AI Fitness Assistant
- Fitness consultation
- Workout recommendations
- Nutrition suggestions
- Gym-related Q&A
- Fitness goal guidance

### 8. Dashboard
- Revenue Statistics
- Membership Analytics
- Booking Analytics
- Trainer Performance
- Payment Analytics

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
- Chart.js

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
- TrainerSchedule
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
| Week 2 | Membership Management | ✅ Completed |
| Week 3 | Trainer & Booking Management | ✅ Completed |
| Week 4 | Progress & Payment | ✅ Completed |
| Week 5 | AI Fitness Assistant | ✅ Completed |
| Week 6 | Dashboard & Final Release | 🔄 In Progress |

---

## 📂 Project Structure
GYM-MANAGEMENT-SYSTEM/
│
├── Controllers/
│ ├── AccountController.cs
│ ├── HomeController.cs
│ ├── PackageController.cs
│ ├── MembershipController.cs
│ ├── TrainerController.cs
│ ├── ScheduleController.cs
│ ├── BookingController.cs
│ ├── WorkoutController.cs
│ ├── PaymentController.cs
│ └── DashboardController.cs
│
├── Models/
│ ├── ApplicationUser.cs
│ ├── UserProfile.cs
│ ├── MembershipPackage.cs
│ ├── Membership.cs
│ ├── Trainer.cs
│ ├── TrainerSchedule.cs
│ ├── Booking.cs
│ ├── Payment.cs
│ ├── WorkoutProgress.cs
│ └── ...
│
├── ViewModels/
├── Services/
├── Repositories/
├── Data/
├── Views/
├── wwwroot/
├── AI/
│ ├── Services/
│ ├── Models/
│ └── Datasets/
└── Tests/

text

---

## 🔒 Security Features

- ASP.NET Identity
- Password Hashing
- Role-based Authorization
- Session Management
- Secure Payment Verification (VNPay)

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
📊 API Endpoints (Dashboard)
Endpoint	Method	Description
/Dashboard/Revenue	GET	Monthly revenue data
/Dashboard/Membership	GET	Monthly membership data
/Dashboard/BookingStatus	GET	Booking status distribution
/Dashboard/PaymentStatus	GET	Payment status distribution
/Dashboard/RevenueByMethod	GET	Revenue by payment method
/Dashboard/DailyRevenue	GET	Daily revenue (30 days)
/Dashboard/Trainers	GET	Top trainers
/Dashboard/PackageDistribution	GET	Membership package distribution
📝 User Manual
For Members
Register an account

Login to access the system

Browse membership packages

Register for a membership

Book training sessions with trainers

Track workout progress

View booking history

Renew membership when expired

For Trainers
View assigned bookings

Manage schedule

Confirm or cancel bookings

Track member progress

For Admins
Manage membership packages (CRUD)

Manage trainers (CRUD)

View all bookings

View payment history

Access dashboard analytics

Manage user roles

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