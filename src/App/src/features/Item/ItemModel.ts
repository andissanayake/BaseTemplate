export interface Item {
  id: number;
  name: string;
  description?: string;
  price: number;
  category?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}
