# Eventix - Multi-Tenant Event Management Platform

## Distributed Systems 2025/26 Project

Eventix is a modern multi-tenant event management platform developed as part of the **Distributed Systems 2025/26** course. The platform enables organizations to create and manage events, sell tickets, handle attendees, process bookings and payments, and generate reports while maintaining complete tenant data isolation.

The project follows distributed systems principles and implements a scalable client-server architecture using ASP.NET Core, React, PostgreSQL, Entity Framework Core, JWT Authentication, OpenAI integration, caching, and background jobs.

---

# Table of Contents

* Project Overview
* System Architecture
* Technologies Used
* Distributed Systems Requirements Coverage
* Main Features
* Folder Structure
* Database Architecture
* Multi-Tenancy
* API Documentation
* Installation
* Running the Application
* User Manual
* Testing
* Project Management
* Future Improvements

---

# Project Overview

Eventix provides:

* Event Management
* Venue Management
* Ticket Management
* Booking System
* Payment Processing
* Attendee Tracking
* Staff Management
* Review System
* Coupon System
* Reporting & Analytics
* Audit Logging
* OpenAI Integration
* Background Processing
* Multi-Tenant Support

The platform supports multiple organizations while ensuring that each tenant's data remains completely isolated from other tenants.

---

# System Architecture

```text
┌──────────────────────┐
│   React Frontend     │
└──────────┬───────────┘
           │ HTTPS
           ▼
┌──────────────────────┐
│ ASP.NET Core Web API │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ PostgreSQL Database  │
└──────────────────────┘
```

## Architecture Characteristics

### Client Layer

* React Frontend
* Context API State Management
* Axios HTTP Client
* Role-Based Routing

### API Layer

* ASP.NET Core Web API
* RESTful Endpoints
* Swagger Documentation
* Authentication Middleware
* Logging Middleware

### Data Layer

* PostgreSQL
* Entity Framework Core
* Repository Pattern
* Schema-Based Multi-Tenancy

---

# Technologies Used

## Backend

* ASP.NET Core 8
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* Swagger/OpenAPI
* FluentValidation
* xUnit Testing
* OpenAI API
* Background Services
* Distributed Caching
* Mailtrap
* Expo Go
* React Native / Expo

## Frontend

* React
* Context API
* React Router
* Axios
* CSS

## Tools

* GitHub
* Jira
* Visual Studio
* Visual Studio Code
* Postman

---

# Distributed Systems Requirements Coverage

| Requirement                    | Status    |
| ------------------------------ | --------- |
| Client-Server Architecture     | Completed |
| HTTP/HTTPS Communication       | Completed |
| 20+ REST Endpoints             | Completed |
| .NET REST Framework            | Completed |
| OOP Principles                 | Completed |
| Swagger Documentation          | Completed |
| ORM Integration                | Completed |
| Authentication & Authorization | Completed |
| Middleware Implementation      | Completed |
| React + Context API            | Completed |
| Unit & Integration Testing     | Completed |
| 20+ Models & Migrations        | Completed |
| Project Documentation          | Completed |
| Jira Project Management        | Completed |
| GitHub Collaboration           | Completed |
| OpenAI Integration             | Completed |
| Caching System                 | Completed |
| Background Jobs                | Completed |
| Multi-Tenancy                  | Completed |
| Search & Filtering             | Completed |

---

# Main Features

## Authentication & Authorization

The authentication module secures the entire platform using JWT tokens and role-based access control. Users can register, log in, reset passwords, and access protected resources according to their assigned roles. Authorization policies ensure that only permitted users can perform administrative actions. Middleware validates tokens on every protected request. Multiple roles are supported including Super Admin, Tenant Admin, Staff, and Buyer. Security is enforced throughout both the frontend and backend layers. Forgot password functionality is implemented using secure reset tokens and email delivery through Mailtrap. When a buyer requests a password reset, the system generates a temporary reset token and sends a reset link to the user's email in the Mailtrap sandbox environment. This keeps the reset flow secure and prevents users from changing another user's password without access to their email inbox.

### Included Functionality

* Login
* Register
* Forgot Password
* Reset Password
* JWT Authentication
* Role-Based Authorization

---

## Tenant Management

The tenant management module enables the creation and administration of multiple organizations within the same platform. Each tenant operates independently using its own database schema. Tenant provisioning is automated and creates the required structures during registration. Administrators can activate, suspend, and manage tenants. Tenant-specific branding and settings can be configured separately. This architecture ensures complete data isolation while sharing common infrastructure.

### Included Functionality

