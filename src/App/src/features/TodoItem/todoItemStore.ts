/* eslint-disable @typescript-eslint/no-explicit-any */
import { create } from "zustand";
import { TodoGroup, TodoGroupService } from "./todoItemService";

// Define the state interface
interface TodoGroupState {
  todoGroupList: TodoGroup[];
  loading: boolean;
  editTodoGroup: TodoGroup | null;
  setTodoGroupEdit: (data: TodoGroup | null) => void;
  cleanTodoGroupEdit: () => void;
  fetchTodoGroups: () => Promise<void>;
}

// Create the Zustand store
export const useTodoGroupStore = create<TodoGroupState>((set) => ({
  todoGroupList: [],
  loading: false,
  editId: null,
  editTodoGroup: null,
  setTodoGroupEdit: (data: TodoGroup | null) => set({ editTodoGroup: data }),
  cleanTodoGroupEdit: () => set({ editTodoGroup: null }),

  fetchTodoGroups: async () => {
    set({ loading: true });
    try {
      const response = await TodoGroupService.fetchTodoGroups();
      if (response && response.data && response.data.lists) {
        set({ todoGroupList: response.data.lists, loading: false });
      }
    } catch (error: unknown) {
      console.error(error);
      set({ loading: false });
    }
  },
}));
