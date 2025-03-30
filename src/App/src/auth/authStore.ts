// stores/authStore.ts
import { create } from "zustand";
import { persist, devtools } from "zustand/middleware";
import axiosInstance from "./axiosInstance";

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
  fetchRoles: () => Promise<void>;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      (set, get) => ({
        user: null,
        roles: [],
        setUser: async (user) => {
          set({ user });
          if (user) {
            await get().fetchRoles();
          }
        }, // Fetch roles when user is set
        fetchRoles: async () => {
          axiosInstance
            .get("/api/user/roles")
            .then((response) => {
              if (response.data) {
                set({ roles: response.data });
              }
            })
            .catch((error) => {
              console.error("Error fetching roles:", error);
              set({ roles: [] });
            });
        },
      }),
      {
        name: "auth",
      }
    )
  )
);
