import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import { Item } from "./ItemModel";

export class ItemService {
  static async fetchItems(
    pageNumber: number,
    pageSize: number,
    tenantId: number
  ): Promise<Result<{ items: Item[]; totalCount: number }>> {
    return await handleApi(
      axiosInstance.get(
        `/api/tenants/${tenantId}/items?PageNumber=${pageNumber}&PageSize=${pageSize}&TenantId=${tenantId}`
      )
    );
  }

  static async fetchItemById(
    tenantId: string,
    id: string
  ): Promise<Result<Item>> {
    return await handleApi(
      axiosInstance.get(`/api/tenants/${tenantId}/items/${id}`)
    );
  }

  static async createItem(
    tenantId: string,
    data: Item
  ): Promise<Result<number>> {
    return await handleApi(
      axiosInstance.post(`/api/tenants/${tenantId}/items`, data)
    );
  }

  static async updateItem(
    tenantId: string,
    data: Item
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.put(`/api/tenants/${tenantId}/items/${data.id}`, data)
    );
  }

  static async deleteItem(
    tenantId: string,
    id: number
  ): Promise<Result<boolean>> {
    return await handleApi(
      axiosInstance.delete(`/api/tenants/${tenantId}/items/${id}`)
    );
  }
}
