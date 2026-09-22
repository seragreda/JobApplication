# Job Application API

ASP.NET Core Web API for managing job postings and applications.
Built with Clean Architecture, CQRS (MediatR), JWT, and EF Core.

---

## Architecture

- **Domain** — Entities, Enums
- **Application** — Features (CQRS), DTOs, Interfaces, Behaviors
- **Infrastructure** — EF Core, Identity, Repositories, UnitOfWork
- **API** — Controllers, Middleware, Program.cs

---

## Features

- Register / Login with JWT (Roles: Recruiter, Candidate)
- Create Job (Recruiter)
- Close Job (Recruiter — ownership check)
- Apply to Job (Candidate)
- Update Application Status (Recruiter — forward-only)
- Cancel Application (Candidate — ownership check)

---

## Tech Stack

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core 9
- SQL Server
- ASP.NET Identity + JWT
- MediatR (CQRS)
- FluentValidation
- Scalar (OpenAPI UI)

---

## What Changed — CQRS Migration

All features were moved from **Services** to **CQRS Handlers**.

### Before
```
Controller → JobService / ApplicationService / AuthService → UnitOfWork → DB
```

### After
```
Controller → IMediator → Command/Query Handler → UnitOfWork → DB
```

### Removed
- `JobApplication.Application/Services/` folder (all services)

### Added
- `JobApplication.Application/Features/` (Commands + Queries)
- `JobApplication.Application/Common/Behaviors/`
  - `ValidationBehavior.cs`
  - `LoggingBehavior.cs`
- `JobApplication.Application/AssemblyReference.cs`

### New Packages
```xml
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="FluentValidation" Version="11.10.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.10.0" />
```

### Program.cs
```csharp
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
```

---

## Folder Structure (Application Layer)

```
JobApplication.Application/
├── AssemblyReference.cs
├── Common/
│   ├── Interfaces/
│   └── Behaviors/
├── DTOs/
└── Features/
    ├── Auth/Commands/
    │   ├── Register/
    │   ├── RegisterRecruiter/
    │   └── Login/
    ├── Jobs/
    │   ├── Commands/
    │   │   ├── CreateJob/
    │   │   └── CloseJob/
    │   └── Queries/
    │       ├── GetJobById/
    │       └── GetAllOpenJobs/
    └── JobCandidateApplications/
        ├── Commands/
        │   ├── ApplyToJob/
        │   ├── UpdateApplicationStatus/
        │   └── CancelApplication/
        └── Queries/
            └── GetApplicationsByJob/
```

## Default Accounts

| Role | Email | Password |
|------|-------|----------|
| Recruiter | recruiter@test.com | Recruiter@123 |

Register a Candidate via `POST /api/auth/register`.

---

## Endpoints

| Method | Endpoint | Role |
|--------|----------|------|
| POST | /api/auth/register | Anonymous |
| POST | /api/auth/register-recruiter | Anonymous + Code |
| POST | /api/auth/login | Anonymous |
| GET | /api/jobs | Anonymous |
| GET | /api/jobs/{id} | Anonymous |
| POST | /api/jobs | Recruiter |
| PUT | /api/jobs/{id}/close | Recruiter (Owner) |
| POST | /api/applications | Candidate |
| PUT | /api/applications/{id}/status | Recruiter (Owner) |
| DELETE | /api/applications/{id} | Candidate (Owner) |
| GET | /api/applications/job/{jobId} | Recruiter |

---

## Status Enum

| Value | Name |
|-------|------|
| 1 | Applied |
| 2 | UnderReview |
| 3 | Interview |
| 4 | Accepted |
| 5 | Rejected |
| 6 | Cancelled |

Forward-only transitions only. Cancel allowed from `Applied` or `UnderReview`.

