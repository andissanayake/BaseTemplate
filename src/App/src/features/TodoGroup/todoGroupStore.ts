import { create } from "zustand";
import axiosInstance from "../../auth/axiosInstance";

export enum PriorityLevel {
  None = 0,
  Low = 1,
  Medium = 2,
  High = 3,
}

export interface TodoGroup {
  id: number;
  title?: string;
  colour?: string;
}

// Define the state interface
interface TodoGroupState {
  todoGroupList: TodoGroup[];
  loading: boolean;
  editTodoGroup: TodoGroup | null;
  setTodoGroupEdit: (data: TodoGroup | null) => void;
  cleanTodoGroupEdit: () => void;
  fetchTodoGroups: () => Promise<void>;
  fetchTodoGroupById: (id: string) => Promise<void>;
  createTodoGroup: (data: TodoGroup) => Promise<void>;
  updateTodoGroup: (data: TodoGroup) => Promise<void>;
  deleteTodoGroup: (data: TodoGroup) => Promise<void>;
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
      const response = await axiosInstance.get<{ lists: TodoGroup[] }>(
        "/api/todoLists"
      );
      if (response && response.data && response.data.lists) {
        set({ todoGroupList: response.data.lists, loading: false });
      }
    } catch (error: unknown) {
      console.log(error);
      set({ loading: false });
    }
  },

  fetchTodoGroupById: async (id: string) => {
    set({ loading: true });
    try {
      const response = await axiosInstance.get<TodoGroup>(
        `/api/todoLists/${id}`
      );
      if (response && response.data && response.data) {
        set({ editTodoGroup: response.data, loading: false });
      }
    } catch (error: unknown) {
      set({ loading: false });
      console.log(error);
    }
  },

  createTodoGroup: async (data: TodoGroup) => {
    try {
      const response = await axiosInstance.post<number>("/api/todoLists", data);
      if (response && response.data) {
        await useTodoGroupStore.getState().fetchTodoGroups();
      }
    } catch (error) {
      console.error("Error creating todo group:", error);
    }
  },

  updateTodoGroup: async (data: TodoGroup) => {
    try {
      const response = await axiosInstance.put(
        `/api/todoLists/${data.id}`,
        data
      );
      if (response) {
        await useTodoGroupStore.getState().fetchTodoGroups();
      }
    } catch (error) {
      console.error("Error updating todo group:", error);
    }
  },

  deleteTodoGroup: async (data: TodoGroup) => {
    try {
      const response = await axiosInstance.delete(`/api/todoLists/${data.id}`);
      if (response) {
        await useTodoGroupStore.getState().fetchTodoGroups();
      }
    } catch (error) {
      console.error("Error deleting todo group:", error);
    }
  },
}));
