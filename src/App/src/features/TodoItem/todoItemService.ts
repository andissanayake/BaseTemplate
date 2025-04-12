import axiosInstance from "../../auth/axiosInstance";
import { PriorityLevel, TodoItem } from "./TodoItemModel";

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

  static async deleteTodoItem(id: number) {
    return await axiosInstance.delete(`/api/todoItems/${id}`);
  }
  static async updateTodoItemStatus(data: { id: number; done: boolean }) {
    return await axiosInstance.put(
      `/api/todoItems/updateItemStatus?id=${data.id}`,
      data
    );
  }
  static getPriorityLevels() {
    const priorityOptions = [
      { label: "None", value: PriorityLevel.None },
      { label: "Low", value: PriorityLevel.Low },
      { label: "Medium", value: PriorityLevel.Medium },
      { label: "High", value: PriorityLevel.High },
    ];
    return priorityOptions;
  }
}
