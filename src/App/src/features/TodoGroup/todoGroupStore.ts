import { create } from "zustand";
import { TodoGroupService } from "./todoGroupService";
import { TodoGroup } from "./Model";

interface TodoGroupState {
  todoGroupList: TodoGroup[];
  loading: boolean;
  currentTodoGroup: TodoGroup | null;
  setTodoGroupCurrent: (data: TodoGroup | null) => void;
  fetchTodoGroups: () => Promise<void>;
  createTodoGroup: (data: TodoGroup) => Promise<void>;
  updateTodoGroup: (data: TodoGroup) => Promise<void>;
  deleteTodoGroup: (data: TodoGroup) => Promise<void>;
}

export const useTodoGroupStore = create<TodoGroupState>((set) => ({
  todoGroupList: [],
  loading: false,
  currentTodoGroup: null,

  fetchTodoGroups: async () => {
    set({ loading: true });
    try {
      const response = await TodoGroupService.fetchTodoGroups();
      if (response && response.data && response.data.lists) {
        set({ todoGroupList: response.data.lists, loading: false });
      }
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  createTodoGroup: async (data) => {
    set({ loading: true });
    try {
      await TodoGroupService.createTodoGroup(data);
      await useTodoGroupStore.getState().fetchTodoGroups();
      set({ loading: false });
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  updateTodoGroup: async (data) => {
    set({ loading: true });
    try {
      await TodoGroupService.updateTodoGroup(data);
      await useTodoGroupStore.getState().fetchTodoGroups();
      set({ loading: false });
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  deleteTodoGroup: async (data) => {
    set({ loading: true });
    try {
      await TodoGroupService.deleteTodoGroup(data);
      await useTodoGroupStore.getState().fetchTodoGroups();
      set({ loading: false });
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },
  setTodoGroupCurrent: (data: TodoGroup | null) =>
    set({ currentTodoGroup: data }),
}));
