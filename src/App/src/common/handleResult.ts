import { Result } from "./result";
import { ResultCodeMapper } from "./resultCodeMapper";

export interface ResultHandlers<T> {
  onSuccess?: (data: T) => void;
  onValidationError?: (
    details: Record<string, string[]>,
    message?: string
  ) => void;
  onUnauthorized?: (details?: Record<string, string[]>) => void;
  onForbidden?: (details?: Record<string, string[]>) => void;
  onServerError?: (
    details?: Record<string, string[]>,
    message?: string
  ) => void;
  onFallback?: (result?: Result<T>) => void;
  onFinally?: () => void;
}

export function handleResult<T>(
  result: Result<T>,
  handlers: ResultHandlers<T> = {}
): boolean {
  const {
    onSuccess = (r) => console.log("[Result:Success]", r),
    onValidationError = (r) => console.warn("[Result:ValidationError]", r),
    onUnauthorized = (r) => console.warn("[Result:Unauthorized]", r),
    onForbidden = (r) => console.warn("[Result:Forbidden]", r),
    onServerError = (r) => console.error("[Result:ServerError]", r),
    onFallback = (r) => console.error("[Result:Fallback]", r),
    onFinally = () => {},
  } = handlers;
  let success = false;
  try {
    switch (result.code) {
      case ResultCodeMapper.DefaultSuccessCode:
        onSuccess(result.data as T);
        success = true;
        break;

      case ResultCodeMapper.DefaultValidationErrorCode:
        onValidationError(result.details ?? {}, result.message);
        break;

      case ResultCodeMapper.DefaultUnauthorizedCode:
        onUnauthorized(result.details);
        break;

      case ResultCodeMapper.DefaultForbiddenCode:
        onForbidden(result.details);
        break;

      case ResultCodeMapper.DefaultServerErrorCode:
        onServerError(result.details, result.message);
        break;

      default:
        onFallback(result);
        break;
    }
  } catch (ex) {
    console.error("[Result:Exception]", ex);
    onFallback(result);
  } finally {
    onFinally();
  }
  return success;
}
