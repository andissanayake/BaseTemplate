export interface Item {
  id: number;
  tenantId: number;
  name: string;
  description?: string;
  price: number;
  tags?: string;
  isActive: boolean;
  specificationId: number;
  specificationFullPath: string;
}
