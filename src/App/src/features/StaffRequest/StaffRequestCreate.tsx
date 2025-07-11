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

  const availableRoles = [
    "ItemManager",
    "StaffRequestManager",
    "TenantManager",
    "StaffManager",
  ];

  const handleCreateRequest = async (values: {
    staffEmail: string;
    staffName: string;
    roles: string[];
  }) => {
    setLoading(true);
    const requestData: CreateStaffRequestRequest = {
      staffEmail: values.staffEmail,
      staffName: values.staffName,
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
        // Handle general errors (like tenant not found, access denied)
        const fields = Object.entries(errors).map(([name, errors]) => ({
          name: name.toLowerCase(),
          errors,
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
