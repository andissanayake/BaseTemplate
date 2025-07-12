import { Result } from "../common/result";
import axiosInstance from "./axiosInstance";
import { handleApi } from "../common/handleApi";
import { UserDetails } from "./UserModel";

export class UserService {
  static async details(): Promise<Result<UserDetails>> {
    return await handleApi(axiosInstance.post("/api/user/userDetails"));
  }
}
