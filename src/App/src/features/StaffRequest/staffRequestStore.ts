import { create } from "zustand";
import { StaffRequestDto } from "./StaffRequestModel";

interface StaffRequestState {
  staffRequests: StaffRequestDto[];
  loading: boolean;
  error: string | null;

  // Actions
  setStaffRequests: (requests: StaffRequestDto[]) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  clearError: () => void;
}

export const useStaffRequestStore = create<StaffRequestState>((set) => ({
  staffRequests: [],
  loading: false,
  error: null,

  setStaffRequests: (requests: StaffRequestDto[]) => {
    set({ staffRequests: requests });
  },

  setLoading: (loading: boolean) => {
    set({ loading });
  },

  setError: (error: string | null) => {
    set({ error });
  },

  clearError: () => {
    set({ error: null });
  },
}));
