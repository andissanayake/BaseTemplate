import axiosInstance from "../../auth/axiosInstance";

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
