export interface Item {
  id: number;
  name: string;
  description?: string;
  price: number;
  quantity: number;
  category?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}
