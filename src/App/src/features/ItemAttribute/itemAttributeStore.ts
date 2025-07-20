import { create } from "zustand";
import { ItemAttribute } from "./ItemAttributeModel";

interface ItemAttributeStore {
  itemAttributeList: ItemAttribute[];
  loading: boolean;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  setItemAttributeList: (list: ItemAttribute[]) => void;
  setLoading: (loading: boolean) => void;
  setTotalCount: (count: number) => void;
  setCurrentPage: (page: number) => void;
  setPagination: (page: number, pageSize: number) => void;
}

export const useItemAttributeStore = create<ItemAttributeStore>((set) => ({
  itemAttributeList: [],
  loading: false,
  totalCount: 0,
  currentPage: 1,
  pageSize: 10,
  setItemAttributeList: (list) => set({ itemAttributeList: list }),
  setLoading: (loading) => set({ loading }),
  setTotalCount: (count) => set({ totalCount: count }),
  setCurrentPage: (page) => set({ currentPage: page }),
  setPagination: (page, pageSize) => set({ currentPage: page, pageSize }),
}));
