// stores/authStore.ts
import { create } from "zustand";
import { persist, devtools } from "zustand/middleware";
import { StaffInvitationDetails, TenantDetails } from "./UserModel";
import { Roles } from "./RolesEnum";

export interface AppUser {
  uid: string;
  email: string | null;
  displayName: string | null;
  photoURL: string | null;
  token: string | null;
}

interface AuthState {
  user: AppUser | null;
  roles: Roles[];
  tenant: TenantDetails | null;
  staffInvitation: StaffInvitationDetails | null;
  setUser: (user: AppUser | null) => void;
  setRoles: (roles: Roles[]) => void;
  setTenant: (tenant: TenantDetails | null) => void;
  setStaffInvitation: (staffInvitation: StaffInvitationDetails | null) => void;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      (set) => ({
        user: null,
        roles: [],
        tenant: null,
        staffInvitation: null,
        setUser: async (user) => {
          set({ user });
        },
        setRoles: (roles) => set({ roles }),
        setTenant: (tenant) => set({ tenant }),
        setStaffInvitation: (staffInvitation) => set({ staffInvitation }),
      }),
      {
        name: "auth",
      }
    )
  )
);
