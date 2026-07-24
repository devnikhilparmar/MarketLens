# Changelog

All notable changes to MarketLens will be documented in this file.

## [Unreleased]

### Template baseline

- Clean Architecture: Domain, Application, Infrastructure, Api
- .NET 10 / C# 14 with `.slnx` solution format
- Manual CQRS (ICommand, IQuery, handlers) — zero commercial dependencies
- Minimal APIs with TypedResults
- FluentValidation 12 + Result pattern + ProblemDetails (RFC 9457)
- EF Core 10 + SQL Server (manage via SSMS)
- Microsoft HybridCache (L1 in-memory + L2 Redis)
- ASP.NET Identity + JWT authentication with refresh tokens
- Scalar API documentation (modern OpenAPI UI)
- Serilog 10 structured logging
- .NET Aspire 13 + OpenTelemetry (traces, metrics, logs)
- Global exception handler
- Database seeder (roles + admin user)
- Central Package Management
- Docker Compose for standalone usage (SQL Server + Redis)
- Architecture tests (NetArchTest)

### Notes

- Removed the sample Todos feature; the Identity feature is the reference implementation of the CQRS pattern.
- Run `dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Api --output-dir Persistence/Migrations` to generate the SQL Server migration before first run.
