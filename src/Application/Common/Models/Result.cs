namespace BaseTemplate.Application.Common.Models;

public class Result
{
    public string Code { get; set; } = ResultCodeMapper.DefaultSuccessCode;
    public string? Message { get; set; }
    public Dictionary<string, string[]> Details { get; set; } = new();

    public static Result Success() => new() { Code = ResultCodeMapper.DefaultSuccessCode };

    public static Result Failure(string code, string? message = null, Dictionary<string, string[]>? details = null) =>
        new()
        {
            Code = code,
            Message = message,
            Details = details ?? new()
        };

    public static Result Validation(string? message, Dictionary<string, string[]> errors) =>
        Failure(ResultCodeMapper.DefaultValidationErrorCode, message, errors);

    public static Result Unauthorized(string? message = null) =>
        Failure(ResultCodeMapper.DefaultUnauthorizedCode, message ?? "Unauthorized");

    public static Result Forbidden(string? message = null) =>
     Failure(ResultCodeMapper.DefaultForbiddenCode, message ?? "Forbidden");

    public static Result NotFound(string? message = null) =>
        Failure(ResultCodeMapper.DefaultNotFoundCode, message ?? "Resource not found");

    public static Result ServerError(string? message = null) =>
        Failure(ResultCodeMapper.DefaultServerErrorCode, message ?? "Unexpected error occurred");
}
