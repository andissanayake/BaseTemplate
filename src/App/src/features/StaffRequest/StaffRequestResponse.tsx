import React, { useState } from "react";
import {
  Card,
  Alert,
  Button,
  Space,
  Typography,
  Tag,
  Popconfirm,
  Form,
  Input,
  notification,
  Descriptions,
} from "antd";
import { CheckOutlined, CloseOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { useAuthStore } from "../../auth/authStore";
import { useTenantStore } from "../Tenant/tenantStore";

const { Title, Text } = Typography;
const { TextArea } = Input;

export const StaffRequestResponse: React.FC = () => {
  const { staffRequest, setStaffRequest } = useAuthStore();
  const { setCurrentTenant } = useTenantStore();
  const [loading, setLoading] = useState(false);
  const [rejectForm] = Form.useForm();
  const [showRejectForm, setShowRejectForm] = useState(false);
  const navigate = useNavigate();

  if (!staffRequest) {
    return (
      <Card>
        <Alert
          message="No Staff Invitation Found"
          description="You don't have any pending staff invitations to respond to."
          type="info"
          showIcon
        />
      </Card>
    );
  }

  const handleAccept = async () => {
    setLoading(true);
    const payload = {
      StaffInvitationId: staffRequest.id,
      IsAccepted: true,
    };

    apiClient.post<boolean>(
      `/api/tenants/staff-invitations/${staffRequest.id}/respond`,
      payload,
      {
        onSuccess: () => {
          notification.success({
            message: "Staff invitation accepted successfully!",
            description: "You have been added to the tenant.",
          });
          // Clear the staff invitation from auth store
          setStaffRequest(null);
          // Redirect to home page
          navigate("/");
          setCurrentTenant({
            id: -1,
            name: staffRequest.tenantName,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to accept staff invitation!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
  };

  const handleReject = async (values: { rejectionReason: string }) => {
    setLoading(true);
    const payload = {
      StaffInvitationId: staffRequest.id,
      IsAccepted: false,
      RejectionReason: values.rejectionReason,
    };

    apiClient.post<boolean>(
      `/api/tenants/staff-invitations/${staffRequest.id}/respond`,
      payload,
      {
        onSuccess: () => {
          notification.success({
            message: "Staff invitation rejected successfully!",
          });
          // Clear the staff invitation from auth store
          setStaffRequest(null);
          // Redirect to home page
          navigate("/");
        },
        onServerError: () => {
          notification.error({ message: "Failed to reject staff invitation!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
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
      case 4:
        return <Tag color="default">Expired</Tag>;
      default:
        return <Tag color="default">Unknown</Tag>;
    }
  };

  return (
    <Card>
      <Space direction="vertical" size="large" style={{ width: "100%" }}>
        <div>
          <Title level={2}>Staff Invitation Response</Title>
          <Text type="secondary">
            You have a pending staff invitation that requires your response.
          </Text>
        </div>

        <Card size="small" title="Invitation Details">
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
                  Please choose whether to accept or reject this staff
                  invitation:
                </Text>
              </div>

              <Space>
                <Popconfirm
                  title="Accept Staff Invitation"
                  description="Are you sure you want to accept this staff invitation? You will be added to the tenant with the requested roles."
                  onConfirm={handleAccept}
                >
                  <Button
                    type="primary"
                    icon={<CheckOutlined />}
                    loading={loading}
                    size="large"
                  >
                    Accept Invitation
                  </Button>
                </Popconfirm>

                <Button
                  type="default"
                  icon={<CloseOutlined />}
                  onClick={() => setShowRejectForm(true)}
                  loading={loading}
                  size="large"
                >
                  Reject Invitation
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
                          "Please provide a reason for rejecting this invitation.",
                      },
                    ]}
                  >
                    <TextArea
                      rows={4}
                      placeholder="Please provide a reason for rejecting this staff invitation..."
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
            message="Invitation Already Processed"
            description="This staff invitation has already been processed and cannot be modified."
            type="warning"
            showIcon
          />
        )}
      </Space>
    </Card>
  );
};
