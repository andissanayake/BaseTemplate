import { Result } from "../common/result";
import axiosInstance from "./axiosInstance";
import { handleApi } from "../common/handleApi";

export interface TenantDetails {
  id: string;
  name: string;
}

export interface StaffRequestDetails {
  id: number;
  requesterName: string;
  requesterEmail: string;
  roles: string[];
  status: number;
  created: string; // ISO date string
  tenantName: string;
}

export interface UserDetails {
  roles: string[];
  tenant?: TenantDetails;
  staffRequest?: StaffRequestDetails;
}

export class userService {
  static async details(): Promise<Result<UserDetails>> {
    return await handleApi(axiosInstance.post("/api/user/userDetails"));
  }
}
