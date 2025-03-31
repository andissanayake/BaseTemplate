/* eslint-disable @typescript-eslint/no-explicit-any */
import { create } from "zustand";
import { TodoItem, TodoItemService } from "./todoItemService";

// Define the state interface
interface TodoItemState {
  todoItemList: TodoItem[];
  totalCount: number;
  loading: boolean;
  editTodoItem: TodoItem | null;
  setTodoItemEdit: (data: TodoItem | null) => void;
  cleanTodoItemEdit: () => void;
  fetchTodoItems: (
    listId: number,
    pageNumber: number,
    pageSize: number
  ) => Promise<void>;
}

export const useTodoItemStore = create<TodoItemState>((set) => ({
  todoItemList: [],
  loading: false,
  editTodoItem: null,
  totalCount: 0,
  setTodoItemEdit: (data: TodoItem | null) => set({ editTodoItem: data }),
  cleanTodoItemEdit: () => set({ editTodoItem: null }),
  fetchTodoItems: async (
    listId: number,
    pageNumber: number,
    pageSize: number
  ) => {
    set({ loading: true });
    try {
      const response = await TodoItemService.fetchTodoItems(
        listId,
        pageNumber,
        pageSize
      );
      if (response && response.data && response.data) {
        set({
          todoItemList: response.data.items,
          loading: false,
          totalCount: response.data.totalCount,
        });
      }
    } catch (error: unknown) {
      console.error(error);
      set({ loading: false });
    }
  },
}));
