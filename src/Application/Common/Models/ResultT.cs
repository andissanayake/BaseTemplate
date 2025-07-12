namespace BaseTemplate.Application.Common.Models;

public class Result<T>
{
    public T? Data { get; set; }
    public required string Code { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]> Details { get; set; } = new();

    public static Result<T> Success(T data, string? message = null) =>
        new()
        {
            Data = data,
            Code = ResultCodeMapper.DefaultSuccessCode,
            Message = message
        };

    public static Result<T> Failure(string code, string? message = null, Dictionary<string, string[]>? details = null) =>
        new()
        {
            Code = code,
            Message = message,
            Details = details ?? new()
        };

    public static Result<T> Validation(string message, Dictionary<string, string[]> errors) =>
        Failure(ResultCodeMapper.DefaultValidationErrorCode, message, errors);
    public static Result<T> Validation(string message) =>
        Failure(ResultCodeMapper.DefaultValidationErrorCode, message);
    public static Result<T> Unauthorized(string? message = null) =>
        Failure(ResultCodeMapper.DefaultUnauthorizedCode, message ?? "Unauthorized");
    public static Result<T> Forbidden(string? message = null) =>
        Failure(ResultCodeMapper.DefaultForbiddenCode, message ?? "Forbidden");
    public static Result<T> NotFound(string? message = null) =>
        Failure(ResultCodeMapper.DefaultNotFoundCode, message ?? "Resource not found");

    public static Result<T> ServerError(string? message = null) =>
        Failure(ResultCodeMapper.DefaultServerErrorCode, message ?? "Unexpected error occurred");
} 