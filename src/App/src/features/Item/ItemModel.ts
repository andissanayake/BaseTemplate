export interface Item {
  id: number;
  tenantId: number;
  name: string;
  description?: string;
  price: number;
  category?: string;
  isActive: boolean;
}
