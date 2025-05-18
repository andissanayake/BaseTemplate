import React from "react";
import {
  Form,
  Input,
  notification,
  InputNumber,
  Button,
  Space,
  Typography,
} from "antd";
import { useItemStore } from "./itemStore";
import { useNavigate, useParams } from "react-router-dom";
import { ItemService } from "./itemService";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleResult } from "../../common/handleResult";

const ItemEdit: React.FC = () => {
  const { setLoading } = useItemStore();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { itemId } = useParams();
  if (!itemId) throw new Error("itemId is required");

  const handleSaveItem = () => {
    form.validateFields().then(async (values) => {
      values.id = +itemId;
      setLoading(true);
      const response = await ItemService.updateItem(values);
      return handleResult(response, {
        onSuccess: () => {
          notification.success({
            message: "Item updated successfully!",
          });
          navigate("/items");
        },
        onValidationError: (updateErrors) => {
          const fields = Object.entries(updateErrors).map(([name, errors]) => ({
            name: name.toLowerCase(),
            errors,
          }));
          form.setFields(fields);
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
    const response = await ItemService.fetchItemById(itemId);
    handleResult(response, {
      onSuccess: (data) => {
        form.setFieldsValue(data);
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

        <Form.Item
          label="Quantity"
          name="quantity"
          rules={[
            { required: true, message: "Please enter the item quantity!" },
          ]}
        >
          <InputNumber
            min={0}
            style={{ width: "100%" }}
            placeholder="Enter item quantity"
          />
        </Form.Item>

        <Form.Item label="Category" name="category">
          <Input placeholder="Enter item category" />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button type="default" onClick={() => navigate("/items")}>
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export default ItemEdit;
