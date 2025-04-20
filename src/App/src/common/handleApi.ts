/* eslint-disable @typescript-eslint/no-explicit-any */
import { AxiosResponse } from "axios";
import { Result } from "../common/result";
import { ResultCodeMapper } from "./ResultCodeMapper";

export async function handleApi<T>(
  promise: Promise<AxiosResponse<T>>
): Promise<Result<T>> {
  try {
    const response = await promise;
    const result = response.data as Result<T>;

    // Valid structured response
    if (result && typeof result.code === "string") {
      return result;
    }

    // Fallback for malformed success response
    return {
      code: ResultCodeMapper.DefaultServerErrorCode,
      message: "Unexpected response format.",
      details: {},
    };
  } catch (error: any) {
    // API responded with a structured error (e.g., 400, 401, 500)
    const result = error?.response?.data as Result<T>;

    if (result && typeof result.code === "string") {
      return result;
    }

    // Total fallback (network error, CORS failure, etc.)
    return {
      code: ResultCodeMapper.DefaultServerErrorCode,
      message: error?.message || "Unexpected error occurred.",
      details: {},
    };
  }
}
