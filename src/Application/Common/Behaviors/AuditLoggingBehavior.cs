using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BaseTemplate.Application.Common.Behaviors;

/// <summary>
/// Interface for auditable requests
/// </summary>
public interface IAuditableRequest
{
    /// <summary>
    /// Whether this request should be audited
    /// </summary>
    bool ShouldAudit => true;
    
    /// <summary>
    /// Sensitive data to exclude from audit logs
    /// </summary>
    string[] SensitiveProperties => Array.Empty<string>();
}

/// <summary>
/// Audit logging behavior that tracks user actions
/// </summary>
public class AuditLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AuditLoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IUser _user;

    public AuditLoggingBehavior(ILogger<AuditLoggingBehavior<TRequest, TResponse>> logger, IUser user)
    {
        _logger = logger;
        _user = user;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var auditableRequest = request as IAuditableRequest;
        
        // Skip audit logging if not auditable
        if (auditableRequest?.ShouldAudit != true)
        {
            return await next();
        }

        var auditEntry = new AuditLogEntry
        {
            Timestamp = DateTime.UtcNow,
            RequestType = requestName,
            UserId = _user.Id,
            UserEmail = _user.Email,
            TenantId = _user.TenantId,
            RequestData = SerializeRequest(request, auditableRequest.SensitiveProperties)
        };

        try
        {
            _logger.LogInformation("Audit: User {UserId} ({UserEmail}) executing {RequestName}", 
                _user.Id, _user.Email, requestName);

            var response = await next();
            
            // Log successful completion
            auditEntry.Success = IsSuccessfulResponse(response);
            auditEntry.ResponseData = SerializeResponse(response);
            
            LogAuditEntry(auditEntry);
            
            return response;
        }
        catch (Exception ex)
        {
            auditEntry.Success = false;
            auditEntry.ErrorMessage = ex.Message;
            auditEntry.ErrorDetails = ex.ToString();
            
            LogAuditEntry(auditEntry);
            
            throw;
        }
    }

    private static bool IsSuccessfulResponse(TResponse response)
    {
        return response switch
        {
            Result<object> result => ResultCodeMapper.IsSuccess(result.Code),
            _ => true // Assume success if not a Result type
        };
    }

    private static string SerializeRequest(TRequest request, string[] sensitiveProperties)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(request, options);
            
            // Remove sensitive data
            foreach (var sensitiveProperty in sensitiveProperties)
            {
                json = RedactSensitiveData(json, sensitiveProperty);
            }
            
            return json;
        }
        catch (Exception)
        {
            return $"[Serialization failed for {typeof(TRequest).Name}]";
        }
    }

    private static string SerializeResponse(TResponse response)
    {
        try
        {
            // Only serialize basic response info, not the full data
            if (response is Result<object> result)
            {
                return JsonSerializer.Serialize(new
                {
                    Code = result.Code,
                    Message = result.Message,
                    Success = ResultCodeMapper.IsSuccess(result.Code)
                });
            }
            
            return "[Response serialized]";
        }
        catch (Exception)
        {
            return "[Response serialization failed]";
        }
    }

    private static string RedactSensitiveData(string json, string propertyName)
    {
        // Simple redaction - in production, use more sophisticated JSON manipulation
        var pattern = $"\"{propertyName}\"\\s*:\\s*\"[^\"]*\"";
        return System.Text.RegularExpressions.Regex.Replace(
            json, pattern, $"\"{propertyName}\":\"[REDACTED]\"", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    private void LogAuditEntry(AuditLogEntry entry)
    {
        _logger.LogInformation("AUDIT: {@AuditEntry}", entry);
        
        // TODO: Store in dedicated audit table or external system
        // Example: await _auditRepository.SaveAsync(entry);
    }

    private class AuditLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? TenantId { get; set; }
        public string RequestData { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ResponseData { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDetails { get; set; }
    }
}