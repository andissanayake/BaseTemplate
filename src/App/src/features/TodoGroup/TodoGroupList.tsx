/* eslint-disable @typescript-eslint/no-explicit-any */
// TodoGroupList.tsx
import React, { useEffect } from "react";
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
import { TodoGroup } from "./Model";
import {
  DeleteOutlined,
  EditOutlined,
  FundOutlined,
  PlusOutlined,
} from "@ant-design/icons";

const TodoGroupList: React.FC = () => {
  const {
    todoGroupList,
    loading,
    fetchTodoGroups,
    deleteTodoGroup,
    setTodoGroupCurrent,
  } = useTodoGroupStore();
  const navigate = useNavigate();

  useEffect(() => {
    fetchTodoGroups(); // Fetch the todo groups when the component loads
  }, [fetchTodoGroups]);

  const handleView = (record: TodoGroup) => {
    setTodoGroupCurrent(record);
    navigate(`view/${record.id}`);
  };

  const handleEdit = (record: TodoGroup) => {
    setTodoGroupCurrent(record);
    navigate(`edit/${record.id}`);
  };

  const handleDelete = (record: TodoGroup) => {
    deleteTodoGroup(record);
    notification.success({ message: "Operation successful!" });
  };

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
