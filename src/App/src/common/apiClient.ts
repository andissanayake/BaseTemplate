import axiosInstance from "../auth/axiosInstance";
import { ResultCodeMapper } from "./resultCodeMapper";
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

      // Handle success
      if (result.code === ResultCodeMapper.DefaultSuccessCode) {
        onSuccess?.(result.data as T);
        return;
      }

      // Handle validation errors
      if (result.code === ResultCodeMapper.DefaultValidationErrorCode) {
        onValidationError?.(result.details || {});
        return;
      }

      // Handle unauthorized
      if (result.code === ResultCodeMapper.DefaultUnauthorizedCode) {
        onUnauthorized?.(result.details);
        return;
      }

      // Handle forbidden
      if (result.code === ResultCodeMapper.DefaultForbiddenCode) {
        onForbidden?.(result.details);
        return;
      }

      // Handle server errors
      if (result.code === ResultCodeMapper.DefaultServerErrorCode) {
        onServerError?.(result.details);
        return;
      }

      // Handle other API errors
      onFallback?.(result);
      return;
    } catch (error: unknown) {
      // Handle network errors
      if (error && typeof error === "object" && "response" in error) {
        // Server responded with error status
        const response = (
          error as { response: { status: number; data?: { message?: string } } }
        ).response;
        const status = response.status;

        switch (status) {
          case 400:
            break;
          case 401:
            onUnauthorized?.();
            break;
          case 403:
            onForbidden?.();
            break;
          case 404:
            break;
          case 500:
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

/*
SAMPLE USAGE EXAMPLES:

1. Basic GET request with success handling:
```typescript
apiClient.get<User>("/api/users/1", {
  onSuccess: (user) => {
    console.log("User loaded:", user);
    // Update state, show notification, etc.
  },
  onFallback: (result) => {
    console.error("Failed to load user:", result);
    // Handle any other errors
  }
});
```

2. POST request with comprehensive error handling:
```typescript
apiClient.post<User>("/api/users", userData, {
  onSuccess: (newUser) => {
    console.log("User created:", newUser);
    // Update UI, show success message, redirect, etc.
  },
  onValidationError: (details) => {
    console.warn("Validation errors:", details);
    // Display field-specific validation errors
    // details format: { "email": ["Invalid email"], "name": ["Required"] }
  },
  onUnauthorized: () => {
    console.warn("User not authorized");
    // Redirect to login, show auth error, etc.
  },
  onForbidden: () => {
    console.warn("Access denied");
    // Show permission error, redirect to dashboard, etc.
  },
  onServerError: () => {
    console.error("Server error occurred");
    // Show server error message, retry logic, etc.
  },
  onFallback: (result) => {
    console.error("Unexpected error:", result);
    // Handle any other error types
  },
  onFinally: () => {
    console.log("Request completed");
    // Clean up loading states, close modals, etc.
  }
});
```

3. PUT request with loading state management:
```typescript
const [isLoading, setIsLoading] = useState(false);

apiClient.put<User>("/api/users/1", updatedData, {
  onSuccess: (updatedUser) => {
    setIsLoading(false);
    // Update user in state
    setUser(updatedUser);
  },
  onValidationError: (details) => {
    setIsLoading(false);
    // Show validation errors in form
    setFormErrors(details);
  },
  onUnauthorized: () => {
    setIsLoading(false);
    // Handle auth error
    navigate("/login");
  },
  onFinally: () => {
    setIsLoading(false);
  }
});
```

4. DELETE request with confirmation:
```typescript
apiClient.delete<void>("/api/users/1", undefined, {
  onSuccess: () => {
    // User deleted successfully
    // Remove from list, show success message, etc.
  },
  onForbidden: () => {
    // User doesn't have permission to delete
    // Show permission error
  },
  onFallback: () => {
    // Handle any other errors
    // Show generic error message
  }
});
```

5. Using with async/await pattern:
```typescript
const loadUsers = async () => {
  return new Promise<User[]>((resolve, reject) => {
    apiClient.get<User[]>("/api/users", {
      onSuccess: (users) => {
        resolve(users);
      },
      onFallback: () => {
        reject(new Error("Failed to load users"));
      }
    });
  });
};

// Usage:
try {
  const users = await loadUsers();
  console.log("Users loaded:", users);
} catch (error) {
  console.error("Error loading users:", error);
}
```

6. Custom API client instance:
```typescript
const customApiClient = new ApiClient("/api/v2");

customApiClient.get<Data>("/custom-endpoint", {
  onSuccess: (data) => {
    // Handle success
  },
  onFallback: () => {
    // Handle errors
  }
});
```

7. With Ant Design notifications:
```typescript
import { notification } from "antd";

apiClient.post<User>("/api/users", userData, {
  onSuccess: (user) => {
    notification.success({
      message: "User created successfully",
      description: `User ${user.name} has been created.`
    });
  },
  onValidationError: (details) => {
    notification.error({
      message: "Validation Error",
      description: "Please check your input and try again."
    });
  },
  onUnauthorized: () => {
    notification.error({
      message: "Authentication Required",
      description: "Please log in to continue."
    });
  },
  onServerError: () => {
    notification.error({
      message: "Server Error",
      description: "Please try again later."
    });
  }
});
```

8. With React state management:
```typescript
const [data, setData] = useState<Data[]>([]);
const [loading, setLoading] = useState(false);
const [error, setError] = useState<string | null>(null);

const loadData = () => {
  setLoading(true);
  setError(null);
  
  apiClient.get<Data[]>("/api/data", {
    onSuccess: (newData) => {
      setData(newData);
    },
    onValidationError: () => {
      setError("Invalid request");
    },
    onUnauthorized: () => {
      setError("Please log in");
    },
    onServerError: () => {
      setError("Server error");
    },
    onFallback: () => {
      setError("An error occurred");
    },
    onFinally: () => {
      setLoading(false);
    }
  });
};
```

KEY BENEFITS:
- Granular error handling for different scenarios
- Clean separation of concerns (API vs UI)
- Consistent interface across all HTTP methods
- Easy to test and mock
- Flexible notification handling per component
- Type-safe with TypeScript generics
*/
