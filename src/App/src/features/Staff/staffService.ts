import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import {
  StaffMemberDto,
  StaffMemberDetailDto,
  UpdateStaffRolesRequest,
} from "./StaffModel";

export class StaffService {
  private static readonly baseUrl = "/api/tenants";

  /**
   * Get all staff members for a tenant
   */
  static async getStaffMembers(): Promise<Result<StaffMemberDto[]>> {
    return await handleApi(axiosInstance.get(`${this.baseUrl}/staff`));
  }

  /**
   * Get details of a specific staff member
   */
  static async getStaffMember(
    staffId: number
  ): Promise<Result<StaffMemberDetailDto>> {
    return await handleApi(
      axiosInstance.get(`${this.baseUrl}/staff/${staffId}`)
    );
  }

  /**
   * Remove a staff member from the tenant
   */
  static async removeStaff(staffId: number): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.delete(`${this.baseUrl}/staff/${staffId}`)
    );
  }

  /**
   * Update roles for a staff member
   */
  static async updateStaffRoles(
    staffId: number,
    request: UpdateStaffRolesRequest
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.put(`${this.baseUrl}/staff/${staffId}/roles`, {
        staffId,
        newRoles: request.newRoles,
      })
    );
  }
}
