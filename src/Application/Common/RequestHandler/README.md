# Mediator Pattern Refactoring

## Problem

The original `Mediator` class was too complex with multiple responsibilities:

- Null validation
- Authorization (roles, policies, tenant access)
- Request validation
- Handler resolution and execution
- Error handling

This made the code hard to test, maintain, and extend.

## Solutions

### Option 1: Separated Concerns Mediator

The refactored `Mediator` class now delegates to specialized components:

- `IRequestValidator` - Handles null checks and data validation
- `IRequestAuthorizer` - Handles all authorization logic
- `IRequestHandlerResolver` - Handles handler resolution and execution

**Benefits:**

- Single Responsibility Principle
- Easier to test individual components
- Better separation of concerns
- More maintainable

**Usage:**

```csharp
// In DependencyInjection.cs
services.AddRequestHandlers(Assembly.GetExecutingAssembly());

// In controllers
public async Task<ActionResult<Result<T>>> SendAsync<T>(IRequest<T> request)
{
    return await Mediator.SendAsync(request);
}
```

### Option 2: Pipeline Mediator (Recommended)

The `MediatorPipeline` uses a chain of responsibility pattern with behaviors:

- `NullValidationBehavior` - Validates request is not null
- `RequestValidationBehavior` - Validates request data
- `AuthorizationBehavior` - Handles authorization
- `HandlerExecutionBehavior` - Executes the handler

**Benefits:**

- Even simpler architecture
- Easy to add/remove/reorder behaviors
- Each behavior is completely independent
- Highly testable
- Follows Open/Closed Principle

**Usage:**

```csharp
// In DependencyInjection.cs
services.AddRequestHandlersWithPipeline(Assembly.GetExecutingAssembly());

// In controllers
public async Task<ActionResult<Result<T>>> SendAsync<T>(IRequest<T> request)
{
    return await MediatorPipeline.SendAsync(request);
}
```

## Adding Custom Behaviors

With the pipeline approach, you can easily add custom behaviors:

```csharp
public class LoggingBehavior : IMediatorBehavior
{
    private readonly ILogger<LoggingBehavior> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior> logger)
    {
        _logger = logger;
    }

    public async Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        _logger.LogInformation("Processing request: {RequestType}", context.RequestType.Name);

        // Continue to next behavior
        return Result.Success();
    }
}

// Register in ServiceCollectionExtensions
services.AddScoped<IMediatorBehavior, LoggingBehavior>();
```

## Migration Guide

1. **For existing code**: The original `IMediator` interface remains unchanged, so existing controllers work without modification.

2. **For new code**: Consider using the pipeline approach for better maintainability.

3. **Testing**: Each behavior can be tested independently, making unit tests much simpler.

## Performance Considerations

- Both approaches have similar performance characteristics
- Pipeline approach has slightly more overhead due to context object creation
- The benefits of maintainability outweigh the minimal performance cost
- Consider caching behaviors if performance becomes critical
