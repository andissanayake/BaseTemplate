import React, { useEffect, useCallback } from "react";
import {
  Table,
  Button,
  Space,
  notification,
  Popconfirm,
  Typography,
  Tag,
} from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  PlusOutlined,
  EyeOutlined,
} from "@ant-design/icons";
import { useItemAttributeTypeStore } from "./itemAttributeTypeStore";
import { ItemAttributeType } from "./ItemAttributeTypeModel";
import { apiClient } from "../../common/apiClient";
import { Link } from "react-router-dom";
import { useAuthStore } from "../../auth/authStore";

const ItemAttributeTypeList: React.FC = () => {
  const { tenant } = useAuthStore();

  const {
    itemAttributeTypeList,
    loading,
    totalCount,
    currentPage,
    pageSize,
    setPagination,
    setLoading,
    setTotalCount,
    setItemAttributeTypeList,
    setCurrentPage,
  } = useItemAttributeTypeStore();

  if (!tenant?.id) throw new Error("Tenant ID is required");

  const loadItemAttributeTypes = useCallback(async () => {
    setLoading(true);
    apiClient.get<ItemAttributeType[]>(`/api/itemAttributeTypes`, {
      onSuccess: (data) => {
        setTotalCount(data?.length || 0);
        setItemAttributeTypeList(data || []);
      },
      onServerError: () => {
        setItemAttributeTypeList([]);
        notification.error({ message: "Failed to load item attribute types!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [setLoading, setTotalCount, setItemAttributeTypeList, tenant?.id]);

  useEffect(() => {
    loadItemAttributeTypes();
  }, [loadItemAttributeTypes]);

  const handleDelete = async (id: number) => {
    setLoading(true);
    apiClient.delete<boolean>(`/api/itemAttributeTypes/${id}`, undefined, {
      onSuccess: () => {
        const newTotalCount = totalCount - 1;
        const lastPage = Math.ceil(newTotalCount / pageSize);
        if (currentPage > lastPage && lastPage > 0) {
          setCurrentPage(lastPage);
        }
        notification.success({
          message: "Item attribute type deleted successfully!",
        });
        loadItemAttributeTypes();
      },
      onServerError: () => {
        notification.error({
          message: "Failed to delete item attribute type!",
        });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const handlePaginationChange = (page: number, pageSize: number) => {
    setPagination(page, pageSize);
  };

  const getBasePath = () => `/item-attribute-types`;

  const columns = [
    {
      title: "Name",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Description",
      dataIndex: "description",
      key: "description",
    },
    {
      title: "Status",
      dataIndex: "isActive",
      key: "isActive",
      render: (isActive: boolean) => (
        <Tag color={isActive ? "green" : "red"}>
          {isActive ? "Active" : "Inactive"}
        </Tag>
      ),
    },
    {
      title: "Created",
      dataIndex: "created",
      key: "created",
      render: (created: string) => new Date(created).toLocaleDateString(),
    },
    {
      title: "Created By",
      dataIndex: "createdBy",
      key: "createdBy",
    },
    {
      title: "Actions",
      render: (_: unknown, record: ItemAttributeType) => (
        <Space>
          <Link
            to={`${getBasePath()}/view/${record.id}`}
            rel="noopener noreferrer"
          >
            <Button type="link" icon={<EyeOutlined />} />
          </Link>
          <Link
            to={`${getBasePath()}/edit/${record.id}`}
            rel="noopener noreferrer"
          >
            <Button type="link" icon={<EditOutlined />} />
          </Link>
          <Popconfirm
            title="Are you sure to delete this item attribute type?"
            onConfirm={() => handleDelete(record.id)}
          >
            <Button type="link" icon={<DeleteOutlined />} />
          </Popconfirm>
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
          Item Attribute Types
        </Typography.Title>
        <Link to={`${getBasePath()}/create`} rel="noopener noreferrer">
          <Button type="primary" icon={<PlusOutlined />}>
            Create
          </Button>
        </Link>
      </Space>
      <Table
        columns={columns}
        dataSource={itemAttributeTypeList}
        loading={loading}
        pagination={{
          current: currentPage,
          pageSize,
          total: totalCount,
          onChange: handlePaginationChange,
        }}
        rowKey="id"
      />
    </>
  );
};

export default ItemAttributeTypeList;
