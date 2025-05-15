// stores/authStore.ts
import { create } from "zustand";
import { persist, devtools } from "zustand/middleware";

export interface AppUser {
  uid: string;
  email: string | null;
  displayName: string | null;
  photoURL: string | null;
  token: string | null;
}

interface AuthState {
  user: AppUser | null;
  roles: string[];
  tenantId: string | null;
  tenantName: string | null;
  setUser: (user: AppUser | null) => void;
  setRoles: (roles: string[]) => void;
  setTenantId: (tenantId: string | null) => void;
  setTenantName: (tenantName: string | null) => void;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      (set) => ({
        user: null,
        roles: [],
        tenantId: null,
        tenantName: null,
        setUser: async (user) => {
          set({ user });
        },
        setRoles: (roles) => set({ roles }),
        setTenantId: (tenantId) => set({ tenantId }),
        setTenantName: (tenantName) => set({ tenantName }),
      }),
      {
        name: "auth",
      }
    )
  )
);
