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
    request: CreateStaffRequestRequest
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.post(`${this.baseUrl}/request-staff`, request)
    );
  }

  static async getStaffRequests(): Promise<Result<StaffRequestDto[]>> {
    return await handleApi(axiosInstance.get(`${this.baseUrl}/staff-requests`));
  }

  /**
   * Update a staff request (revoke/reject by tenant owner)
   */
  static async updateStaffRequest(
    staffRequestId: number,
    rejectionReason: string
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.post(
        `${this.baseUrl}/staff-requests/${staffRequestId}/update`,
        {
          staffRequestId,
          rejectionReason,
        }
      )
    );
  }

  static async respondToStaffRequest(
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
        `${this.baseUrl}/staff-requests/${staffRequestId}/respond`,
        payload
      )
    );
  }
}
