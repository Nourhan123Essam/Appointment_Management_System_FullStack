# **README - Appointment Management System**

A full-stack healthcare appointment management system built with .NET 8 and Angular 19. It focuses on security, role-based access, structured design, and maintainability.

---

**Temporarily**, the frontend is hosted here until the issue is resolved: [Frontend part of the project](https://github.com/Nourhan123Essam/Temp_Appointment_system_Frontend)

---

## 📌 **Overview**
This project manages **doctor-patient bookings** with:
- **Backend:** .NET 8, Entity Framework Core, Fluent API
- **Frontend:** Angular 19, PrimeNG, Bootstrap
- **Database:** SQL Server
- **Security:** JWT Authentication, Role-based Authorization, CAPTCHA

The goal was to implement **a scalable and secure system**, though some parts are still being finalized.

---

## **Backend Development**

### **Key Features**:
- **Layered Architecture** for scalability and maintainability.
- **JWT Authentication** and **Role-based Authorization** (Admin, Doctor, Patient).
- **Admin Auto-seeding** with credentials in `appsettings.json`.
- **CAPTCHA Authentication** for added security.
- **Database Design & Fluent API Configuration** for data integrity.

### **Challenges**:
- Focused heavily on **security and authentication** before implementing CRUD operations.
- **CAPTCHA integration** was complex and required debugging.

---

## **Frontend Development**

- **Routing & Role-based Guards** to ensure proper access control.
- **CAPTCHA Integration** is still being debugged after accidental loss of work.
- **Basic UI** with Bootstrap and PrimeNG.

### **Key Challenges**:
1. **Routing & Guards** for role-based access.
2. **CAPTCHA Integration** bug after work recovery.
3. **Custom & Built-in Validation** for the Register form.

---

## **Time Investment & Project Decisions**

1. Designed a proper structure instead of rushing code.
2. Researched healthcare appointment business logic to cover realistic scenarios.
3. Focused on security & user role management first, as it's crucial in a professional system.
4. Lost time recovering work after accidental deletion.

---

## **Challenges & Lost Progress**

Unfortunately, due to **fatigue and a mistake, most frontend work was deleted** and had to be redone, which caused delays.  
This also led to **unexpected new errors** that required debugging.  
Despite this, **I managed to recover most of the work**, except for a bug in CAPTCHA that still needs fixing.

---

## **Screenshots**
![Here where CAPTCHA loading correctly](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/login%20with%20captcha.png)

![Login after improved styling](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/admin%20login.png)

![Patient only can register](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/patient%20login.png)

![Register form with built-in and custom validation](https://github.com/Nourhan123Essam/Appointment_Management_System_FullStack/blob/main/Schreens/register%20form.png)
