# Local Development and API Testing Guide

## Prerequisites

- .NET SDK 10.x
- SQL Server LocalDB, SQL Server Express, or SQL Server
- Node.js 22.x and npm
- Git
- Optional: Postman

## 1. Get the latest code

```powershell
git checkout main
git pull origin main
```

For a feature branch:

```powershell
git checkout feature/fix-tests-swagger-local-dev
git pull origin feature/fix-tests-swagger-local-dev
```

## 2. Configure the local database

The API contains `src/PersonalWealth.Api/appsettings.Development.json` with a LocalDB connection string:

```text
Server=(localdb)\MSSQLLocalDB;Database=PersonalWealthDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

If SQL Server Express or another SQL Server instance is used, replace `ConnectionStrings:Default` with the local connection string for that instance.

Do not put production credentials into source control.

## 3. Restore, build and test the backend

From the repository root:

```powershell
dotnet restore PersonalWealth.sln
dotnet build PersonalWealth.sln
dotnet test PersonalWealth.sln
```

Run only unit tests:

```powershell
dotnet test tests/PersonalWealth.UnitTests/PersonalWealth.UnitTests.csproj
```

Run integration tests:

```powershell
dotnet test tests/PersonalWealth.IntegrationTests/PersonalWealth.IntegrationTests.csproj
```

## 4. Create/update the database

Install the EF CLI if required:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.12
```

If already installed:

```powershell
dotnet tool update --global dotnet-ef --version 10.0.12
```

List migrations:

```powershell
dotnet ef migrations list `
  --project src/PersonalWealth.Infrastructure/PersonalWealth.Infrastructure.csproj `
  --startup-project src/PersonalWealth.Api/PersonalWealth.Api.csproj
```

Apply all source-controlled migrations:

```powershell
dotnet ef database update `
  --project src/PersonalWealth.Infrastructure/PersonalWealth.Infrastructure.csproj `
  --startup-project src/PersonalWealth.Api/PersonalWealth.Api.csproj
```

This creates the database if it does not exist and applies the migrations in order. The repository uses EF Core migrations; there is no requirement for manually created stored procedures for the current API. The migrations create the tables, indexes and foreign keys required by the current model.

To inspect the generated SQL without applying it:

```powershell
dotnet ef migrations script `
  --project src/PersonalWealth.Infrastructure/PersonalWealth.Infrastructure.csproj `
  --startup-project src/PersonalWealth.Api/PersonalWealth.Api.csproj
```

## 5. Run the API

From the repository root:

```powershell
dotnet run --project src/PersonalWealth.Api/PersonalWealth.Api.csproj
```

The configured local development URLs are:

```text
https://localhost:60833
http://localhost:60834
```

Development authentication is intentionally simple. Requests must contain:

```text
X-Tenant-Id: 11111111-1111-1111-1111-111111111111
X-Dev-User: local-dev-user
```

The development authentication handler is active only when `ASPNETCORE_ENVIRONMENT=Development`. Production uses the configured JWT bearer authority/audience.

## 6. Swagger

With the API running in Development, open:

```text
https://localhost:60833/swagger
```

Swagger JSON is available at:

```text
https://localhost:60833/swagger/v1/swagger.json
```

Use Swagger to inspect the `/api/v1` endpoints and execute requests. For local development, send the two development headers shown above. For production, use the Bearer JWT authentication scheme.

## 7. Postman

Create an environment:

```text
baseUrl = https://localhost:60833
tenantId = 11111111-1111-1111-1111-111111111111
devUser = local-dev-user
```

Add these headers to local requests:

```text
X-Tenant-Id: {{tenantId}}
X-Dev-User: {{devUser}}
```

Example requests:

```http
GET {{baseUrl}}/api/v1/wealth/dashboard
```

```http
GET {{baseUrl}}/api/v1/investments/holdings
```

```http
GET {{baseUrl}}/api/v1/banking/accounts
```

For HTTPS certificate warnings in local development, trust the ASP.NET Core development certificate or temporarily disable SSL certificate verification in Postman. Do not disable certificate validation in production.

## 8. Run Angular

From `web/PersonalWealth.Web`:

```powershell
npm install
npm start
```

Angular runs on its normal development-server URL, typically:

```text
http://localhost:4200
```

The Angular development proxy forwards `/api` requests to the local API at `https://localhost:60833`, so CORS configuration is not required for the normal local workflow.

The local Angular API client supplies the development tenant headers automatically. This is a Development-only convenience and must not be treated as production authentication.

Build the Angular application:

```powershell
npm run build
```

## 9. Recommended validation sequence

Run in this order:

1. Start SQL Server/LocalDB.
2. Run `dotnet restore PersonalWealth.sln`.
3. Run `dotnet build PersonalWealth.sln`.
4. Run `dotnet test PersonalWealth.sln`.
5. Run `dotnet ef database update` with the project/startup-project arguments above.
6. Start the API with `dotnet run --project src/PersonalWealth.Api/PersonalWealth.Api.csproj`.
7. Open Swagger and test the dashboard/financial endpoints.
8. Start Angular with `npm start`.
9. Open `http://localhost:4200` and verify the dashboard and investment screen.
10. Use Postman for repeatable API scenarios and negative cases.

## 10. What is currently covered

Backend: Clean Architecture projects, domain/application logic, EF Core persistence, migrations, versioned API endpoints, development authentication boundary, Swagger/OpenAPI documentation and automated unit/integration test projects.

Frontend: Angular standalone application, API client, routing, dashboard, banking, expenses, investments, assets, liabilities and import screens, plus a development API proxy.

CI: GitHub Actions validates .NET restore/build/test and Angular dependency installation/build on pull requests and pushes to `main`.

## 11. What still requires environment-specific setup

- A running local SQL Server/LocalDB instance.
- Any real production JWT authority and audience.
- Real external market-data, notification or AI provider credentials where those integrations are enabled later.
- Production deployment configuration and secrets.

The local development authentication mechanism is not a production identity provider.
