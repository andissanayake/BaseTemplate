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
  createTodoGroup: (data: TodoGroup) => Promise<boolean>;
  updateTodoGroup: (data: TodoGroup) => Promise<boolean>;
  deleteTodoGroup: (data: TodoGroup) => Promise<void>;
  createErrors: Record<string, string[]>;
  updateErrors: Record<string, string[]>;
  getTodoGroupById: (id: string) => Promise<boolean>;
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
    let success = false;
    const response = await TodoGroupService.createTodoGroup(data);
    handleResult(response, {
      onSuccess: () => {
        success = true;
        useTodoGroupStore.getState().fetchTodoGroups();
      },
      onValidationError: (errors) => {
        console.log("errors", errors);
        set({ createErrors: errors });
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
    return success;
  },

  updateTodoGroup: async (data) => {
    set({ loading: true });
    let success = false;
    const response = await TodoGroupService.updateTodoGroup(data);
    handleResult(response, {
      onSuccess: () => {
        success = true;
        useTodoGroupStore.getState().fetchTodoGroups();
      },
      onValidationError: (errors) => {
        console.log("errors", errors);
        set({ updateErrors: errors });
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
    return success;
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
  createErrors: {},
  updateErrors: {},
  getTodoGroupById: async (id: string) => {
    set({ loading: true });
    let success = false;
    const response = await TodoGroupService.fetchTodoGroupById(id);
    handleResult(response, {
      onSuccess: (data) => {
        set({ currentTodoGroup: data });
        success = true;
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
    return success;
  },
}));
