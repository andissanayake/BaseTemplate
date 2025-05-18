import React, { useEffect, useCallback } from "react";
import {
  Table,
  Button,
  Space,
  notification,
  Popconfirm,
  Typography,
} from "antd";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";
import { useItemStore } from "./itemStore";
import { Item } from "./ItemModel";
import { ItemService } from "./itemService";
import { handleResult } from "../../common/handleResult";

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
    const response = await ItemService.fetchItems(currentPage, pageSize);
    handleResult(response, {
      onSuccess: (data) => {
        setTotalCount(data?.totalCount || 0);
        setItemList(data?.items || []);
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [currentPage, pageSize, setLoading, setTotalCount, setItemList]);

  useEffect(() => {
    loadItems();
  }, [loadItems]);

  const handleDelete = async (id: number) => {
    setLoading(true);
    const response = await ItemService.deleteItem(id);
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
      title: "Quantity",
      dataIndex: "quantity",
      key: "quantity",
    },
    {
      title: "Category",
      dataIndex: "category",
      key: "category",
    },
    {
      title: "Actions",
      render: (_: unknown, record: Item) => (
        <Space>
          <Button type="link" icon={<EditOutlined />} />
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
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Item List
        </Typography.Title>
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
