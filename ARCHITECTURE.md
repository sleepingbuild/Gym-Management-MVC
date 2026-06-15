# 🏋️ Gym Management System

A comprehensive Gym Management Platform built with ASP.NET Core MVC, supporting membership management, trainer scheduling, workout tracking, online payments, and an AI-powered Fitness Assistant.

---

# 📌 Project Overview

Gym Management System is a web-based application designed to support gym operations and improve member experience.

The system allows:

* Member management
* Membership package management
* Trainer management
* Training schedule booking
* Workout progress tracking
* Online payment processing
* AI-based fitness consultation

A key innovation of this project is the integration of a Hybrid AI Architecture that combines a fine-tuned domain-specific language model with Gemini API to provide intelligent fitness guidance.

---

# 🎯 Project Objectives

* Digitalize gym management processes.
* Improve operational efficiency.
* Provide personalized fitness consultation.
* Track workout progress effectively.
* Reduce manual administrative workload.
* Apply AI technology to fitness management.

---

# 👥 Team Members

| Name        | Role                                |
| ----------- | ----------------------------------- |
| Phi         | Team Leader, Frontend Lead, AI Lead |
| Quang Trung | Backend Developer                   |
| Kiệt        | Backend Developer                   |
| Văn Quang   | AI Support, Documentation           |
| Hoàng Long  | Tester, Documentation Support       |

---

# 🚀 Main Features

## 1. User Management

* User Registration
* User Login
* Profile Management
* Password Management
* Role-based Access Control

Roles:

* Admin
* Trainer
* Member

---

## 2. Membership Management

* View Membership Packages
* Register Membership
* Renew Membership
* Membership Status Tracking

Admin Features:

* Create Package
* Update Package
* Disable Package

---

## 3. Trainer Management

* Trainer Profile Management
* Trainer Availability Management
* Trainer Assignment

---

## 4. Booking Management

* Book Training Sessions
* Cancel Bookings
* Booking History
* Schedule Tracking

---

## 5. Workout Progress Tracking

Track:

* Weight
* Height
* BMI
* Body Fat Percentage
* Workout Performance

Progress data is stored historically for long-term analysis.

---

## 6. Online Payment

Supported:

* VNPay

Functions:

* Membership Payment
* Membership Renewal Payment
* Transaction History
* Payment Status Tracking

---

## 7. AI Fitness Assistant

The AI Assistant supports:

* Fitness consultation
* Workout recommendations
* Nutrition suggestions
* Gym-related Q&A
* Fitness goal guidance

Examples:

* Weight loss plans
* Muscle gain programs
* Beginner workout recommendations
* Nutrition planning

---

# 🤖 AI Architecture

The system uses a Hybrid AI Architecture.

Flow:

User Question

↓

Knowledge Base Retrieval

↓

Fine-Tuned Gym Model

(Qwen 2.5 + LoRA)

↓

Confidence Evaluation

↓

If confidence is high

→ Return Response

Else

↓

Gemini API

↓

Return Response

↓

Save Chat History

---

# 🧠 AI Components

## Fine-Tuned Gym Model

Base Model:

* Qwen 2.5 Instruct

Fine-Tuning Method:

* LoRA (Low-Rank Adaptation)

Training Domain:

* Fitness
* Gym Training
* Nutrition
* Workout Planning
* Personal Training Knowledge

Objectives:

* Improve accuracy for gym-related questions
* Reduce dependency on external APIs
* Provide personalized recommendations

---

## Knowledge Base

The knowledge base contains:

* Workout Programs
* Exercise Library
* Nutrition Guidelines
* Gym FAQs
* Training Methodologies

Purpose:

* Enhance AI responses
* Support Retrieval-Augmented Generation (RAG)

---

## Gemini Fallback Service

Gemini API is used when:

* Questions are outside the gym domain
* Model confidence is low
* General knowledge is required

Benefits:

* Better answer coverage
* Improved user experience
* Increased reliability

---

# 🏗️ System Architecture

Architecture Pattern:

MVC + Service Layer + Repository Pattern

Layers:

Presentation Layer

* ASP.NET Core MVC
* Razor Views
* Bootstrap 5

Business Layer

* Services
* Business Logic
* AI Integration

Data Layer

* Repository Pattern
* Entity Framework Core
* SQL Server

External Services

* Gemini API
* VNPay

---

# 💻 Technology Stack

## Frontend

* ASP.NET Core MVC
* Razor View
* Bootstrap 5
* HTML5
* CSS3
* JavaScript

## Backend

* ASP.NET Core 8
* C#
* Entity Framework Core

## Database

* SQL Server 2022

## Authentication

* ASP.NET Core Identity
* Cookie Authentication

## AI

* Qwen 2.5 Instruct
* LoRA Fine-Tuning
* RAG
* Gemini API

## Payment

* VNPay

## Logging

* Serilog

---

# 🗄️ Core Database Entities

Main Entities:

* User
* MembershipPackage
* Membership
* Trainer
* Booking
* Payment
* WorkoutProgress
* ChatHistory

---

# 📅 Development Roadmap

Duration:

6 Weeks

Milestones:

### Week 1

Foundation & Setup

### Week 2

Membership Management

### Week 3

Trainer & Booking Management

### Week 4

Workout Progress & Payment

### Week 5

AI Fitness Assistant

### Week 6

Dashboard & Final Release

Detailed roadmap can be found in:

ROADMAP.md

---

# 📂 Project Structure

```text
GymManagementSystem
│
├── Controllers
├── Models
├── ViewModels
├── Services
├── Repositories
├── Data
├── Validators
├── Views
├── wwwroot
│
├── AI
│   ├── Training
│   ├── Models
│   ├── KnowledgeBase
│   └── Services
│
└── Documentation
```

---

# 🔒 Security Features

* ASP.NET Identity
* Password Hashing
* Role-based Authorization
* Session Management
* Secure Payment Verification

---

# 📈 Future Enhancements

Potential future improvements:

* Personalized Workout Recommendation Engine
* Nutrition Recommendation System
* Voice-Based AI Assistant
* Mobile Application
* Wearable Device Integration
* Advanced Analytics Dashboard

---

# 📜 License

This project is developed for educational purposes as a university software engineering project.

---

# ❤️ Acknowledgements

Special thanks to:

* ASP.NET Core Community
* Microsoft Learn
* Qwen Team
* Google Gemini
* VNPay
* Open Source Contributors
