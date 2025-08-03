import { SpecificationModel } from "./SpecificationModel";

export const getAllSpecifications = (
  specs: SpecificationModel[]
): SpecificationModel[] => {
  const allSpecs: SpecificationModel[] = [];

  const addSpecsRecursively = (specList: SpecificationModel[]) => {
    specList.forEach((spec) => {
      allSpecs.push(spec);
      if (spec.children && spec.children.length > 0) {
        addSpecsRecursively(spec.children);
      }
    });
  };

  addSpecsRecursively(specs);
  return allSpecs;
};
