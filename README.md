# 🏥 CurexMind - Hospital & Clinic Management System

<div align="center">

![CurexMind Banner](CurexMind.Frontend/src/assets/curexmind_logo.png)

[![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?style=for-the-badge&logo=react&logoColor=black)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.7-3178C6?style=for-the-badge&logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite-6.0-646CFF?style=for-the-badge&logo=vite&logoColor=white)](https://vitejs.dev/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-4.0-06B6D4?style=for-the-badge&logo=tailwindcss&logoColor=white)](https://tailwindcss.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

**An End-to-End Enterprise Hospital & Clinic Management Platform**  
*Built with ASP.NET Core 9 Web API backend & React 19 + TypeScript modern frontend.*

[Live Backend API](http://curexmind1.runasp.net/) • [Swagger Docs](http://curexmind1.runasp.net/swagger) • [Features](#-core-features) • [Getting Started](#-getting-started)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Live Demo & Deployment](#-live-demo--deployment)
- [Core Features](#-core-features)
  - [🌐 Patient Portal](#-patient-portal)
  - [🏢 Management & Receptionist Portal](#-management--receptionist-portal)
  - [🔐 Security & Role-Based Access Control](#-security--role-based-access-control)
- [Technology Stack](#-technology-stack)
- [Test Accounts](#-test-accounts)
- [System Architecture](#-system-architecture)
- [API Endpoints Summary](#-api-endpoints-summary)
- [Getting Started](#-getting-started)
  - [Backend API Setup](#1-backend-api-setup)
  - [Frontend Setup](#2-frontend-setup)
- [License & Author](#-license--author)

---

## 🌟 Overview

**CurexMind** is a complete, production-ready Hospital & Clinic Management System designed to streamline healthcare operations. It supports patient appointment bookings, doctor schedule management, financial invoicing, payment gateways (Instapay, E-Wallets, Cards), inpatient stay tracking, and comprehensive analytics.

The application features full **Bilingual Support (Arabic RTL / English LTR)**, dynamic gender-aware doctor avatars, dark mode, responsive web design, and ownership-based access control.

---

## 🌐 Live Demo & Deployment

| Service | Environment | URL |
| :--- | :--- | :--- |
| **Backend Web API** | Production | [http://curexmind1.runasp.net/](http://curexmind1.runasp.net/) |
| **Swagger Interactive Docs** | Production | [http://curexmind1.runasp.net/swagger](http://curexmind1.runasp.net/swagger) |
| **Frontend Dev Server** | Local | `http://localhost:5173/` |
| **Backend Local API** | Local | `https://localhost:7113/` |

---

## 🚀 Core Features

### 🌐 Patient Portal (`/patient`)
- **Doctor Discovery**: Interactive doctor catalog with advanced filtering by specialty, rating, gender, visit type (In-person / Online), and availability.
- **Smart Booking Stepper**: 4-step booking process (`Select Doctor` → `Choose Date & Time` → `Payment Method` → `Summary`).
- **Doctor Profiles**: Rich doctor biography, medical experience, language fluencies, patient reviews, star ratings, and consultation pricing.
- **Payment Gateway**: Integrated support for Instapay, Mobile E-Wallets (Vodafone Cash/Etisalat Cash), and Credit/Debit Cards.
- **My Appointments Management**: Filter appointments by status (`Upcoming`, `Past`, `Cancelled`), view receipt breakdowns, and cancel appointments with persistent cache sync.
- **Doctor Ratings & Reviews**: Interactive 5-star rating system with optional public feedback comments.

### 🏢 Management & Receptionist Portal (`/management`)
- **Dashboard Analytics**: Real-time KPI cards for Total Patients, Today's Appointments, Unpaid Invoices, and New Patients.
- **Patient Management**: Full CRUD operations for patient records, social status, medical history, contact info, and national IDs.
- **Appointments Control**: Filter and manage hospital-wide appointments by clinic, doctor, date range, and status.
- **Invoices & Billing**: Automatic invoice generation, payment status tracking (Paid, Partial, Unpaid), and PDF/Excel export.
- **Inpatient Stays & Admissions**: Manage hospital room allocations, admission dates, stay types, and daily care activity logs.

### 🔐 Security & Role-Based Access Control
- **Authentication**: Secure JWT Bearer token authentication with ASP.NET Identity.
- **Role-Based Authorization**: Distinct access permissions for `Admin`, `Receptionist`, `Doctor`, and `Patient`.
- **Resource Ownership Validation**: Patients are restricted to viewing and managing only their personal appointments and invoices.

---

## 🧰 Technology Stack

### Backend Web API (`CurexMind.API`)
* **Framework**: ASP.NET Core Web API (.NET 9.0)
* **Database & ORM**: Entity Framework Core 9.0 with SQL Server
* **Authentication**: ASP.NET Identity + JWT (JSON Web Tokens)
* **DTO Mapping**: Mapster
* **API Documentation**: Swagger / OpenAPI with JWT Authorization button

### Frontend Web SPA (`CurexMind.Frontend`)
* **Framework**: React 19 + TypeScript + Vite 6
* **Styling**: Tailwind CSS 4 + FontAwesome 6 + Google Fonts (Cairo & Tajawal)
* **State & Store**: Zustand + LocalStorage Persistence
* **HTTP Client**: Axios with global JWT request interceptor & error handling
* **Alerts & Modals**: SweetAlert2 + Lucide React Icons

---

## 🔑 Test Accounts

You can test the system using the following seeded credentials:

| Role | Email | Password | Access Scope |
| :--- | :--- | :--- | :--- |
| **Admin** | `Admin@mohamed.com` | `Admin@123456` | Full system access & management |
| **Doctor** | `doctor_0ad98dc6@curexmind.com` | `Doctor@123456` | Doctor profile & appointments |
| **Patient** | `patient_test@curexmind.com` | `Patient@123456` | Patient portal, booking & payments |

---

## 🏗️ System Architecture

```text
CurexMind/
├── CurexMind.API/             # ASP.NET Core Web API (.NET 9)
│   ├── Controllers/           # REST API Endpoints (Auth, Doctors, Patients, Appointments, Invoices, Stays)
│   ├── Entities/              # Domain Models (EF Core Entities)
│   ├── Services/              # Business Logic & Service Layer
│   ├── Validations/           # Fluent Validation Rules
│   └── appsettings.json       # App Configuration & JWT Settings
│
├── CurexMind.Frontend/        # React 19 + TypeScript + Vite SPA
│   ├── src/
│   │   ├── api/               # Axios Instance & Interceptors
│   │   ├── components/        # Reusable UI Components & Guards
│   │   ├── layouts/           # Patient & Management Layouts
│   │   ├── pages/             # App Screens (Home, Profile, Booking, Dashboard, Invoices, etc.)
│   │   ├── store/             # Zustand Global State (Auth, Settings, i18n)
│   │   └── utils/             # Helper Functions (Gender Detection, Avatars)
│
└── CurexMind.sln              # Visual Studio Solution File
```

---

## 📦 API Endpoints Summary

| Method | Endpoint | Authorization | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/sign-in` | Public | Authenticate user & return JWT token |
| `POST` | `/api/Auth/sign-up` | Public | Register new patient account |
| `GET` | `/api/Doctors` | Public | Fetch paginated list of doctors with filters |
| `GET` | `/api/Doctors/{id}` | Public | Get detailed doctor profile |
| `POST` | `/api/Appointments/patient/{patientId}/book` | Patient / Admin | Book a new appointment |
| `GET` | `/api/Appointments/patient/{patientId}` | Patient / Admin | Get patient appointments |
| `DELETE` | `/api/Appointments/{id}` | Patient / Admin | Cancel appointment |
| `POST` | `/api/Payments/{appointmentId}` | Patient / Admin | Process payment for appointment |
| `GET` | `/api/Patients` | Admin / Receptionist | List all registered patients |
| `GET` | `/api/Invoices` | Admin / Receptionist | List all hospital invoices |
| `GET` | `/api/Stays` | Admin / Receptionist | List inpatient stays & room assignments |

---

## ⚙️ Getting Started

### 1. Backend API Setup

```bash
# Clone the repository
git clone https://github.com/mohamed68909/CurexMind.git
cd CurexMind/CurexMind.API

# Update database (EF Core Migrations)
dotnet ef database update

# Run the backend API server
dotnet run --urls "https://localhost:7113;http://localhost:5252"
```
The Swagger UI will be available at: `https://localhost:7113/swagger`

### 2. Frontend Setup

```bash
# Navigate to Frontend directory
cd ../CurexMind.Frontend

# Install dependencies
npm install

# Start the Vite development server
npm run dev
```
The Web App will be available at: `http://localhost:5173/`

---

## 👨‍💻 Author

**Mohamed Ashraf**  
*.NET Backend & Full-Stack Software Engineer*  

- **GitHub**: [mohamed68909](https://github.com/mohamed68909)
- **Project Repository**: [mohamed68909/CurexMind](https://github.com/mohamed68909/CurexMind)

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).
