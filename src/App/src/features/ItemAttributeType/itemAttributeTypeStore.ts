import { create } from "zustand";
import { ItemAttributeType } from "./ItemAttributeTypeModel";

interface ItemAttributeTypeState {
  itemAttributeTypeList: ItemAttributeType[];
  totalCount: number;
  loading: boolean;
  currentPage: number;
  pageSize: number;
  setPagination: (page: number, size: number) => void;
  setLoading: (loading: boolean) => void;
  setTotalCount: (count: number) => void;
  setItemAttributeTypeList: (list: ItemAttributeType[]) => void;
  setCurrentPage: (page: number) => void;
}

export const useItemAttributeTypeStore = create<ItemAttributeTypeState>(
  (set) => ({
    itemAttributeTypeList: [],
    totalCount: 0,
    loading: false,
    currentPage: 1,
    pageSize: 5,
    setPagination: (page, size) => set({ currentPage: page, pageSize: size }),
    setLoading: (loading) => set({ loading: loading }),
    setTotalCount: (count) => set({ totalCount: count }),
    setItemAttributeTypeList: (list) => set({ itemAttributeTypeList: list }),
    setCurrentPage: (page) => set({ currentPage: page }),
  })
);
