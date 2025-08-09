export interface ItemVariant {
  id: number;
  itemId: number;
  code: string;
  price: number;
  characteristics: ItemVariantCharacteristic[];
  item: ItemDetail;
}

export interface ItemDetail {
  id: number;
  name: string;
  code: string;
  description?: string;
  isActive: boolean;
  tags?: string;
  specificationId: number;
  specificationFullPath: string;
  hasVariant: boolean;
}

export interface ItemVariantCharacteristic {
  id: number;
  characteristicId: number;
  characteristicName: string;
  characteristicCode: string;
  characteristicValue?: string;
  characteristicTypeId: number;
  characteristicTypeName: string;
}
