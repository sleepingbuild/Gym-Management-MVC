# Gym Management System

<div align="center">

# 🏋️ Gym Management System

### AI-Powered Gym Management Platform

A comprehensive gym management platform built with ASP.NET Core MVC, designed to streamline membership management, trainer scheduling, payment processing, and AI-assisted fitness consultation.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-purple)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-blue)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-green)
![Gemini AI](https://img.shields.io/badge/Gemini-AI-orange)
![License](https://img.shields.io/badge/License-Educational-lightgrey)

</div>

---

# 📌 Project Overview

Gym Management System is a web-based platform that helps fitness centers manage members, trainers, memberships, bookings, workout progress, online payments, and AI-powered fitness consultation.

The system is developed as a university software engineering project and follows modern web application architecture principles using ASP.NET Core MVC.

---

# 🎯 Project Objectives

The project aims to:

- Digitize gym management operations.
- Simplify membership registration and renewal.
- Improve trainer scheduling and booking management.
- Track workout progress effectively.
- Support online payment processing.
- Provide personalized fitness consultation using AI.
- Enhance user experience through a centralized management platform.

---

# 👥 User Roles

## Member

Members can:

- Register account
- Login to system
- Update profile
- View membership packages
- Register membership
- Renew membership
- Book trainer sessions
- Cancel bookings
- Track workout progress
- Make online payments
- Chat with AI assistant

---

## Trainer

Trainers can:

- View assigned members
- Manage schedules
- View booking requests
- Update workout progress records

---

## Admin

Administrators can:

- Manage users
- Manage trainers
- Manage membership packages
- Manage bookings
- Monitor payments
- View system statistics
- Manage AI knowledge base

---

# 🚀 Core Features

## 🔐 Authentication & Authorization

- User Registration
- Login / Logout
- Change Password
- Profile Management
- Role-Based Authorization
- ASP.NET Identity Integration

---

## 🏋 Membership Management

- Membership Package Management
- Membership Registration
- Membership Renewal
- Membership Status Tracking

---

## 👨‍🏫 Trainer Management

- Trainer Profiles
- Trainer Scheduling
- Trainer Assignment
- Trainer Availability Tracking

---

## 📅 Booking Management

- Session Booking
- Booking Cancellation
- Booking History
- Schedule Management

---

## 📈 Workout Progress Tracking

- Weight Tracking
- Height Tracking
- BMI Monitoring
- Body Fat Tracking
- Progress History

---

## 💳 Online Payment

- VNPay Integration
- Payment History
- Payment Status Tracking
- Membership Payment Processing

---

## 🤖 AI Fitness Assistant

### Features

- Fitness Consultation
- Nutrition Suggestions
- Workout Recommendations
- Frequently Asked Questions
- Personalized Guidance

### AI Architecture

```text
User Question
      ↓
Knowledge Base
      ↓
RAG Retrieval
      ↓
Prompt Builder
      ↓
Gemini API
      ↓
AI Response
```

---

## 📊 Dashboard & Reporting

### Admin Dashboard

- Total Members
- Active Memberships
- Revenue Statistics
- Booking Statistics

### Trainer Dashboard

- Assigned Members
- Upcoming Sessions

### Member Dashboard

- Membership Status
- Upcoming Bookings
- Workout Progress

---

# 🏗 System Architecture Overview

```text
┌─────────────────────────────┐
│      Presentation Layer     │
│ ASP.NET MVC + Razor + BS5   │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│      Business Layer         │
│ Service + Business Logic    │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│      Data Access Layer      │
│ Entity Framework Core       │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│        SQL Server 2022      │
└─────────────────────────────┘
```

---

# 🛠 Technology Stack

## Frontend

| Technology | Purpose |
|------------|----------|
| ASP.NET Core MVC | Web Framework |
| Razor View | Dynamic UI |
| Bootstrap 5 | Responsive Design |
| HTML5 | Page Structure |
| CSS3 | Styling |
| JavaScript | Client-side Interaction |

---

## Backend

| Technology | Purpose |
|------------|----------|
| ASP.NET Core 8 | Application Framework |
| C# | Programming Language |
| Entity Framework Core | ORM |
| ASP.NET Identity | Authentication |
| SignalR | Real-time Communication |

---

## Database

| Technology | Purpose |
|------------|----------|
| SQL Server 2022 | Relational Database |

---

## AI Services

| Technology | Purpose |
|------------|----------|
| Gemini API | LLM Provider |
| RAG | Context Retrieval |
| Knowledge Base | Domain Knowledge |

---

## Payment

| Technology | Purpose |
|------------|----------|
| VNPay | Online Payment Gateway |

---

## Logging & Validation

| Technology | Purpose |
|------------|----------|
| Serilog | Logging |
| Data Annotation | Validation |
| FluentValidation | Advanced Validation |

---

# 📂 Project Structure

```text
GymManagementSystem
│
├── docs/
│   ├── UseCase/
│   ├── ERD/
│   ├── ClassDiagram/
│   ├── SequenceDiagram/
│   └── Report/
│
├── src/
│   │
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   ├── Services/
│   ├── Repositories/
│   ├── Data/
│   ├── Middleware/
│   └── Helpers/
│
├── tests/
│
├── README.md
├── ROADMAP.md
├── ARCHITECTURE.md
├── AGENTS.md
└── LICENSE
```

---

# 👨‍💻 Development Team

| Member | Responsibility |
|----------|----------|
| Phi | Team Leader, Frontend Lead, AI Lead |
| Quang Trung | Backend Development |
| Kiệt | Backend Development |
| Văn Quang | AI Support, UI Testing |
| Hoàng Long | Testing & Documentation |

---

# 📅 Development Timeline

| Week | Milestone |
|--------|--------|
| Week 1 | Setup & Database |
| Week 2 | Authentication & Membership |
| Week 3 | Trainer & Booking |
| Week 4 | Progress & Payment |
| Week 5 | AI Chatbot |
| Week 6 | Dashboard & Release |

For detailed planning, see:

```text
ROADMAP.md
```

---

# 📖 Documentation

Project documents:

- README.md
- ROADMAP.md
- ARCHITECTURE.md
- AGENTS.md
- User Manual
- Installation Guide

---

# ⚠ Development Guidelines

All contributors and AI agents must follow:

- Service Layer architecture.
- No business logic inside Controllers.
- No direct SQL in Controllers.
- Entity Framework Core only.
- SQL Server only.
- ASP.NET MVC only.
- Razor View only.
- Pull Request required before merge.
- Follow naming conventions.
- Use migration for schema changes.

Detailed rules can be found in:

```text
AGENTS.md
```

---

# 🎓 Academic Purpose

This project is developed for educational and academic purposes as part of a university software engineering course.

---

# 📄 License

This project is intended for educational use only.

© 2026 Gym Management System Team
