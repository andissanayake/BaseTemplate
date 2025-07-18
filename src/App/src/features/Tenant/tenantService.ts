import axiosInstance from "../../auth/axiosInstance";
import { handleApi } from "../../common/handleApi";
import { Result } from "../../common/result";
import { Tenant } from "./TenantModel";

export class TenantService {
  static async fetchTenant(): Promise<Result<Tenant>> {
    return await handleApi(axiosInstance.get(`/api/tenants`));
  }

  static async createTenant(data: Tenant): Promise<Result<number>> {
    return await handleApi(axiosInstance.post("/api/tenants", data));
  }

  static async updateTenant(data: Tenant): Promise<Result<boolean>> {
    // Only send name and address
    const payload = { name: data.name, address: data.address };
    return await handleApi(axiosInstance.put(`/api/tenants`, payload));
  }
}
