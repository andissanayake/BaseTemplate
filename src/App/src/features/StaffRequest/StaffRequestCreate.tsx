import React from "react";
import {
  Form,
  Input,
  notification,
  Button,
  Space,
  Checkbox,
  Modal,
} from "antd";
import { useStaffRequestStore } from "./staffRequestStore";
import { StaffRequestService } from "./staffRequestService";
import { handleResult } from "../../common/handleResult";
import { CreateStaffRequestRequest } from "./StaffRequestModel";

interface StaffRequestCreateProps {
  tenantId: number;
  visible: boolean;
  onCancel: () => void;
  onSuccess: () => void;
}

const StaffRequestCreate: React.FC<StaffRequestCreateProps> = ({
  tenantId,
  visible,
  onCancel,
  onSuccess,
}) => {
  const [form] = Form.useForm();
  const { setLoading } = useStaffRequestStore();

  const availableRoles = ["TenantOwner", "Administrator"];

  const handleCreateRequest = async (values: {
    email: string;
    name: string;
    roles: string[];
  }) => {
    setLoading(true);
    const requestData: CreateStaffRequestRequest = {
      staffEmail: values.email,
      staffName: values.name,
      roles: values.roles,
    };

    const response = await StaffRequestService.createStaffRequest(
      tenantId,
      requestData
    );

    handleResult(response, {
      onSuccess: () => {
        notification.success({
          message: "Staff request created successfully!",
        });
        form.resetFields();
        onSuccess();
      },
      onValidationError: (errors) => {
        const fields = Object.entries(errors).map(([name, errorMessages]) => ({
          name: name.toLowerCase(),
          errors: errorMessages,
        }));
        form.setFields(fields);
      },
      onServerError: (error) => {
        notification.error({
          message: "Failed to create staff request!",
          description:
            error?.message ||
            "An error occurred while creating the staff request.",
        });
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
      title="Create New Staff Request"
      open={visible}
      onCancel={handleCancel}
      footer={null}
      destroyOnClose
    >
      <Form form={form} layout="vertical" onFinish={handleCreateRequest}>
        <Form.Item
          label="Email Address"
          name="email"
          rules={[
            { required: true, message: "Please enter email address!" },
            { type: "email", message: "Please enter a valid email!" },
          ]}
        >
          <Input placeholder="Enter email address" />
        </Form.Item>

        <Form.Item
          label="Full Name"
          name="name"
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
            <Button type="primary" htmlType="submit">
              Create Request
            </Button>
            <Button onClick={handleCancel}>Cancel</Button>
          </Space>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default StaffRequestCreate;
