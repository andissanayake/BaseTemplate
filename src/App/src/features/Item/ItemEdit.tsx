import React, { useState, useRef } from "react";
import { Button, Space, Typography, Steps } from "antd";
import { useItemStore } from "./itemStore";
import { useNavigate, useParams } from "react-router-dom";
import {
  ItemBasicInfoStep,
  ItemBasicInfoStepHandle,
} from "./ItemBasicInfoStep";
import {
  ItemCharacteristicTypesStep,
  ItemCharacteristicTypesStepHandle,
} from "./ItemCharacteristicTypesStep";
import { ItemVariantsStep, ItemVariantsStepHandle } from "./ItemVariantsStep";

const ItemEdit: React.FC = () => {
  const { setLoading } = useItemStore();
  const navigate = useNavigate();
  const { itemId } = useParams();
  const [currentStep, setCurrentStep] = useState(0);

  const basicInfoRef = useRef<ItemBasicInfoStepHandle>(null);
  const characteristicTypesRef =
    useRef<ItemCharacteristicTypesStepHandle>(null);
  const variantsRef = useRef<ItemVariantsStepHandle>(null);

  if (!itemId) throw new Error("itemId is required");

  const handleStepSave = () => {
    if (currentStep === 0) {
      basicInfoRef.current?.save();
    } else if (currentStep === 1) {
      characteristicTypesRef.current?.save();
    } else if (currentStep === 2) {
      // Variants step doesn't require saving, just navigate
      navigate(`/items`);
    }
  };

  const handleStepSuccess = () => {
    if (currentStep === 0) {
      setCurrentStep(1);
    } else if (currentStep === 1) {
      setCurrentStep(2);
      // Refresh variants when moving to variants step
      setTimeout(() => {
        variantsRef.current?.refresh();
      }, 100);
    } else {
      navigate(`/items`);
    }
  };

  const steps = [
    {
      title: "Basic Information",
      content: (
        <ItemBasicInfoStep
          ref={basicInfoRef}
          itemId={itemId}
          onSaveSuccess={handleStepSuccess}
          onLoadingChange={setLoading}
        />
      ),
    },
    {
      title: "Characteristic Types",
      content: (
        <ItemCharacteristicTypesStep
          ref={characteristicTypesRef}
          itemId={itemId}
          onSaveSuccess={handleStepSuccess}
          onLoadingChange={setLoading}
        />
      ),
    },
    {
      title: "Item Variants",
      content: (
        <ItemVariantsStep
          ref={variantsRef}
          itemId={itemId}
          onLoadingChange={setLoading}
        />
      ),
    },
  ];

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Edit Item
        </Typography.Title>
      </Space>

      <Steps current={currentStep} items={steps} style={{ marginBottom: 24 }} />

      <div style={{ marginBottom: 24 }}>{steps[currentStep].content}</div>

      <div style={{ textAlign: "right" }}>
        <Space>
          <Button onClick={() => navigate(`/items`)}>Cancel</Button>
          {currentStep > 0 && (
            <Button onClick={() => setCurrentStep(currentStep - 1)}>
              Previous
            </Button>
          )}
          {currentStep === 0 && (
            <Button type="primary" onClick={handleStepSave}>
              Next
            </Button>
          )}
          {currentStep === 1 && (
            <Button type="primary" onClick={handleStepSave}>
              Next
            </Button>
          )}
          {currentStep === 2 && (
            <Button type="primary" onClick={handleStepSave}>
              Finish
            </Button>
          )}
        </Space>
      </div>
    </>
  );
};

export { ItemEdit };
