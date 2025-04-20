import { create } from "zustand";
import { TodoGroupService } from "./todoGroupService";
import { TodoGroup } from "./Model";
import { handleResult } from "../../common/handleResult";

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
    const response = await TodoGroupService.fetchTodoGroups();
    handleResult(response, {
      onSuccess: (data) => {
        set({ todoGroupList: data?.lists ?? [] });
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },

  createTodoGroup: async (data) => {
    set({ loading: true });
    const response = await TodoGroupService.createTodoGroup(data);
    handleResult(response, {
      onSuccess: () => {
        useTodoGroupStore.getState().fetchTodoGroups();
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },

  updateTodoGroup: async (data) => {
    set({ loading: true });
    const response = await TodoGroupService.updateTodoGroup(data);
    handleResult(response, {
      onSuccess: () => {
        useTodoGroupStore.getState().fetchTodoGroups();
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },

  deleteTodoGroup: async (data) => {
    set({ loading: true });
    const response = await TodoGroupService.deleteTodoGroup(data);
    handleResult(response, {
      onSuccess: () => {
        useTodoGroupStore.getState().fetchTodoGroups();
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },
  setTodoGroupCurrent: (data: TodoGroup | null) =>
    set({ currentTodoGroup: data }),
}));
