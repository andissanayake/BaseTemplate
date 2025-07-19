import { create } from "zustand";
import { Tenant } from "./TenantModel";

interface TenantState {
  currentTenant: Tenant;
  loading: boolean;
  setLoading: (loading: boolean) => void;
  setCurrentTenant: (tenant: Tenant) => void;
  cleanCurrentTenant: () => void;
}

const defaultTenant: Tenant = {
  id: -1,
  name: "",
  address: "",
};
export const useTenantStore = create<TenantState>((set) => ({
  currentTenant: defaultTenant,
  loading: false,
  setLoading: (loading: boolean) => set({ loading }),
  setCurrentTenant: (tenant: Tenant) => set({ currentTenant: tenant }),
  cleanCurrentTenant: () => set({ currentTenant: defaultTenant }),
}));
