using System.Collections.Concurrent;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using MediatorS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaseTemplate.Infrastructure.Events;

public class InMemoryDomainEventWorker : BackgroundService, IDomainEventQueue
{
    private readonly ConcurrentQueue<BaseEvent> _queue = new();
    private readonly IServiceProvider _provider;

    public InMemoryDomainEventWorker(IServiceProvider provider)
    {
        _provider = provider;
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
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    //await mediator.Publish(domainEvent, stoppingToken);
                }
                catch (Exception ex)
                {
                    // TODO: Replace with real logging or retry strategy
                    Console.WriteLine($"Error processing domain event: {ex.Message}");
                }
            }

            await Task.Delay(50, stoppingToken); // adjustable polling delay
        }
    }
}
