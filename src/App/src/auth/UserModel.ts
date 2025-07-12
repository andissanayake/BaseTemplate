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
