# Job Application System - ASP.NET Core Web API

A Clean Architecture Web API for managing job postings and applications.

 Architecture
- **Domain** — Entities, Enums, Common
- **Application** — Services, DTOs, Interfaces
- **Infrastructure** — EF Core, Repositories, Identity, JWT
- **API** — Controllers, Middleware, Program.cs

 Features
 JWT Authentication (Register / Login)
-  Role-based Authorization (Recruiter / Candidate)
-  Create Job (Recruiter)
-  Close Job (Recruiter — Ownership check)
-  Apply to Job (Candidate)
-  Update Application Status (Recruiter — Forward-only transitions)
-  Cancel Application (Candidate — Ownership check)

 Tech Stack
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core 9
- SQL Server
- ASP.NET Identity
- JWT Bearer Authentication
- Scalar (OpenAPI UI)

Recruiter Acount	recruiter@test.com	Recruiter@123
