namespace BaseTemplate.Application.Common.RequestHandler.Behaviors;
using System.ComponentModel.DataAnnotations;

public class RequestValidationBehavior : IMediatorBehavior
{
    public Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(context.Request, null, null);

        if (!Validator.TryValidateObject(context.Request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .SelectMany(vr => vr.MemberNames.Select(mn => new { MemberName = mn, vr.ErrorMessage }))
                .GroupBy(e => e.MemberName)
                .ToDictionary(g => g.Key.ToLower(), g => g.Select(e => e.ErrorMessage ?? "Validation error").ToArray());

            return Task.FromResult(Result.Validation("Request validation failed", errors));
        }

        return Task.FromResult(Result.Success());
    }
} 