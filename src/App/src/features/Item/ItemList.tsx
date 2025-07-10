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
import { ItemService } from "./itemService";
import { handleResult } from "../../common/handleResult";
import { Link } from "react-router-dom";
import { useAuthStore } from "../../auth/authStore";

const ItemList: React.FC = () => {
  const { tenantId } = useAuthStore();

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
    if (!tenantId) {
      notification.error({ message: "Tenant ID is required to fetch items." });
      return;
    }
    setLoading(true);
    const response = await ItemService.fetchItems(
      currentPage,
      pageSize,
      parseInt(tenantId)
    );
    handleResult(response, {
      onSuccess: (data) => {
        setTotalCount(data?.totalCount || 0);
        setItemList(data?.items || []);
      },
      onServerError: (error) => {
        setItemList([]);
        notification.error({ message: error?.message });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [currentPage, pageSize, setLoading, setTotalCount, setItemList, tenantId]);

  useEffect(() => {
    if (tenantId) {
      loadItems();
    }
  }, [loadItems, tenantId]);

  if (!tenantId) {
    return (
      <div style={{ textAlign: "center", padding: "20px" }}>
        <Typography.Title level={3}>No Tenant Selected</Typography.Title>
        <Typography.Text>Please select a tenant to view items.</Typography.Text>
      </div>
    );
  }

  const handleDelete = async (id: number) => {
    setLoading(true);
    const response = await ItemService.deleteItem(tenantId, id);
    handleResult(response, {
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

  // Always use the standalone items path since we're using auth store tenant ID
  const getBasePath = () => `/items`;

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
      title: "Price",
      dataIndex: "price",
      key: "price",
      render: (price: number) => `$${price.toFixed(2)}`,
    },
    {
      title: "Categories",
      dataIndex: "category",
      key: "category",
      render: (category: string) => (
        <Space wrap>
          {category
            ?.split(",")
            .filter(Boolean)
            .map((cat) => (
              <Tag key={cat}>{cat}</Tag>
            ))}
        </Space>
      ),
    },
    {
      title: "Actions",
      render: (_: unknown, record: Item) => (
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
        <Link to={`${getBasePath()}/create`} rel="noopener noreferrer">
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

export default ItemList;
