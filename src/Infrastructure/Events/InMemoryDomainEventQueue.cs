using System.Collections.Concurrent;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Infrastructure.Events;
public class InMemoryDomainEventQueue : IDomainEventQueue, IDisposable
{
    private readonly ConcurrentQueue<BaseEvent> _queue = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly Task _processorTask;

    public InMemoryDomainEventQueue(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _processorTask = Task.Run(ProcessQueueAsync);
    }

    public void Enqueue(BaseEvent domainEvent)
    {
        _queue.Enqueue(domainEvent);
    }

    private async Task ProcessQueueAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            while (_queue.TryDequeue(out var domainEvent))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Publish(domainEvent, _cts.Token);
                }
                catch (Exception ex)
                {
                    // TODO: Add logging or retry
                    Console.WriteLine($"Error processing domain event: {ex.Message}");
                }
            }

            await Task.Delay(50); // polling delay
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _processorTask.Wait();
        _cts.Dispose();
    }
}
