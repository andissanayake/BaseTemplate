export interface Characteristic {
  id: number;
  name: string;
  code: string;
  value: string;
  isActive: boolean;
  characteristicTypeId: number;
  characteristicTypeName: string;
}

export interface CreateCharacteristicRequest {
  name: string;
  code: string;
  value: string;
  characteristicTypeId: number;
}

export interface UpdateCharacteristicRequest {
  id: number;
  name: string;
  code: string;
  value: string;
}
