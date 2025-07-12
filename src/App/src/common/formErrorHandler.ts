import { FormInstance } from "antd";
import { notification } from "antd";

export interface FormErrorHandlerOptions {
  form: FormInstance;
  errors: Record<string, string[]>;
  showNotification?: boolean;
}

export function handleFormValidationErrors({
  form,
  errors,
  showNotification = true,
}: FormErrorHandlerOptions): void {
  const formErrors: Array<{ name: string; errors: string[] }> = [];
  const nonFormErrors: string[] = [];

  // Get all form field names (convert to lowercase for comparison)
  const fieldNames = form.getFieldsValue();
  const formFieldNames = Object.keys(fieldNames).map((name) =>
    name.toLowerCase()
  );

  // Process each error
  Object.entries(errors).forEach(([errorKey, errorMessages]) => {
    const normalizedErrorKey = errorKey.toLowerCase();

    // Check if this error key matches any form field
    if (formFieldNames.includes(normalizedErrorKey)) {
      // Find the original field name (preserve case)
      const originalFieldName = Object.keys(fieldNames).find(
        (name) => name.toLowerCase() === normalizedErrorKey
      );

      if (originalFieldName) {
        formErrors.push({
          name: originalFieldName,
          errors: errorMessages,
        });
      }
    } else {
      // This error doesn't match any form field, add to non-form errors
      nonFormErrors.push(...errorMessages);
    }
  });

  // Set form field errors
  if (formErrors.length > 0) {
    form.setFields(formErrors);
  }

  // Show notification for non-form errors
  if (nonFormErrors.length > 0 && showNotification) {
    notification.error({
      message: "Validation Errors",
      description: nonFormErrors.join(", "),
    });
  }
}
