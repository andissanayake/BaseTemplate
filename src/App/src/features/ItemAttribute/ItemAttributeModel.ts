export interface ItemAttribute {
  id: number;
  name: string;
  code: string;
  value: string;
  isActive: boolean;
  itemAttributeTypeId: number;
  itemAttributeTypeName: string;
}

export interface CreateItemAttributeRequest {
  name: string;
  code: string;
  value: string;
  itemAttributeTypeId: number;
}

export interface UpdateItemAttributeRequest {
  id: number;
  name: string;
  code: string;
  value: string;
}
