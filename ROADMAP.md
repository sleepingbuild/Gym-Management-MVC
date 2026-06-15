
# Gym Management System Roadmap

Project Duration: 6 Weeks

Development Methodology: Agile Sprint-Based Development

Repository Strategy:

* main → Stable Release
* develop → Integration Branch
* feature/* → Feature Development

---

# Team Responsibilities

## Phi

Role:

* Team Leader
* Frontend Lead
* AI Lead

Responsibilities:

* UI/UX Development
* Dashboard Development
* AI Chatbot Development
* Integration Review
* Final Release

---

## Quang Trung

Role:

* Backend Developer

Responsibilities:

* Authentication
* Membership Module
* Trainer Module
* Payment Integration

---

## Kiệt

Role:

* Backend Developer

Responsibilities:

* Database
* Booking Module
* Workout Progress
* Dashboard Statistics

---

## Văn Quang

Role:

* AI Support
* Documentation

Responsibilities:

* Knowledge Base
* Documentation
* AI Testing Support

---

## Hoàng Long

Role:

* Tester
* Documentation Support

Responsibilities:

* System Testing
* Bug Reporting
* User Manual
* Acceptance Testing

---

# Milestone 1 — Foundation & Setup

Duration: Week 1

Goal:

Setup development environment, database architecture and authentication infrastructure.

Issues:

### Issue #1

Project Setup & Solution Structure

Owner: Phi

### Issue #2

Database Design

Owner: Kiệt

### Issue #3

Entity Framework Core Configuration

Owner: Kiệt

### Issue #4

Authentication Infrastructure

Owner: Quang Trung

### Issue #5

Project Documentation Setup

Owner: Văn Quang + Hoàng Long

Deliverables:

* Solution Structure
* Database Schema
* Initial Migration
* Authentication Setup
* Core Documentation

---

# Milestone 2 — Membership Management

Duration: Week 2

Goal:

Develop membership management module.

Issues:

### Issue #6

Membership Package CRUD Backend

Owner: Quang Trung

### Issue #7

Membership Package UI

Owner: Phi

### Issue #8

Membership Registration Module

Owner: Kiệt

### Issue #9

Membership Renewal Module

Owner: Kiệt

### Issue #10

Membership Testing

Owner: Hoàng Long

Deliverables:

* Membership Package Management
* Membership Registration
* Membership Renewal
* Membership Validation

---

# Milestone 3 — Trainer & Booking

Duration: Week 3

Goal:

Develop trainer management and booking system.

Issues:

### Issue #11

Trainer CRUD Backend

Owner: Quang Trung

### Issue #12

Trainer Management UI

Owner: Phi

### Issue #13

Trainer Schedule Module

Owner: Kiệt

### Issue #14

Booking Creation Module

Owner: Kiệt

### Issue #15

Booking Cancellation & History

Owner: Kiệt

### Issue #16

Booking UI

Owner: Phi

### Issue #17

Booking Testing

Owner: Hoàng Long

Deliverables:

* Trainer Management
* Scheduling
* Booking System

---

# Milestone 4 — Progress & Payment

Duration: Week 4

Goal:

Implement workout tracking and payment system.

Issues:

### Issue #18

Workout Progress Backend

Owner: Kiệt

### Issue #19

Workout Progress UI

Owner: Phi

### Issue #20

VNPay Configuration

Owner: Quang Trung

### Issue #21

VNPay Payment Processing

Owner: Quang Trung

### Issue #22

Payment Testing

Owner: Hoàng Long

Deliverables:

* Workout Tracking
* Payment Processing
* Payment Validation

---

# Milestone 5 — AI Chatbot

Duration: Week 5

Goal:

Develop AI Fitness Assistant.

Issues:

### Issue #23

Knowledge Base Preparation

Owner: Văn Quang

### Issue #24

RAG Retrieval Service

Owner: Phi

### Issue #25

Gemini API Integration

Owner: Phi

### Issue #26

Chat History Module

Owner: Quang Trung

### Issue #27

AI Chat Interface

Owner: Phi

### Issue #28

AI Testing

Owner: Văn Quang + Hoàng Long

Deliverables:

* AI Chatbot
* RAG Pipeline
* Knowledge Base
* Chat History

---

# Milestone 6 — Dashboard & Release

Duration: Week 6

Goal:

Finalize project and prepare for defense.

Issues:

### Issue #29

Dashboard Statistics Backend

Owner: Kiệt

### Issue #30

Dashboard UI

Owner: Phi

### Issue #31

System Integration Testing

Owner: Hoàng Long

### Issue #32

User Manual & Documentation

Owner: Văn Quang

### Issue #33

Release Preparation

Owner: Phi

Deliverables:

* Dashboard
* Final Documentation
* User Manual
* Release v1.0.0

---

# Definition of Done

A task is considered completed only when:

* Code compiles successfully.
* No critical bugs remain.
* Pull Request approved.
* Merged into develop branch.
* Issue checklist completed.
* Documentation updated.

---

# Notes For AI Agents

Mandatory Rules:

* Follow ARCHITECTURE.md.
* Follow AGENTS.md.
* Do not change technology stack.
* Do not introduce React, Angular or Flutter.
* Do not write SQL in Controllers.
* Business Logic belongs to Services.
* Each Issue should produce one Pull Request.
* Do not modify unrelated modules.
* Always update documentation when changing architecture.

Project Stack:

* ASP.NET Core MVC 8
* Razor View
* Bootstrap 5
* SQL Server 2022
* Entity Framework Core
* ASP.NET Identity
* VNPay
* Gemini API
* Serilog
