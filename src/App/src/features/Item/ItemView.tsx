import React, { useEffect } from "react";
import { Card, Descriptions, Space, Tag, Typography, Button } from "antd";
import { useParams, useNavigate } from "react-router-dom";
import { useItemStore } from "./itemStore";
import { ItemService } from "./itemService";
import { handleResult } from "../../common/handleResult";
import { handleServerError } from "../../common/serverErrorHandler";
import { useAuthStore } from "../../auth/authStore";
import { Item } from "./ItemModel";

const ItemView: React.FC = () => {
  const { setLoading } = useItemStore();
  const { tenant } = useAuthStore();
  const navigate = useNavigate();
  const { itemId } = useParams();

  if (!itemId) throw new Error("itemId is required");
  if (!tenant?.id) throw new Error("Tenant ID is required");

  const [item, setItem] = React.useState<Item | null>(null);

  useEffect(() => {
    const fetchItem = async () => {
      const response = await ItemService.fetchItemById(tenant.id, itemId);
      setLoading(true);
      handleResult(response, {
        onSuccess: (data) => {
          if (data) {
            setItem(data);
          }
        },
        onServerError: (errors) => {
          handleServerError(errors, "Failed to fetch item!");
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    };
    fetchItem();
  }, [itemId, tenant?.id]);

  if (!item) {
    return (
      <Card>
        <Typography.Text>Loading...</Typography.Text>
      </Card>
    );
  }

  const categories = item.category
    ? item.category.split(",").filter(Boolean)
    : [];

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

        <Descriptions.Item label="Categories">
          {categories.length > 0 ? (
            <Space wrap>
              {categories.map((category: string) => (
                <Tag color="blue" key={category}>
                  {category.trim()}
                </Tag>
              ))}
            </Space>
          ) : (
            "No categories assigned"
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
