

## Gym Management System

This document defines the rules that all AI Coding Agents must follow.

Applies to:

- Claude Code
- GitHub Copilot Agent
- Cursor Agent
- OpenAI Codex Agents
- Any automated coding assistant

---

# 1. Project Overview

Gym Management System is a web-based application developed using ASP.NET Core MVC.

Main features:

- User Management
- Membership Management
- Trainer Management
- Booking System
- Workout Progress Tracking
- VNPay Integration
- AI Fitness Assistant

---

# 2. Technology Stack

Frontend:

- ASP.NET Core MVC
- Razor View
- Bootstrap 5
- HTML
- CSS
- JavaScript

Backend:

- ASP.NET Core 8
- C#

Database:

- SQL Server 2022
- Entity Framework Core

Authentication:

- ASP.NET Identity
- Cookie Authentication

AI:

- Fine-Tuned Qwen 2.5
- LoRA
- Gemini API
- RAG Knowledge Base

Payment:

- VNPay

Logging:

- Serilog

---

# 3. Project Structure

Agents MUST follow this structure.

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

Do NOT create new top-level folders without approval.

---

# 4. Architectural Rules

Mandatory Rules:

- Controllers must remain thin.
- Controllers must not contain business logic.
- Controllers must not access DbContext directly.
- Controllers must not execute raw SQL.

Business Logic:

- Must be placed in Services.

Data Access:

- Must be placed in Repositories.

Database:

- Must be accessed through Entity Framework Core.

---

# 5. Dependency Rules

Allowed:

Controller
→ Service
→ Repository
→ DbContext

Not Allowed:

Controller
→ DbContext

Controller
→ SQL Query

Controller
→ External API

---

# 6. Frontend Rules

Frontend Lead:

Phi

Agents working on frontend MUST:

- Use Razor View
- Use Bootstrap 5
- Use ViewModels

Do NOT:

- Add React
- Add Angular
- Add Vue
- Add Flutter

Without approval.

---

# 7. Backend Rules

Backend Developers:

- Quang Trung
- Kiệt

Backend Agents MUST:

- Follow Service Layer pattern
- Follow Repository pattern
- Use Dependency Injection

Do NOT:

- Put business logic inside Controllers
- Duplicate repository logic
- Use static database helpers

---

# 8. AI Module Rules

AI Lead:

Phi

AI Support:

Văn Quang

Architecture:

User Question
    ↓
Knowledge Retrieval
    ↓
Fine-Tuned Qwen Model
    ↓
Confidence Evaluation
    ↓
Gemini Fallback
    ↓
Response

Agents MAY:

- Train Qwen-based models
- Improve prompts
- Improve retrieval quality
- Extend Knowledge Base

Agents MUST NOT:

- Replace Qwen without approval
- Remove Gemini fallback
- Remove chat history logging
- Change AI architecture drastically

---

# 9. Database Rules

Primary Database:

SQL Server 2022

Migration Rules:

- Use EF Core Migration
- Never modify database manually
- Update migration files properly

Naming Convention:

Tables:

- Users
- Trainers
- Memberships
- MembershipPackages
- Bookings
- Payments
- WorkoutProgresses
- ChatHistories

---

# 10. Payment Rules

Gateway:

VNPay

Agents MUST:

- Verify payment callback
- Validate transaction signature
- Log payment events

Agents MUST NOT:

- Store sensitive payment information
- Skip payment verification

---

# 11. Security Rules

Required:

- ASP.NET Identity
- Password Hashing
- Role Authorization
- Input Validation

Agents MUST NOT:

- Store plain text passwords
- Disable authentication
- Disable authorization

---

# 12. Logging Rules

Logging Framework:

Serilog

Must Log:

- Authentication events
- Payment events
- AI requests
- Errors

---

# 13. Git Workflow

Branch Naming:

feature/<feature-name>

bugfix/<bug-name>

hotfix/<hotfix-name>

Examples:

feature/membership-registration

feature/ai-chatbot

feature/payment-vnpay

---

# 14. Commit Convention

Format:

type(scope): description

Examples:

feat(membership): add registration service

feat(ai): integrate gemini fallback

fix(payment): verify callback signature

docs(readme): update installation guide

---

# 15. Pull Request Rules

One Issue = One Pull Request

Each Pull Request must:

- Build successfully
- Pass tests
- Follow architecture
- Update documentation if required

---

# 16. Issue Assignment Rules

Phi

- Frontend
- Dashboard
- AI
- Final Integration

Quang Trung

- Authentication
- Membership
- Trainer
- Payment

Kiệt

- Database
- Booking
- Workout Progress
- Dashboard Statistics

Văn Quang

- Dataset Collection
- Knowledge Base
- Documentation

Hoàng Long

- Testing
- User Manual
- QA

---

# 17. Definition of Done

A task is completed only when:

- Code compiles successfully
- No critical bugs remain
- Pull Request approved
- Merged into develop branch
- Documentation updated
- Issue checklist completed

---

# 18. Agent Restrictions

Agents MUST NOT:

- Change architecture without approval
- Introduce new frameworks
- Delete existing modules
- Rename database entities without approval
- Modify unrelated modules

When in doubt:

Follow Architecture.md first.
