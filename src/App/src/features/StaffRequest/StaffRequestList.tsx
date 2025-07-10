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
  Checkbox,
  notification,
  Popconfirm,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { StaffRequestDto, StaffRequestStatus } from "./StaffRequestModel";
import { useStaffRequestStore } from "./staffRequestStore";
import { StaffRequestService } from "./staffRequestService";
import { handleResult } from "../../common/handleResult";
import { useParams } from "react-router-dom";

export const StaffRequestList: React.FC = () => {
  const { tenantId } = useParams<{ tenantId: string }>();
  const { staffRequests, loading, setStaffRequests, setLoading } =
    useStaffRequestStore();
  const [createModalVisible, setCreateModalVisible] = useState(false);
  const [rejectModalVisible, setRejectModalVisible] = useState(false);
  const [selectedRequest, setSelectedRequest] =
    useState<StaffRequestDto | null>(null);
  const [createForm] = Form.useForm();
  const [rejectForm] = Form.useForm();

  if (!tenantId) throw new Error("Tenant ID is required");

  const fetchStaffRequests = async (tenantId: number) => {
    setLoading(true);
    const response = await StaffRequestService.getStaffRequests(tenantId);
    handleResult(response, {
      onSuccess: (data) => {
        setStaffRequests(data || []);
      },
      onServerError: () => {
        notification.error({
          message: "Failed to fetch staff requests",
          description: "An error occurred while loading staff requests.",
        });
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

  const handleCreateRequest = async (values: {
    email: string;
    name: string;
    roles: string[];
  }) => {
    setLoading(true);
    const response = await StaffRequestService.createStaffRequest(+tenantId, {
      staffEmail: values.email,
      staffName: values.name,
      roles: values.roles,
    });

    handleResult(response, {
      onSuccess: () => {
        setCreateModalVisible(false);
        createForm.resetFields();
        notification.success({
          message: "Staff request created successfully!",
        });
        fetchStaffRequests(+tenantId);
      },
      onValidationError: (errors) => {
        const fields = Object.entries(errors).map(([name, errorMessages]) => ({
          name: name.toLowerCase(),
          errors: errorMessages,
        }));
        createForm.setFields(fields);
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

  const handleReject = async (values: { rejectionReason?: string }) => {
    if (!selectedRequest) return;
    setLoading(true);
    const response = await StaffRequestService.updateStaffRequest(+tenantId, {
      staffRequestId: selectedRequest.id,
      accept: false,
      rejectionReason: values.rejectionReason,
    });

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
      onServerError: (error) => {
        notification.error({
          message: "Failed to reject staff request!",
          description:
            error?.message ||
            "An error occurred while rejecting the staff request.",
        });
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
      default:
        return <Tag color="default">Unknown</Tag>;
    }
  };

  const availableRoles = ["TenantOwner", "Administrator"];

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
          {record.status === StaffRequestStatus.Rejected && (
            <Tag color="red">✗ Rejected</Tag>
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

      {/* Create Request Modal */}
      <Modal
        title="Create New Staff Request"
        open={createModalVisible}
        onCancel={() => {
          setCreateModalVisible(false);
          createForm.resetFields();
        }}
        footer={null}
        destroyOnClose
      >
        <Form
          form={createForm}
          layout="vertical"
          onFinish={handleCreateRequest}
        >
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
              <Button
                onClick={() => {
                  setCreateModalVisible(false);
                  createForm.resetFields();
                }}
              >
                Cancel
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>

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
                label="Rejection Reason (Optional)"
                name="rejectionReason"
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
