# Application Layer - BaseTemplate

## Description

The Application layer is the heart of the BaseTemplate's business logic. It orchestrates operations, processes data, and enforces application-specific rules. It acts as an intermediary between the presentation layer (API) and the domain layer, ensuring that use cases are executed correctly and efficiently.

This layer is built following **CQRS (Command Query Responsibility Segregation)** principles. Commands are used for operations that change state, and Queries are used for retrieving data. Request handlers are automatically discovered and registered.

## Core Patterns & Technologies

- **CQRS**: Commands (for write operations) and Queries (for read operations) are used to separate concerns.
- **Request Handlers**: Custom logic in `Common/RequestHandler/` (via `AddRequestHandlers` in `DependencyInjection.cs`) scans the assembly to register command and query handlers, providing an in-house implementation of the mediator pattern.
- **Result Pattern**: Uses a standardized `Result<T>` (and non-generic `Result`) class (`Common/Models/Result.cs`) for returning operation outcomes from handlers. This provides a consistent way to indicate success (with optional data) or failure (with error codes, messages, and details).
- **Feature-Sliced Architecture**: Code is organized by feature (e.g., `TodoItems`, `Users`, `Tenants`), making it easier to navigate and manage.
- **.NET 8**: The project targets the .NET 8 framework.
- **Dependency Injection**: Services and handlers are registered and resolved using .NET's built-in DI container. The `AddApplication()` extension method in `DependencyInjection.cs` bootstraps this layer.

## Project Structure Overview

The Application layer is organized into several key directories:

- **`Common/`**: Contains shared components like:
  - `RequestHandler/`: Logic for discovering and registering command/query handlers.
  - `Interfaces/`: Contains interfaces for services (e.g., for data access, identity) typically implemented by the Infrastructure layer.
  - `Models/`: Contains shared Data Transfer Objects (DTOs), request/response models (like the crucial `Result.cs` and `Result<T>.cs` for operation outcomes), or other common model classes.
  - `Security/`: Contains components related to application security, such as custom authorization handlers or user context information.
- **Feature Folders (e.g., `TodoItems/`, `TodoLists/`, `Users/`, `Tenants/`)**: Each feature is a vertical slice containing its own:
  - **`Commands/`**: Contains command definitions and their handlers for write operations.
    - `Create[Feature]/Create[Feature]Command.cs`
    - `Create[Feature]/Create[Feature]CommandHandler.cs`
    - `Update[Feature]/...`
    - `Delete[Feature]/...`
  - **`Queries/`**: Contains query definitions and their handlers for read operations.
    - `Get[Feature]ById/Get[Feature]ByIdQuery.cs`
    - `Get[Feature]ById/Get[Feature]ByIdQueryHandler.cs`
    - `GetAll[Features]/...`
  - **`DTOs/`** (optional, within feature folder): Data Transfer Objects specific to this feature.
  - **`Validators/`** (optional, within feature folder): Validation rules for commands/queries, typically using Data Annotations directly on command/query properties, or custom validation logic.

## Prerequisites

- **.NET SDK**: Version 8.0.408 (or as specified in the `global.json` file).
- **`BaseTemplate.Domain` Project**: This Application project depends on the Domain project for entities and core domain logic.

## Integration

This `Application` project is primarily consumed by the Presentation layer (e.g., `BaseTemplate.API`).

To integrate and use this layer:

1. The Presentation layer adds a project reference to `BaseTemplate.Application`.
2. In the Presentation layer's `Startup.cs` or `Program.cs`, call the `AddApplication()` extension method:

   ```csharp
   // Example in API's Program.cs or Startup.cs
   builder.Services.AddApplication();
   // or
   // services.AddApplication();
   ```

   This registers all necessary services (like command/query handlers) from this project. The Presentation layer then dispatches Commands or Queries to this layer to execute business logic.

## How to Add a New Feature (Example: Creating a "Todo Item")

Here's a step-by-step guide on how to add a new "Create Todo Item" feature:

### 1. Define the Command

Create a new command to represent the intent to create a todo item. Include Data Annotations for validation. The command should specify what type of `Result` it returns (e.g., `Result<int>` if returning an ID, or `Result` if no data is returned on success).

**File**: `src/Application/TodoItems/Commands/CreateTodoItem/CreateTodoItemCommand.cs`

```csharp
// Make sure to import your IRequest and Result types
// using BaseTemplate.Application.Common.RequestHandler; // Or your actual IRequest location
// using BaseTemplate.Application.Common.Models;      // For Result<T>
using System.ComponentModel.DataAnnotations;        // Required for Data Annotations

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

// Assuming IRequest is defined in your Common/RequestHandler directory
// and your handler will return an integer ID wrapped in a Result on success.
public class CreateTodoItemCommand : IRequest<Result<int>>
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string? Title { get; set; }

    public string? Note { get; set; }
    // Add other properties needed to create a TodoItem
}
```

