Centralized Logging & Monitoring API – Phase 1

A centralized service for logging application errors. Built with .NET 9 Web API, Entity Framework Core, and SQL Server, this project provides a foundation for capturing, storing, and querying application logs.

Features (Phase 1)

.NET 9 Web API with REST endpoints

Entity Framework Core with SQL Server integration

Database schema for Applications and ErrorLogs

Database seeding with sample data

Dockerized setup (API + SQL Server)

Supports Development and Production environments

Can run both inside Visual Studio (local dev) or via Docker Compose (containerized deployment)

Running the Project
Option 1: Visual Studio (Development)

By default, the project runs with appsettings.Development.json.

Database: Local SQL Server instance (or LocalDB).

Simply press F5 in Visual Studio to run.

Option 2: Docker (Production-like)

Ensure Docker Desktop is running.

Run the containers:

docker-compose up --build


This starts two containers:

centralized_logging_api → .NET 9 API

centralized_logging_db → SQL Server 2022

Access the API at:

HTTP: http://localhost:5000/api/Applications

HTTP: http://localhost:5000/api/ErrorLogs

ℹ️ The API listens on port 8080 inside the container, mapped to 5000 on the host.

Roadmap

Phase 2: Serilog integration (file + database sinks), exception handling middleware, structured JSON logging.

Phase 3: API documentation with Swagger/NSwag.

Phase 4: JWT authentication, role-based access, and monitoring dashboard.