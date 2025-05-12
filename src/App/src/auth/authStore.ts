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
  setUser: (user: AppUser | null) => void;
  setRoles: (roles: string[]) => void;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      (set) => ({
        user: null,
        roles: [],
        setUser: async (user) => {
          set({ user });
        },
        setRoles: (roles) => set({ roles }),
      }),
      {
        name: "auth",
      }
    )
  )
);
