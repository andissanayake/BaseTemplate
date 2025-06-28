import { Result } from "../common/result";
import axiosInstance from "./axiosInstance";
import { handleApi } from "../common/handleApi";
export class userService {
  static async details(): Promise<
    Result<{
      roles: string[];
      tenantId: string | null;
      tenantName: string | null;
    }>
  > {
    return await handleApi(axiosInstance.post("/api/user/userDetails"));
  }
}
