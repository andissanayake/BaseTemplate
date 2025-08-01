export interface StaffInvitationDto {
  id: number;
  requestedEmail: string;
  requestedName: string;
  roles: string[];
  requestedByAppUserId: number;
  requestedByAppUserName: string;
  requestedByAppUserEmail: string;
  status: StaffInvitationStatus;
  created: string; // ISO date string
  acceptedAt?: string; // ISO date string
  acceptedByAppUserId?: number;
  acceptedByAppUserName?: string;
  acceptedByAppUserEmail?: string;
  rejectionReason?: string;
}

export enum StaffInvitationStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
  Revoked = 3,
  Expired = 4,
}

export interface CreateStaffInvitationRequest {
  staffEmail: string;
  staffName: string;
  roles: string[];
}

export interface RespondToStaffInvitationRequest {
  staffInvitationId: number;
  rejectionReason: string; // Now required
}

export interface StaffInvitationResponse {
  success: boolean;
  message?: string;
  data?: StaffInvitationDto[];
}
