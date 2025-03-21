# ** README - Appointment Management System**
A full-stack healthcare appointment management system built with .NET 8 and Angular 19. The project focuses on security, role-based access, structured design, and maintainability.

---
**Temporarly** the frontend will be here until completely resolve the issue [Frontend part of the project](https://github.com/Nourhan123Essam/Temp_Appointment_system_Frontend);
---
## 📌 Overview
This project is a **full-stack appointment management system** for handling **doctor-patient bookings** with authentication, role-based access, and CAPTCHA verification for security.  
It is built with:  
- **Backend:**  .NET 8, Entity Framework Core, Fluent API  
- **Frontend:** Angular 19, PrimeNG, Bootstrap  
- **Database:** SQL Server
- **Security**: JWT Authentication, Role-based Authorization, CAPTCHA

The focus was on implementing **a structured, scalable, and secure system**, even if some parts are not yet finalized.  

---

## **🛠 Backend Development**
**Layered architecture** Layered architecture for scalability & maintainability & clean architecture.  
**Authentication & Role-based Authorization**:
  - Implemented JWT authentication.
  - Three roles: Admin, Doctor, and Patient (Auto-seeded).
  - Ensured roles are re-added if accidentally deleted to prevent errors.
**Admin account auto-seeding**:
  - Admin credentials are stored in appsettings.json for easy modification upon deployment (security measure to avoid hardcoded credentials).
  - The default admin is always ensured in the database to allow system access.
**CAPTCHA authentication** to enhance security.  
**Database design & API structure** for CRUD operations.  
**Fluent API Configuration** Defined relationships between tables to maintain data integrity.
  

**🔹 Why This Took Time:**  
- Designed a clean, expandable structure instead of rushing features.
- Carefully handled role persistence & security to avoid admin lockout.  
- **CAPTCHA integration was complex** and required debugging.  
- Focused on **security & proper authentication flow** before moving to CRUD operations.  

---

## **🎨 Frontend Development**
**Routes & Role-based Authorization Guards** (Ensuring proper access).  
**Structured frontend with planned component hierarchy** for maintainability.  
**CAPTCHA Integration (Bug currently being debugged)**:
  - Worked successfully before but now fails to load due to work loss & recent recovery.  
**Basic UI setup with Bootstrap & PrimeNG.**  

🔹 **Most challenging parts were:**  
1. **Routing & Guards** → Ensuring correct role-based access.  
2. **CAPTCHA Integration** → Frontend was loading but had a bug after GitHub issues.
3. **Implemented custome & built-in validation** → for the Register form.
4. **Planned & Partially Implemented:**  
   - **Doctor Caching Strategy for Efficiency**:
     - Used BehaviorSubject to store doctor data in-memory.
     - **Why?** The doctor list is relatively small but frequently used (search, filtering, dropdowns), making in-memory caching efficient.
   - **Add-Appointment Page**:
     - Dropdowns for selecting doctors & specializations.
     - Searchable input field (Filters by specialization & name).

🔹 **Why Sorting & Filtering Were Lower Priority?**
Sorting & filtering are simple with PrimeNG’s built-in tools, so they were given lower priority in favor of more complex backend logic & security features.

---

## **⏳ What’s Left (Would Take Just a Few Hours)**
- CRUD operations for **Appointments, Doctors, and Patients** back and front.  
- Fixing **CAPTCHA issue** (Debugging required).  
- Implementing Sorting & Filtering with PrimeNG (Minimal effort required)

These tasks are **straightforward** because:  
✔️ **Most of the complex logic is already implemented.**  
✔️ **PrimeNG provides built-in UI tools for tables & filtering.**  
✔️ **Authorization & API structure is already working.**  

---

## **Time Investment & Project Decisions**
1- Designed a proper structure instead of rushing code.
2- Researched healthcare appointment business logic to cover realistic scenarios.
3- Focused on security & user role management first, as it's crucial in a professional system.
4- Lost time recovering work after accidental deletion.
---

## **⚠️ Challenges & Lost Progress**
Unfortunately, due to **fatigue and a mistake, most frontend work was deleted** and had to be redone, which caused delays.  
This also led to **unexpected new errors** that required debugging.  
Despite this, **I managed to recover most of the work**, except for a bug in CAPTCHA that still needs fixing.  

---

## **Schreens**
![Here where Cpathca loading correctly](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/login%20with%20captcha.png)

![Login after improve styling](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/admin%20login.png)

![Patient only can register](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/patient%20login.png)

![Register form with built-in and custom validation](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/register%20form.png)

