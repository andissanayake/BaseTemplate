import React from "react";
import { Form, Input, notification, Button, Space, Typography } from "antd";
import { useTenantStore } from "./tenantStore";
import { useNavigate } from "react-router-dom";
import { TenantService } from "./tenantService";
import { handleResult } from "../../common/handleResult";

const TenantCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { setLoading, setCurrentTenant } = useTenantStore();

  const handleSaveTenant = () => {
    form.validateFields().then(async (values) => {
      setLoading(true);
      const response = await TenantService.createTenant(values);
      handleResult(response, {
        onSuccess: (newTenantId) => {
          if (typeof newTenantId === "number") {
            notification.success({ message: "Tenant created successfully!" });
            const newTenantData = { ...values, id: newTenantId };
            setCurrentTenant(newTenantData);
            form.resetFields();
            navigate(`/tenants/view/${newTenantId}`);
          } else {
            notification.error({ message: "Failed to get new Tenant ID." });
            navigate("/tenants/create");
          }
        },
        onValidationError: (createErrors) => {
          const fields = Object.entries(createErrors).map(([name, errors]) => ({
            name: name.toLowerCase(),
            errors,
          }));
          form.setFields(fields);
        },
        onServerError: () => {
          notification.error({ message: "Failed to create tenant!" });
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
          Create Tenant
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
              Submit
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export default TenantCreate;
