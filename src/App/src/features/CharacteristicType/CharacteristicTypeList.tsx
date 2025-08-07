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
import { useCharacteristicTypeStore } from "./characteristicTypeStore";
import { CharacteristicType } from "./CharacteristicTypeModel";
import { apiClient } from "../../common/apiClient";
import { Link } from "react-router-dom";

const CharacteristicTypeList: React.FC = () => {
  const {
    characteristicTypeList,
    loading,
    totalCount,
    currentPage,
    pageSize,
    setPagination,
    setLoading,
    setTotalCount,
    setCharacteristicTypeList,
    setCurrentPage,
  } = useCharacteristicTypeStore();

  const loadCharacteristicTypes = useCallback(async () => {
    setLoading(true);
    apiClient.get<CharacteristicType[]>(`/api/characteristic-type`, {
      onSuccess: (data) => {
        setTotalCount(data?.length || 0);
        setCharacteristicTypeList(data || []);
      },
      onServerError: () => {
        setCharacteristicTypeList([]);
        notification.error({ message: "Failed to load characteristic types!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [setLoading, setTotalCount, setCharacteristicTypeList]);

  useEffect(() => {
    loadCharacteristicTypes();
  }, [loadCharacteristicTypes]);

  const handleDelete = async (id: number) => {
    setLoading(true);
    apiClient.delete<boolean>(`/api/characteristic-type/${id}`, undefined, {
      onSuccess: () => {
        const newTotalCount = totalCount - 1;
        const lastPage = Math.ceil(newTotalCount / pageSize);
        if (currentPage > lastPage && lastPage > 0) {
          setCurrentPage(lastPage);
        }
        notification.success({
          message: "Characteristic type deleted successfully!",
        });
        loadCharacteristicTypes();
      },
      onServerError: () => {
        notification.error({
          message: "Failed to delete characteristic type!",
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
      title: "Actions",
      render: (_: unknown, record: CharacteristicType) => (
        <Space>
          <Link
            to={`/characteristic-types/view/${record.id}`}
            rel="noopener noreferrer"
          >
            <Button type="link" icon={<EyeOutlined />} />
          </Link>
          <Link
            to={`/characteristic-types/edit/${record.id}`}
            rel="noopener noreferrer"
          >
            <Button type="link" icon={<EditOutlined />} />
          </Link>
          <Popconfirm
            title="Are you sure to delete this characteristic type?"
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
          Characteristic Types
        </Typography.Title>
        <Link to={`/characteristic-types/create`} rel="noopener noreferrer">
          <Button type="primary" icon={<PlusOutlined />}>
            Create
          </Button>
        </Link>
      </Space>
      <Table
        columns={columns}
        dataSource={characteristicTypeList}
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

export { CharacteristicTypeList };
