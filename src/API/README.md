# API Layer - BaseTemplate

## Description

The API layer serves as the primary entry point and presentation layer for the BaseTemplate application. It exposes RESTful APIs, handles client requests, manages authentication and authorization, and delegates all business logic operations to the Application layer. It is built using ASP.NET Core.

## Key Features & Responsibilities

- **RESTful Endpoints**: Defines API controllers (located in the `Controllers/` directory) that map HTTP requests to specific application functionalities.
- **Authentication**: Implements JWT Bearer token authentication. Configuration is managed in `appsettings.json` and set up via the `AddApiAuthentication` extension method (likely found in `Extensions/`).
- **Authorization**: Leverages ASP.NET Core's built-in authorization mechanisms to protect endpoints.
- **Request Handling & Validation**:
  - Receives HTTP requests and routes them to appropriate controller actions.
  - Automatic model state validation is suppressed (`SuppressModelStateInvalidFilter = true`). Validation is expected to be handled by the Application layer, which returns a `Result` object indicating success or validation failures. The API layer then translates this `Result` into appropriate HTTP responses.
- **Response Formatting**: Constructs and returns HTTP responses (e.g., `OkObjectResult`, `BadRequestObjectResult`, `NotFoundResult`) based on the `Result` object received from the Application layer.
- **CORS (Cross-Origin Resource Sharing)**: Configured in `Program.cs` to allow requests from specified origins (e.g., `http://localhost:5000` for development).
- **Logging**: Utilizes Serilog for structured logging to both the console and rolling daily log files (in `Logs/`).
- **Swagger/OpenAPI Documentation**: Integrates Swashbuckle to automatically generate interactive API documentation, accessible in development environments (typically at `/swagger`).
- **Health Checks**: Exposes a health check endpoint at `/health`.
- **Static File Serving**: Configured to serve static files from the `wwwroot/` directory (e.g., for a frontend Single Page Application) and uses `MapFallbackToFile("index.html")` for SPA routing.
- **Database Initialization (Development)**: On application startup in the development environment, it triggers database schema migrations using `DatabaseInitializer` from the Infrastructure layer.
- **Dependency Injection**: Wires up services from all layers (`Application`, `Infrastructure`, and API-specific services) in `Program.cs` and `DependencyInjection.cs`.

## Project Structure

- **`Controllers/`**: Contains ASP.NET Core API controllers that handle incoming HTTP requests.
- **`Program.cs`**: The main application entry point. Configures the web host, dependency injection container, and middleware pipeline.
- **`DependencyInjection.cs`**: Contains the `AddAPI()` extension method for registering API-specific services (e.g., `CurrentUserService`, Swagger, authentication).
- **`Extensions/`**: Typically holds extension methods, such as `AddApiAuthentication.cs` for setting up JWT authentication.
- **`Services/`**: Contains API-specific services, like `CurrentUserService.cs` for resolving the current user.
- **`appsettings.json` / `appsettings.Development.json`**: Configuration files for settings like database connections, JWT parameters, Serilog, etc.
- **`Dockerfile`**: Instructions for building a Docker image of the API application.
- **`wwwroot/`**: Root folder for static files served by the application (e.g., HTML, CSS, JavaScript for a frontend SPA).
- **`Logs/`**: Directory where Serilog file logs are stored.

## Prerequisites

- **.NET 8 SDK & Runtime**: Required to build and run the project.
- **PostgreSQL Database**: A running instance of PostgreSQL, configured as per `appsettings.json`.
- **Docker** (Optional): If you intend to run the application using Docker containers via `docker-compose.yml`.

## Configuration (`appsettings.json`)

Key configuration sections typically include:

- **`ConnectionStrings`**:
  - `DefaultConnection`: For the primary PostgreSQL database (used by the Infrastructure layer).
- **`Jwt`**:
  - `Authority`, `Audience`, `Issuer`, `Key`: Parameters for JWT Bearer token validation and generation.
- **`Serilog`**: Configuration for logging levels and sinks (though much is configured in code in `Program.cs`).
- **Allowed CORS Origins**: Defined in `Program.cs` but could be moved to configuration if needed.

## How to Run the API

1.  **Using .NET CLI**:

    - Ensure your PostgreSQL database is running and accessible.
    - Update `appsettings.json` (or `appsettings.Development.json`) with the correct database connection string and JWT settings if necessary.
    - Navigate to the `src/API` directory in your terminal.
    - Run `dotnet restore` (if first time or dependencies changed).
    - Run `dotnet run`.
    - The API will typically be available at `http://localhost:5001` or `https://localhost:7001` (check console output).

2.  **Using Docker Compose** (if configured in the root `docker-compose.yml`):
    - Ensure Docker Desktop is running.
    - Navigate to the solution root directory.
    - Run `docker-compose up --build`.
    - The API will be accessible based on the port mappings in `docker-compose.yml`.

## Integration with Other Layers

- **Initialization**: In `Program.cs`, services from the `Application` and `Infrastructure` layers are registered using their respective `AddApplication()` and `AddInfrastructure()` extension methods.
- **Request Delegation**: API controllers receive HTTP requests and typically delegate the core logic by sending commands or queries to the Application layer.
- **Service Consumption**: Relies on services provided by the Infrastructure layer (e.g., database connection, identity) which are configured during startup.

## Adding a New API Endpoint (Example: Get Todo Item by ID)

1.  **Ensure Application Layer Support**: Verify that a corresponding query (e.g., `GetTodoItemByIdQuery`) and handler exist in the Application layer, returning a `Result<TodoItemDto>`.

2.  **Create/Update Controller Action**:
    Add a new action to an existing controller (e.g., `TodoItemsController.cs`) or create a new controller.

    **File**: `src/API/Controllers/TodoItemsController.cs` (Simplified Example)

    ```csharp
    // using BaseTemplate.Application.TodoItems.Queries; // For a hypothetical GetTodoItemByIdQuery and TodoItemDto
    // using BaseTemplate.Application.Common.Models;      // For Result<T>
    // using BaseTemplate.Application.Common.RequestHandler; // For IRequest if you were to define the query directly here
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    // Assuming ApiControllerBase is in a namespace like BaseTemplate.API.Controllers or similar
    // using BaseTemplate.API.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class TodoListsController : ApiControllerBase // Inherits from ApiControllerBase
    {
        // Constructor is typically not needed for this setup if ApiControllerBase handles MediatR
        // public TodoListsController()
        // {
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<TodoListDto>>> GetById(int id)
        {
            return await SendAsync(new GetTodoListByIdQuery(id));
        }
    }
    ```

3.  **Handling `Result<T>`**: The controller action (or the `SendAsync` method in `ApiControllerBase`) inspects the `Code` and `Data` from the `Result<T>` object returned by the Application layer to determine the appropriate `IActionResult` (e.g., `Ok()`, `NotFound()`, `BadRequest()`).

This approach keeps controller actions lean and focused on HTTP concerns, while the Application layer handles the business logic and validation.
