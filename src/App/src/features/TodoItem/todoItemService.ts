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
  static async fetchTodoItems(
    listId: number,
    pageNumber: number,
    pageSize: number
  ) {
    return await axiosInstance.get<{ items: TodoItem[]; totalCount: number }>(
      `/api/todoItems?ListId=${listId}&PageNumber=${pageNumber}&PageSize=${pageSize}`
    );
  }

  static async fetchTodoItemById(id: string) {
    return await axiosInstance.get<TodoItem>(`/api/todoItems/${id}`);
  }

  static async createTodoItem(data: TodoItem) {
    return await axiosInstance.post<number>("/api/todoItems", data);
  }

  static async updateTodoItem(data: TodoItem) {
    return await axiosInstance.put(`/api/todoItems/${data.id}`, data);
  }

  static async deleteTodoItem(data: TodoItem) {
    return await axiosInstance.delete(`/api/todoItems/${data.id}`);
  }
}
