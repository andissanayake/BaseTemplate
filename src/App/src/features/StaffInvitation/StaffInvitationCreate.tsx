import React, { useMemo, useState } from "react";
import {
  Modal,
  Form,
  Input,
  Button,
  Space,
  Checkbox,
  notification,
} from "antd";
import { apiClient } from "../../common/apiClient";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { CreateStaffInvitationRequest } from "./StaffInvitationModel";
import { Roles } from "../../auth/RolesEnum";
import { useAuthStore } from "../../auth/authStore";

interface StaffInvitationCreateProps {
  visible: boolean;
  onCancel: () => void;
  onSuccess: () => void;
}

const StaffInvitationCreate: React.FC<StaffInvitationCreateProps> = ({
  visible,
  onCancel,
  onSuccess,
}) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const { roles } = useAuthStore();
  // Available roles - you can customize this based on your application
  const availableRoles = useMemo(
    () => roles.filter((role) => role !== Roles.TenantOwner),
    [roles]
  );

  const handleCreateInvitation = async (values: {
    staffEmail: string;
    staffName: string;
    roles: string[];
  }) => {
    setLoading(true);
    const requestData: CreateStaffInvitationRequest = {
      staffEmail: values.staffEmail,
      staffName: values.staffName,
      roles: values.roles,
    };

    apiClient.post<boolean>("/api/staff-invitation", requestData, {
      onSuccess: () => {
        notification.success({
          message: "Staff invitation created successfully!",
        });
        form.resetFields();
        onSuccess();
      },
      onValidationError: (errors) => {
        // Use the utility function to handle validation errors
        handleFormValidationErrors({
          form,
          errors,
        });
      },
      onServerError: () => {
        notification.error({ message: "Failed to create staff invitation!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const handleCancel = () => {
    form.resetFields();
    onCancel();
  };

  return (
    <Modal
      title="Create New Staff Invitation"
      open={visible}
      onCancel={handleCancel}
      footer={null}
      destroyOnClose
    >
      <Form form={form} layout="vertical" onFinish={handleCreateInvitation}>
        <Form.Item
          label="Email Address"
          name="staffEmail"
          rules={[
            { required: true, message: "Please enter email address!" },
            { type: "email", message: "Please enter a valid email!" },
          ]}
        >
          <Input placeholder="Enter email address" />
        </Form.Item>

        <Form.Item
          label="Full Name"
          name="staffName"
          rules={[
            { required: true, message: "Please enter full name!" },
            { min: 2, message: "Name must be at least 2 characters!" },
          ]}
        >
          <Input placeholder="Enter full name" />
        </Form.Item>

        <Form.Item
          label="Roles"
          name="roles"
          rules={[
            { required: true, message: "Please select at least one role!" },
          ]}
        >
          <Checkbox.Group options={availableRoles} />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit" loading={loading}>
              Create Invitation
            </Button>
            <Button onClick={handleCancel}>Cancel</Button>
          </Space>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export { StaffInvitationCreate };
