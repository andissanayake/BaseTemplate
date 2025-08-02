import React from "react";
import { Form, Input, notification, Button, Space, Typography } from "antd";
import { useItemAttributeTypeStore } from "./itemAttributeTypeStore";
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { handleFormValidationErrors } from "../../common/formErrorHandler";

const ItemAttributeTypeCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { setLoading } = useItemAttributeTypeStore();

  const handleSaveItemAttributeType = () => {
    form.validateFields().then(async (values) => {
      setLoading(true);
      apiClient.post<number>(
        `/api/item-attribute-type`,
        {
          ...values,
        },
        {
          onSuccess: () => {
            notification.success({
              message: "Item attribute type created successfully!",
            });
            form.resetFields();
            navigate(`/item-attribute-types`);
          },
          onValidationError: (errors) => {
            handleFormValidationErrors({
              form,
              errors: errors,
            });
          },
          onServerError: () => {
            notification.error({
              message: "Failed to create item attribute type!",
            });
          },
          onFinally: () => {
            setLoading(false);
          },
        }
      );
    });
  };

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Create Item Attribute Type
        </Typography.Title>
      </Space>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSaveItemAttributeType}
      >
        <Form.Item
          label="Name"
          name="name"
          rules={[
            {
              required: true,
              message: "Please enter the attribute type name!",
            },
          ]}
        >
          <Input placeholder="Enter attribute type name" />
        </Form.Item>

        <Form.Item label="Description" name="description">
          <Input.TextArea placeholder="Enter attribute type description" />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button
              type="default"
              onClick={() => navigate(`/item-attribute-types`)}
            >
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export default ItemAttributeTypeCreate;
