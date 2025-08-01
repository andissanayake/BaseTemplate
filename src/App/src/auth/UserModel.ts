import { Roles } from "./RolesEnum";

export interface TenantDetails {
  id: number;
  name: string;
}

export interface StaffInvitationDetails {
  id: number;
  requesterName: string;
  requesterEmail: string;
  roles: Roles[];
  status: number;
  created: string; // ISO date string
  tenantName: string;
}

export interface UserDetails {
  roles: Roles[];
  tenant?: TenantDetails;
  staffInvitation?: StaffInvitationDetails;
}
