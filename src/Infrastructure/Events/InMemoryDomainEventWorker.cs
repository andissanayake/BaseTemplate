using System.Collections.Concurrent;
using BaseTemplate.Application.Common.Events;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Infrastructure.Events;

public class InMemoryDomainEventWorker : BackgroundService, IDomainEventQueue
{
    private readonly ConcurrentQueue<BaseEvent> _queue = new();
    private readonly IServiceProvider _provider;
    private readonly ILogger<InMemoryDomainEventWorker> _logger;

    public InMemoryDomainEventWorker(IServiceProvider provider, ILogger<InMemoryDomainEventWorker> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public void Enqueue(BaseEvent domainEvent)
    {
        _queue.Enqueue(domainEvent);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing domain event ${domainEvent.GetType()}");
                }
            }

            await Task.Delay(50, stoppingToken); // adjustable polling delay
        }
    }
}
