import React from "react";
import { Form, Input, notification, Button, Space, Typography } from "antd";
import { useTenantStore } from "./tenantStore";
import { useParams, useNavigate } from "react-router-dom";
import { TenantService } from "./tenantService";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleResult } from "../../common/handleResult";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { handleServerError } from "../../common/serverErrorHandler";
import { Tenant } from "./TenantModel";

const TenantEdit: React.FC = () => {
  const { setLoading, setCurrentTenant } = useTenantStore();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { tenantId } = useParams<{ tenantId: string }>();

  if (!tenantId) throw new Error("tenantId is required for editing.");

  const handleSaveTenant = () => {
    form.validateFields().then(async (values) => {
      const payload: Tenant = { ...values, id: +tenantId };
      setLoading(true);
      const response = await TenantService.updateTenant(payload);
      handleResult(response, {
        onSuccess: () => {
          notification.success({
            message: "Tenant updated successfully!",
          });
          setCurrentTenant(payload);
          navigate(`/tenants/view/${tenantId}`);
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: (errors) => {
          handleServerError(errors, "Failed to update tenant!");
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  useAsyncEffect(async () => {
    if (!tenantId) return;
    setLoading(true);
    form.resetFields();
    const response = await TenantService.fetchTenantById(tenantId);
    handleResult(response, {
      onSuccess: (data) => {
        if (data) {
          form.setFieldsValue(data);
          setCurrentTenant(data);
        } else {
          notification.error({ message: "Tenant not found!" });
        }
      },
      onServerError: () => {
        handleServerError(undefined, "Failed to fetch tenant details!");
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [tenantId, form, setLoading, setCurrentTenant]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Edit Tenant #{tenantId}
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
