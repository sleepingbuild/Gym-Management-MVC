# Gym Management System Roadmap

**Project Duration:** 6 Weeks  
**Development Methodology:** Agile Sprint-Based Development  
**Current Status:** Phase 1 Completed ✅

---

## Repository Strategy

- `main` → Stable Release
- `develop` → Integration Branch
- `feature/*` → Feature Development

---

## Team Responsibilities

### Phi
- Team Leader
- Frontend Lead
- AI Lead
- UI/UX Development
- Dashboard Development
- AI Chatbot Development

### Quang Trung
- Backend Developer
- Authentication
- Membership Module
- Trainer Module
- Payment Integration

### Kiệt
- Backend Developer
- Database
- Booking Module
- Workout Progress
- Dashboard Statistics

### Văn Quang
- AI Support
- Documentation
- Knowledge Base
- AI Testing Support

### Hoàng Long
- Tester
- Documentation Support
- System Testing
- User Manual

---

## Milestone 1 — Foundation & Setup ✅ (Completed)

**Duration:** Week 1

| Issue | Title | Owner | Status |
|-------|-------|-------|--------|
| #1 | Project Setup & Solution Structure | Phi | ✅ Done |
| #2 | Database Design | Kiệt | ✅ Done |
| #3 | Entity Framework Core Configuration | Kiệt | ✅ Done |
| #4 | Authentication Infrastructure | Quang Trung | ✅ Done |
| #5 | Project Documentation Setup | Văn Quang + Hoàng Long | ✅ Done |

**Deliverables:**
- [x] Solution Structure
- [x] Database Schema
- [x] Initial Migration
- [x] Authentication Setup
- [x] Core Documentation

---

## Milestone 2 — Membership Management 🔄 (In Progress)

**Duration:** Week 2

| Issue | Title | Owner | Status |
|-------|-------|-------|--------|
| #6 | Membership Package CRUD Backend | Quang Trung | ⏳ Pending |
| #7 | Membership Package UI | Phi | ⏳ Pending |
| #8 | Membership Registration Module | Kiệt | ⏳ Pending |
| #9 | Membership Renewal Module | Kiệt | ⏳ Pending |
| #10 | Membership Testing | Hoàng Long | ⏳ Pending |

**Deliverables:**
- [ ] Membership Package Management
- [ ] Membership Registration
- [ ] Membership Renewal
- [ ] Membership Validation

---

## Milestone 3 — Trainer & Booking (Week 3)

| Issue | Title | Owner | Status |
|-------|-------|-------|--------|
| #11 | Trainer CRUD Backend | Quang Trung | ⏳ Pending |
| #12 | Trainer Management UI | Phi | ⏳ Pending |
| #13 | Trainer Schedule Module | Kiệt | ⏳ Pending |
| #14 | Booking Creation Module | Kiệt | ⏳ Pending |
| #15 | Booking Cancellation & History | Kiệt | ⏳ Pending |
| #16 | Booking UI | Phi | ⏳ Pending |
| #17 | Booking Testing | Hoàng Long | ⏳ Pending |

---

## Milestone 4 — Progress & Payment (Week 4)

| Issue | Title | Owner | Status |
|-------|-------|-------|--------|
| #18 | Workout Progress Backend | Kiệt | ⏳ Pending |
| #19 | Workout Progress UI | Phi | ⏳ Pending |
| #20 | VNPay Configuration | Quang Trung | ⏳ Pending |
| #21 | VNPay Payment Processing | Quang Trung | ⏳ Pending |
| #22 | Payment Testing | Hoàng Long | ⏳ Pending |

---

## Milestone 5 — AI Fitness Assistant (Week 5)

| Issue | Title | Owner | Status |
|-------|-------|-------|--------|
| #23 | Knowledge Base Preparation | Văn Quang | ✅ Done |
| #24 | RAG Retrieval Service | Phi | ✅ Done |
| #25 | Gemini API Integration | Phi | ✅ Done |
| #26 | Chat History Module | Quang Trung | ✅ Done |
| #27 | AI Chat Interface | Phi | ✅ Done |
| #28 | AI Testing | Văn Quang + Hoàng Long | ⏳ Pending |

---

## Milestone 6 — Dashboard & Release (Week 6)

| Issue | Title | Owner | Status |
|-------|-------|-------|--------|
| #29 | Dashboard Statistics Backend | Kiệt | ⏳ Pending |
| #30 | Dashboard UI | Phi | ⏳ Pending |
| #31 | System Integration Testing | Hoàng Long | ⏳ Pending |
| #32 | User Manual & Documentation | Văn Quang | ⏳ Pending |
| #33 | Release Preparation | Phi | ⏳ Pending |

---

## Definition of Done

A task is considered completed only when:
- [x] Code compiles successfully
- [x] No critical bugs remain
- [x] Pull Request approved
- [x] Merged into `develop` branch
- [x] Issue checklist completed
- [x] Documentation updated

---

## Notes For AI Agents

### Mandatory Rules
- Follow `ARCHITECTURE.md`
- Follow `AGENTS.md`
- Do not change technology stack
- Do not introduce React, Angular or Flutter
- Do not write SQL in Controllers
- Business Logic belongs to Services
- Each Issue should produce one Pull Request
- Do not modify unrelated modules
- Always update documentation when changing architecture

### Project Stack
- ASP.NET Core MVC 8
- Razor View
- Bootstrap 5
- SQL Server 2022
- Entity Framework Core
- ASP.NET Identity
- VNPay
- Gemini API
- Serilog