import { Result } from "../common/result";
import axiosInstance from "./axiosInstance";
import { handleApi } from "../common/handleApi";
export class UserService {
  static async fetchRoles(): Promise<Result<string[]>> {
    return await handleApi(axiosInstance.get("/api/user/roles"));
  }
}
