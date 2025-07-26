import axiosInstance from "../auth/axiosInstance";
import { ResultHandlers } from "./handleResult";

class ApiClient {
  private baseURL: string;

  constructor(baseURL: string = "") {
    this.baseURL = baseURL;
  }

  private async request<T>(
    method: "GET" | "POST" | "PUT" | "DELETE",
    url: string,
    data?: unknown,
    handlers?: ResultHandlers<T>
  ): Promise<void> {
    const {
      onSuccess,
      onValidationError,
      onUnauthorized,
      onForbidden,
      onServerError,
      onFallback,
      onFinally,
    } = handlers || {};

    try {
      const config: Record<string, unknown> = {
        method,
        url: this.baseURL + url,
      };

      if (data) {
        if (method === "GET") {
          config.params = data;
        } else {
          config.data = data;
        }
      }

      const response = await axiosInstance(config);
      const result = response.data;
      onSuccess?.(result.data as T);
      return;
    } catch (error: unknown) {
      // Handle network errors
      if (error && typeof error === "object" && "response" in error) {
        // Server responded with error status
        const response = (
          error as {
            response: {
              status: number;
              data?: { message?: string; details?: Record<string, string[]> };
            };
          }
        ).response;
        const status = response.status;
        const data = response.data;
        const details = data?.details;
        switch (status) {
          case 400:
            if (details) {
              onValidationError?.(details);
            }
            break;
          case 401:
            if (details) {
              onUnauthorized?.(details);
            }
            onUnauthorized?.();
            break;
          case 403:
            if (details) {
              onForbidden?.(details);
            }
            onForbidden?.();
            break;
          case 404:
            break;
          case 500:
            if (details) {
              onServerError?.(details);
            }
            onServerError?.();
            break;
          default:
            break;
        }
      }

      onFallback?.();
      return;
    } finally {
      onFinally?.();
    }
  }

  async get<T>(url: string, handlers?: ResultHandlers<T>): Promise<void> {
    return this.request<T>("GET", url, undefined, handlers);
  }

  async post<T>(
    url: string,
    data?: unknown,
    handlers?: ResultHandlers<T>
  ): Promise<void> {
    return this.request<T>("POST", url, data, handlers);
  }

  async put<T>(
    url: string,
    data?: unknown,
    handlers?: ResultHandlers<T>
  ): Promise<void> {
    return this.request<T>("PUT", url, data, handlers);
  }

  async delete<T>(
    url: string,
    data?: unknown,
    handlers?: ResultHandlers<T>
  ): Promise<void> {
    return this.request<T>("DELETE", url, data, handlers);
  }
}

// Create default instance
export const apiClient = new ApiClient();

// Export the class for custom instances
export { ApiClient };
