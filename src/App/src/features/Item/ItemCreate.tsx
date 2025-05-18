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
import { useNavigate } from "react-router-dom";
import { ItemService } from "./itemService";
import { handleResult } from "../../common/handleResult";
import { useAuthStore } from "../../auth/authStore";

const ItemCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { setLoading } = useItemStore();
  const { tenantId } = useAuthStore();

  const handleSaveItem = () => {
    if (!tenantId) {
      notification.error({
        message: "Tenant ID is required to create an item.",
      });
      return;
    }

    form.validateFields().then(async (values) => {
      setLoading(true);
      const response = await ItemService.createItem({ ...values, tenantId });
      handleResult(response, {
        onSuccess: () => {
          notification.success({ message: "Item created successfully!" });
          form.resetFields();
          navigate(`/tenants/view/${tenantId}`);
        },
        onValidationError: (createErrors) => {
          const fields = Object.entries(createErrors).map(([name, errors]) => ({
            name: name.toLowerCase(),
            errors,
          }));
          form.setFields(fields);
        },
        onServerError: () => {
          notification.error({ message: "Failed to create item!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Create Item
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

export default ItemCreate;
