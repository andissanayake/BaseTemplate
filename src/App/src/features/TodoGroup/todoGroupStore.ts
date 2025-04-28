import { create } from "zustand";
import { TodoGroup } from "./TodoGroupModel";

interface TodoGroupState {
  todoGroupList: TodoGroup[];
  loading: boolean;
  setLoading: (loading: boolean) => void;
  setTodoGroupList: (list: TodoGroup[]) => void;
}

export const useTodoGroupStore = create<TodoGroupState>((set) => ({
  todoGroupList: [],
  loading: false,
  setLoading: (loading: boolean) => set({ loading }),
  setTodoGroupList: (list: TodoGroup[]) => set({ todoGroupList: list }),
}));
