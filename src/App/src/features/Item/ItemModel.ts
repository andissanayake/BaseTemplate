export interface Item {
  id: number;
  name: string;
  code: string;
  description?: string;
  tags?: string;
  isActive: boolean;
  specificationId: number;
  specificationFullPath: string;
  hasVariant?: boolean;
}
