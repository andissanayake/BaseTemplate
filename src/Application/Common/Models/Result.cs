namespace BaseTemplate.Application.Common.Models;

public static class ResultCodeMapper
{
    public static Func<string, bool> IsSuccess { get; set; } = code => code == DefaultSuccessCode;

    public static string DefaultSuccessCode { get; set; } = "success";
    public static string DefaultValidationErrorCode { get; set; } = "validation_error";
    public static string DefaultUnauthorizedCode { get; set; } = "unauthorized";
    public static string DefaultForbiddenCode { get; set; } = "forbidden";
    public static string DefaultNotFoundCode { get; set; } = "not_found";
    public static string DefaultServerErrorCode { get; set; } = "server_error";
}

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

    public static Result<T> Validation(string? message, Dictionary<string, string[]> errors) =>
        Failure(ResultCodeMapper.DefaultValidationErrorCode, message, errors);

    public static Result<T> Unauthorized(string? message = null) =>
        Failure(ResultCodeMapper.DefaultUnauthorizedCode, message ?? "Unauthorized");
    public static Result<T> Forbidden(string? message = null) =>
        Failure(ResultCodeMapper.DefaultForbiddenCode, message ?? "Forbidden");
    public static Result<T> NotFound(string? message = null) =>
        Failure(ResultCodeMapper.DefaultNotFoundCode, message ?? "Resource not found");

    public static Result<T> ServerError(string? message = null) =>
        Failure(ResultCodeMapper.DefaultServerErrorCode, message ?? "Unexpected error occurred");
}
