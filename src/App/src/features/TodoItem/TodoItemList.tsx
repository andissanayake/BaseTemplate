/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect } from "react";
import { Table, Button, Space, notification, Popconfirm } from "antd";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { useTodoGroupStore } from "./todoItemStore";
import { TodoGroup, TodoGroupService } from "./todoItemService";

const TodoGroupList: React.FC = () => {
  const { todoGroupList, loading, setTodoGroupEdit, fetchTodoGroups } =
    useTodoGroupStore();

  const handleEdit = (record: TodoGroup) => {
    setTodoGroupEdit(record);
  };

  useEffect(() => {
    fetchTodoGroups();
  }, [fetchTodoGroups]);

  const handleDelete = async (values: TodoGroup) => {
    await TodoGroupService.deleteTodoGroup(values);
    notification.success({ message: "Operation successful!" });
    await fetchTodoGroups();
  };

  const columns = [
    {
      title: "Todo List Title",
      key: "title",
      render: (_: any, record: TodoGroup) => (
        <span style={{ display: "flex", alignItems: "center" }}>
          {record.title}
          <span
            style={{
              display: "inline-block",
              width: 20,
              height: 20,
              backgroundColor: record.colour,
              marginLeft: 10,
              borderRadius: "50%",
            }}
          />
        </span>
      ),
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: any, record: TodoGroup) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
            size="large"
          />
          <Popconfirm
            title="Are you sure to delete this todo group?"
            onConfirm={() => handleDelete(record)}
          >
            <Button type="link" icon={<DeleteOutlined />} size="large"></Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
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
