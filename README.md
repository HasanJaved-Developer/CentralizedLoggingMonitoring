# Centralized Logging & Monitoring API

A centralized error logging and monitoring API built with **.NET 9**, Entity Framework Core, and SQL Server.  
This project is designed to serve as a foundation for collecting, storing, and managing error logs from multiple applications.

This project implements **Phase 1–3** (completed) and outlines future **Phase 4–6** enhancements.  

---

## 📌 Project Phases

### ✅ Phase 1: Core API
- Create centralized logging API
- Store logs in SQL Server
- Basic Serilog integration
- Swagger/OpenAPI setup

### ✅ Phase 2: Containerization
- Add `Dockerfile`
- Add `docker-compose.yml` for API + SQL Server
- Persistent volumes for database

### ✅ Phase 3: Logging Enhancements
- Structured logging with Serilog sinks
- Support for File output
- Correlation ID, environment, and service enrichment

---

## 📂 Project Structure

```bash

CentralizedLoggingMonitoring/
├── CentralizedLoggingApi/ # Core API project
│ ├── Controllers/ # API controllers
│ ├── Models/ # EF Core models
│ ├── Data/ # DbContext
│ ├── Program.cs
│ ├── Startup.cs (if any)
│ └── appsettings.json
├── docker-compose.yml # Docker Compose setup
├── README.md # Documentation
└── .gitignore

```

---

## ⚙️ Environments
The project supports multiple environments:

- **Development**
  - Connection string uses local SQL Server (LocalDB).
  - Loaded from `appsettings.Development.json`.

- **Production**
  - Connection string points to Docker SQL Server.
  - Loaded from `appsettings.Production.json`.
  - Environment set via:
    ```bash
    ASPNETCORE_ENVIRONMENT=Production
    ```

---

## 🖥️ Running with Visual Studio (Development)
Open CentralizedLoggingMonitoring.sln in Visual Studio.

Press F5 or run the project.

By default, it uses Development environment with appsettings.Development.json.

---

## 📦 Running with Docker (Production)

- **Build and start the containers**
  - docker-compose up --build
  
- **Stop the containers**
  - docker-compose down


The API will be available at:

HTTP → http://localhost:5000/api

(HTTPS optional, future phase)


---

## 📈 Future Enhancements

### 🔒 Phase 4: User Management API
- Authentication and Authorization using JWT
- Role-based access for viewing logs
- Claims-based authorization

### 🐛 Phase 5: Try-Catch Integration
- Using the centralized logging API in other apps  
- Capture and post exceptions automatically from services

### 🌐 Phase 6: Web Dashboard
- Lightweight web app with login  
- Display logs from DB  
- Search & filter logs by date, service, environment, severity
  
---

## 📬 Sample API Requests
Create Application

```bash

http POST /api/Applications
Content-Type: application/json

{
  "name": "Payment Service",
  "environment": "Production"
}

```

Create Error Log

```bash

http POST /api/ErrorLogs
Content-Type: application/json

{
  "applicationId": 1,
  "severity": "Error",
  "message": "Null reference exception in payment processing",
  "stackTrace": "at PaymentService.Process()...",
  "source": "PaymentService",
  "userId": "user123",
  "requestId": "req-456"
}

```

---

## 🤝 Contributing

Future phases will be added in branches (phase-2, phase-3, …).
Main branch will always contain the latest stable version.

---

## 📂 Repository Roadmap

```bash

phase-1-core-api → Completed Phase 1.
phase-2-logging → Planned logging integration.
phase-3-swagger → Planned API documentation.
main → Always up to date with the latest stable phase.

```

---

## 📜 License
This project is licensed under the MIT License.


---
