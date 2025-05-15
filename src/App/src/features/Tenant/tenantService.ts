import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import { Tenant } from "./TenantModel";

export class TenantService {
  static async fetchTenantById(id: string): Promise<Result<Tenant>> {
    return await handleApi(axiosInstance.get(`/api/tenants/${id}`));
  }

  static async createTenant(data: Tenant): Promise<Result<number>> {
    return await handleApi(axiosInstance.post("/api/tenants", data));
  }

  static async updateTenant(data: Tenant): Promise<Result<boolean>> {
    return await handleApi(axiosInstance.put(`/api/tenants/${data.id}`, data));
  }
}
