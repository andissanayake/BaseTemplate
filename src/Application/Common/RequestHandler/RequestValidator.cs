namespace BaseTemplate.Application.Common.RequestHandler;
using System.ComponentModel.DataAnnotations;

public class RequestValidator : IRequestValidator
{
    public Result ValidateNull<TResponse>(IRequest<TResponse>? request)
    {
        if (request is null)
        {
            return Result.Validation("Request is required", new Dictionary<string, string[]>
            {
                ["request"] = ["Request cannot be null."]
            });
        }
        return Result.Success();
    }

    public Result ValidateRequest<TResponse>(IRequest<TResponse> request)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request, null, null);

        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .SelectMany(vr => vr.MemberNames.Select(mn => new { MemberName = mn, vr.ErrorMessage }))
                .GroupBy(e => e.MemberName)
                .ToDictionary(g => g.Key.ToLower(), g => g.Select(e => e.ErrorMessage ?? "Validation error").ToArray());

            return Result.Validation("Request validation failed", errors);
        }

        return Result.Success();
    }
} 