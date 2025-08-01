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
  notification,
} from "antd";
import { DeleteOutlined, EditOutlined, UserOutlined } from "@ant-design/icons";
import { apiClient } from "../../common/apiClient";
import { useStaffStore } from "./staffStore";
import { StaffMemberDto } from "./StaffModel";
import StaffRoleEdit from "./StaffRoleEdit";

const { Title } = Typography;

const StaffList: React.FC = () => {
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

  const loadStaffMembers = async () => {
    setLoading(true);
    apiClient.get<StaffMemberDto[]>("/api/staff", {
      onSuccess: (data) => {
        setStaffMembers(data || []);
      },
      onServerError: () => {
        notification.error({
          message:
            "Failed to load staff members. An error occurred while loading staff members.",
        });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  useEffect(() => {
    loadStaffMembers();
  }, []);

  const handleRemoveStaff = async (staffId: number) => {
    apiClient.delete<boolean>(`/api/staff/${staffId}`, undefined, {
      onSuccess: () => {
        removeStaffMember(staffId.toString());
        message.success("Staff member removed successfully");
        loadStaffMembers();
      },
      onServerError: () => {
        notification.error({ message: "Failed to remove staff member" });
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
            onConfirm={() => handleRemoveStaff(record.id)}
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
        rowKey={(record) => record.id.toString()}
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
