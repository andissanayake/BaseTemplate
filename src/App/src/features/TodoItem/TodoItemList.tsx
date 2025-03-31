/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect } from "react";
import { Table, Button, Space, notification, Popconfirm } from "antd";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { useTodoItemStore } from "./todoItemStore";
import { TodoItem, TodoItemService } from "./todoItemService";

const TodoItemList: React.FC = () => {
  const { todoItemList, loading, setTodoItemEdit, fetchTodoItems } =
    useTodoItemStore();

  const handleEdit = (record: TodoItem) => {
    setTodoItemEdit(record);
  };

  useEffect(() => {
    fetchTodoItems();
  }, [fetchTodoItems]);

  const handleDelete = async (values: TodoItem) => {
    await TodoItemService.deleteTodoItem(values);
    notification.success({ message: "Operation successful!" });
    await fetchTodoItems();
  };

  const columns = [
    {
      title: "Todo Item Title",
      key: "title",
      render: (_: any, record: TodoItem) => (
        <span style={{ display: "flex", alignItems: "center" }}>
          {record.title}
        </span>
      ),
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: any, record: TodoItem) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
            size="large"
          />
          <Popconfirm
            title="Are you sure to delete this todo item?"
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
        dataSource={todoItemList}
        loading={loading}
        pagination={{ pageSize: 5 }}
        rowKey="id"
      />
    </>
  );
};

export default TodoItemList;
