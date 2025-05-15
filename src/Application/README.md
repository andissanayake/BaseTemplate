# Application Layer - BaseTemplate

## Description

The Application layer is the heart of the BaseTemplate's business logic. It orchestrates operations, processes data, and enforces application-specific rules. It acts as an intermediary between the presentation layer (API) and the domain layer, ensuring that use cases are executed correctly and efficiently.

This layer is built following **CQRS (Command Query Responsibility Segregation)** principles. Commands are used for operations that change state, and Queries are used for retrieving data. Request handlers are automatically discovered and registered.

## Core Patterns & Technologies

- **CQRS**: Commands (for write operations) and Queries (for read operations) are used to separate concerns.
- **Request Handlers**: Custom logic in `Common/RequestHandler/` (via `AddRequestHandlers` in `DependencyInjection.cs`) scans the assembly to register command and query handlers, providing an in-house implementation of the mediator pattern.
- **Feature-Sliced Architecture**: Code is organized by feature (e.g., `TodoItems`, `Users`, `Tenants`), making it easier to navigate and manage.
- **.NET 8**: The project targets the .NET 8 framework.
- **Dependency Injection**: Services and handlers are registered and resolved using .NET's built-in DI container. The `AddApplication()` extension method in `DependencyInjection.cs` bootstraps this layer.

## Project Structure Overview

The Application layer is organized into several key directories:

- **`Common/`**: Contains shared components like:
  - `RequestHandler/`: Logic for discovering and registering command/query handlers.
  - `Interfaces/`: Contains interfaces for services (e.g., for data access, identity) typically implemented by the Infrastructure layer.
  - `Models/`: Contains shared Data Transfer Objects (DTOs), request/response models, or other common model classes.
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

Create a new command to represent the intent to create a todo item. Include Data Annotations for validation.

**File**: `src/Application/TodoItems/Commands/CreateTodoItem/CreateTodoItemCommand.cs`

```csharp
// Assuming a Result type for operation outcomes
// using BaseTemplate.Application.Common.Models; // Or wherever your Result type is
// using BaseTemplate.Application.Common.RequestHandler; // For IRequest
using System.ComponentModel.DataAnnotations; // Required for Data Annotations

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommand // : IRequest<Result<int>> // Assuming IRequest and Result<T>
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string? Title { get; set; }

    public string? Note { get; set; }
    // Add other properties needed to create a TodoItem
}
```

_(Note: You'll need to inject dependencies like your DbContext or repositories via constructor. The example assumes you have these interfaces defined in `Application/Common/Interfaces` and implemented in `Infrastructure`.)_

### 2. Create the Command Handler

The handler will contain the logic to process the command.

**File**: `src/Application/TodoItems/Commands/CreateTodoItem/CreateTodoItemCommandHandler.cs`

```csharp
// using BaseTemplate.Domain.Entities; // For TodoItem entity
// using BaseTemplate.Application.Common.Interfaces; // For IApplicationDbContext or repository interfaces
// using BaseTemplate.Application.Common.RequestHandler; // For IRequestHandler
// using System.Threading;
// using System.Threading.Tasks;

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommandHandler //: IRequestHandler<CreateTodoItemCommand, Result<int>>
{
    // private readonly IApplicationDbContext _context; // Example: if using DbContext directly (less common)
    // private readonly ITodoItemRepository _todoItemRepository; // Example: if using a repository

    // public CreateTodoItemCommandHandler(ITodoItemRepository todoItemRepository)
    // {
    //     _todoItemRepository = todoItemRepository;
    // }

    public async Task<object> /*Task<Result<int>>*/ Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        // var entity = new TodoItem
        // {
        //     Title = request.Title,
        //     Note = request.Note,
        //     Done = false
        //     // Map other properties
        // };

        // _context.TodoItems.Add(entity); // Or _todoItemRepository.AddAsync(entity);
        // await _context.SaveChangesAsync(cancellationToken); // Or await _todoItemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        // return Result<int>.Success(entity.Id); // Return a success result with the new item's ID

        // Placeholder until actual interfaces and entities are confirmed
        await Task.CompletedTask;
        Console.WriteLine($"Simulating creation of TodoItem: {request.Title}");
        return new { Id = new Random().Next() }; // Placeholder
    }
}
```

_(Note: You'll need to inject dependencies like your DbContext or repositories via constructor. The example assumes you have these interfaces defined in `Application/Common/Interfaces` and implemented in `Infrastructure`.)_

### 3. Add Request Validation

Validate the command's input. This is often done using .NET's built-in **Data Annotations** directly on the command properties (as shown in the `CreateTodoItemCommand` example above). Alternatively, you can implement custom validation logic within the handler or a dedicated validation class if more complex rules are needed.

When using Data Annotations, validation can be automatically triggered by the ASP.NET Core pipeline if the command is a parameter in an API controller action. You can also manually trigger validation if needed:

```csharp
// Example of manually triggering validation in a handler (less common if API handles it)
// var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
// var validationResults = new List<ValidationResult>();
// bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);
// if (!isValid)
// {
//     // Handle validation errors (e.g., return a failure result)
// }
```

_(Note: If using Data Annotations with ASP.NET Core, ensure your controllers are set up to automatically validate model state. For custom validation, integrate it into your command handling pipeline as appropriate.)_

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
