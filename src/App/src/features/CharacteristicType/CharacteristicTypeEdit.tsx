import React from "react";
import { Form, Input, notification, Button, Space, Typography } from "antd";
import { useCharacteristicTypeStore } from "./characteristicTypeStore";
import { useNavigate, useParams } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { CharacteristicType } from "./CharacteristicTypeModel";

const CharacteristicTypeEdit: React.FC = () => {
  const { setLoading } = useCharacteristicTypeStore();

  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { characteristicTypeId } = useParams();

  if (!characteristicTypeId)
    throw new Error("characteristicTypeId is required");

  const handleSaveCharacteristicType = () => {
    form.validateFields().then(async (values) => {
      values.id = +characteristicTypeId;
      setLoading(true);
      apiClient.put<boolean>(`/api/characteristic-type`, values, {
        onSuccess: () => {
          notification.success({
            message: "Characteristic type updated successfully!",
          });
          navigate(`/characteristic-types`);
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: () => {
          notification.error({
            message: "Failed to save characteristic type!",
          });
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
    apiClient.get<CharacteristicType>(
      `/api/characteristic-type/${characteristicTypeId}`,
      {
        onSuccess: (data) => {
          if (data) {
            form.setFieldsValue({
              ...data,
            });
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
  }, [characteristicTypeId, form]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Edit Characteristic Type
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

export { CharacteristicTypeEdit };
