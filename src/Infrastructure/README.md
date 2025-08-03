# BaseTemplate Infrastructure

This is the Infrastructure layer of the BaseTemplate project, providing data access, identity management, and external service integrations for the application.

## Overview

The Infrastructure project is built on .NET 8.0 and serves as the data access and external service layer for the BaseTemplate application. It implements the repository pattern and provides abstractions for database operations, identity management, and CSV processing.

## Project Structure

```
Infrastructure/
├── Data/                          # Data access layer
│   ├── DatabaseInitializer.cs     # Database migration and initialization
│   ├── NameResolver.cs           # Dapper column/table name resolution
│   ├── SqlConnectionFactory.cs   # Database connection factories
│   ├── UnitOfWork.cs            # Unit of work pattern implementation
│   └── Scripts/                 # Database migration scripts
│       ├── 000_master.psql      # Master schema setup
│       ├── 001_app_user.psql    # User table creation
│       ├── 002_user_role.psql   # User roles table
│       ├── 003_tenant.psql      # Tenant management
│       ├── 006_item.psql        # Generic items
│       ├── 008_migrate_user_tenant.psql  # User-tenant migration
│       └── 009_populate_tenant_owner_roles.psql  # Role population
├── Identity/                     # Identity and authorization
│   └── IdentityService.cs       # User authentication and authorization
├── Services/                     # External service integrations
│   └── CsvService.cs           # CSV processing service
├── DependencyInjection.cs       # DI container configuration
├── GlobalUsings.cs             # Global using statements
└── Infrastructure.csproj       # Project file
```

## Key Components

### Data Access Layer

- **DatabaseInitializer**: Handles database migrations using SQL scripts
- **SqlConnectionFactory**: Provides database connections (supports both SQL Server and PostgreSQL)
- **UnitOfWork**: Implements the Unit of Work pattern for transaction management
- **NameResolver**: Handles snake_case to PascalCase conversion for Dapper

### Identity Management

- **IdentityService**: Provides user authentication, authorization, and role-based access control
- Supports JWT Bearer token authentication
- Implements tenant-based access control
- Role-based authorization with policy support

### External Services

- **CsvService**: Handles CSV file generation and processing using CsvHelper

## Dependencies

### NuGet Packages

- **CsvHelper** (33.0.1): CSV processing
- **Dapper** (2.1.66): Micro ORM for data access
- **Dapper.SimpleCRUD** (2.3.0): Simplified CRUD operations
- **Microsoft.AspNetCore.Authorization** (8.0.17): Authorization framework
- **Microsoft.Data.SqlClient** (6.0.1): SQL Server connectivity
- **Microsoft.Extensions.Configuration.Abstractions** (8.0.0): Configuration management
- **Microsoft.Extensions.Hosting.Abstractions** (8.0.1): Hosting abstractions
- **Npgsql** (9.0.3): PostgreSQL connectivity

### Project References

- **Application**: References the Application layer for interfaces and domain models

## Database Support

The infrastructure supports multiple database providers:

- **PostgreSQL**: Primary database (using Npgsql)
- **SQL Server**: Alternative database (using SqlClient)

### Database Migration

The project includes a comprehensive migration system:

1. **Migration History**: Tracks executed scripts in `migration_history` table
2. **Ordered Execution**: Scripts are executed in numerical order (000*, 001*, etc.)
3. **Idempotent**: Safe to run multiple times - already executed scripts are skipped
4. **Error Handling**: Stops execution on first error with detailed logging

### Database Schema

The migration scripts create the following schema:

- **app_user**: User management with SSO integration
- **user_role**: Role-based access control
- **tenant**: Multi-tenancy support
- **todo_list**: Todo list functionality
- **todo_item**: Todo items with list relationships
- **item**: Generic item management

## Configuration

### Connection String

The infrastructure expects a connection string named "DefaultConnection" in the configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=BaseTemplate;Username=postgres;Password=password"
  }
}
```

### Dependency Injection

The infrastructure provides extension methods for easy DI registration:

```csharp
services.AddInfrastructure(configuration);
```

This registers:

- Database connection factories
- Unit of work factory
- Identity service
- Database initializer
- CSV service

## Usage Examples

### Database Operations

```csharp
// Using Unit of Work
using var uow = _unitOfWorkFactory.Create();
var users = await uow.QueryAsync<User>("SELECT * FROM app_user");
```

### Identity and Authorization

```csharp
// Check if user is in role
var isAdmin = await _identityService.IsInRoleAsync("Admin");

// Authorize with policy
var canEdit = await _identityService.AuthorizeAsync("EditPolicy");

// Check tenant access
var hasAccess = await _identityService.HasTenantAccessAsync(tenantId);
```

### CSV Processing

```csharp
// Generate CSV from data
var csvData = await _csvService.WriteToCsvAsync(
    items,
    new[] { "Name", "Email", "Role" },
    item => new[] { item.Name, item.Email, item.Role }
);
```

## Development

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL or SQL Server database
- Visual Studio 2022 or VS Code

### Building

```bash
dotnet build
```

### Running Migrations

The database migrations are automatically executed when the application starts, but you can also run them manually:

```csharp
var initializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
using var connection = connectionFactory.CreateConnection();
initializer.Migrate(connection, "Data/Scripts");
```

## Architecture Patterns

- **Repository Pattern**: Abstracted data access through interfaces
- **Unit of Work**: Transaction management and data consistency
- **Dependency Injection**: Loose coupling through DI container
- **Factory Pattern**: Connection and unit of work factories
- **Service Layer**: Business logic encapsulation

## Security Features

- **JWT Authentication**: Bearer token support
- **Role-based Authorization**: Fine-grained access control
- **Tenant Isolation**: Multi-tenant data separation
- **Policy-based Authorization**: Flexible authorization policies

## Contributing

1. Follow the existing code structure and patterns
2. Add appropriate unit tests for new functionality
3. Update migration scripts for database changes
4. Ensure proper error handling and logging
5. Follow the established naming conventions (snake_case for database, PascalCase for C#)

## License

This project is part of the BaseTemplate solution and follows the same licensing terms.



dotnet ef migrations add "SpecificationForItem" --project src\Infrastructure --startup-project src\API --output-dir Data\Migrations --context AppDbContext

dotnet ef database update --project src\Infrastructure --startup-project src\API --context AppDbContext
