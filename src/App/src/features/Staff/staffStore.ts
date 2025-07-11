import { create } from "zustand";
import { StaffMemberDto, StaffMemberDetailDto } from "./StaffModel";

// Interface defining the staff store state and actions
interface StaffStore {
  // State properties
  staffMembers: StaffMemberDto[]; // List of all staff members
  selectedStaffMember: StaffMemberDetailDto | null; // Currently selected staff member for detailed view
  loading: boolean; // Loading state for async operations

  // Action methods
  setStaffMembers: (staffMembers: StaffMemberDto[]) => void; // Update the staff members list
  setSelectedStaffMember: (staffMember: StaffMemberDetailDto | null) => void; // Set the selected staff member
  setLoading: (loading: boolean) => void; // Update loading state
  removeStaffMember: (ssoId: string) => void; // Remove a staff member by SSO ID
  updateStaffMemberRoles: (ssoId: string, roles: string[]) => void; // Update roles for a specific staff member
}

// Zustand store for managing staff member state
export const useStaffStore = create<StaffStore>((set) => ({
  // Initial state
  staffMembers: [],
  selectedStaffMember: null,
  loading: false,

  // State setters
  setStaffMembers: (staffMembers) => set({ staffMembers }),
  setSelectedStaffMember: (selectedStaffMember) => set({ selectedStaffMember }),
  setLoading: (loading) => set({ loading }),

  // Business logic actions
  removeStaffMember: (ssoId) =>
    set((state) => ({
      staffMembers: state.staffMembers.filter((staff) => staff.ssoId !== ssoId),
    })),

  updateStaffMemberRoles: (ssoId, roles) =>
    set((state) => ({
      staffMembers: state.staffMembers.map((staff) =>
        staff.ssoId === ssoId ? { ...staff, roles } : staff
      ),
      selectedStaffMember:
        state.selectedStaffMember?.ssoId === ssoId
          ? { ...state.selectedStaffMember, roles }
          : state.selectedStaffMember,
    })),
}));
