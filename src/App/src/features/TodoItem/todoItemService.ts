import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import { PriorityLevel, TodoItem } from "./TodoItemModel";

export class TodoItemService {
  static async fetchTodoItems(
    listId: number,
    pageNumber: number,
    pageSize: number
  ): Promise<Result<{ items: TodoItem[]; totalCount: number }>> {
    return await handleApi(
      axiosInstance.get(
        `/api/todoItems?ListId=${listId}&PageNumber=${pageNumber}&PageSize=${pageSize}`
      )
    );
  }

  static async fetchTodoItemById(id: string): Promise<Result<TodoItem>> {
    return await handleApi(axiosInstance.get(`/api/todoItems/${id}`));
  }

  static async createTodoItem(data: TodoItem): Promise<Result<number>> {
    return await handleApi(axiosInstance.post("/api/todoItems", data));
  }

  static async updateTodoItem(data: TodoItem): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.put(`/api/todoItems/${data.id}`, data)
    );
  }

  static async deleteTodoItem(id: number): Promise<Result<boolean>> {
    return await handleApi(axiosInstance.delete(`/api/todoItems/${id}`));
  }

  static async updateTodoItemStatus(data: {
    id: number;
    done: boolean;
  }): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.put(`/api/todoItems/updateItemStatus?id=${data.id}`, data)
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
