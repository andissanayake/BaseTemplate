import React, { useEffect } from "react";
import { Card, Descriptions, Space, Tag, Typography, Button } from "antd";
import { useParams, useNavigate } from "react-router-dom";
import { useItemAttributeTypeStore } from "./itemAttributeTypeStore";
import { apiClient } from "../../common/apiClient";
import { useAuthStore } from "../../auth/authStore";
import { ItemAttributeType } from "./ItemAttributeTypeModel";
import { notification } from "antd";
import ItemAttributeDashboard from "../ItemAttribute/ItemAttributeDashboard";

const ItemAttributeTypeView: React.FC = () => {
  const { setLoading } = useItemAttributeTypeStore();
  const { tenant } = useAuthStore();
  const navigate = useNavigate();
  const { itemAttributeTypeId } = useParams();

  if (!itemAttributeTypeId) throw new Error("itemAttributeTypeId is required");
  if (!tenant?.id) throw new Error("Tenant ID is required");

  const [itemAttributeType, setItemAttributeType] =
    React.useState<ItemAttributeType | null>(null);

  useEffect(() => {
    const fetchItemAttributeType = async () => {
      setLoading(true);
      apiClient.get<ItemAttributeType>(
        `/api/itemAttributeTypes/${itemAttributeTypeId}`,
        {
          onSuccess: (data) => {
            if (data) {
              setItemAttributeType(data);
            }
          },
          onServerError: () => {
            notification.error({
              message: "Failed to fetch item attribute type!",
            });
          },
          onFinally: () => {
            setLoading(false);
          },
        }
      );
    };
    fetchItemAttributeType();
  }, [itemAttributeTypeId, tenant?.id]);

  if (!itemAttributeType) {
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
          Item Attribute Type Details
        </Typography.Title>
        <Space>
          <Button
            type="primary"
            onClick={() =>
              navigate(`/item-attribute-types/edit/${itemAttributeTypeId}`)
            }
          >
            Edit
          </Button>
        </Space>
      </Space>

      <Descriptions column={1} bordered styles={{ label: { width: 120 } }}>
        <Descriptions.Item label="ID">{itemAttributeType.id}</Descriptions.Item>

        <Descriptions.Item label="Name">
          {itemAttributeType.name || "-"}
        </Descriptions.Item>

        <Descriptions.Item label="Description">
          {itemAttributeType.description || "-"}
        </Descriptions.Item>

        <Descriptions.Item label="Status">
          <Tag color={itemAttributeType.isActive ? "green" : "red"}>
            {itemAttributeType.isActive ? "Active" : "Inactive"}
          </Tag>
        </Descriptions.Item>
      </Descriptions>

      {/* Item Attributes Dashboard */}
      <ItemAttributeDashboard
        itemAttributeTypeId={parseInt(itemAttributeTypeId)}
      />
    </Card>
  );
};

export { ItemAttributeTypeView };
