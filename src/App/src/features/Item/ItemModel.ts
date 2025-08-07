export interface Item {
  id: number;
  name: string;
  description?: string;
  tags?: string;
  isActive: boolean;
  specificationId: number;
  specificationFullPath: string;
}
