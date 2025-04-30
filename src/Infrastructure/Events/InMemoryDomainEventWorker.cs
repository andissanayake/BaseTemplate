using System.Collections.Concurrent;
using System.Text.Json;
using BaseTemplate.Application.Common.Events;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Infrastructure.Events;

public class InMemoryDomainEventWorker : BackgroundService, IDomainEventQueue
{
    private readonly ConcurrentQueue<BaseEvent> _queue = new();
    private readonly IServiceProvider _provider;
    private readonly ILogger<InMemoryDomainEventWorker> _logger;
    private readonly IUnitOfWorkFactory _factory;

    public InMemoryDomainEventWorker(IServiceProvider provider, ILogger<InMemoryDomainEventWorker> logger, IUnitOfWorkFactory factory)
    {
        _provider = provider;
        _logger = logger;
        _factory = factory;
    }

    public async Task Enqueue(BaseEvent domainEvent)
    {
        _queue.Enqueue(domainEvent);
        await SaveEventToDb(domainEvent);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadEventsFromDb();
        while (!stoppingToken.IsCancellationRequested)
        {
            while (_queue.TryDequeue(out var domainEvent))
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var scopedProvider = scope.ServiceProvider;

                    var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                    var handlers = scopedProvider.GetServices(handlerType);

                    foreach (var handler in handlers)
                    {
                        var handleMethod = handlerType.GetMethod("HandleAsync");
                        if (handleMethod != null)
                        {
                            var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent, stoppingToken })!;
                            await task;
                        }
                    }
                    await UpdateEventStatus(domainEvent, "Processed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing domain event ${domainEvent.GetType()}");
                    await UpdateEventStatus(domainEvent, "Failed", ex.Message);
                }
            }

            await Task.Delay(50, stoppingToken); // adjustable polling delay
        }
    }

    private async Task SaveEventToDb(BaseEvent domainEvent)
    {
        try
        {
            using var uow = _factory.CreateUOW();
            var ed = new DomainEvent
            {
                EventId = domainEvent.EventId,
                EventType = domainEvent.GetType().Name,
                Status = "Pending",
                EventData = JsonSerializer.Serialize(domainEvent)

            };
            await uow.InsertAsync(ed);
            uow.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving event to database.");
        }
    }

    private async Task LoadEventsFromDb()
    {
        try
        {
            using var uow = _factory.CreateUOW();
            var domainEvents = await uow.QueryAsync<DomainEvent>("SELECT * FROM DomainEvents WHERE Status = 'Pending'");
            foreach (var domainEvent in domainEvents)
            {
                var eventType = Type.GetType(domainEvent.EventType);
                if (eventType != null)
                {
                    // Deserialize the event data back into the correct type
                    var eventData = JsonSerializer.Deserialize(domainEvent.EventData, eventType);
                    if (eventData is BaseEvent eventInstance)  // Check if it's of BaseEvent type
                    {
                        _queue.Enqueue(eventInstance);
                    }
                }
                else
                {
                    _logger.LogWarning($"Event type {domainEvent.EventType} not found.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading events from database.");
        }
    }
    private async Task UpdateEventStatus(BaseEvent domainEvent, string status, string result = null)
    {
        try
        {
            using var uow = _factory.CreateUOW();
            var domainEvents = await uow.ExecuteAsync("UPDATE DomainEvents SET Status = @Status, ProcessedAt = @ProcessedAt, Result = @Result WHERE EventId = @EventId",
                    new
                    {
                        Status = status,
                        ProcessedAt = DateTime.UtcNow,
                        Result = result,
                        EventId = domainEvent.EventId
                    });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event status in the database.");
        }
    }
}
