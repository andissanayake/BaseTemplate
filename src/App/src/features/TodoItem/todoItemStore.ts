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
  updateItemStatus: (id: number, done: boolean) => Promise<void>;
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
  updateItemStatus: async (id: number, done: boolean) => {
    set({ loading: true });
    const res = await TodoItemService.updateTodoItemStatus({
      id: id,
      done: done,
    });
    if (res) {
      set((state) => {
        const updatedTodoItemList = state.todoItemList.map((item) =>
          item.id === id ? { ...item, done: done } : item
        );
        return { todoItemList: updatedTodoItemList };
      });
    }
    set({ loading: false });
  },
}));
