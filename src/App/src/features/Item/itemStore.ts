import { create } from "zustand";
import { Item } from "./ItemModel";

interface ItemState {
  itemList: Item[];
  totalCount: number;
  loading: boolean;
  currentPage: number;
  pageSize: number;
  setPagination: (page: number, size: number) => void;
  setLoading: (loading: boolean) => void;
  setTotalCount: (count: number) => void;
  setItemList: (list: Item[]) => void;
  setCurrentPage: (page: number) => void;
}

export const useItemStore = create<ItemState>((set) => ({
  itemList: [],
  totalCount: 0,
  loading: false,
  currentPage: 1,
  pageSize: 5,
  setPagination: (page, size) => set({ currentPage: page, pageSize: size }),
  setLoading: (loading) => set({ loading: loading }),
  setTotalCount: (count) => set({ totalCount: count }),
  setItemList: (list) => set({ itemList: list }),
  setCurrentPage: (page) => set({ currentPage: page }),
}));
