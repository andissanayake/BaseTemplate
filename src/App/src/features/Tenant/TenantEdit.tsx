import React from "react";
import { Form, Input, notification, Button, Space, Typography } from "antd";
import { useTenantStore } from "./tenantStore";
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { Tenant } from "./TenantModel";

const TenantEdit: React.FC = () => {
  const { setLoading, setCurrentTenant, currentTenant } = useTenantStore();
  const [form] = Form.useForm();
  const navigate = useNavigate();

  const handleSaveTenant = () => {
    form.validateFields().then(async (values) => {
      if (!currentTenant?.id) {
        notification.error({ message: "No tenant selected for update!" });
        return;
      }
      // Include id from current tenant for the update
      const payload = {
        id: currentTenant.id,
        name: values.name,
        address: values.address,
      };
      setLoading(true);
      apiClient.put<boolean>("/api/tenants", payload, {
        onSuccess: () => {
          notification.success({ message: "Tenant updated successfully!" });
          setCurrentTenant({ ...currentTenant, ...payload });
          navigate(`/tenants/view`);
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to update tenant!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  useAsyncEffect(async () => {
    setLoading(true);
    form.resetFields();
    apiClient.get<Tenant>("/api/tenants", {
      onSuccess: (data) => {
        if (data) {
          form.setFieldsValue(data);
          setCurrentTenant(data);
        } else {
          notification.error({ message: "Tenant not found!" });
        }
      },
      onServerError: () => {
        notification.error({ message: "Failed to fetch tenant details!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [form, setLoading, setCurrentTenant]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Edit Tenant
        </Typography.Title>
      </Space>
      <Form form={form} layout="vertical" onFinish={handleSaveTenant}>
        <Form.Item
          label="Tenant Name"
          name="name"
          rules={[{ required: true, message: "Please enter the tenant name!" }]}
        >
          <Input placeholder="Enter tenant name" />
        </Form.Item>
        <Form.Item
          label="Address"
          name="address"
          rules={[{ required: true, message: "Please enter the address!" }]}
        >
          <Input.TextArea placeholder="Enter address" rows={3} />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Save Changes
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export default TenantEdit;