_(Note: `IRequest<Result<int>>` signifies that this command, when handled, will produce a `Result` containing an `int` (e.g., the new item's ID) upon success, or a `Result` indicating an error.)_

### 2. Create the Command Handler

The handler will contain the logic to process the command and return an appropriate `Result`.

**File**: `src/Application/TodoItems/Commands/CreateTodoItem/CreateTodoItemCommandHandler.cs`

```csharp
// using BaseTemplate.Domain.Entities;                   // For TodoItem entity
// using BaseTemplate.Application.Common.Interfaces;     // For IUnitOfWork or repository interfaces
// using BaseTemplate.Application.Common.Models;         // For Result<T>
// using BaseTemplate.Application.Common.RequestHandler; // For IRequestHandler
// using System.Threading;
// using System.Threading.Tasks;

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Result<int>>
{
    // private readonly IUnitOfWork _unitOfWork; // Example: Inject your Unit of Work or specific repository

    // public CreateTodoItemCommandHandler(IUnitOfWork unitOfWork)
    // {
    //     _unitOfWork = unitOfWork;
    // }

    public async Task<Result<int>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        // Example: Validate request (can also be done via middleware/pipeline if Data Annotations aren't enough)
        // var validationContext = new ValidationContext(request);
        // var validationResults = new List<ValidationResult>();
        // if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        // {
        //     var errors = validationResults.ToDictionary(r => r.MemberNames.FirstOrDefault() ?? string.Empty, r => r.ErrorMessage ?? "Validation Error");
        //     // Assuming a helper to convert ValidationResult to Dictionary<string, string[]> for Result.Validation
        //     // return Result<int>.Validation("Validation failed", details: /* map validationResults to Dictionary */);
        // }

        // var entity = new TodoItem
        // {
        //     Title = request.Title,
        //     Note = request.Note,
        //     Done = false
        //     // Map other properties
        // };

        // await _unitOfWork.TodoItems.AddAsync(entity); // Assuming a TodoItems repository on UoW
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        // return Result<int>.Success(entity.Id, "Todo item created successfully.");

        // Placeholder until actual interfaces and entities are confirmed
        await Task.CompletedTask;
        Console.WriteLine($"Simulating creation of TodoItem: {request.Title}");
        // Simulate success
        return Result<int>.Success(new Random().Next(), "Todo item simulated successfully.");
        // Simulate validation error
        // return Result<int>.Validation("Validation Error", new Dictionary<string, string[]> { { "Title", new[] { "Title is invalid" } } });
    }
}
```

### 3. Add Request Validation

Validate the command's input. This is often done using .NET's built-in **Data Annotations** directly on the command properties (as shown in the `CreateTodoItemCommand` example above). The command handler can then check `ModelState` if in an API context, or manually trigger validation if needed. The `Result.Validation()` static method can be used to return validation failures.

When using Data Annotations, validation can be automatically triggered by the ASP.NET Core pipeline if the command is a parameter in an API controller action. If more complex validation is needed that cannot be expressed with Data Annotations, or if you need to perform validation within the handler itself (e.g., cross-field validation or database lookups), you can implement that logic in the handler and return `Result<T>.Validation(...)` or `Result<T>.Failure(...)`.

```csharp
// Example of manually triggering validation in a handler and returning a Result
// (This is simplified, actual conversion from ValidationResult to Dictionary for Result.Details might be needed)
// var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
// var validationResults = new List<ValidationResult>();
// bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);
// if (!isValid)
// {
//     var errorDetails = validationResults
//         .GroupBy(vr => vr.MemberNames.FirstOrDefault() ?? string.Empty)
//         .ToDictionary(g => g.Key, g => g.Select(vr => vr.ErrorMessage ?? "Invalid").ToArray());
//     return Result<int>.Validation("Request is invalid.", errorDetails); // Assuming Result<int> is expected by command
// }
```

### 4. Define DTOs (for Queries or complex results)

If your command returns complex data or if you're creating a Query, define DTOs.

**File (Example for a query)**: `src/Application/TodoItems/Queries/TodoItemDto.cs`

```csharp
namespace BaseTemplate.Application.TodoItems.Queries;

public class TodoItemDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Note { get; set; }
    public bool Done { get; set; }
    // Add other properties to expose to the client
}
```

### How Handlers are Discovered

The `services.AddRequestHandlers(Assembly.GetExecutingAssembly());` line in `DependencyInjection.cs` is responsible for finding and registering all your command and query handlers that implement the relevant `IRequestHandler<,>` interface.

This structure helps keep the Application layer organized, testable, and focused on business logic.
