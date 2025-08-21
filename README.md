# Centralized Logging & Monitoring API

A centralized error logging and monitoring API built with **.NET 9**, Entity Framework Core, and SQL Server.  
This project is designed to serve as a foundation for collecting, storing, and managing error logs from multiple applications.

---

## 🚀 Features (Phase 1)
- .NET 9 Web API project
- Entity Framework Core integration
- SQL Server (Docker container)
- Application & Error Log models with relationships
- API endpoints to manage Applications and Error Logs
- Environment-based configuration (`Development` and `Production`)
- Runs in both **Visual Studio** and **Docker Compose**

---

## Project Structure

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

## 🐳 Running with Docker
1. **Build and run containers:**

   ```bash
   docker-compose up --build
	
2. Stop containers:

	docker-compose down
	
3. Check running containers:

	docker ps -a
	
	
The API will be available at:

HTTP → http://localhost:5000/api

(HTTPS optional, future phase)

🖥️ Running with Visual Studio
Open CentralizedLoggingMonitoring.sln in Visual Studio.

Press F5 or run the project.

By default, it uses Development environment with appsettings.Development.json.

📌 Future Enhancements
Phase 2 – Logging Integration
Add Serilog (file + SQL Server sink).

Middleware for capturing unhandled exceptions.

Structured JSON logging support.

Phase 3 – API Documentation
Add Swagger / Swashbuckle for documentation.

Optionally generate client SDK with NSwag.

📬 Sample API Requests
Create Application
http
Copy
POST /api/Applications
Content-Type: application/json

{
  "name": "Payment Service",
  "environment": "Production"
}
Create Error Log
http
Copy
POST /api/ErrorLogs
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
🤝 Contributing
Future phases will be added in branches (phase-2, phase-3, …).
Main branch will always contain the latest stable version.

📜 License
This project is licensed under the MIT License.


---
