# ARCHITECTURE.md

# Gym Management System Architecture

Version: 1.0

---

# 1. Architecture Overview

Gym Management System được xây dựng theo mô hình:

```text
ASP.NET Core MVC
+
Layered Architecture
+
Repository Pattern
+
Entity Framework Core
```

Mục tiêu:

* Dễ bảo trì
* Dễ mở rộng
* Tách biệt trách nhiệm giữa các tầng
* Thuận tiện cho phát triển nhóm
* Hỗ trợ AI Agent coding

---

# 2. High-Level Architecture

```text
┌──────────────────────────────┐
│         Client Browser       │
└───────────────┬──────────────┘
                │
                ▼
┌──────────────────────────────┐
│ ASP.NET MVC Controllers      │
└───────────────┬──────────────┘
                │
                ▼
┌──────────────────────────────┐
│ Service Layer                │
│ Business Logic               │
└───────────────┬──────────────┘
                │
                ▼
┌──────────────────────────────┐
│ Repository Layer             │
└───────────────┬──────────────┘
                │
                ▼
┌──────────────────────────────┐
│ Entity Framework Core        │
└───────────────┬──────────────┘
                │
                ▼
┌──────────────────────────────┐
│ SQL Server 2022              │
└──────────────────────────────┘
```

---

# 3. System Architecture Layers

---

# 3.1 Presentation Layer

## Technologies

* ASP.NET Core MVC
* Razor View
* Bootstrap 5
* HTML5
* CSS3
* JavaScript

---

## Responsibilities

### User Interface

* Login
* Register
* Dashboard
* Membership Pages
* Booking Pages
* Payment Pages
* AI Chat Interface

### Form Validation

* Client-side validation
* Display error messages
* User interaction

---

## Folder Structure

```text
Views/
├── Account/
├── Membership/
├── Trainer/
├── Booking/
├── Payment/
├── Dashboard/
└── AIChat/
```

---

# 3.2 Controller Layer

Controllers xử lý request từ người dùng.

Controllers không chứa business logic.

---

## Responsibilities

### Receive Request

Ví dụ:

```http
GET /Membership
POST /Booking/Create
```

---

### Validate Request

Ví dụ:

```csharp
ModelState.IsValid
```

---

### Call Services

Ví dụ:

```csharp
_membershipService.Create();
```

---

### Return Response

Ví dụ:

```csharp
View()
RedirectToAction()
Json()
```

---

# Controllers

```text
AccountController
MembershipController
TrainerController
BookingController
PaymentController
DashboardController
AIChatController
```

---

# 3.3 Service Layer

Service Layer chứa toàn bộ nghiệp vụ của hệ thống.

Đây là tầng quan trọng nhất.

---

## Responsibilities

### Membership Logic

* Đăng ký gói tập
* Gia hạn gói tập
* Kiểm tra trạng thái

---

### Booking Logic

* Đặt lịch
* Hủy lịch
* Kiểm tra xung đột lịch

---

### Payment Logic

* Khởi tạo thanh toán
* Xác thực giao dịch
* Cập nhật trạng thái

---

### AI Logic

* Truy vấn Knowledge Base
* Xây dựng Prompt
* Gọi Gemini API
* Lưu lịch sử chat

---

## Services

```text
AccountService
MembershipService
TrainerService
BookingService
PaymentService
WorkoutProgressService
AIService
DashboardService
```

---

# 3.4 Repository Layer

Repository chịu trách nhiệm truy cập dữ liệu.

---

## Responsibilities

### Data Access

* Select
* Insert
* Update
* Delete

---

## Repositories

```text
UserRepository
MembershipRepository
TrainerRepository
BookingRepository
PaymentRepository
WorkoutProgressRepository
ChatHistoryRepository
```

---

## Rules

Repositories MUST NOT:

* Chứa business logic
* Gọi API bên ngoài

---

# 3.5 Data Layer

---

## Entity Framework Core

Sử dụng:

```text
Code First
Migration
LINQ
```

---

## DbContext

```csharp
GymDbContext
```

---

## DbSets

```csharp
Users
MembershipPackages
Memberships
Trainers
Bookings
Payments
WorkoutProgresses
ChatHistories
```

