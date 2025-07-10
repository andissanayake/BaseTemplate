import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import {
  StaffRequestDto,
  CreateStaffRequestRequest,
  RespondToStaffRequestRequest,
} from "./StaffRequestModel";

export class StaffRequestService {
  private static readonly baseUrl = "/api/tenants";

  /**
   * Create a new staff request for a tenant
   */
  static async createStaffRequest(
    tenantId: number,
    request: CreateStaffRequestRequest
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.post(`${this.baseUrl}/${tenantId}/request-staff`, {
        tenantId,
        ...request,
      })
    );
  }

  /**
   * Get all staff requests for a tenant
   */
  static async getStaffRequests(
    tenantId: number
  ): Promise<Result<StaffRequestDto[]>> {
    return await handleApi(
      axiosInstance.get(`${this.baseUrl}/${tenantId}/staff-requests`)
    );
  }

  /**
   * Update a staff request (accept or reject)
   */
  static async updateStaffRequest(
    tenantId: number,
    request: RespondToStaffRequestRequest
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.post(
        `${this.baseUrl}/${tenantId}/staff-requests/${request.staffRequestId}/update`,
        {
          TenantId: tenantId,
          StaffRequestId: request.staffRequestId,
          Accept: request.accept,
          RejectionReason: request.rejectionReason,
        }
      )
    );
  }
}
