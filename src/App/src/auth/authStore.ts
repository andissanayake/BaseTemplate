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
  setUser: (user: AppUser | null) => void;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      (set) => ({
        user: null,
        setUser: (user) => set({ user }),
      }),
      {
        name: "auth",
      }
    )
  )
);
