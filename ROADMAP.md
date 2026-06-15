# Gym Management System - Development Roadmap

> Version: 1.0
> Duration: 6 Weeks
> Team Size: 5 Members
> Development Methodology: Agile Sprint-Based Development

---

# 1. Project Overview

Gym Management System là hệ thống quản lý phòng tập gym được xây dựng nhằm hỗ trợ quản lý hội viên, huấn luyện viên, gói tập, lịch tập, thanh toán trực tuyến và AI Chatbot hỗ trợ luyện tập.

## Core Features

- User Authentication & Authorization
- Membership Management
- Trainer Management
- Booking & Scheduling
- Workout Progress Tracking
- Online Payment (VNPay)
- AI Fitness Chatbot (RAG + Gemini API)
- Dashboard & Reporting

---

# 2. Team Structure

| Member | Role |
|----------|----------|
| Phi | Team Leader, Frontend Lead, AI Lead |
| Quang Trung | Backend Developer |
| Kiệt | Backend Developer |
| Văn Quang | Junior Developer, AI Support |
| Hoàng Long | Junior Developer, Testing & Documentation |

---

# 3. Development Timeline

---

# WEEK 1 - Foundation & Project Setup

## Sprint Goal

Hoàn thành nền tảng dự án và cơ sở dữ liệu.

---

## Main Tasks

### System Architecture

- Finalize software architecture
- Finalize folder structure
- Define coding standards
- Define Git workflow

### Database Design

- ERD
- Database Schema
- Entity Relationships
- Initial Migration

### Project Setup

- ASP.NET Core MVC Solution
- SQL Server Connection
- Entity Framework Core
- ASP.NET Identity
- Bootstrap Integration
- Serilog Configuration

---

## Deliverables

### Documents

- README Draft
- Architecture Draft
- ERD
- Class Diagram

### Source Code

- Initial Project Structure
- Database Migration v1

---

## Team Assignment

### Phi

- Project Structure
- Frontend Base Layout

### Quang Trung

- EF Core Setup
- Database Configuration

### Kiệt

- Database Design

### Văn Quang

- Documentation Support

### Hoàng Long

- Diagram Support

---

# WEEK 2 - Authentication & Membership

## Sprint Goal

Hoàn thành xác thực người dùng và quản lý gói tập.

---

## Features

### Authentication

- Register
- Login
- Logout
- Change Password
- Profile Management

### Authorization

- Member Role
- Trainer Role
- Admin Role

### Membership

- View Packages
- Register Package
- Renew Package
- Membership Status

---

## Deliverables

### Backend

- Identity Integration
- Membership Services

### Frontend

- Login Page
- Register Page
- Membership Pages

---

## Team Assignment

### Phi

- Authentication UI
- Membership UI

### Quang Trung

- ASP.NET Identity
- Membership Logic

### Kiệt

- Membership API & Services

### Văn Quang

- Form Validation
- UI Testing

### Hoàng Long

- Membership Testing

---

# WEEK 3 - Trainer Management & Booking

## Sprint Goal

Hoàn thành quản lý PT và lịch tập.

---

## Features

### Trainer Management

- Trainer Profile
- Trainer CRUD
- Trainer Schedule

### Booking

- Create Booking
- Cancel Booking
- Booking History
- Schedule Management

---

## Deliverables

### Backend

- Booking Service
- Trainer Service

### Frontend

- Booking Interface
- Trainer Pages

---

## Team Assignment

### Phi

- Trainer UI
- Booking UI

### Quang Trung

- Trainer Backend

### Kiệt

- Booking Backend

### Văn Quang

- Trainer Detail Page

### Hoàng Long

- Booking Test Cases

---

# WEEK 4 - Workout Progress & Payment

## Sprint Goal

Hoàn thành theo dõi tiến độ tập luyện và thanh toán.

---

## Features

### Workout Progress

- Weight Tracking
- Height Tracking
- BMI Calculation
- Body Fat Tracking
- Progress History

### Payment

- VNPay Integration
- Payment Status
- Payment History

---

## Deliverables

### Backend

- WorkoutProgress Service
- Payment Service

### Frontend

- Progress Dashboard
- Payment Pages

---

## Team Assignment

### Phi

- Progress UI
- Payment UI

### Quang Trung

- Payment Backend

### Kiệt

- Progress Backend

### Văn Quang

- Form Testing

### Hoàng Long

- Payment Testing

---

# WEEK 5 - AI Chatbot Integration

## Sprint Goal

Triển khai AI Chatbot hỗ trợ tập luyện.

---

## Features

### AI Chatbot

- Chat Interface
- Fitness Advice
- Nutrition Suggestions
- Workout Recommendations

### RAG Pipeline

- Knowledge Base
- Context Retrieval
- Prompt Building
- Gemini API Integration

### Chat History

- Conversation Storage
- Conversation Retrieval

---

## Deliverables

### Backend

- AI Service
- RAG Service

### Frontend

- Chat Interface
- Chat History Page

---

## Team Assignment

### Phi

- Gemini Integration
- Prompt Engineering
- Chat UI

### Văn Quang

- Knowledge Base Collection
- Prompt Testing

### Quang Trung

- Chat History Backend

### Kiệt

- AI Service Integration

### Hoàng Long

- AI Testing

---

# WEEK 6 - Dashboard, Testing & Release

## Sprint Goal

Hoàn thiện hệ thống và chuẩn bị bảo vệ.

---

## Features

### Dashboard

- Total Members
- Active Memberships
- Revenue Statistics
- Booking Statistics

### Testing

- Functional Testing
- Integration Testing
- Bug Fixing

### Release

- Final Documentation
- Deployment Guide
- Demo Preparation

---

## Deliverables

### Documents

- README.md
- Architecture.md
- User Manual
- Installation Guide

### Source Code

- Release Candidate
- Production Build

### Presentation

- Slide Deck
- Demo Script

---

## Team Assignment

### Phi

- Dashboard UI
- Final Review

### Quang Trung

- Statistics Backend

### Kiệt

- Reporting Services

### Văn Quang

- User Guide

### Hoàng Long

- Test Report

---

# 4. Milestone Summary

| Week | Milestone | Status |
|--------|--------|--------|
| Week 1 | Setup & Database | Planned |
| Week 2 | Authentication & Membership | Planned |
| Week 3 | Trainer & Booking | Planned |
| Week 4 | Progress & Payment | Planned |
| Week 5 | AI Chatbot | Planned |
| Week 6 | Dashboard & Release | Planned |

---

# 5. Definition of Done

Một tính năng được xem là hoàn thành khi:

- Source code đã commit lên GitHub.
- Không có lỗi build.
- Đã được review.
- Đã được test thành công.
- Có tài liệu mô tả.
- Có giao diện hoàn chỉnh.
- Không phá vỡ các chức năng hiện có.

---

# 6. Risk Management

## Technical Risks

- AI API quota exceeded.
- VNPay sandbox issues.
- Database migration conflicts.

## Team Risks

- Thành viên chậm tiến độ.
- Merge conflict.
- Thiếu tài liệu.

## Mitigation

- Weekly progress review.
- Pull Request workflow.
- Daily communication.
- Backup database thường xuyên.

---

# 7. Success Criteria

Dự án được xem là thành công khi:

- Hoàn thành toàn bộ Use Cases chính.
- AI Chatbot hoạt động ổn định.
- Thanh toán VNPay hoạt động.
- Dashboard hiển thị chính xác.
- Báo cáo hoàn chỉnh.
- Demo thành công trong buổi bảo vệ.
