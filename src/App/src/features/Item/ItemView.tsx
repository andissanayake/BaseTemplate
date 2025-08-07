import React, { useEffect } from "react";
import { Card, Descriptions, Space, Tag, Typography, Button } from "antd";
import { useParams, useNavigate } from "react-router-dom";
import { useItemStore } from "./itemStore";
import { apiClient } from "../../common/apiClient";
import { Item } from "./ItemModel";
import { notification } from "antd";

const ItemView: React.FC = () => {
  const { setLoading } = useItemStore();
  const navigate = useNavigate();
  const { itemId } = useParams();

  if (!itemId) throw new Error("itemId is required");

  const [item, setItem] = React.useState<Item | null>(null);

  useEffect(() => {
    const fetchItem = async () => {
      setLoading(true);
      apiClient.get<Item>(`/api/item/${itemId}`, {
        onSuccess: (data) => {
          if (data) {
            setItem(data);
          }
        },
        onServerError: () => {
          notification.error({ message: "Failed to fetch item!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    };
    fetchItem();
  }, [itemId]);

  if (!item) {
    return (
      <Card>
        <Typography.Text>Loading...</Typography.Text>
      </Card>
    );
  }

  const tags = item.tags ? item.tags.split(",").filter(Boolean) : [];

  return (
    <Card>
      <Space
        className="mb-4"
        style={{ width: "100%", justifyContent: "space-between" }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          Item Details
        </Typography.Title>
        <Space>
          <Button
            type="primary"
            onClick={() => navigate(`/items/edit/${itemId}`)}
          >
            Edit
          </Button>
        </Space>
      </Space>

      <Descriptions column={1} bordered styles={{ label: { width: 120 } }}>
        <Descriptions.Item label="ID">{item.id}</Descriptions.Item>

        <Descriptions.Item label="Name">{item.name || "-"}</Descriptions.Item>

        <Descriptions.Item label="Description">
          {item.description || "-"}
        </Descriptions.Item>

        <Descriptions.Item label="Price">
          ${item.price?.toFixed(2) || "0.00"}
        </Descriptions.Item>

        <Descriptions.Item label="Specification">
          {item.specificationFullPath || "No specification assigned"}
        </Descriptions.Item>

        <Descriptions.Item label="Tags">
          {tags.length > 0 ? (
            <Space wrap>
              {tags.map((tag: string) => (
                <Tag color="blue" key={tag}>
                  {tag.trim()}
                </Tag>
              ))}
            </Space>
          ) : (
            "No tags assigned"
          )}
        </Descriptions.Item>

        <Descriptions.Item label="Status">
          <Tag color={item.isActive ? "green" : "red"}>
            {item.isActive ? "Active" : "Inactive"}
          </Tag>
        </Descriptions.Item>
      </Descriptions>
    </Card>
  );
};

export { ItemView };
