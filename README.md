# Surfiju

A surf camp management and booking platform. Organizers create and manage surf camps; participants browse, discover, and book spots.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 8 Web API |
| Database | PostgreSQL via Entity Framework Core 9 |
| Architecture | Clean Architecture + CQRS (MediatR) |
| Frontend | Blazor Server + MudBlazor |
| Auth | JWT Bearer tokens + ASP.NET Core Identity |
| Validation | FluentValidation |
| Email | SMTP (Gmail) |

## Features

- **Camps** — Browse, search by location, view details with photos
- **Bookings** — Book camps, view booking history, cancel bookings
- **Auth** — Register, email confirmation, login, refresh tokens, password reset
- **Organizer dashboard** — Create, edit, delete camps with photo uploads
- **Admin panel** — Manage organizer users

## Project Structure

```
DevKnowledgeBase/           # ASP.NET Core Web API
DevKnowledgeBase.Application/   # Commands, Queries, Validators, Services
DevKnowledgeBase.Domain/        # Entities and DTOs
DevKnowledgeBase.Infrastructure/ # EF Core DbContext and Migrations
DevKnowledgeBase.UI/            # Blazor Server frontend
DevKnowledgeBase.Tests/         # Unit tests
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [PostgreSQL](https://www.postgresql.org/) running locally or remotely

## Configuration

The following settings must be provided via environment variables, `dotnet user-secrets`, or `appsettings.Development.json` (never commit real values to `appsettings.json`).

### API (`DevKnowledgeBase/`)

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Secret` | JWT signing secret (min 32 chars, use a strong random value) |
| `Email:Username` | SMTP username (Gmail address) |
| `Email:Password` | SMTP app password |
| `Email:SenderEmail` | From address for outgoing emails |
| `AdminSettings:Email` | Email of the user to promote to Admin on startup |
| `CorsSettings:AllowedOrigins` | Comma-separated list of allowed frontend origins |
| `AppSettings:FrontendBaseUrl` | Base URL of the Blazor frontend (used in email links) |

### UI (`DevKnowledgeBase.UI/`)

| Key | Description |
|---|---|
| `ApiSettings:BaseUrl` | Base URL of the backend API |
| `GoogleMaps:ApiKey` | Google Maps / Places API key |

## Running Locally

### 1. Configure secrets

Create `DevKnowledgeBase/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=DevKnowledgeBaseDb;Username=postgres;Password=yourpassword"
  },
  "Jwt": { "Secret": "your-strong-secret-key-min-32-chars" },
  "Email": {
    "Username": "your@gmail.com",
    "Password": "your-app-password",
    "SenderEmail": "your@gmail.com"
  },
  "AdminSettings": { "Email": "admin@yourdomain.com" }
}
```

Create `DevKnowledgeBase.UI/appsettings.Development.json`:

```json
{
  "GoogleMaps": { "ApiKey": "your-google-maps-api-key" }
}
```

### 2. Apply database migrations

```bash
cd DevKnowledgeBase
dotnet ef database update -p ../DevKnowledgeBase.Infrastructure
```

### 3. Run the API

```bash
dotnet run --project DevKnowledgeBase
# API available at https://localhost:7046
# Swagger UI at https://localhost:7046/swagger
```

### 4. Run the UI

```bash
dotnet run --project DevKnowledgeBase.UI
# UI available at https://localhost:7063
```

## Roles

| Role | Description |
|---|---|
| `Admin` | Full access; manages organizers; assigned on startup to `AdminSettings:Email` |
| `Organizer` | Can create and manage camps |
| `Participant` | Default role; can browse and book camps |

## Running Tests

```bash
dotnet test DevKnowledgeBase.Tests
```