* Create Tenant
* Update Tenant
* Activate Tenant
* Suspend Tenant
* Tenant Settings
* Tenant Email Domains

---

## Event Management

The event management system allows organizations to create, update, publish, and monitor events. Event details include schedules, venues, categories, visibility settings, and ticket limits. Organizers can manage the full lifecycle of an event from creation to completion. Search and filtering make event discovery simple. Event information is stored separately for each tenant. This module represents the core business functionality of the platform.

### Included Functionality

* Create Event
* Edit Event
* Publish Event
* Event Categories
* Event Visibility
* Event Status Tracking

---

## Venue Management

Venues represent physical locations where events are hosted. Each venue stores capacity, location information, accessibility options, and configuration details. Organizers can reuse venues across multiple events. Venues support sections that allow more advanced seating arrangements. Proper validation ensures consistency of venue information. This module provides the foundation for event planning and ticket allocation.

### Included Functionality

* Create Venue
* Update Venue
* Delete Venue
* Venue Capacity Management
* Venue Sections
* Venue Search

---

## Ticket Management

The ticketing system handles the creation and management of ticket types for events. Organizers can configure pricing, quantity limits, and availability periods. Ticket inventory is automatically tracked. The system prevents overselling by validating available quantities. Tickets are connected to bookings and attendees. This module forms the basis of event revenue generation.

### Included Functionality

* Ticket Types
* Ticket Pricing
* Ticket Availability
* Inventory Management
* Capacity Validation
* Ticket Reporting

---

## Booking System

The booking module allows customers to reserve and purchase tickets. Booking records maintain relationships between customers, events, payments, and tickets. Availability is validated before confirmation. Background jobs handle booking cleanup when reservations expire. Organizers can monitor booking activity through dashboards. The booking workflow ensures consistency and data integrity.

### Included Functionality

* Create Booking
* Booking History
* Booking Validation
* Reservation Management
* Booking Cleanup Jobs
* Booking Reports

---

## Payment System

The payment module records payment transactions and supports multiple payment methods. Payments are associated with bookings and audit logs. Status tracking enables monitoring of successful and failed transactions. Audit records improve accountability and traceability. Payment information contributes to reporting and analytics. The architecture allows future integration with external payment gateways.

### Included Functionality

* Payment Methods
* Payment Processing
* Payment Tracking
* Payment Statuses
* Audit Logging
* Revenue Reporting

---

## Coupon System

Coupons help organizers run promotions and marketing campaigns. Discount rules can be configured with expiration dates and usage limits. Validation ensures proper application of discounts. Coupon usage is tracked to prevent abuse. Organizers can monitor performance through reports. This module improves customer engagement and ticket sales.

### Included Functionality

* Create Coupons
* Coupon Validation
* Usage Limits
* Expiration Dates
* Discount Management
* Coupon Reporting

---

## Attendee Management

Attendee records provide visibility into event participation. Organizers can search, filter, and review attendees. Attendee information is connected to bookings and tickets. This helps with event operations and customer support. Reports provide attendance insights. Future extensions can include communication and engagement features.

### Included Functionality

* Attendee Listing
* Attendee Search
* Booking Association
* Event Participation
* Attendance Tracking
* Reporting

---

## Staff Management

Staff management enables delegation of responsibilities within tenant organizations. Permissions are controlled through roles and authorization policies. Staff members can access operational features without receiving full administrative privileges. Administrators can update permissions when organizational needs change. This provides flexibility and improves security. Staff activities are tracked through audit logs.

### Included Functionality

* Staff Creation
* Role Assignment
* Permission Control
* Staff Listing
* Staff Updates
* Audit Tracking

---

## Reporting & Analytics

The reporting system provides valuable business insights. Administrators can review event performance, sales trends, revenue statistics, attendee counts, and booking information. Data can be filtered and searched to focus on specific metrics. Reports assist in decision-making and planning. Dashboards provide quick access to key information. Analytics help improve future events.

### Included Functionality

* Sales Reports
* Revenue Reports
* Event Reports
* Attendee Reports
* Booking Reports
* Dashboard Statistics

---

## Audit Logging

Audit logging records critical system actions. Activities such as entity creation, updates, deletions, payments, and authentication events are tracked. Audit logs improve accountability and security. Administrators can investigate issues using historical records. Logs are useful for compliance and monitoring. This module provides transparency across the platform.

### Included Functionality

* Create Logs
* Update Logs
* Delete Logs
* Login Logs
* Payment Logs
* Activity Monitoring

---

## OpenAI Integration

