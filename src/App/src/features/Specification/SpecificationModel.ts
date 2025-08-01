export interface SpecificationModel {
  id: number;
  name: string;
  description: string;
  parentSpecificationId?: number;
  fullPath: string;
  children: SpecificationModel[];
}

export interface SpecificationCreateModel {
  name: string;
  description: string;
  parentSpecificationId?: number;
}

export interface SpecificationUpdateModel {
  id: number;
  name: string;
  description: string;
  parentSpecificationId?: number;
}
