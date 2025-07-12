import React, { useEffect, useState } from "react";
import {
  Table,
  Button,
  Space,
  Tag,
  Modal,
  message,
  Popconfirm,
  Typography,
  Tooltip,
} from "antd";
import { DeleteOutlined, EditOutlined, UserOutlined } from "@ant-design/icons";
import { StaffService } from "./staffService";
import { useStaffStore } from "./staffStore";
import { StaffMemberDto } from "./StaffModel";
import StaffRoleEdit from "./StaffRoleEdit";
import { handleResult } from "../../common/handleResult";
import { handleServerError } from "../../common/serverErrorHandler";
import { useAuthStore } from "../../auth/authStore";

const { Title } = Typography;

const StaffList: React.FC = () => {
  const { tenantId } = useAuthStore();
  const {
    staffMembers,
    loading,
    setStaffMembers,
    setLoading,
    removeStaffMember,
  } = useStaffStore();

  const [roleEditVisible, setRoleEditVisible] = useState(false);
  const [selectedStaff, setSelectedStaff] = useState<StaffMemberDto | null>(
    null
  );
  if (!tenantId) throw new Error("Tenant ID is required");
  const loadStaffMembers = async () => {
    setLoading(true);
    const result = await StaffService.getStaffMembers(+tenantId);
    handleResult(result, {
      onSuccess: (data) => {
        setStaffMembers(data || []);
      },
      onServerError: (errors) => {
        handleServerError(
          errors,
          "Failed to load staff members. An error occurred while loading staff members."
        );
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  useEffect(() => {
    loadStaffMembers();
  }, [tenantId]);

  const handleRemoveStaff = async (staffSsoId: string) => {
    const result = await StaffService.removeStaff(+tenantId, staffSsoId);
    handleResult(result, {
      onSuccess: () => {
        removeStaffMember(staffSsoId);
        message.success("Staff member removed successfully");
      },
      onServerError: (errors) => {
        handleServerError(errors, "Failed to remove staff member");
      },
    });
  };

  const handleEditRoles = (staff: StaffMemberDto) => {
    setSelectedStaff(staff);
    setRoleEditVisible(true);
  };

  const handleRoleUpdate = () => {
    setRoleEditVisible(false);
    setSelectedStaff(null);
    loadStaffMembers();
  };

  const columns = [
    {
      title: "Name",
      dataIndex: "name",
      key: "name",
      render: (name: string, record: StaffMemberDto) => (
        <Space>
          <UserOutlined />
          <span>{name || record.email || "Unknown"}</span>
        </Space>
      ),
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
      render: (email: string) => email || "N/A",
    },
    {
      title: "Roles",
      dataIndex: "roles",
      key: "roles",
      render: (roles: string[]) => (
        <Space wrap>
          {roles.length > 0 ? (
            roles.map((role) => (
              <Tag key={role} color="blue">
                {role}
              </Tag>
            ))
          ) : (
            <Tag color="default">No roles</Tag>
          )}
        </Space>
      ),
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
      render: (_: unknown, record: StaffMemberDto) => (
        <Space>
          <Tooltip title="Edit Roles">
            <Button
              type="primary"
              icon={<EditOutlined />}
              size="small"
              onClick={() => handleEditRoles(record)}
            >
              Edit Roles
            </Button>
          </Tooltip>
          <Popconfirm
            title="Remove Staff Member"
            description="Are you sure you want to remove this staff member? This action cannot be undone."
            onConfirm={() => handleRemoveStaff(record.ssoId)}
            okText="Yes"
            cancelText="No"
            placement="topRight"
          >
            <Tooltip title="Remove Staff Member">
              <Button
                type="primary"
                danger
                icon={<DeleteOutlined />}
                size="small"
              >
                Remove
              </Button>
            </Tooltip>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: "16px" }}>
        <Title level={3}>Staff Members</Title>
        <p>Manage your team members and their roles.</p>
      </div>

      <Table
        columns={columns}
        dataSource={staffMembers}
        rowKey="ssoId"
        loading={loading}
        pagination={{
          pageSize: 10,
          showSizeChanger: true,
          showQuickJumper: true,
          showTotal: (total, range) =>
            `${range[0]}-${range[1]} of ${total} staff members`,
        }}
        locale={{
          emptyText: "No staff members found",
        }}
      />

      <Modal
        title="Edit Staff Roles"
        open={roleEditVisible}
        onCancel={() => setRoleEditVisible(false)}
        footer={null}
        width={600}
      >
        {selectedStaff && (
          <StaffRoleEdit
            staffMember={selectedStaff}
            onSuccess={handleRoleUpdate}
            onCancel={() => setRoleEditVisible(false)}
          />
        )}
      </Modal>
    </div>
  );
};

export default StaffList;
