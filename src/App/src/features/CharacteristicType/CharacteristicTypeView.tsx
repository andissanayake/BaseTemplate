import React, { useEffect } from "react";
import { Card, Descriptions, Space, Tag, Typography, Button } from "antd";
import { useParams, useNavigate } from "react-router-dom";
import { useCharacteristicTypeStore } from "./characteristicTypeStore";
import { apiClient } from "../../common/apiClient";
import { CharacteristicType } from "./CharacteristicTypeModel";
import { notification } from "antd";
import { CharacteristicDashboard } from "../Characteristic/CharacteristicDashboard";

const CharacteristicTypeView: React.FC = () => {
  const { setLoading } = useCharacteristicTypeStore();
  const navigate = useNavigate();
  const { characteristicTypeId } = useParams();

  if (!characteristicTypeId)
    throw new Error("characteristicTypeId is required");

  const [characteristicType, setCharacteristicType] =
    React.useState<CharacteristicType | null>(null);

  useEffect(() => {
    const fetchCharacteristicType = async () => {
      setLoading(true);
      apiClient.get<CharacteristicType>(
        `/api/characteristic-type/${characteristicTypeId}`,
        {
          onSuccess: (data) => {
            if (data) {
              setCharacteristicType(data);
            }
          },
          onServerError: () => {
            notification.error({
              message: "Failed to fetch characteristic type!",
            });
          },
          onFinally: () => {
            setLoading(false);
          },
        }
      );
    };
    fetchCharacteristicType();
  }, [characteristicTypeId]);

  if (!characteristicType) {
    return (
      <Card>
        <Typography.Text>Loading...</Typography.Text>
      </Card>
    );
  }

  return (
    <Card>
      <Space
        className="mb-4"
        style={{ width: "100%", justifyContent: "space-between" }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          Characteristic Type Details
        </Typography.Title>
        <Space>
          <Button
            type="primary"
            onClick={() =>
              navigate(`/characteristic-types/edit/${characteristicTypeId}`)
            }
          >
            Edit
          </Button>
        </Space>
      </Space>

      <Descriptions column={1} bordered styles={{ label: { width: 120 } }}>
        <Descriptions.Item label="ID">
          {characteristicType.id}
        </Descriptions.Item>

        <Descriptions.Item label="Name">
          {characteristicType.name || "-"}
        </Descriptions.Item>

        <Descriptions.Item label="Description">
          {characteristicType.description || "-"}
        </Descriptions.Item>

        <Descriptions.Item label="Status">
          <Tag color={characteristicType.isActive ? "green" : "red"}>
            {characteristicType.isActive ? "Active" : "Inactive"}
          </Tag>
        </Descriptions.Item>
      </Descriptions>

      {/* Characteristics Dashboard */}
      <CharacteristicDashboard
        characteristicTypeId={parseInt(characteristicTypeId)}
      />
    </Card>
  );
};

export { CharacteristicTypeView };
