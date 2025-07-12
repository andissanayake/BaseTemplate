import React, { useState } from "react";
import {
  Card,
  Typography,
  Button,
  Space,
  Form,
  Input,
  notification,
  Popconfirm,
  Descriptions,
  Tag,
  Alert,
} from "antd";
import { CheckOutlined, CloseOutlined } from "@ant-design/icons";
import { useAuthStore } from "../../auth/authStore";
import { StaffRequestService } from "./staffRequestService";
import { handleResult } from "../../common/handleResult";
import { handleServerError } from "../../common/serverErrorHandler";
import { useNavigate } from "react-router-dom";

const { Title, Text } = Typography;
const { TextArea } = Input;

export const StaffRequestResponse: React.FC = () => {
  const { staffRequest, setStaffRequest } = useAuthStore();
  const [loading, setLoading] = useState(false);
  const [rejectForm] = Form.useForm();
  const [showRejectForm, setShowRejectForm] = useState(false);
  const navigate = useNavigate();

  if (!staffRequest) {
    return (
      <Card>
        <Alert
          message="No Staff Request Found"
          description="You don't have any pending staff requests to respond to."
          type="info"
          showIcon
        />
      </Card>
    );
  }

  const handleAccept = async () => {
    setLoading(true);
    const response = await StaffRequestService.respondToStaffRequest(
      staffRequest.id,
      staffRequest.id,
      true
    );

    handleResult(response, {
      onSuccess: () => {
        notification.success({
          message: "Staff request accepted successfully!",
          description: "You have been added to the tenant.",
        });
        // Clear the staff request from auth store
        setStaffRequest(null);
        // Redirect to home page
        navigate("/");
      },
      onServerError: (errors) => {
        handleServerError(errors, "Failed to accept staff request!");
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const handleReject = async (values: { rejectionReason: string }) => {
    setLoading(true);
    const response = await StaffRequestService.respondToStaffRequest(
      staffRequest.id,
      staffRequest.id,
      false,
      values.rejectionReason
    );

    handleResult(response, {
      onSuccess: () => {
        notification.success({
          message: "Staff request rejected successfully!",
        });
        // Clear the staff request from auth store
        setStaffRequest(null);
        // Redirect to home page
        navigate("/");
      },
      onServerError: (errors) => {
        handleServerError(errors, "Failed to reject staff request!");
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const getStatusTag = (status: number) => {
    switch (status) {
      case 0:
        return <Tag color="orange">Pending</Tag>;
      case 1:
        return <Tag color="green">Accepted</Tag>;
      case 2:
        return <Tag color="red">Rejected</Tag>;
      case 3:
        return <Tag color="red">Revoked</Tag>;
      default:
        return <Tag color="default">Unknown</Tag>;
    }
  };

  return (
    <Card>
      <Space direction="vertical" size="large" style={{ width: "100%" }}>
        <div>
          <Title level={2}>Staff Request Response</Title>
          <Text type="secondary">
            You have a pending staff request that requires your response.
          </Text>
        </div>

        <Card size="small" title="Request Details">
          <Descriptions column={1} size="small">
            <Descriptions.Item label="Tenant">
              <Text strong>{staffRequest.tenantName}</Text>
            </Descriptions.Item>
            <Descriptions.Item label="Requester Name">
              {staffRequest.requesterName}
            </Descriptions.Item>
            <Descriptions.Item label="Requester Email">
              {staffRequest.requesterEmail}
            </Descriptions.Item>
            <Descriptions.Item label="Requested Roles">
              <Space wrap>
                {staffRequest.roles.map((role) => (
                  <Tag key={role} color="blue">
                    {role}
                  </Tag>
                ))}
              </Space>
            </Descriptions.Item>
            <Descriptions.Item label="Status">
              {getStatusTag(staffRequest.status)}
            </Descriptions.Item>
            <Descriptions.Item label="Created">
              {new Date(staffRequest.created).toLocaleDateString()}
            </Descriptions.Item>
          </Descriptions>
        </Card>

        {staffRequest.status === 0 && (
          <Card size="small" title="Your Response">
            <Space direction="vertical" size="middle" style={{ width: "100%" }}>
              <div>
                <Text>
                  Please choose whether to accept or reject this staff request:
                </Text>
              </div>

              <Space>
                <Popconfirm
                  title="Accept Staff Request"
                  description="Are you sure you want to accept this staff request? You will be added to the tenant with the requested roles."
                  onConfirm={handleAccept}
                >
                  <Button
                    type="primary"
                    icon={<CheckOutlined />}
                    loading={loading}
                    size="large"
                  >
                    Accept Request
                  </Button>
                </Popconfirm>

                <Button
                  type="default"
                  icon={<CloseOutlined />}
                  onClick={() => setShowRejectForm(true)}
                  loading={loading}
                  size="large"
                >
                  Reject Request
                </Button>
              </Space>

              {showRejectForm && (
                  <Form
                    form={rejectForm}
                    onFinish={handleReject}
                    layout="vertical"
                  >
                    <Form.Item
                      name="rejectionReason"
                      label="Rejection Reason"
                      rules={[
                        {
                          required: true,
                          message:
                            "Please provide a reason for rejecting this request.",
                        },
                      ]}
                    >
                      <TextArea
                        rows={4}
                        placeholder="Please provide a reason for rejecting this staff request..."
                      />
                    </Form.Item>

                    <Form.Item>
                      <Space>
                        <Button
                          type="primary"
                          danger
                          htmlType="submit"
                          loading={loading}
                        >
                          Confirm Rejection
                        </Button>
                        <Button
                          onClick={() => {
                            setShowRejectForm(false);
                            rejectForm.resetFields();
                          }}
                        >
                          Cancel
                        </Button>
                      </Space>
                    </Form.Item>
                  </Form>
              )}
            </Space>
          </Card>
        )}

        {staffRequest.status !== 0 && (
          <Alert
            message="Request Already Processed"
            description="This staff request has already been processed and cannot be modified."
            type="warning"
            showIcon
          />
        )}
      </Space>
    </Card>
  );
};
