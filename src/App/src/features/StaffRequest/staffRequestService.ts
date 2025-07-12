import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import {
  StaffRequestDto,
  CreateStaffRequestRequest,
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
   * Respond to a staff request (accept or reject)
   */
  static async respondToStaffRequest(
    tenantId: number,
    staffRequestId: number,
    isAccepted: boolean,
    rejectionReason?: string
  ): Promise<Result<boolean>> {
    const payload: {
      StaffRequestId: number;
      IsAccepted: boolean;
      RejectionReason?: string;
    } = {
      StaffRequestId: staffRequestId,
      IsAccepted: isAccepted,
    };

    if (!isAccepted && rejectionReason) {
      payload.RejectionReason = rejectionReason;
    }

    return await handleApi(
      axiosInstance.post(
        `${this.baseUrl}/${tenantId}/staff-requests/${staffRequestId}/respond`,
        payload
      )
    );
  }
}
