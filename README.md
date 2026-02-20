

# 🏥 Clinic Management System API

A production-ready **RESTful Web API** built using **ASP.NET Core (.NET 9)** and **Entity Framework Core** to manage clinic operations securely and efficiently.

This system provides a complete backend solution for managing patients, doctors, appointments, invoices, payments, and hospital stays — with strong security and clean architecture principles.

---

## 🚀 Core Features

### 🏥 Clinic Management

* Manage doctors, patients, and hospital stays
* Full appointment management (CRUD)
* ⭐ Doctor reviews and ratings
* 🔎 Pagination & filtering support

### 📅 Online Appointment Booking (Patient Feature)

* Patients can book appointments online
* JWT-protected booking endpoint
* Ownership-based access validation
* Patients can only access their own appointments

### 💰 Financial System

* Invoice creation and management
* Secure payment processing
* PDF invoice export
* Ownership-based invoice protection

### 🔐 Authentication & Authorization

* JWT Authentication
* ASP.NET Identity integration
* Role-Based Authorization (Admin / Patient)
* Resource-Level Ownership validation
* Secure Swagger integration (JWT enabled)

---

## 🏗️ Architecture & Design

This project follows a clean layered architecture:

```
Controllers → Services → Data → Entities
```

### Key Design Patterns Used

* Service Layer Pattern
* DTOs (Data Transfer Objects)
* Result<T> wrapper for consistent API responses
* Fluent API for entity configuration
* Centralized authorization helper
* Dependency Injection

---

## 🧰 Technologies Used

| Category       | Technology                 |
| -------------- | -------------------------- |
| Language       | C# (.NET 9)                |
| Framework      | ASP.NET Core Web API       |
| ORM            | Entity Framework Core      |
| Database       | SQL Server                 |
| Authentication | ASP.NET Identity + JWT     |
| Authorization  | Role-Based + Ownership     |
| Object Mapping | Mapster                    |
| Documentation  | Swagger                    |
| Architecture   | Clean Layered Architecture |

---

## 🔒 Authentication Flow

1️⃣ Register or log in:

```
POST /api/Auth/sign-up
POST /api/Auth/sign-in
```

2️⃣ Receive a JWT token in the response.

3️⃣ Use the token in secured endpoints:

```
Authorization: Bearer {your_token}
```

4️⃣ Access control supports:

* Role-based restrictions (Admin / Patient)
* Ownership validation (users can only access their own resources)

---

## 📅 Online Booking Flow (Patient)

Patients can book appointments directly via the API.

### Booking Process:

1. Patient logs in and receives JWT.
2. Patient calls the booking endpoint:

```
POST /api/appointments/patient/{patientId}/book
```

3. System validates:

   * User is authenticated
   * User role is Patient
   * The patientId belongs to the logged-in user
4. Appointment is created securely.

---

## 📦 Example Endpoints

| Method | Endpoint                                     | Description                |
| ------ | -------------------------------------------- | -------------------------- |
| GET    | `/api/appointments?page=1&pageSize=10`       | Get paginated appointments |
| POST   | `/api/appointments`                          | Create appointment (Admin) |
| POST   | `/api/appointments/patient/{patientId}/book` | Patient books appointment  |
| GET    | `/api/patients/{id}`                         | Get patient details        |
| GET    | `/api/invoices/{id}/pdf`                     | Export invoice as PDF      |
| POST   | `/api/payments/{appointmentId}`              | Create payment             |
| GET    | `/api/doctors`                               | List doctors               |

---

## ⚙️ How to Run Locally

### Prerequisites

* .NET 9 SDK
* SQL Server
* EF Core CLI tools

### Steps

```bash
git clone https://github.com/mohamed68909/ClincManagement.git
cd ClincManagement/ClincManagement.API
dotnet ef database update
dotnet run
```

Open in browser:

```
https://localhost:{port}/swagger
```

---

## 📸 API Preview

### 🔐 Swagger UI (JWT Enabled)

![Swagger UI](assets/swagger-ui-home.png)

### 📅 Appointments Module

![Appointments](assets/swagger-ui-appointment-endpoint.png)

### 👨‍⚕️ Doctors & Reviews

![Doctors](assets/swagger-ui-doctors.png)

### 🧾 Invoices & Payments

![Invoices](assets/swagger-ui-invoices.png)

### 🏥 Patients & Stays

![Patients](assets/swagger-ui-patients.png)

---

## 🧠 What This Project Demonstrates

* Secure backend API development
* JWT-based authentication pipeline
* Role-based & ownership-based authorization
* Clean service-based architecture
* Pagination & filtering implementation
* Real-world backend system design

---

## 🔮 Future Improvements

* Unit & Integration Testing (xUnit + Moq)
* Structured logging with Serilog
* CI/CD pipeline setup
* Cloud deployment (Azure App Service)
* Frontend integration (React or Angular)
* Role expansion (Doctor / Receptionist)

---

## 👨‍💻 Author

**Mohamed Ashraf**
.NET Backend Developer

GitHub:
[https://github.com/mohamed68909](https://github.com/mohamed68909)

---

## 📜 License

MIT License
Developed for educational and portfolio purposes.

---