The OpenAI module demonstrates integration with external AI services. The platform can generate text, provide chatbot functionality, and perform AI-assisted operations. Requests and responses are logged for monitoring. AI endpoints are exposed through the REST API. This satisfies the AI integration requirement of the course. The architecture allows future AI enhancements.

### Included Functionality

* AI Requests
* Chatbot Support
* Text Generation
* Request Logging
* API Integration
* AI Analytics

---

### Mobile Ticket Scanner

A separate mobile scanner application was developed using React Native and Expo Go. The mobile app is intended for staff users and allows them to scan ticket QR codes at the event entrance. After scanning a ticket, the app communicates with the backend API to validate the ticket and perform check-in.

*Included Functionality
*Staff Login
*QR Code Scanning
*Ticket Validation
*Ticket Check-In
*Already Used Ticket Detection
*Invalid Ticket Handling
*Mobile API Communication

# Folder Structure

```text
Eventix
│
├── Eventix.Api
│   ├── Controllers
│   ├── Middlewares
│   ├── Extensions
│   └── Program.cs
│
├── Eventix.Application
│   ├── DTOs
│   ├── Interfaces
│   │   ├── Repositories
│   │   └── Services
│   ├── Services
│   └── Validators
│
├── Eventix.Domain
│   ├── Entities
│   ├── Enums
│   └── Common
│
├── Eventix.Infrastructure
│   ├── Persistence
│   │   ├── Database
│   │   ├── Repositories
│   │   └── Migrations
│   ├── Services
│   ├── MultiTenancy
│   └── BackgroundJobs
│
├── Eventix.Tests
│   ├── UnitTests
│   └── IntegrationTests
│
└── Frontend
    ├── src
    │   ├── pages
    │   ├── components
    │   ├── services
    │   ├── contexts
    │   ├── layouts
    │   └── routes
    └── public
```

---

# Database Architecture

## Public Schema

Stores shared platform data:

* Tenants
* Users
* Roles
* UserRoles
* Global Configuration

## Tenant Schemas

Stores isolated tenant data:

* Events
* Venues
* Venue Sections
* Event Sections
* Tickets
* Bookings
* Payments
* Reviews
* Coupons
* Reports
* Audit Logs

---

# Multi-Tenancy

The platform implements schema-based multi-tenancy.

Benefits:

* Complete data isolation
* Better security
* Easier maintenance
* Improved scalability
* Shared infrastructure costs

Tenant resolution is performed using:

```http
X-Tenant-Slug
```

header values.

---

# API Documentation

The backend exposes a RESTful API built with ASP.NET Core Web API. The API is organized into controllers, where each controller is responsible for one main feature of the system. All communication between the frontend and backend is done through HTTP/HTTPS requests using JSON format.

## Example API Endpoints

### Authentication Endpoints

| Method | Endpoint                    | Description                                  |
| ------ | --------------------------- | -------------------------------------------- |
| POST   | `/api/Auth/register`        | Registers a new user                         |
| POST   | `/api/Auth/login`           | Authenticates a user and returns a JWT token |
| POST   | `/api/Auth/forgot-password` | Sends password reset request                 |
| POST   | `/api/Auth/reset-password`  | Resets the user password                     |

### Tenant Endpoints

| Method | Endpoint            | Description                  |
| ------ | ------------------- | ---------------------------- |
| GET    | `/api/Tenants`      | Returns all tenants          |
| GET    | `/api/Tenants/{id}` | Returns tenant details by ID |
| POST   | `/api/Tenants`      | Creates a new tenant         |
| PUT    | `/api/Tenants/{id}` | Updates tenant information   |
| DELETE | `/api/Tenants/{id}` | Deletes or disables a tenant |

### Event Endpoints

| Method | Endpoint           | Description                               |
| ------ | ------------------ | ----------------------------------------- |
| GET    | `/api/Events`      | Returns all events for the current tenant |
| GET    | `/api/Events/{id}` | Returns event details                     |
| POST   | `/api/Events`      | Creates a new event                       |
| PUT    | `/api/Events/{id}` | Updates an existing event                 |
| DELETE | `/api/Events/{id}` | Deletes an event                          |

### Venue Endpoints

| Method | Endpoint          | Description           |
| ------ | ----------------- | --------------------- |
| GET    | `/api/Venue`      | Returns tenant venues |
| GET    | `/api/Venue/{id}` | Returns venue details |
| POST   | `/api/Venue`      | Creates a new venue   |
| PUT    | `/api/Venue/{id}` | Updates a venue       |
| DELETE | `/api/Venue/{id}` | Deletes a venue       |

