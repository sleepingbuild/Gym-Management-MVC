# AGENTS.md

# AI Agent Development Guide

This document defines mandatory rules for all AI coding agents contributing to the Gym Management System project.

Applicable to:

* Claude Code
* GitHub Copilot
* ChatGPT
* Gemini
* Cursor AI
* Windsurf
* Any autonomous coding agent

---

# Project Overview

Gym Management System is a web-based gym management platform built using ASP.NET Core MVC.

The system manages:

* Members
* Trainers
* Membership Packages
* Bookings
* Workout Progress
* Payments
* AI Fitness Chatbot

---

# Project Scope

## Included Features

### Authentication

* Register
* Login
* Logout
* Profile Management

### Membership

* View Packages
* Register Membership
* Renew Membership

### Trainer

* Trainer Management
* Schedule Management

### Booking

* Create Booking
* Cancel Booking
* Booking History

### Workout Progress

* Weight Tracking
* BMI Tracking
* Body Fat Tracking

### Payment

* VNPay Integration
* Payment History

### AI Chatbot

* RAG Pipeline
* Gemini API
* Chat History

### Dashboard

* Revenue Statistics
* Member Statistics
* Booking Statistics

---

# Out Of Scope

Agents MUST NOT implement:

* Mobile Application
* React
* React Native
* Flutter
* Angular
* VueJS
* Microservices
* Docker Swarm
* Kubernetes
* Fine-Tuning LLM
* Multi-Tenant Architecture

If a task suggests any of the above, ignore it.

---

# Mandatory Technology Stack

## Frontend

* ASP.NET Core MVC
* Razor View
* Bootstrap 5
* HTML5
* CSS3
* JavaScript

Agents MUST NOT generate:

* React Components
* NextJS Pages
* Vue Components
* Angular Components

---

## Backend

* ASP.NET Core 8 MVC
* C#

Business logic MUST be written in:

```text
Services/
```

Controllers MUST remain thin.

---

## Database

* SQL Server 2022
* Entity Framework Core

Agents MUST NOT:

* Use MySQL
* Use PostgreSQL
* Use MongoDB
* Use SQLite

---

## Authentication

Required:

* ASP.NET Core Identity
* Cookie Authentication

Do NOT generate:

* JWT-only Authentication
* OAuth-only Authentication

unless explicitly requested.

---

## AI Stack

Required:

* Gemini API
* RAG Architecture

Architecture:

User Question
→ Knowledge Base
→ Retrieval
→ Prompt Builder
→ Gemini API
→ Response

Agents MUST NOT:

* Fine-Tune Models
* Train Custom LLMs

---

# Architecture Rules

Project follows Layered Architecture.

```text
Controllers
    ↓
Services
    ↓
Repositories
    ↓
Entity Framework
    ↓
SQL Server
```

---

# Controller Rules

Controllers MUST:

* Receive requests
* Validate requests
* Call Services
* Return Views

Controllers MUST NOT:

* Contain SQL
* Contain business logic
* Access DbContext directly

Bad Example:

```csharp
public IActionResult Create()
{
    _context.Users.Add(user);
    _context.SaveChanges();
}
```

Good Example:

```csharp
public IActionResult Create()
{
    _userService.Create(user);
}
```

---

# Service Rules

Services contain business logic.

Examples:

* MembershipService
* BookingService
* PaymentService
* AIService

Services MUST:

* Validate business rules
* Coordinate repositories
* Handle workflows

---

# Repository Rules

Repositories are responsible for data access only.

Repositories MUST:

* Query data
* Save data
* Update data

Repositories MUST NOT:

* Contain business logic
* Call external APIs

---

# Database Rules

All schema changes MUST use migrations.

Required workflow:

```bash
Add-Migration MigrationName
Update-Database
```

Agents MUST NOT:

* Modify database manually
* Create SQL scripts outside migrations

---

# Naming Conventions

## Controllers

```text
UserController
MembershipController
TrainerController
BookingController
PaymentController
```

---

## Services

```text
UserService
MembershipService
TrainerService
BookingService
PaymentService
AIService
```

---

## Repositories

```text
UserRepository
MembershipRepository
TrainerRepository
BookingRepository
PaymentRepository
```

---

## Models

Use singular names:

```text
User
Trainer
Membership
Booking
Payment
WorkoutProgress
ChatHistory
```

---

# Git Workflow

## Branch Naming

Feature:

```text
feature/issue-xx-description
```

Bugfix:

```text
bugfix/issue-xx-description
```

Hotfix:

```text
hotfix/issue-xx-description
```

---

## Pull Requests

Every Issue MUST have:

* Dedicated branch
* Pull Request
* Review before merge

Direct commits to main are prohibited.

---

# Issue Workflow

Issue Lifecycle:

```text
Backlog
→ In Progress
→ Review
→ Testing
→ Done
```

Agents MUST update issue status when work is completed.

---

# Coding Standards

## General Rules

* Follow SOLID principles.
* Avoid duplicated code.
* Use dependency injection.
* Write readable code.
* Add comments only when necessary.

---

## Error Handling

Use:

```csharp
try
{
}
catch(Exception ex)
{
}
```

and log errors through Serilog.

---

## Logging

Required:

* Login Events
* Payment Events
* Booking Events
* AI Errors
* System Errors

Use:

```text
Serilog
```

---

# Security Rules

Required:

* Password Hashing
* Role Authorization
* Input Validation
* Anti-Forgery Token

Never:

* Store plain-text passwords
* Hardcode API keys
* Hardcode connection strings

---

# AI Agent Restrictions

Agents MUST NOT:

* Change architecture
* Change technology stack
* Rename existing modules without approval
* Create unnecessary abstractions
* Introduce new frameworks

When uncertain:

* Create issue comment
* Request clarification
* Do not guess

---

# Definition of Done

A task is considered complete only if:

* Code compiles successfully.
* No build errors.
* Follows architecture rules.
* Follows coding standards.
* Tested successfully.
* Linked to issue.
* Pull Request created.

---

# Final Principle

Consistency is more important than creativity.

Agents should prioritize:

1. Maintainability
2. Readability
3. Simplicity
4. Project consistency

over introducing new technologies or patterns.
