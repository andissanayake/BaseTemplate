import { create } from "zustand";
import { CharacteristicType } from "./CharacteristicTypeModel";

interface CharacteristicTypeState {
  characteristicTypeList: CharacteristicType[];
  totalCount: number;
  loading: boolean;
  currentPage: number;
  pageSize: number;
  setPagination: (page: number, size: number) => void;
  setLoading: (loading: boolean) => void;
  setTotalCount: (count: number) => void;
  setCharacteristicTypeList: (list: CharacteristicType[]) => void;
  setCurrentPage: (page: number) => void;
}

export const useCharacteristicTypeStore = create<CharacteristicTypeState>(
  (set) => ({
    characteristicTypeList: [],
    totalCount: 0,
    loading: false,
    currentPage: 1,
    pageSize: 5,
    setPagination: (page, size) => set({ currentPage: page, pageSize: size }),
    setLoading: (loading) => set({ loading: loading }),
    setTotalCount: (count) => set({ totalCount: count }),
    setCharacteristicTypeList: (list) => set({ characteristicTypeList: list }),
    setCurrentPage: (page) => set({ currentPage: page }),
  })
);
