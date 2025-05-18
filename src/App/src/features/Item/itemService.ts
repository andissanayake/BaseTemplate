import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import { Item } from "./ItemModel";

export class ItemService {
  static async fetchItems(
    pageNumber: number,
    pageSize: number
  ): Promise<Result<{ items: Item[]; totalCount: number }>> {
    return await handleApi(
      axiosInstance.get(
        `/api/items?PageNumber=${pageNumber}&PageSize=${pageSize}`
      )
    );
  }

  static async fetchItemById(id: string): Promise<Result<Item>> {
    return await handleApi(axiosInstance.get(`/api/items/${id}`));
  }

  static async createItem(data: Item): Promise<Result<number>> {
    return await handleApi(axiosInstance.post("/api/items", data));
  }

  static async updateItem(data: Item): Promise<Result<boolean>> {
    return await handleApi(axiosInstance.put(`/api/items/${data.id}`, data));
  }

  static async deleteItem(id: number): Promise<Result<boolean>> {
    return await handleApi(axiosInstance.delete(`/api/items/${id}`));
  }
}
