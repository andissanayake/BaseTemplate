import { create } from "zustand";
import { TodoItem } from "./TodoItemModel";

interface TodoItemState {
  todoItemList: TodoItem[];
  totalCount: number;
  loading: boolean;
  currentPage: number;
  pageSize: number;
  setPagination: (page: number, size: number) => void;
  setLoading: (loading: boolean) => void;
  setTotalCount: (count: number) => void;
  setTodoItemList: (list: TodoItem[]) => void;
  setCurrentPage: (page: number) => void;
}

export const useTodoItemStore = create<TodoItemState>((set) => ({
  todoItemList: [],
  totalCount: 0,
  loading: false,
  currentPage: 1,
  pageSize: 5,
  setPagination: (page, size) => set({ currentPage: page, pageSize: size }),
  setLoading: (loading) => set({ loading: loading }),
  setTotalCount: (count) => set({ totalCount: count }),
  setTodoItemList: (list) => set({ todoItemList: list }),
  setCurrentPage: (page) => set({ currentPage: page }),
}));
