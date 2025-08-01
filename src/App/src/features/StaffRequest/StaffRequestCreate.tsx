import React, { useState } from "react";
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
import { CreateStaffInvitationRequest } from "./StaffRequestModel";
import { Roles } from "../../auth/RolesEnum";

interface StaffRequestCreateProps {
  visible: boolean;
  onCancel: () => void;
  onSuccess: () => void;
}

const StaffRequestCreate: React.FC<StaffRequestCreateProps> = ({
  visible,
  onCancel,
  onSuccess,
}) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const availableRoles = Object.values(Roles).filter((role) =>
    [
      Roles.TenantManager,
      Roles.StaffManager,
      Roles.StaffRequestManager,
      Roles.ItemManager,
    ].includes(role)
  );

  const handleCreateRequest = async (values: {
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

    apiClient.post<boolean>("/api/tenants/staff-invitation", requestData, {
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
      <Form form={form} layout="vertical" onFinish={handleCreateRequest}>
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

export default StaffRequestCreate;
