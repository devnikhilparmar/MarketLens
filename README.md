# MarketLens

**.NET 10 | C# 14 | Aspire 13 | EF Core 10 | SQL Server | xUnit v3**

MarketLens is built on a Clean Architecture foundation (Domain, Application, Infrastructure, Api) with authentication, caching, observability, and testing wired up from day one — ready for feature work.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Architecture** | Clean Architecture (Domain, Application, Infrastructure, Api) |
| **Runtime** | .NET 10 / C# 14 |
| **API** | Minimal APIs with TypedResults |
| **CQRS** | Manual handlers — zero dependencies, zero licensing risk |
| **Validation** | FluentValidation 12 + Result pattern |
| **Errors** | ProblemDetails (RFC 9457) + global exception handler |
| **Database** | EF Core 10 + SQL Server (manage via SSMS) |
| **Caching** | Microsoft HybridCache (L1 in-memory + L2 Redis) |
| **Auth** | ASP.NET Identity + JWT with refresh tokens |
| **API Docs** | Scalar (modern OpenAPI UI) |
| **Logging** | Serilog 10 structured logging |
| **Observability** | .NET Aspire 13 + OpenTelemetry (traces, metrics, logs) |
| **Testing** | xUnit v3 + FluentAssertions + NSubstitute + NetArchTest |
| **Solution** | `.slnx` format + Central Package Management |

## Architecture

```
┌──────────────────────────────────────────────────┐
│                    Api Layer                      │
│         Endpoints, Program.cs, Scalar            │
└──────────────────┬───────────────────────────────┘
                   │ depends on
┌──────────────────▼───────────────────────────────┐
│              Infrastructure Layer                 │
│     EF Core, Identity, JWT, HybridCache          │
└──────────────────┬───────────────────────────────┘
                   │ depends on
┌──────────────────▼───────────────────────────────┐
│              Application Layer                    │
│      CQRS Handlers, Validators, DTOs             │
└──────────────────┬───────────────────────────────┘
                   │ depends on
┌──────────────────▼───────────────────────────────┐
│                Domain Layer                       │
│      Entities, Result, abstractions (interfaces) │
└──────────────────────────────────────────────────┘
```

**Dependency rule:** Each layer only depends on the layer below it. Domain has zero external dependencies. Architecture tests enforce this at build time.

## Project Structure

```
├── src/
│   ├── Domain/           # MarketLens.Domain          — entities, Result, common types
│   ├── Application/      # MarketLens.Application      — CQRS commands/queries, handlers, validators
│   ├── Infrastructure/   # MarketLens.Infrastructure  — EF Core, Identity, JWT, caching
│   ├── Api/              # MarketLens.Api             — Minimal API endpoints, Scalar, middleware
│   ├── AppHost/          # MarketLens.AppHost         — Aspire orchestration (SQL Server + Redis)
│   └── ServiceDefaults/  # MarketLens.ServiceDefaults — OpenTelemetry, health checks, resilience
├── tests/
│   ├── Architecture.Tests/    # MarketLens.Architecture.Tests    — dependency-rule enforcement
│   └── Application.UnitTests/  # MarketLens.Application.UnitTests  — handler unit tests (scaffolding)
├── Directory.Build.props        # .NET 10, C# latest, nullable enabled
├── Directory.Packages.props     # Central Package Management
├── docker-compose.yml           # SQL Server + Redis (standalone, no Aspire)
└── README.md
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB, Express, or full) — manage it with **SSMS**
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (only if you run with Aspire or docker-compose)

### 1. Configure the database connection

The API reads a connection string named `marketlens-db`. The default in `src/Api/appsettings.json` targets a local SQL Server with Windows auth:

```json
"ConnectionStrings": {
  "marketlens-db": "Server=localhost;Database=MarketLens;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

Adjust `Server` to match your instance as shown in SSMS — e.g. `Server=localhost\\SQLEXPRESS`, `Server=(localdb)\\MSSQLLocalDB`, or a SQL-auth string:
`Server=localhost,1433;Database=MarketLens;User Id=sa;Password=Your_password123;TrustServerCertificate=True`.

### 2. Create the initial EF Core migration

Migrations are provider-specific, so generate them against SQL Server on your machine:

```bash
dotnet tool install --global dotnet-ef   # once, if you don't have it

dotnet ef migrations add InitialCreate \
  --project src/Infrastructure \
  --startup-project src/Api \
  --output-dir Persistence/Migrations
```

The API applies migrations automatically on startup in Development (`AppDbSeeder`), which also seeds roles and an admin user.

### Run with Aspire (recommended)

```bash
cd src/AppHost
dotnet run
```

This starts everything: a **SQL Server** container, **Redis** cache (with RedisInsight), the **API** (auto-migrate + seed), and the **Aspire Dashboard** for OpenTelemetry. Aspire injects its own `marketlens-db` / `marketlens-cache` connection strings, overriding `appsettings.json`.

### Run without Aspire

```bash
# Start SQL Server + Redis
docker compose up -d

# Point marketlens-db at the container (SQL auth) in appsettings, then:
cd src/Api
dotnet run
```

### Explore the API

Navigate to `https://localhost:7200/scalar/v1` for the interactive Scalar API docs.

**Default admin credentials** (seeded automatically in Development):
- Email: `admin@marketlens.local`
- Password: `Admin@123`

### Run Tests

```bash
dotnet test src/MarketLens.slnx
```

## Authentication

The template ships a ready-to-use auth feature under `Application/Features/Identity` and `Api/Endpoints/IdentityEndpoints.cs`:

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/identity/register` | POST | Register a new user |
| `/api/identity/login` | POST | Login, returns a JWT + refresh token |
| `/api/identity/refresh` | POST | Exchange a refresh token for a new access token |

## Adding a New Feature

The **Identity** feature is a working reference for the CQRS + Result pattern. Follow it:

1. **Domain** — Add your entity in `src/Domain/Entities/`.
2. **Application** — Create a feature folder under `src/Application/Features/YourFeature/` with command/query records, handlers, and validators. Expose new tables on `IAppDbContext` if needed.
3. **Infrastructure** — Add the entity `DbSet` to `AppDbContext`, an EF Core configuration in `Persistence/Configurations/`, and a migration.
4. **Api** — Add endpoints in `src/Api/Endpoints/` and register them in `Program.cs`.

## Key Design Decisions

| Decision | Why |
|----------|-----|
| **Manual CQRS** over MediatR | Zero licensing risk. You own the pattern, not a library. |
| **Scalar** over Swagger UI | Modern, faster, better UX. |
| **HybridCache** over IMemoryCache | Built-in stampede protection, L1+L2 cache, automatic serialization. |
| **Result pattern** over exceptions | Explicit error handling, no hidden control flow, better API contracts. |
| **Manual handler registration** | Zero dependencies for DI scanning — assembly reflection is ~40 lines. |
| **.slnx** over .sln | XML-based, merge-friendly, smaller. |

## License

MIT License.
