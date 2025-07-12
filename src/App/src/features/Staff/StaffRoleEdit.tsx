import React, { useState } from "react";
import {
  Form,
  Select,
  Button,
  Space,
  message,
  Typography,
  notification,
} from "antd";
import { StaffService } from "./staffService";
import { StaffMemberDto } from "./StaffModel";
import { handleResult } from "../../common/handleResult";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { handleServerError } from "../../common/serverErrorHandler";
import { useAuthStore } from "../../auth/authStore";

const { Title } = Typography;
const { Option } = Select;

interface StaffRoleEditProps {
  staffMember: StaffMemberDto;
  onSuccess: () => void;
  onCancel: () => void;
}

const StaffRoleEdit: React.FC<StaffRoleEditProps> = ({
  staffMember,
  onSuccess,
  onCancel,
}) => {
  const { tenantId } = useAuthStore();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  // Available roles - you can customize this based on your application
  const availableRoles = [
    "ItemManager",
    "StaffRequestManager",
    "TenantManager",
    "StaffManager",
  ];

  const handleSubmit = async (values: { newRoles: string[] }) => {
    if (!tenantId) {
      notification.error({
        message: "Tenant ID is required",
        description: "Please ensure you are logged in with a valid tenant.",
      });
      return;
    }

    setLoading(true);

    const result = await StaffService.updateStaffRoles(
      +tenantId,
      staffMember.ssoId,
      {
        newRoles: values.newRoles,
      }
    );

    handleResult(result, {
      onSuccess: () => {
        message.success("Staff roles updated successfully");
        onSuccess();
      },
      onValidationError: (errors) => {
        handleFormValidationErrors({
          form,
          errors,
        });
      },
      onServerError: (errors) => {
        handleServerError(
          errors,
          "Failed to update staff roles. An error occurred while updating staff roles."
        );
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  return (
    <>
      <div style={{ marginBottom: "16px" }}>
        <Title level={4}>
          Edit Roles for {staffMember.name || staffMember.email}
        </Title>
        <p>Select the roles you want to assign to this staff member.</p>
      </div>

      <Form
        form={form}
        layout="vertical"
        initialValues={{ newRoles: staffMember.roles }}
        onFinish={handleSubmit}
      >
        <Form.Item
          label="Roles"
          name="newRoles"
          rules={[
            {
              required: true,
              message: "Please select at least one role",
            },
          ]}
        >
          <Select
            mode="multiple"
            placeholder="Select roles"
            style={{ width: "100%" }}
            allowClear
          >
            {availableRoles.map((role) => (
              <Option key={role} value={role}>
                {role}
              </Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit" loading={loading}>
              Update Roles
            </Button>
            <Button onClick={onCancel}>Cancel</Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export default StaffRoleEdit;
