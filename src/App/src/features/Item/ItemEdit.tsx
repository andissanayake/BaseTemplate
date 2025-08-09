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

const ItemEdit: React.FC = () => {
  const { setLoading } = useItemStore();
  const navigate = useNavigate();
  const { itemId } = useParams();
  const [currentStep, setCurrentStep] = useState(0);

  const basicInfoRef = useRef<ItemBasicInfoStepHandle>(null);
  const characteristicTypesRef =
    useRef<ItemCharacteristicTypesStepHandle>(null);

  if (!itemId) throw new Error("itemId is required");

  const handleStepSave = () => {
    if (currentStep === 0) {
      basicInfoRef.current?.save();
    } else if (currentStep === 1) {
      characteristicTypesRef.current?.save();
    }
  };

  const handleStepSuccess = () => {
    if (currentStep === 0) {
      setCurrentStep(1);
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
              Save & Finish
            </Button>
          )}
        </Space>
      </div>
    </>
  );
};

export { ItemEdit };
