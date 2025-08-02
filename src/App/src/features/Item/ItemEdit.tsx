import React from "react";
import {
  Form,
  Input,
  notification,
  InputNumber,
  Button,
  Space,
  Typography,
  Select,
} from "antd";
import { useItemStore } from "./itemStore";
import { useNavigate, useParams } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { Item } from "./ItemModel";

const ItemEdit: React.FC = () => {
  const { setLoading } = useItemStore();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { itemId } = useParams();

  if (!itemId) throw new Error("itemId is required");

  const handleSaveItem = () => {
    form.validateFields().then(async (values) => {
      values.id = +itemId;
      values.category = values.category?.join(",") || "";
      setLoading(true);
      apiClient.put<boolean>(`/api/item`, values, {
        onSuccess: () => {
          notification.success({
            message: "Item updated successfully!",
          });
          navigate(`/items`);
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to save item!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  useAsyncEffect(async () => {
    form.resetFields();
    setLoading(true);
    apiClient.get<Item>(`/api/item/${itemId}`, {
      onSuccess: (data) => {
        if (data) {
          form.setFieldsValue({
            ...data,
            category: data.category
              ? data.category.split(",").filter(Boolean)
              : [],
          });
        }
      },
      onServerError: () => {
        notification.error({ message: "Failed to fetch item!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [itemId, form]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Edit Item
        </Typography.Title>
      </Space>
      <Form form={form} layout="vertical" onFinish={handleSaveItem}>
        <Form.Item
          label="Name"
          name="name"
          rules={[{ required: true, message: "Please enter the item name!" }]}
        >
          <Input placeholder="Enter item name" />
        </Form.Item>

        <Form.Item label="Description" name="description">
          <Input.TextArea placeholder="Enter item description" />
        </Form.Item>

        <Form.Item
          label="Price"
          name="price"
          rules={[{ required: true, message: "Please enter the item price!" }]}
        >
          <InputNumber
            min={0}
            step={0.01}
            style={{ width: "100%" }}
            placeholder="Enter item price"
          />
        </Form.Item>

        <Form.Item label="Categories" name="category">
          <Select
            mode="tags"
            style={{ width: "100%" }}
            placeholder="Enter categories"
            tokenSeparators={[","]}
          />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button type="default" onClick={() => navigate(`/items`)}>
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export default ItemEdit;
