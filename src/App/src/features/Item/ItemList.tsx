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
import { useItemStore } from "./itemStore";
import { Item } from "./ItemModel";
import { apiClient } from "../../common/apiClient";
import { Link } from "react-router-dom";

const ItemList: React.FC = () => {
  const {
    itemList,
    loading,
    totalCount,
    currentPage,
    pageSize,
    setPagination,
    setLoading,
    setTotalCount,
    setItemList,
    setCurrentPage,
  } = useItemStore();

  const loadItems = useCallback(async () => {
    setLoading(true);
    apiClient.get<{ items: Item[]; totalCount: number }>(
      `/api/item?PageNumber=${currentPage}&PageSize=${pageSize}`,
      {
        onSuccess: (data) => {
          setTotalCount(data?.totalCount || 0);
          setItemList(data?.items || []);
        },
        onServerError: () => {
          setItemList([]);
          notification.error({ message: "Failed to load items!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
  }, [currentPage, pageSize, setLoading, setTotalCount, setItemList]);

  useEffect(() => {
    loadItems();
  }, [loadItems]);

  const handleDelete = async (id: number) => {
    setLoading(true);
    apiClient.delete<boolean>(`/api/item/${id}`, undefined, {
      onSuccess: () => {
        const newTotalCount = totalCount - 1;
        const lastPage = Math.ceil(newTotalCount / pageSize);
        if (currentPage > lastPage && lastPage > 0) {
          setCurrentPage(lastPage);
        }
        notification.success({ message: "Item deleted successfully!" });
        loadItems();
      },
      onServerError: () => {
        notification.error({ message: "Failed to delete item!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const handlePaginationChange = (page: number, pageSize: number) => {
    setPagination(page, pageSize);
  };

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
      title: "Specification",
      dataIndex: "specificationFullPath",
      key: "specificationFullPath",
      render: (specificationFullPath: string) =>
        specificationFullPath || "No specification",
    },
    {
      title: "Tags",
      dataIndex: "tags",
      key: "tags",
      render: (tags: string) => (
        <Space wrap>
          {tags
            ?.split(",")
            .filter(Boolean)
            .map((tag) => (
              <Tag key={tag}>{tag}</Tag>
            ))}
        </Space>
      ),
    },
    {
      title: "Actions",
      render: (_: unknown, record: Item) => (
        <Space>
          <Link to={`/items/view/${record.id}`} rel="noopener noreferrer">
            <Button type="link" icon={<EyeOutlined />} />
          </Link>
          <Link to={`/items/edit/${record.id}`} rel="noopener noreferrer">
            <Button type="link" icon={<EditOutlined />} />
          </Link>
          <Popconfirm
            title="Are you sure to delete this item?"
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
          Item List
        </Typography.Title>
        <Link to={`/items/create`} rel="noopener noreferrer">
          <Button type="primary" icon={<PlusOutlined />}>
            Create
          </Button>
        </Link>
      </Space>
      <Table
        columns={columns}
        dataSource={itemList}
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

export { ItemList };
