# Infrastructure - BaseTemplate

## Description

This project provides core infrastructure services for the BaseTemplate application. It handles data access using Dapper with PostgreSQL, identity management, CSV processing, and other essential services. This project primarily implements interfaces defined in the `BaseTemplate.Application` layer.

## Key Components & Responsibilities

### Data Access

- **ORM**: Utilizes Dapper and Dapper.SimpleCRUD for efficient data operations against a PostgreSQL database.
- **Unit of Work**: Implements a Unit of Work pattern (`Data/UnitOfWork.cs`, registered as `IUnitOfWorkFactory`) to ensure atomic operations.
- **Connection Management**: Uses `Infrastructure/Data/PostgresConnectionFactory.cs` (registered as `IDbConnectionFactory`) to create and manage database connections. This is configured via the "DefaultConnection" connection string.
- **Naming Conventions**: Handles mapping between database snake_case and C# PascalCase naming conventions (see `Data/NameResolver.cs` and Dapper setup in `DependencyInjection.cs`).
- **Database Initialization**: Includes a `Data/DatabaseInitializer.cs` component, which is responsible for setting up the database schema and potentially seeding initial data. It may use SQL scripts located in the `Data/Scripts/` directory.

### Identity Management

- Provides an `IIdentityService` implementation (`Identity/IdentityService.cs`) for user-related operations and authentication/authorization concerns. _(Specific functionalities depend on the implementation within `IdentityService.cs`)_.

### CSV Handling

- Integrates the `CsvHelper` library, suggesting capabilities for processing CSV files.

### Dependency Injection

- All core services are registered via the `AddInfrastructure()` extension method in `DependencyInjection.cs`. This method configures Dapper, connection factories, unit of work, identity services, and the database initializer.

## Prerequisites

- **.NET SDK**: Version 8.0.408 (or as specified in the `global.json` file).
- **PostgreSQL Database**: A running instance of PostgreSQL is required.
- **`BaseTemplate.Application` Project**: This infrastructure project depends on `BaseTemplate.Application` for contracts (interfaces) it implements.

## Configuration

This project expects the following configurations to be provided by the consuming application (e.g., `BaseTemplate.API` via its `appsettings.json`):

- **Connection Strings**:
  - `DefaultConnection`: The connection string for the PostgreSQL database.
    Example: `"Server=your_postgres_server;Database=your_database;User Id=your_user;Password=your_password;"`

## Integration

This `Infrastructure` project is designed to be consumed by other layers of the BaseTemplate application, typically the API or main application host.

To integrate:

1. Add a project reference to `BaseTemplate.Infrastructure`.
2. In the main application's `Startup.cs` or `Program.cs`, call the `AddInfrastructure()` extension method on `IServiceCollection`, passing the `IConfiguration` instance:

   ```csharp
   // Example in Program.cs or Startup.cs
   builder.Services.AddInfrastructure(builder.Configuration);
   // or
   // services.AddInfrastructure(Configuration);
   ```

This will register all necessary services provided by this project into the application's dependency injection container.

## Contributing

Standard contribution guidelines apply:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -am 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Create a new Pull Request.

## License

Specify the license for this project (e.g., MIT, Apache 2.0).
