export interface StaffMemberDto {
  ssoId: string;
  name?: string;
  email?: string;
  roles: string[];
  created: string;
  lastModified?: string;
}

export interface StaffMemberDetailDto {
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
