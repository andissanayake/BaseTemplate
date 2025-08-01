import { create } from "zustand";
import { SpecificationModel } from "./SpecificationModel";

interface SpecificationState {
  specifications: SpecificationModel[];
  loading: boolean;
  setSpecifications: (specifications: SpecificationModel[]) => void;
  setLoading: (loading: boolean) => void;
}

export const useSpecificationStore = create<SpecificationState>((set) => ({
  specifications: [],
  loading: false,
  setSpecifications: (specifications) => set({ specifications }),
  setLoading: (loading) => set({ loading }),
}));
