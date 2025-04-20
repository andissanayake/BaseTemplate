import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import { TodoGroup } from "./Model";

export class TodoGroupService {
  static async fetchTodoGroups(): Promise<Result<{ lists: TodoGroup[] }>> {
    return await handleApi(axiosInstance.get("/api/todoLists"));
  }

  static async fetchTodoGroupById(id: string): Promise<Result<TodoGroup>> {
    return await handleApi(axiosInstance.get(`/api/todoLists/${id}`));
  }

  static async createTodoGroup(data: TodoGroup): Promise<Result<number>> {
    return await handleApi(axiosInstance.post("/api/todoLists", data));
  }

  static async updateTodoGroup(data: TodoGroup): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.put(`/api/todoLists/${data.id}`, data)
    );
  }

  static async deleteTodoGroup(data: TodoGroup): Promise<Result<boolean>> {
    return await handleApi(axiosInstance.delete(`/api/todoLists/${data.id}`));
  }

  static getColours() {
    const predefinedColors = [
      { label: "White", value: "#FFFFFF" },
      { label: "Red", value: "#FF5733" },
      { label: "Orange", value: "#FFC300" },
      { label: "Yellow", value: "#FFFF66" },
      { label: "Green", value: "#CCFF99" },
      { label: "Blue", value: "#6666FF" },
      { label: "Purple", value: "#9966CC" },
      { label: "Grey", value: "#999999" },
    ];
    return predefinedColors;
  }
}
