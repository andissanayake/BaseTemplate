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