### Booking Endpoints

| Method | Endpoint             | Description                  |
| ------ | -------------------- | ---------------------------- |
| GET    | `/api/Bookings`      | Returns bookings             |
| GET    | `/api/Bookings/{id}` | Returns booking details      |
| POST   | `/api/Bookings`      | Creates a new booking        |
| PUT    | `/api/Bookings/{id}` | Updates booking status       |
| DELETE | `/api/Bookings/{id}` | Cancels or deletes a booking |

---

# Authentication and Authorization

The system uses **JWT-based authentication** to secure API access. When a user logs in successfully, the backend generates a JWT token that contains user information such as user ID, email, role, and expiration time. This token is returned to the frontend and stored on the client side. For every protected request, the frontend sends the token in the `Authorization` header. The backend validates the token before allowing access to secured endpoints.

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

## Authentication Flow

* User sends login credentials to `/api/Auth/login`.
* Backend validates the email and password.
* If credentials are valid, the backend generates a JWT token.
* The frontend stores the token.
* Future requests include the token in the request header.
* Middleware validates the token before the request reaches the controller.

## Authorization Flow

Authorization is handled using **role-based access control**. Each user has one or more roles, such as `SuperAdmin`, `TenantAdmin`, `Staff`, or `Buyer`. Controllers and endpoints can be protected using authorization attributes. This ensures that only users with the correct role can access specific functionality.

Example:

```csharp
[Authorize(Roles = "SuperAdmin")]
```

This means that only users with the `SuperAdmin` role can access that endpoint.

## Supported Roles

| Role        | Description                                                                    |
| ----------- | ------------------------------------------------------------------------------ |
| SuperAdmin  | Manages tenants, global settings, and platform-level data                      |
| TenantAdmin | Manages events, venues, tickets, staff, and reports for one tenant             |
| Staff       | Helps with operational tasks such as attendees, check-in, and event management |
| Buyer       | Browses events, creates bookings, purchases tickets, and submits reviews       |

---

# Swagger Usage

Swagger is used to document and test the API endpoints directly from the browser. After running the backend project, Swagger can be opened using:

```bash
https://localhost:5001/swagger
```

or:

```bash
http://localhost:5000/swagger
```

depending on the backend launch profile.

## How to Use Swagger

* Run the backend application.
* Open the Swagger URL in the browser.
* Browse the available API controllers.
* Select an endpoint to view request and response details.
* Use the **Try it out** button to test endpoints.
* For protected endpoints, first log in using `/api/Auth/login`.
* Copy the returned JWT token.
* Click the **Authorize** button in Swagger.
* Enter the token in this format:

```text
Bearer YOUR_JWT_TOKEN
```

After authorization, protected endpoints can be tested directly from Swagger.

---

# Installation

## Clone Repository

```bash
git clone https://github.com/ssh-Eventix/Grupi18-SSH-Eventix-Backend.git
```

## Restore Dependencies

```bash
dotnet restore
```

## Apply Database Migrations

```bash
dotnet ef database update
```

## Run Backend

```bash
dotnet run
```

---

# Running Frontend

```bash
npm install
npm run dev
```

Frontend URL:

```text
http://localhost:5173
```

---

# User Manual

## Super Admin

* Create tenants
* Manage tenants
* Monitor audit logs
* View platform statistics
* Manage global settings

## Tenant Admin

* Create venues
* Create event categories
* Manage events
* Configure tickets
* Manage staff
* Review reports

## Staff

* View attendees
* Assist with operations
* Manage assigned tasks
* Support event execution

## Buyer

* Browse events
* Search events
* Purchase tickets
* View bookings
* Submit reviews

---

# Testing

Run all tests:

```bash
dotnet test
```

Testing includes:

* Unit Tests
* Integration Tests
* Repository Tests
* Service Tests
* API Tests

---

# Project Management

Development practices:

* Jira Project Management
* GitHub Projects
* Pull Requests
* Code Reviews
* Branch Strategy
* Continuous Integration

---

# Future Improvements

* QR Code Check-In
* Mobile Application
* Real Payment Gateway Integration
* Email Notification Service
* Advanced AI Recommendations
* Real-Time Analytics Dashboard
* Enhanced Reporting

---

# Conclusion

Eventix is a complete distributed event management platform that demonstrates modern software engineering practices including RESTful APIs, multi-tenancy, authentication, authorization, caching, background jobs, AI integration, testing, and scalable architecture. The project fulfills the requirements of the Distributed Systems 2025/26 course while providing a strong foundation for a production-ready event management solution.
