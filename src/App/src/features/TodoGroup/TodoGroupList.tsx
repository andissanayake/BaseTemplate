/* eslint-disable @typescript-eslint/no-explicit-any */
// TodoGroupList.tsx
import React from "react";
import {
  Table,
  Button,
  Space,
  notification,
  Popconfirm,
  Typography,
} from "antd";
import { useTodoGroupStore } from "./todoGroupStore";
import { useNavigate } from "react-router-dom";
import { TodoGroup } from "./TodoGroupModel";
import {
  DeleteOutlined,
  EditOutlined,
  FundOutlined,
  PlusOutlined,
} from "@ant-design/icons";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { TodoGroupService } from "./todoGroupService";
import { handleResult } from "../../common/handleResult";

const TodoGroupList: React.FC = () => {
  const { todoGroupList, loading, setLoading, setTodoGroupList } =
    useTodoGroupStore();
  const navigate = useNavigate();

  const loadTodoGroupList = async () => {
    setLoading(true);
    const response = await TodoGroupService.fetchTodoGroups();
    handleResult(response, {
      onSuccess: (data) => {
        setTodoGroupList(data?.lists ?? []);
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const handleView = (record: TodoGroup) => {
    navigate(`view/${record.id}`);
  };

  const handleEdit = (record: TodoGroup) => {
    navigate(`edit/${record.id}`);
  };

  const handleDelete = async (record: TodoGroup) => {
    setLoading(true);
    const response = await TodoGroupService.deleteTodoGroup(record);
    return handleResult(response, {
      onSuccess: () => {
        loadTodoGroupList();
        notification.success({ message: "Todo list deleted successfully!" });
      },
      onServerError: () => {
        notification.error({ message: "Failed to delete todo list!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  useAsyncEffect(async () => {
    loadTodoGroupList();
  }, []);

  const columns = [
    {
      title: "Todo List Title",
      key: "title",
      render: (_: any, record: TodoGroup) => <span>{record.title}</span>,
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: any, record: TodoGroup) => (
        <Space>
          <Button
            type="link"
            icon={<FundOutlined />}
            onClick={() => handleView(record)}
            size="large"
          />
          <Button
            type="link"
            onClick={() => handleEdit(record)}
            icon={<EditOutlined />}
          ></Button>
          <Popconfirm
            title="Are you sure to delete this todo list?"
            onConfirm={() => handleDelete(record)}
          >
            <Button type="link" icon={<DeleteOutlined />}></Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo Lists
        </Typography.Title>
        <Button
          type="primary"
          shape="circle"
          icon={<PlusOutlined />}
          onClick={() => navigate("create")}
        />
      </Space>
      <Table
        columns={columns}
        dataSource={todoGroupList}
        loading={loading}
        pagination={{ pageSize: 5 }}
        rowKey="id"
      />
    </>
  );
};

export default TodoGroupList;
