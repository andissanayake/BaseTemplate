import { create } from "zustand";
import { Tenant } from "./TenantModel";

interface TenantState {
  currentTenant: Tenant | null;
  loading: boolean;
  setLoading: (loading: boolean) => void;
  setCurrentTenant: (tenant: Tenant | null) => void;
}

export const useTenantStore = create<TenantState>((set) => ({
  currentTenant: null,
  loading: false,
  setLoading: (loading: boolean) => set({ loading }),
  setCurrentTenant: (tenant: Tenant | null) => set({ currentTenant: tenant }),
}));
