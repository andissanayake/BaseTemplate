import React, { useEffect, useState } from "react";
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
import { SpecificationModel } from "../Specification/SpecificationModel";
import { getAllSpecifications } from "../Specification/SpecificationUtils";

const ItemEdit: React.FC = () => {
  const { setLoading } = useItemStore();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { itemId } = useParams();
  const [specificationOptions, setSpecificationOptions] = useState<
    { value: number; label: string }[]
  >([]);

  if (!itemId) throw new Error("itemId is required");

  useEffect(() => {
    loadSpecifications();
  }, []);

  const loadSpecifications = () => {
    apiClient.get<{ specifications: SpecificationModel[] }>(
      "/api/specification",
      {
        onSuccess: (data) => {
          if (data?.specifications) {
            // Flatten specifications for dropdown
            const allSpecs = getAllSpecifications(data.specifications);
            const options = allSpecs.map((spec) => ({
              value: spec.id,
              label: spec.fullPath || spec.name,
            }));
            setSpecificationOptions(options);
          }
        },
        onServerError: () => {
          notification.error({ message: "Failed to load specifications!" });
        },
      }
    );
  };

  const handleSaveItem = () => {
    form.validateFields().then(async (values) => {
      values.id = +itemId;
      values.tags = values.tags?.join(",") || "";
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
            tags: data.tags ? data.tags.split(",").filter(Boolean) : [],
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

        <Form.Item
          label="Specification"
          name="specificationId"
          rules={[
            { required: true, message: "Please select a specification!" },
          ]}
        >
          <Select
            placeholder="Select specification"
            options={specificationOptions}
            showSearch
            filterOption={(input, option) =>
              (option?.label ?? "").toLowerCase().includes(input.toLowerCase())
            }
          />
        </Form.Item>

        <Form.Item label="Tags" name="tags">
          <Select
            mode="tags"
            style={{ width: "100%" }}
            placeholder="Enter tags"
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

export { ItemEdit };
