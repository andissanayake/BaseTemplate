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
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { SpecificationModel } from "../Specification/SpecificationModel";
import { getAllSpecifications } from "../Specification/SpecificationUtils";

const ItemCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { setLoading } = useItemStore();
  const [specificationOptions, setSpecificationOptions] = useState<
    { value: number; label: string }[]
  >([]);

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
      setLoading(true);
      apiClient.post<number>(
        `/api/item`,
        {
          ...values,
          tags: values.tags?.join(",") || "",
        },
        {
          onSuccess: () => {
            notification.success({ message: "Item created successfully!" });
            form.resetFields();
            navigate(`/items`);
          },
          onValidationError: (errors) => {
            handleFormValidationErrors({
              form,
              errors: errors,
            });
          },
          onServerError: () => {
            notification.error({ message: "Failed to create item!" });
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

export { ItemCreate };
