export class ResultCodeMapper {
  static DefaultSuccessCode = "success";
  static DefaultValidationErrorCode = "validation_error";
  static DefaultUnauthorizedCode = "unauthorized";
  static DefaultForbiddenCode = "forbidden";
  static DefaultNotFoundCode = "not_found";
  static DefaultServerErrorCode = "server_error";

  static isSuccess = (code: string): boolean =>
    code === ResultCodeMapper.DefaultSuccessCode;
}