---

# 4. Database Architecture

## Database

```text
GymManagementDB
```

---

## Main Tables

### Users

Lưu thông tin người dùng.

---

### Trainers

Lưu thông tin huấn luyện viên.

---

### MembershipPackages

Lưu danh sách gói tập.

---

### Memberships

Lưu thông tin đăng ký gói tập.

---

### Bookings

Lưu lịch tập.

---

### Payments

Lưu giao dịch thanh toán.

---

### WorkoutProgresses

Lưu tiến độ luyện tập.

---

### ChatHistories

Lưu lịch sử AI Chatbot.

---

# 5. Authentication Architecture

## Technology

```text
ASP.NET Identity
Cookie Authentication
```

---

## Login Flow

```text
User
 ↓
Login Page
 ↓
AccountController
 ↓
Identity Service
 ↓
Cookie Generated
 ↓
Authenticated User
```

---

## Roles

```text
Admin
Trainer
Member
```

---

# 6. AI Chatbot Architecture

## Technology

```text
Gemini API
RAG
Knowledge Base
```

---

## AI Flow

```text
User Question
        ↓
Knowledge Base Search
        ↓
Retrieve Context
        ↓
Prompt Builder
        ↓
Gemini API
        ↓
AI Response
        ↓
Save Chat History
```

---

## Components

### Knowledge Base

Chứa:

* Gym Knowledge
* Exercise Information
* Nutrition Information

---

### Retrieval Service

Tìm kiếm dữ liệu liên quan.

---

### Prompt Builder

Tạo Prompt hoàn chỉnh.

---

### Gemini Service

Gửi yêu cầu tới Gemini API.

---

# 7. Payment Architecture

## Technology

```text
VNPay Sandbox
```

---

## Payment Flow

```text
Member
 ↓
Choose Package
 ↓
Create Payment
 ↓
VNPay Gateway
 ↓
Return Callback
 ↓
Update Payment Status
```

---

## Payment States

```text
Pending
Success
Failed
Cancelled
```

---

# 8. Logging Architecture

## Technology

```text
Serilog
```

---

## Log Events

### Authentication

* Login Success
* Login Failed

### Membership

* Register Package
* Renew Package

### Payment

* Payment Success
* Payment Failed

### Booking

* Booking Created
* Booking Cancelled

### AI

* Gemini Request
* Gemini Error

---

# 9. Validation Architecture

## Client Side

```text
HTML Validation
JavaScript Validation
```

---

## Server Side

```text
Data Annotation
FluentValidation
```

---

# 10. Project Structure

```text
src/

├── Controllers/
├── Services/
├── Repositories/
├── Models/
├── Data/
├── ViewModels/
├── Views/
├── Middleware/
├── Helpers/
├── Validators/
├── Configurations/
└── Logs/
```

---

# 11. Non-Functional Requirements

## Performance

* Page response < 3 seconds
* Dashboard loading < 5 seconds

---

## Security

* Password Hashing
* Role Authorization
* CSRF Protection
* Input Validation

---

## Reliability

* Error Logging
* Exception Handling
* Database Backup

---

# 12. Deployment Architecture

```text
Client Browser
      ↓
IIS Server
      ↓
ASP.NET Core MVC
      ↓
SQL Server
```

---

# 13. Future Enhancements

Không thuộc phạm vi đồ án hiện tại.

Có thể phát triển:

* Mobile Application
* QR Check-In
* AI Recommendation Engine
* Push Notification
* Cloud Deployment
* Multi-Branch Gym Management

---

# 14. Architectural Principles

Mọi thành viên và AI Agent phải tuân thủ:

1. Controller mỏng.
2. Business logic nằm trong Service.
3. Repository chỉ truy cập dữ liệu.
4. Không SQL trong Controller.
5. Không đổi công nghệ khi chưa được phê duyệt.
6. Ưu tiên đơn giản và dễ bảo trì.
7. Tuân thủ AGENTS.md.
8. Tuân thủ SOLID Principles.
9. Mỗi tính năng phải có Issue tương ứng.
10. Mỗi Issue phải có Pull Request tương ứng.
