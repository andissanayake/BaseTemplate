export interface CharacteristicType {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface ItemCharacteristicType {
  id: number;
  characteristicTypeId: number;
  characteristicTypeName: string;
  characteristicTypeDescription?: string;
}
