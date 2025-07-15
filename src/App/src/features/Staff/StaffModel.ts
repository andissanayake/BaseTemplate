export interface StaffMemberDto {
  id: number;
  ssoId: string;
  name?: string;
  email?: string;
  roles: string[];
  created: string;
  lastModified?: string;
}

export interface StaffMemberDetailDto {
  id: number;
  ssoId: string;
  name?: string;
  email?: string;
  roles: string[];
  created: string;
  lastModified?: string;
  tenantId: number;
}

export interface UpdateStaffRolesRequest {
  newRoles: string[];
}

export interface StaffListResponse {
  staffMembers: StaffMemberDto[];
}

export interface StaffDetailResponse {
  staffMember: StaffMemberDetailDto;
}
