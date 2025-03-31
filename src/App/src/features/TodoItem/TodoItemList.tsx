/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from "react";
import { Table, Button, Space, notification, Popconfirm } from "antd";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { useTodoItemStore } from "./todoItemStore";
import { TodoItem, TodoItemService } from "./todoItemService";
import { useParams } from "react-router-dom";
import { TodoItemEdit } from "./TodoItemEdit";

const TodoItemList: React.FC = () => {
  const { todoItemList, loading, setTodoItemEdit, fetchTodoItems } =
    useTodoItemStore();
  const { listId } = useParams();
  const [isModalVisible, setIsModalVisible] = useState(false);

  if (!listId) throw new Error("listId is required");

  const handleEdit = (record: TodoItem) => {
    setTodoItemEdit(record);
    setIsModalVisible(true);
  };

  const handleCancel = async () => {
    setIsModalVisible(false);
    await fetchTodoItems(+listId, 1, 100);
  };

  useEffect(() => {
    fetchTodoItems(+listId, 1, 100);
  }, [fetchTodoItems, listId]);

  const handleDelete = async (values: TodoItem) => {
    await TodoItemService.deleteTodoItem(values);
    notification.success({ message: "Operation successful!" });
    if (listId) await fetchTodoItems(+listId, 1, 100);
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
      <TodoItemEdit visible={isModalVisible} onClose={handleCancel} />
    </>
  );
};

export default TodoItemList;
