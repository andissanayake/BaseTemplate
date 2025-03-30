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

export class TodoGroupService {
  static async fetchTodoGroups() {
    return await axiosInstance.get<{ lists: TodoGroup[] }>("/api/todoLists");
  }

  static async fetchTodoGroupById(id: string) {
    return await axiosInstance.get<TodoGroup>(`/api/todoLists/${id}`);
  }

  static async createTodoGroup(data: TodoGroup) {
    return await axiosInstance.post<number>("/api/todoLists", data);
  }

  static async updateTodoGroup(data: TodoGroup) {
    return await axiosInstance.put(`/api/todoLists/${data.id}`, data);
  }

  static async deleteTodoGroup(data: TodoGroup) {
    return await axiosInstance.delete(`/api/todoLists/${data.id}`);
  }
}
