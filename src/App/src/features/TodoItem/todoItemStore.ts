/* eslint-disable @typescript-eslint/no-explicit-any */
import { create } from "zustand";
import { TodoItem, TodoItemService } from "./todoItemService";

// Define the state interface
interface TodoItemState {
  listId: number;
  todoItemList: TodoItem[];
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

// Create the Zustand store
export const useTodoItemStore = create<TodoItemState>((set) => ({
  listId: 0,
  todoItemList: [],
  loading: false,
  editTodoItem: null,
  setListId: (id: number) => set({ listId: id }),
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
      if (response && response.data && response.data.items) {
        set({ todoItemList: response.data.items, loading: false });
      }
    } catch (error: unknown) {
      console.error(error);
      set({ loading: false });
    }
  },
}));
