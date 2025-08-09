import {
  useEffect,
  useState,
  forwardRef,
  useImperativeHandle,
  useCallback,
} from "react";
import {
  Typography,
  Row,
  Col,
  Card,
  Space,
  Switch,
  notification,
  Alert,
  Button,
} from "antd";
import { CharacteristicType, ItemCharacteristicType } from "./ItemTypes";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { Item } from "./ItemModel";

interface ItemCharacteristicTypesStepProps {
  itemId: string;
  onSaveSuccess: () => void;
  onLoadingChange: (loading: boolean) => void;
}

export interface ItemCharacteristicTypesStepHandle {
  save: () => void;
}

const ItemCharacteristicTypesStep = forwardRef<
  ItemCharacteristicTypesStepHandle,
  ItemCharacteristicTypesStepProps
>(({ itemId, onSaveSuccess, onLoadingChange }, ref) => {
  const [characteristicTypes, setCharacteristicTypes] = useState<
    CharacteristicType[]
  >([]);
  const [selectedCharacteristicTypes, setSelectedCharacteristicTypes] =
    useState<number[]>([]);
  const [isSelectionEnabled, setIsSelectionEnabled] = useState(false);

  useEffect(() => {
    loadCharacteristicTypes();
  }, []);

  const loadCharacteristicTypes = () => {
    apiClient.get<CharacteristicType[]>("/api/characteristic-type", {
      onSuccess: (data) => {
        if (data) {
          setCharacteristicTypes(data);
        }
      },
      onServerError: () => {
        notification.error({ message: "Failed to load characteristic types!" });
      },
    });
  };

  const handleCharacteristicTypeToggle = (
    characteristicTypeId: number,
    checked: boolean
  ) => {
    if (!isSelectionEnabled) return;

    if (checked) {
      setSelectedCharacteristicTypes((prev) => [...prev, characteristicTypeId]);
    } else {
      setSelectedCharacteristicTypes((prev) =>
        prev.filter((id) => id !== characteristicTypeId)
      );
    }
  };

  const handleEnableSelection = () => {
    setIsSelectionEnabled(true);
  };

  const handleSave = useCallback(() => {
    onLoadingChange(true);
    const command = {
      itemId: +itemId,
      characteristicTypeIds: selectedCharacteristicTypes,
    };

    apiClient.put<boolean>("/api/item/characteristic-type", command, {
      onSuccess: () => {
        notification.success({
          message: "Item characteristic types updated successfully!",
        });
        onSaveSuccess();
      },
      onServerError: () => {
        notification.error({
          message: "Failed to update characteristic types!",
        });
      },
      onFinally: () => {
        onLoadingChange(false);
      },
    });
  }, [itemId, selectedCharacteristicTypes, onLoadingChange, onSaveSuccess]);

  useAsyncEffect(async () => {
    onLoadingChange(true);
    apiClient.get<Item & { characteristicTypes: ItemCharacteristicType[] }>(
      `/api/item/${itemId}`,
      {
        onSuccess: (data) => {
          if (data) {
            // Set initially selected characteristic types
            const currentCharacteristicTypeIds =
              data.characteristicTypes?.map((ct) => ct.characteristicTypeId) ||
              [];
            setSelectedCharacteristicTypes(currentCharacteristicTypeIds);
          }
        },
        onServerError: () => {
          notification.error({ message: "Failed to fetch item!" });
        },
        onFinally: () => {
          onLoadingChange(false);
        },
      }
    );
  }, [itemId]);

  // Expose save method to parent
  useImperativeHandle(
    ref,
    () => ({
      save: handleSave,
    }),
    [handleSave]
  );

  return (
    <div>
      <Typography.Title level={4}>Select Characteristic Types</Typography.Title>
      <Typography.Paragraph>
        Choose which characteristic types should be associated with this item
        for variant generation.
      </Typography.Paragraph>

      {!isSelectionEnabled && (
        <Alert
          message="Characteristic Type Selection Disabled"
          description={
            <div>
              <p>
                If you change characteristic selection, your existing item
                variants will be discontinued and new item variants will be
                generated.
              </p>
              <Button
                type="primary"
                size="small"
                onClick={handleEnableSelection}
                style={{ marginTop: 8 }}
              >
                Enable Characteristic Selection
              </Button>
            </div>
          }
          type="warning"
          showIcon
          style={{ marginBottom: 16 }}
        />
      )}

      <Row gutter={[16, 16]}>
        {characteristicTypes.map((characteristicType) => (
          <Col xs={24} sm={12} md={8} key={characteristicType.id}>
            <Card size="small">
              <Space direction="vertical" style={{ width: "100%" }}>
                <div
                  style={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                  }}
                >
                  <div>
                    <Typography.Text strong>
                      {characteristicType.name}
                    </Typography.Text>
                    {characteristicType.description && (
                      <div>
                        <Typography.Text
                          type="secondary"
                          style={{ fontSize: "12px" }}
                        >
                          {characteristicType.description}
                        </Typography.Text>
                      </div>
                    )}
                  </div>
                  <Switch
                    checked={selectedCharacteristicTypes.includes(
                      characteristicType.id
                    )}
                    disabled={!isSelectionEnabled}
                    onChange={(checked) =>
                      handleCharacteristicTypeToggle(
                        characteristicType.id,
                        checked
                      )
                    }
                  />
                </div>
              </Space>
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
});

ItemCharacteristicTypesStep.displayName = "ItemCharacteristicTypesStep";

export { ItemCharacteristicTypesStep };
