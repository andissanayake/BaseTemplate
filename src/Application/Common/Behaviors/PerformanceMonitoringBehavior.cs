using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.Common.Behaviors;

/// <summary>
/// Performance monitoring behavior that tracks request execution time
/// </summary>
public class PerformanceMonitoringBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceMonitoringBehavior<TRequest, TResponse>> _logger;
    private const int SlowRequestThresholdMs = 1000; // 1 second

    public PerformanceMonitoringBehavior(ILogger<PerformanceMonitoringBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        using var activity = Activity.StartActivity($"Handle_{requestName}");
        activity?.SetTag("request.type", requestName);
        
        var stopwatch = Stopwatch.StartNew();
        var success = false;
        Exception? exception = null;

        try
        {
            _logger.LogDebug("Starting execution of {RequestName}", requestName);
            
            var response = await next();
            
            success = true;
            
            // Check if response indicates success
            if (response is Result<object> result)
            {
                success = ResultCodeMapper.IsSuccess(result.Code);
                activity?.SetTag("result.code", result.Code);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            exception = ex;
            success = false;
            activity?.SetTag("error", true);
            activity?.SetTag("error.message", ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            
            activity?.SetTag("duration.ms", elapsedMs);
            activity?.SetTag("success", success);
            
            // Log performance metrics
            LogPerformanceMetrics(requestName, elapsedMs, success, exception);
        }
    }

    private void LogPerformanceMetrics(string requestName, long elapsedMs, bool success, Exception? exception)
    {
        var logLevel = elapsedMs > SlowRequestThresholdMs ? LogLevel.Warning : LogLevel.Debug;
        
        if (exception != null)
        {
            _logger.LogError(exception, 
                "Request {RequestName} failed after {ElapsedMs}ms", 
                requestName, elapsedMs);
        }
        else if (elapsedMs > SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "Slow request detected: {RequestName} took {ElapsedMs}ms (Success: {Success})", 
                requestName, elapsedMs, success);
        }
        else
        {
            _logger.LogDebug(
                "Request {RequestName} completed in {ElapsedMs}ms (Success: {Success})", 
                requestName, elapsedMs, success);
        }
        
        // TODO: Send metrics to monitoring system (e.g., Prometheus, Application Insights)
        // Example: _metrics.RecordRequestDuration(requestName, elapsedMs, success);
    }
}