# HolidayApp

A .NET 8 Web API for managing and querying public holidays across countries using the [Nager.Date API](https://date.nager.at/Api).

<img width="1712" height="700" alt="image" src="https://github.com/user-attachments/assets/80edca29-d3d8-4ced-8795-88d13375685d" />


## Features

- Load and cache country data from Nager.Date API
- Store public holidays for any country and year in MSSQL database
- Query last celebrated holidays for a country
- Count weekday holidays per country (sorted descending)
- Find common holidays between two countries
- Input validation with FluentValidation
- Resilience with Polly (retry & circuit breaker)
- Health checks endpoint
- OpenAPI/Swagger documentation

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2019+ or SQL Server Express or LocalDB
- (Optional) Visual Studio 2022 or VS Code

## Database Setup

### Option 1: LocalDB (Recommended for Development)

1. **LocalDB comes with Visual Studio** or install via:
   ```bash
   SqlLocalDB.exe create MSSQLLocalDB
   ```

2. **Connection string is pre-configured** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "Database": "Server=localhost\\SQLEXPRESS;Database=HolidayAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"
   }
   ```

3. **Migrations run automatically** when you start the application

### Option 2: SQL Server Instance

1. Update connection string in `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "Database": "Server=YOUR_SERVER;Database=HolidayAppDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true"
   }
   ```

2. Migrations will auto-apply on startup

## Configuration

### Nager.Date API Settings

Configure the external API in `appsettings.json`:

```json
"NagerDateApiConfiguration": {
  "ServiceUrl": "https://date.nager.at/api/v3",
  "ConnectionTimeoutInSeconds": "30"
}
```

### Logging

Adjust logging levels in `appsettings.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

## How to Run

### Using .NET CLI

1. **Clone the repository**
   ```bash
   git clone https://github.com/henrymegwai/HolidayApp.git
   cd HolidayApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   cd src/HolidayApp.Api
   dotnet run
   ```

4. **Access Swagger UI**
   - Open your browser to: `https://localhost:7199` or `http://localhost:5296` or `http://localhost:45058` depending on the project launch profile used.
   - Swagger UI is available at the root URL (`/`)

### Using Visual Studio

1. Open `HolidayApp.sln`
2. Set `HolidayApp.Api` as startup project
3. Press F5 to run
4. Browser will open to Swagger UI

## API Endpoints

### Health Check
- `GET /health` - Application health status

### Countries
- `POST /api/holidays/load-countries` - Load all available countries from Nager.Date API

### Holidays Management
- `POST /api/holidays/load-holidays` - Load holidays for a specific year and country
  ```json
  {
    "year": 2024,
    "countryCode": "US"
  }
  ```

### Queries
- `GET /api/holidays/last-celebrated/{countryCode}` - Get last 3 celebrated holidays
- `GET /api/holidays/weekday-count?year={year}&countryCodes={codes}` - Count weekday holidays (descending order)
- `GET /api/holidays/common?year={year}&countryCode1={code1}&countryCode2={code2}` - Find common holidays


## Example Usage

### 1. Load Countries
```bash
curl -X POST https://localhost:5001/api/holidays/load-countries
```

### 2. Load Holidays for USA in 2024
```bash
curl -X POST https://localhost:5001/api/holidays/load-holidays \
  -H "Content-Type: application/json" \
  -d '{"year": 2024, "countryCode": "US"}'
```

### 3. Get Last Celebrated Holidays for USA
```bash
curl https://localhost:5001/api/holidays/last-celebrated/US
```

### 4. Count Weekday Holidays
```bash
curl "https://localhost:5001/api/holidays/weekday-count?year=2024&countryCodes=US&countryCodes=GB&countryCodes=DE"
```

### 5. Find Common Holidays
```bash
curl "https://localhost:5001/api/holidays/common?year=2024&countryCode1=US&countryCode2=GB"
```



## Architecture

### Clean Architecture Layers

- **HolidayApp.Domain** - Entities and domain logic
- **HolidayApp.Application** - Business logic, CQRS (Commands/Queries), DTOs
- **HolidayApp.Infrastructure** - Data access, external services, EF Core
- **HolidayApp.Api** - Controllers, middleware, API configuration
- **HolidayApp.UnitTests** -  Unit Tests - xUnit, FluentAssertions, NSubstitute

### Design Patterns

- **CQRS** - Command Query Responsibility Segregation with MediatR
- **Decorator Pattern** - Cache Implementation with IMemoryCache leveraging decorators
- **Repository Pattern** - Data access abstraction
- **Mediator Pattern** - Decoupled request/response handling
- **Options Pattern** - Strongly-typed configuration
- **Dependency Injection** - Built-in .NET DI container

### Key Technologies

- **ASP.NET Core 8** - Web API framework
- **Entity Framework Core** - ORM with SQL Server
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Polly** - Resilience and transient fault handling
- **Swagger/OpenAPI** - API documentation
- **xUnit, NSubstitute, FluenAssertion** - Unit Testing Implementation


## Database Schema

### Countries Table
- `Id` (int, PK)
- `CountryCode` (nvarchar(2), unique)
- `Name` (nvarchar(100))

### Holidays Table
- `Id` (int, PK)
- `CountryId` (int, FK)
- `Date` (datetime2)
- `LocalName` (nvarchar(200))
- `GlobalName` (nvarchar(200))
- `IsWeekend` (bit)
- `Year` (int)

**Indexes:**
- `IX_Countries_CountryCode` (unique)
- `IX_Holidays_CountryId_Date`
- `IX_Holidays_Date`
- `IX_Holidays_Year_IsWeekend`

## Performance Optimizations

- **In-Memory Caching** - Countries cached for 24 hours, holidays for 1 hour
- **Bulk Insert** - Efficient batch inserts using EFCore.BulkExtensions
- **Database Indexes** - Optimized for common query patterns
- **AsNoTracking** - Read-only queries for better performance
- **Retry Policy** - Exponential backoff for API failures
- **Circuit Breaker** - Prevents cascading failures

## Error Handling

The API uses a global exception handler that returns standardized error responses:

```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detailed error 1", "Detailed error 2"],
  "traceId": "request-trace-id"
}
```

**HTTP Status Codes:**
- `200 OK` - Success
- `400 Bad Request` - Validation errors
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Unexpected errors

## Development

### Running Migrations Manually

```bash
# Create migration
dotnet ef migrations add MigrationName --project src/HolidayApp.Infrastructure --startup-project src/HolidayApp.Api

# Apply migrations
dotnet ef database update --project src/HolidayApp.Infrastructure --startup-project src/HolidayApp.Api
```

### Building for Production

```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

## CI/CD

The project includes an Azure Pipeline configuration at [azure-pipelines/holiday-app-pipelines.yml](azure-pipelines/holiday-app-pipelines.yml) that automates:
- Building the solution
- Running unit tests
- Building and pushing Docker images to Azure Container Registry


## Troubleshooting

### Database Connection Issues

1. **Connection string errors:**
   - Verify server name in connection string
   - Check SQL Server is running: `services.msc` → SQL Server service



## Acknowledgments

- [Nager.Date](https://date.nager.at) - Public holidays API
- Built with love using .NET 8
