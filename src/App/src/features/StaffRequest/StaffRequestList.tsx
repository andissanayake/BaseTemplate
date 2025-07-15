import React, { useEffect, useState } from "react";
import {
  Table,
  Button,
  Space,
  Typography,
  Tag,
  Modal,
  Form,
  Input,
  notification,
  Popconfirm,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { StaffRequestDto, StaffRequestStatus } from "./StaffRequestModel";
import { useStaffRequestStore } from "./staffRequestStore";
import { StaffRequestService } from "./staffRequestService";
import { handleResult } from "../../common/handleResult";
import { handleServerError } from "../../common/serverErrorHandler";
import { useParams } from "react-router-dom";
import StaffRequestCreate from "./StaffRequestCreate";

export const StaffRequestList: React.FC = () => {
  const { tenantId } = useParams<{ tenantId: string }>();
  const { staffRequests, loading, setStaffRequests, setLoading } =
    useStaffRequestStore();
  const [createModalVisible, setCreateModalVisible] = useState(false);
  const [rejectModalVisible, setRejectModalVisible] = useState(false);
  const [selectedRequest, setSelectedRequest] =
    useState<StaffRequestDto | null>(null);
  const [rejectForm] = Form.useForm();

  if (!tenantId) throw new Error("Tenant ID is required");

  const fetchStaffRequests = async (tenantId: number) => {
    setLoading(true);
    const response = await StaffRequestService.getStaffRequests(tenantId);
    handleResult(response, {
      onSuccess: (data) => {
        setStaffRequests(data || []);
      },
      onServerError: (errors) => {
        handleServerError(
          errors,
          "Failed to fetch staff requests. An error occurred while loading staff requests."
        );
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  useEffect(() => {
    fetchStaffRequests(+tenantId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [tenantId]);

  const handleReject = async (values: { rejectionReason: string }) => {
    if (!selectedRequest) return;
    setLoading(true);
    const response = await StaffRequestService.updateStaffRequest(
      +tenantId,
      selectedRequest.id,
      values.rejectionReason
    );

    handleResult(response, {
      onSuccess: () => {
        setRejectModalVisible(false);
        setSelectedRequest(null);
        rejectForm.resetFields();
        notification.success({
          message: "Staff request rejected successfully!",
        });
        fetchStaffRequests(+tenantId);
      },
      onServerError: (errors) => {
        handleServerError(errors, "Failed to reject staff request!");
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const getStatusTag = (status: StaffRequestStatus) => {
    switch (status) {
      case StaffRequestStatus.Pending:
        return <Tag color="orange">Pending</Tag>;
      case StaffRequestStatus.Accepted:
        return <Tag color="green">Accepted</Tag>;
      case StaffRequestStatus.Rejected:
        return <Tag color="red">Rejected</Tag>;
      case StaffRequestStatus.Revoked:
        return <Tag color="red">Revoked</Tag>;
      case StaffRequestStatus.Expired:
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
      render: (status: StaffRequestStatus) => getStatusTag(status),
    },
    {
      title: "Rejection Reason",
      dataIndex: "rejectionReason",
      key: "rejectionReason",
      render: (rejectionReason: string, record: StaffRequestDto) => {
        if (
          record.status === StaffRequestStatus.Rejected ||
          record.status === StaffRequestStatus.Revoked ||
          record.status === StaffRequestStatus.Expired
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
      render: (_: unknown, record: StaffRequestDto) => (
        <Space>
          {record.status === StaffRequestStatus.Pending && (
            <Popconfirm
              title="Are you sure you want to reject this staff request?"
              onConfirm={() => {
                setSelectedRequest(record);
                setRejectModalVisible(true);
              }}
            >
              <Button type="link" danger>
                Reject
              </Button>
            </Popconfirm>
          )}
          {record.status === StaffRequestStatus.Accepted && (
            <Tag color="green">✓ Accepted</Tag>
          )}
          {(record.status === StaffRequestStatus.Rejected ||
            record.status === StaffRequestStatus.Revoked) && (
            <Tag color="red">
              ✗{" "}
              {record.status === StaffRequestStatus.Revoked
                ? "Revoked"
                : "Rejected"}
            </Tag>
          )}
          {record.status === StaffRequestStatus.Expired && (
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
          Staff Requests
        </Typography.Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setCreateModalVisible(true)}
        >
          Create New Request
        </Button>
      </Space>

      <Table
        columns={columns}
        dataSource={staffRequests}
        loading={loading}
        rowKey="id"
        pagination={{
          showSizeChanger: true,
          showQuickJumper: true,
          showTotal: (total, range) =>
            `${range[0]}-${range[1]} of ${total} items`,
        }}
      />

      <StaffRequestCreate
        tenantId={+tenantId}
        visible={createModalVisible}
        onCancel={() => setCreateModalVisible(false)}
        onSuccess={() => {
          setCreateModalVisible(false);
          fetchStaffRequests(+tenantId);
        }}
      />

      {/* Reject Request Modal */}
      <Modal
        title="Reject Staff Request"
        open={rejectModalVisible}
        onCancel={() => {
          setRejectModalVisible(false);
          setSelectedRequest(null);
          rejectForm.resetFields();
        }}
        footer={null}
        destroyOnClose
      >
        {selectedRequest && (
          <div>
            <p>
              Are you sure you want to reject the staff request for{" "}
              <strong>{selectedRequest.requestedName}</strong>?
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
                <Input.TextArea
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
                      setSelectedRequest(null);
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
