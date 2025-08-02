import React, { useEffect, useState } from "react";
import {
  Table,
  Button,
  Space,
  Tag,
  Modal,
  Form,
  Input,
  notification,
  Typography,
  Popconfirm,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { apiClient } from "../../common/apiClient";
import {
  StaffInvitationDto,
  StaffInvitationStatus,
} from "./StaffInvitationModel";
import StaffInvitationCreate from "./StaffInvitationCreate";

const { TextArea } = Input;

export const StaffInvitationList: React.FC = () => {
  const [staffInvitations, setStaffInvitations] = useState<
    StaffInvitationDto[]
  >([]);
  const [loading, setLoading] = useState(false);
  const [createModalVisible, setCreateModalVisible] = useState(false);
  const [rejectModalVisible, setRejectModalVisible] = useState(false);
  const [selectedInvitation, setSelectedInvitation] =
    useState<StaffInvitationDto | null>(null);
  const [rejectForm] = Form.useForm();

  const fetchStaffInvitations = async () => {
    setLoading(true);
    apiClient.get<StaffInvitationDto[]>("/api/staff-invitation", {
      onSuccess: (data) => {
        setStaffInvitations(data || []);
      },
      onServerError: () => {
        notification.error({
          message:
            "Failed to fetch staff invitations. An error occurred while loading staff invitations.",
        });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  useEffect(() => {
    fetchStaffInvitations();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleReject = async (values: { rejectionReason: string }) => {
    if (!selectedInvitation) return;
    setLoading(true);
    apiClient.post<boolean>(
      `/api/staff-invitations/${selectedInvitation.id}/revoke`,
      {
        staffInvitationId: selectedInvitation.id,
        rejectionReason: values.rejectionReason,
      },
      {
        onSuccess: () => {
          setRejectModalVisible(false);
          setSelectedInvitation(null);
          rejectForm.resetFields();
          notification.success({
            message: "Staff invitation rejected successfully!",
          });
          fetchStaffInvitations();
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

  const getStatusTag = (status: StaffInvitationStatus) => {
    switch (status) {
      case StaffInvitationStatus.Pending:
        return <Tag color="orange">Pending</Tag>;
      case StaffInvitationStatus.Accepted:
        return <Tag color="green">Accepted</Tag>;
      case StaffInvitationStatus.Rejected:
        return <Tag color="red">Rejected</Tag>;
      case StaffInvitationStatus.Revoked:
        return <Tag color="red">Revoked</Tag>;
      case StaffInvitationStatus.Expired:
        return <Tag color="default">Expired</Tag>;
      default:
        return <Tag color="default">Unknown</Tag>;
    }
  };

  const columns = [
    {
      title: "Name",
      dataIndex: "requestedName",
      key: "requestedName",
    },
    {
      title: "Email",
      dataIndex: "requestedEmail",
      key: "requestedEmail",
    },
    {
      title: "Roles",
      dataIndex: "roles",
      key: "roles",
      render: (roles: string[]) => (
        <Space wrap>
          {roles.map((role) => (
            <Tag key={role} color="blue">
              {role}
            </Tag>
          ))}
        </Space>
      ),
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (status: StaffInvitationStatus) => getStatusTag(status),
    },
    {
      title: "Rejection Reason",
      dataIndex: "rejectionReason",
      key: "rejectionReason",
      render: (rejectionReason: string, record: StaffInvitationDto) => {
        if (
          record.status === StaffInvitationStatus.Rejected ||
          record.status === StaffInvitationStatus.Revoked ||
          record.status === StaffInvitationStatus.Expired
        ) {
          return rejectionReason ? (
            <Typography.Text
              type="secondary"
              style={{ maxWidth: 200, display: "block" }}
            >
              {rejectionReason}
            </Typography.Text>
          ) : (
            <Typography.Text type="secondary" italic>
              No reason provided
            </Typography.Text>
          );
        }
        return null;
      },
    },
    {
      title: "Created",
      dataIndex: "created",
      key: "created",
      render: (created: string) => new Date(created).toLocaleDateString(),
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: unknown, record: StaffInvitationDto) => (
        <Space>
          {record.status === StaffInvitationStatus.Pending && (
            <Popconfirm
              title="Are you sure you want to reject this staff invitation?"
              onConfirm={() => {
                setSelectedInvitation(record);
                setRejectModalVisible(true);
              }}
            >
              <Button type="link" danger>
                Reject
              </Button>
            </Popconfirm>
          )}
          {record.status === StaffInvitationStatus.Accepted && (
            <Tag color="green">✓ Accepted</Tag>
          )}
          {(record.status === StaffInvitationStatus.Rejected ||
            record.status === StaffInvitationStatus.Revoked) && (
            <Tag color="red">
              ✗{" "}
              {record.status === StaffInvitationStatus.Revoked
                ? "Revoked"
                : "Rejected"}
            </Tag>
          )}
          {record.status === StaffInvitationStatus.Expired && (
            <Tag color="default">Expired</Tag>
          )}
        </Space>
      ),
    },
  ];

  return (
    <>
      <Space
        className="mb-4"
        style={{ width: "100%", justifyContent: "space-between" }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          Staff Invitations
        </Typography.Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setCreateModalVisible(true)}
        >
          Create New Invitation
        </Button>
      </Space>

      <Table
        columns={columns}
        dataSource={staffInvitations}
        loading={loading}
        rowKey="id"
        pagination={{
          showSizeChanger: true,
          showQuickJumper: true,
          showTotal: (total, range) =>
            `${range[0]}-${range[1]} of ${total} items`,
        }}
      />

      <StaffInvitationCreate
        visible={createModalVisible}
        onCancel={() => setCreateModalVisible(false)}
        onSuccess={() => {
          setCreateModalVisible(false);
          fetchStaffInvitations();
        }}
      />

      {/* Reject Invitation Modal */}
      <Modal
        title="Reject Staff Invitation"
        open={rejectModalVisible}
        onCancel={() => {
          setRejectModalVisible(false);
          setSelectedInvitation(null);
          rejectForm.resetFields();
        }}
        footer={null}
        destroyOnClose
      >
        {selectedInvitation && (
          <div>
            <p>
              Are you sure you want to reject the staff invitation for{" "}
              <strong>{selectedInvitation.requestedName}</strong>?
            </p>
            <Form form={rejectForm} layout="vertical" onFinish={handleReject}>
              <Form.Item
                label="Rejection Reason"
                name="rejectionReason"
                rules={[
                  {
                    required: true,
                    message: "Please provide a rejection reason!",
                  },
                ]}
              >
                <TextArea
                  rows={3}
                  placeholder="Provide a reason for rejection..."
                />
              </Form.Item>

              <Form.Item>
                <Space>
                  <Button type="primary" danger htmlType="submit">
                    Reject
                  </Button>
                  <Button
                    onClick={() => {
                      setRejectModalVisible(false);
                      setSelectedInvitation(null);
                      rejectForm.resetFields();
                    }}
                  >
                    Cancel
                  </Button>
                </Space>
              </Form.Item>
            </Form>
          </div>
        )}
      </Modal>
    </>
  );
};
