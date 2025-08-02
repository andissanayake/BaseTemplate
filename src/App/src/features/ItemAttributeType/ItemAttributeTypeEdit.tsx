import React from "react";
import { Form, Input, notification, Button, Space, Typography } from "antd";
import { useItemAttributeTypeStore } from "./itemAttributeTypeStore";
import { useNavigate, useParams } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { ItemAttributeType } from "./ItemAttributeTypeModel";

const ItemAttributeTypeEdit: React.FC = () => {
  const { setLoading } = useItemAttributeTypeStore();

  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { itemAttributeTypeId } = useParams();

  if (!itemAttributeTypeId) throw new Error("itemAttributeTypeId is required");

  const handleSaveItemAttributeType = () => {
    form.validateFields().then(async (values) => {
      values.id = +itemAttributeTypeId;
      setLoading(true);
      apiClient.put<boolean>(`/api/item-attribute-type`, values, {
        onSuccess: () => {
          notification.success({
            message: "Item attribute type updated successfully!",
          });
          navigate(`/item-attribute-types`);
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: () => {
          notification.error({
            message: "Failed to save item attribute type!",
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
    apiClient.get<ItemAttributeType>(
      `/api/item-attribute-type/${itemAttributeTypeId}`,
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
            message: "Failed to fetch item attribute type!",
          });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
  }, [itemAttributeTypeId, form]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Edit Item Attribute Type
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

export default ItemAttributeTypeEdit;
