import React from "react";
import { Form, Input, notification, Button, Space, Typography } from "antd";
import { useCharacteristicTypeStore } from "./characteristicTypeStore";
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { handleFormValidationErrors } from "../../common/formErrorHandler";

const CharacteristicTypeCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { setLoading } = useCharacteristicTypeStore();

  const handleSaveCharacteristicType = () => {
    form.validateFields().then(async (values) => {
      setLoading(true);
      apiClient.post<number>(
        `/api/characteristic-type`,
        {
          ...values,
        },
        {
          onSuccess: () => {
            notification.success({
              message: "Characteristic type created successfully!",
            });
            form.resetFields();
            navigate(`/characteristic-types`);
          },
          onValidationError: (errors) => {
            handleFormValidationErrors({
              form,
              errors: errors,
            });
          },
          onServerError: () => {
            notification.error({
              message: "Failed to create characteristic type!",
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
          Create Characteristic Type
        </Typography.Title>
      </Space>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSaveCharacteristicType}
      >
        <Form.Item
          label="Name"
          name="name"
          rules={[
            {
              required: true,
              message: "Please enter the characteristic type name!",
            },
          ]}
        >
          <Input placeholder="Enter characteristic type name" />
        </Form.Item>

        <Form.Item label="Description" name="description">
          <Input.TextArea placeholder="Enter characteristic type description" />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button
              type="default"
              onClick={() => navigate(`/characteristic-types`)}
            >
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export { CharacteristicTypeCreate };
