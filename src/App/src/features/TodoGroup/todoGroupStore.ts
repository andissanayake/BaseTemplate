/* eslint-disable @typescript-eslint/no-explicit-any */
import { create } from "zustand";
import { TodoGroupService } from "./todoGroupService";
import { TodoGroup } from "./Model";

// Define the state interface
interface TodoGroupState {
  todoGroupList: TodoGroup[];
  loading: boolean;
  currentTodoGroup: TodoGroup | null;
  setTodoGroupCurrent: (
    data: TodoGroup | null,
    displayMode: "edit" | "view" | null
  ) => void;
  fetchTodoGroups: () => Promise<void>;
}

export const useTodoGroupStore = create<TodoGroupState>((set) => ({
  todoGroupList: [],
  loading: false,
  editId: null,
  currentTodoGroup: null,
  setTodoGroupCurrent: (data: TodoGroup | null) =>
    set({ currentTodoGroup: data }),

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
