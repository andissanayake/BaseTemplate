import { notification } from "antd";

export function handleServerError(
  error?: Record<string, string[]> | undefined,
  defaultMessage?: string | null,
  showNotification: boolean = true
): void {
  if (!showNotification) return;

  // Extract error message from the first error if available
  let errorMessage = "An error occurred while processing your request.";

  // If error object exists, extract and combine all error messages
  if (error && typeof error === "object") {
    const allErrors: string[] = [];

    Object.values(error).forEach((errorArray) => {
      if (Array.isArray(errorArray)) {
        allErrors.push(...errorArray);
      }
    });

    if (allErrors.length > 0) {
      errorMessage = allErrors.join("; ");
    }
  }
  // If no error object but defaultMessage is provided, use it
  else if (defaultMessage) {
    errorMessage = defaultMessage;
  }

  notification.error({
    message: "Server Error",
    description: errorMessage,
  });
}
