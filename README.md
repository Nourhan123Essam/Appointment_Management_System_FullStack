# 🩺 Medical Appointment System

A full-stack appointment platform built with **.NET 8**, **Angular 17**, and **Clean Architecture** — focused on performance, scalability, and real-world complexity.

> 🚧 **Status**: Work in progress — major flows are functional, with ongoing refactoring and feature extension. Many Dockerized UI features are being reconnected after the latest DB redesign.

---

## Key Highlights

- **Clean Architecture** with:
  - CQRS (MediatR), FluentValidation, Unit of Work, Result Pattern, Repository Pattern
- **Middleware**:
  - Exception handling, session validation, rate limiting, timezone resolver, logging (Serilog), localization
- **Testing**:
  - 51 Unit Tests (NUnit + Moq)
- **Dockerized Environments**:
  - Docker Compose for Redis, SQL Server, Backend, Frontend  
  - Run locally or fully inside containers
- **Localization**:
  - Implemented in both frontend and backend
- **Frontend**:
  - Built with PrimeNG, Angular interceptors, guards, and BehaviorSubject-based state management
- **External Services**:
  - Email (SMTP), Google reCAPTCHA, Redis, Session handling
- **Authentication Flow**:
  - JWT, Refresh Tokens, Forgot/Reset Password (via email + Redis)

---

## 🛠️ Planned Features

- **SignalR real-time chat and Notifications**
- **Payment Integration** with **PayPal**
- **CI/CD Pipeline**
- **Admin dashboard** with analytics and management features

--- 

## 📸 Screenshots

This repo is image-driven — results over words.  
Browse the `Schreens/` folder or explore the commit history to see feature progress.

---

**[Docker Compose]**  
Auto-applies migrations and seeds roles + default admin on rebuild.  
Run with `docker-compose up --build` to ensure latest updates.  
**Admin credentials**: `admin@example.com / Admin@1234`
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Docker%20compose.png)

**[ERD]**  
Current DB structure — subject to future refactoring.  
I’ll keep this updated as the design evolves.
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/ERD.png)

**[Redis]**  
Currently used for refresh tokens, session IDs, and reset password tokens.  
Planned for broader use in caching and performance optimization.
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Redis.png)

**[Unit Tests]**  
51 tests written — growing with each update.  
2 failures after DB refactor highlight how tests catch regressions early and clearly.
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Unit%20tests.png)

**[Localization in Backend]**  
System messages localized using `.resx` files — will expand as features grow.  
DB-stored data will also be localized where needed.
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Localization%20in%20back%20using%20.resx%20files.png)

**[Localization in Frontend]**  
Implemented using JSON files — structured for easy scaling and updates.
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Localization%20in%20front%20using%20json%20files.png)

**[Logs using Serilog]**  
Daily logs generated via `RequestLoggingMiddleware` and exception middleware.  
Helps trace system activity and debug errors effectively.
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/logs%20daily.png)

**[Rate Limiting]**  
Implemented built-in ASP.NET Core rate limiting middleware to control request rates,  
prevent abuse, and improve API reliability. 
![Rate Limiting](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Rate%20limiting.png)

**[Session middleware and localization]**  
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/SessionMiddleware%26Localization.png)

**[Localized Error Handling — Result Pattern vs. Middleware]**  
Errors are consistently structured and support localization.  
Depending on the flow, errors are returned using either the `Result<T>` pattern or centralized exception middleware.

- **Result Pattern**: Used in controlled flows (e.g., login failure).  
  No `Accept-Language` header → defaults to English.

<img src="https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Localization_default_language_en-US%26%20Result%20pattern.png" />

- **Exception Middleware**: Handles uncaught exceptions and validation errors.  
  Messages are localized using the `Accept-Language` header.

<div align="center">
  <img src=https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/FluentValidation%26ExceptionMiddleWare%26%26Localization.png" />
  <img src="https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/FluentValidation%26ExceptionMiddleWare%26%26Localization_accepted-language%20header.png" />
</div>


**[Register Form with validation messages and localization]**  
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/RegisterForm%26localization%26validationMessages.png)

**[Login, Session Middleware & Localization Sync]**  
Frontend and backend are fully synchronized for language preference and session handling.  
Below: Two browsers show how language changes are reflected across layers.

![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/sessionMiddlware%26localization%26langaueSameFrontAndBack.png)

![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/sessionMiddlware%26localization%26langaueSameFrontAndBack(2).png)


**[Forget password]**  
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/ForegetPssword.png)

**[Reset Password — Language from Header]**  
Email content is localized based on the `Accept-Language` header.  
The example below shows reset password emails sent in Arabic and English.

![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/ResetEmail%26localization_ar.png)

![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/ResetEmail%26localization_en.png)

**[Change Password]**  
![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/resetPassword.png)

**[API Coverage in Swagger — In Progress]**  
APIs are documented and tested through Swagger.  
Below are a few completed modules — more endpoints will be added soon as development continues.

![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/API%20Authentication.png)

![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/API%20Doctor.png)

![](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/API%20Patient.png)

