import axiosInstance from "../../auth/axiosInstance";

export enum PriorityLevel {
  None = 0,
  Low = 1,
  Medium = 2,
  High = 3,
}

export interface TodoItem {
  id: number;
  title?: string;
}

export class TodoItemService {
  static async fetchTodoItems() {
    return await axiosInstance.get<{ lists: TodoItem[] }>("/api/todoLists");
  }

  static async fetchTodoItemById(id: string) {
    return await axiosInstance.get<TodoItem>(`/api/todoLists/${id}`);
  }

  static async createTodoItem(data: TodoItem) {
    return await axiosInstance.post<number>("/api/todoLists", data);
  }

  static async updateTodoItem(data: TodoItem) {
    return await axiosInstance.put(`/api/todoLists/${data.id}`, data);
  }

  static async deleteTodoItem(data: TodoItem) {
    return await axiosInstance.delete(`/api/todoLists/${data.id}`);
  }
}
