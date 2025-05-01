# 🩺 Medical Appointment System

A full-stack medical appointment platform built with **.NET 8**, **Angular 17**, **Clean Architecture**, and a strong focus on performance, security, and scalability. It supports multiple roles (Admin, Doctor, Patient, Guest), real-time chat, appointment scheduling, feedback, and more.

---

## 📌 Completed Features

### ✅ Backend (ASP.NET Core 8)
- **Docker Compose** setup with:
  - **SQL Server** as the database
  - **Redis** for future caching & notifications
  - Placeholder for SignalR container
- **CQRS** with MediatR and pipeline behaviors
- **Unit of Work** pattern
- **FluentValidation** integrated with MediatR pipeline behaviors
- **Authentication & Authorization**:
  - JWT Token-based auth
  - Identity package
  - Role-based access control (Admin, Doctor, Patient)
- **CRUD & Query Operations** for:
  - Doctor & Specialization
  - Patient
  - Availability
  - Doctor Qualifications
- **Database Seeding** with roles, default users, and sample data
- **NUnit Unit Testing** with **Moq** for DoctorQualification and Availability services
- **Serilog Logging** to log files
- **Centralized Exception Handling** middleware
- **Rate Limiting** using built-in middleware
- **Database Refactoring** with clean ERD after deep exploration of open source projects and domain logic

### ✅ Frontend (Angular 17)
- Role-based navigation with guards
- CRUD operations and queries using services and interfaces
- **Reactive Forms** with validation
- **JWT Interceptor** for automatic auth headers
- **Auth Guard** for route protection
- **reCAPTCHA** integrated for spam prevention
- UI built with **PrimeNG** and **BehaviorSubject** for state management

---

## 🧠 ERD Summary Table

| Entity             | Relationships & Notes                                                                 |
|--------------------|----------------------------------------------------------------------------------------|
| AspNetUsers        | 🔁 One-to-One with Patient & Doctor (Partial from Users' side)                         |
| AspNetRoles        | 🔁 Many-to-Many with AspNetUsers                                                       |
| Patient            | 🔁 Partial participation with Appointment (nullable PatientId for guests)             |
| Doctor             | 🔁 One-to-Many with Availability, Feedback, Appointment                               |
| Specialization     | 🔁 Many-to-Many with Doctor (via DoctorSpecialization)                                |
| Availability       | 🔁 One-to-Many with AvailabilityException                                             |
| Appointment        | 🔁 Optional Prescription, Optional Chat, ParentAppointment (self-referencing)         |
| Prescription       | 🔁 Must relate to Appointment, contains multiple Medicines                            |
| Chat               | 🔁 Optional for Appointment, contains Messages                                        |
| Message            | 🔁 Belongs to Chat, Sender is a User                                                  |
| Feedback           | 🔁 For Doctor only (via Appointment for validation purposes)                          |

👉 View the full ERD here:
![View ERD](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/ERD.png)

---

## 🛠️ Planned Features

- **SignalR real-time chat** for patient-doctor conversations
- **Notifications** system for appointment updates
- **Redis** integration (already added to docker-compose)
- **Payment Integration** with **PayPal**
- **Integration Testing** for Repository layer
- **CI/CD Pipeline**
- **Admin dashboard** with analytics and management features

---

## 🧪 Technologies & Tools

- **Backend**: .NET 8, Identity, MediatR, FluentValidation, Serilog, EF Core
- **Frontend**: Angular 17, PrimeNG, RxJS, reCAPTCHA
- **Testing**: NUnit, Moq
- **Infrastructure**: Docker Compose, SQL Server, Redis (planned)

---

## 📈 GitHub Contribution Streak Support

This project marks a strong comeback in my GitHub streak — with solid plans, real debugging joy, and a clear MVP roadmap. Let the streak continue! 🔥

---
## Random Screens until complete documentation
**[Rate Limiting]**  
![Rate Limiting](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/Rate%20limiting.png)

**[ValidatorBehavior&Exception Middleware work]**  
![ValidatorBehavior&Exception Middleware work](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/ValidatorBehavior%26Exception%20Middleware%20work.png)
