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
  static async getStaffMembers(
    tenantId: number
  ): Promise<Result<StaffMemberDto[]>> {
    return await handleApi(
      axiosInstance.get(`${this.baseUrl}/${tenantId}/staff`)
    );
  }

  /**
   * Get details of a specific staff member
   */
  static async getStaffMember(
    tenantId: number,
    staffSsoId: string
  ): Promise<Result<StaffMemberDetailDto>> {
    return await handleApi(
      axiosInstance.get(`${this.baseUrl}/${tenantId}/staff/${staffSsoId}`)
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
    tenantId: number,
    staffId: number,
    request: UpdateStaffRolesRequest
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.put(`${this.baseUrl}/${tenantId}/staff/${staffId}/roles`, {
        tenantId,
        staffId,
        newRoles: request.newRoles,
      })
    );
  }
}
