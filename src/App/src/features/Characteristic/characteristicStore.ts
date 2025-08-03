import { create } from "zustand";
import { Characteristic } from "./CharacteristicModel";

interface CharacteristicStore {
  characteristicList: Characteristic[];
  loading: boolean;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  setCharacteristicList: (list: Characteristic[]) => void;
  setLoading: (loading: boolean) => void;
  setTotalCount: (count: number) => void;
  setCurrentPage: (page: number) => void;
  setPagination: (page: number, pageSize: number) => void;
}

export const useCharacteristicStore = create<CharacteristicStore>((set) => ({
  characteristicList: [],
  loading: false,
  totalCount: 0,
  currentPage: 1,
  pageSize: 10,
  setCharacteristicList: (list) => set({ characteristicList: list }),
  setLoading: (loading) => set({ loading }),
  setTotalCount: (count) => set({ totalCount: count }),
  setCurrentPage: (page) => set({ currentPage: page }),
  setPagination: (page, pageSize) => set({ currentPage: page, pageSize }),
}));
