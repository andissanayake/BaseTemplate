import { Result } from "../common/result";
import axiosInstance from "./axiosInstance";
import { handleApi } from "../common/handleApi";
export class UserService {
  static async details(): Promise<Result<{ roles: string[] }>> {
    return await handleApi(axiosInstance.post("/api/user/userDetails"));
  }
}
