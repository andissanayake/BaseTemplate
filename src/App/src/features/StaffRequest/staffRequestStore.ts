import { create } from "zustand";
import { StaffRequestDto } from "./StaffRequestModel";

interface StaffRequestState {
  staffRequests: StaffRequestDto[];
  loading: boolean;

  // Actions
  setStaffRequests: (requests: StaffRequestDto[]) => void;
  setLoading: (loading: boolean) => void;
}

export const useStaffRequestStore = create<StaffRequestState>((set) => ({
  staffRequests: [],
  loading: false,

  setStaffRequests: (requests: StaffRequestDto[]) => {
    set({ staffRequests: requests });
  },

  setLoading: (loading: boolean) => {
    set({ loading });
  },
}));
