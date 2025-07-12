export interface StaffRequestDto {
  id: number;
  requestedEmail: string;
  requestedName: string;
  roles: string[];
  requestedBySsoId: string;
  status: StaffRequestStatus;
  created: string; // ISO date string
  acceptedAt?: string; // ISO date string
  acceptedBySsoId?: string;
  rejectionReason?: string;
}

export enum StaffRequestStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
  Revoked = 3,
}

export interface CreateStaffRequestRequest {
  staffEmail: string;
  staffName: string;
  roles: string[];
}

export interface RespondToStaffRequestRequest {
  staffRequestId: number;
  rejectionReason: string; // Now required
}

export interface StaffRequestResponse {
  success: boolean;
  message?: string;
  data?: StaffRequestDto[];
}
