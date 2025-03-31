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
  fetchTodoItems: () => Promise<void>;
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
  fetchTodoItems: async () => {
    set({ loading: true });
    try {
      const response = await TodoItemService.fetchTodoItems();
      if (response && response.data && response.data.lists) {
        set({ todoItemList: response.data.lists, loading: false });
      }
    } catch (error: unknown) {
      console.error(error);
      set({ loading: false });
    }
  },
}));
