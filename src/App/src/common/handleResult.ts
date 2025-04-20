import { Result } from "./result";
import { ResultCodeMapper } from "./ResultCodeMapper";

export interface ResultHandlers<T> {
  onSuccess?: (data?: T) => void;
  onValidationError?: (details?: Record<string, string[]>) => void;
  onUnauthorized?: (details?: Record<string, string[]>) => void;
  onForbidden?: (details?: Record<string, string[]>) => void;
  onServerError?: (details?: Record<string, string[]>) => void;
  onFallback?: (result?: Result<T>) => void;
  onFinally?: () => void; // ✅ default no-op
}

export function handleResult<T>(
  result: Result<T>,
  handlers: ResultHandlers<T> = {}
) {
  const {
    onSuccess = (r) => console.log("[Result:Success]", r),
    onValidationError = (r) => console.warn("[Result:ValidationError]", r),
    onUnauthorized = (r) => console.warn("[Result:Unauthorized]", r),
    onForbidden = (r) => console.warn("[Result:Forbidden]", r),
    onServerError = (r) => console.error("[Result:ServerError]", r),
    onFallback = (r) => console.error("[Result:Fallback]", r),
    onFinally = () => {}, // ✅ default no-op
  } = handlers;

  try {
    switch (result.code) {
      case ResultCodeMapper.DefaultSuccessCode:
        return onSuccess(result.data);

      case ResultCodeMapper.DefaultValidationErrorCode:
        return onValidationError(result.details);

      case ResultCodeMapper.DefaultUnauthorizedCode:
        return onUnauthorized(result.details);

      case ResultCodeMapper.DefaultForbiddenCode:
        return onForbidden(result.details);

      case ResultCodeMapper.DefaultServerErrorCode:
        return onServerError(result.details);

      default:
        return onFallback(result);
    }
  } catch (ex) {
    console.error("[Result:Exception]", ex);
    onFallback(result); // Call fallback in case of exception
  } finally {
    onFinally(); // ✅ always called
  }
}
