/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from "react";
import { Table, Button, Space, notification, Popconfirm, Checkbox } from "antd";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { useTodoItemStore } from "./todoItemStore";
import { PriorityLevel, TodoItem, TodoItemService } from "./todoItemService";
import { useParams } from "react-router-dom";
import { TodoItemEdit } from "./TodoItemEdit";

const TodoItemList: React.FC = () => {
  const {
    todoItemList,
    loading,
    setTodoItemEdit,
    fetchTodoItems,
    totalCount,
    updateItemStatus,
  } = useTodoItemStore();
  const { listId } = useParams();
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(5);

  if (!listId) throw new Error("listId is required");

  const handleEdit = (record: TodoItem) => {
    setTodoItemEdit(record);
    setIsModalVisible(true);
  };

  const handleCancel = async () => {
    setIsModalVisible(false);
    await fetchTodoItems(+listId, pageNumber, pageSize);
  };

  useEffect(() => {
    fetchTodoItems(+listId, pageNumber, pageSize);
  }, [fetchTodoItems, listId, pageNumber, pageSize]);

  const handleDelete = async (values: TodoItem) => {
    await TodoItemService.deleteTodoItem(values);
    notification.success({ message: "Operation successful!" });
    await fetchTodoItems(+listId, pageNumber, pageSize);
  };

  const columns = [
    {
      title: "Done",
      key: "done",
      render: (_: any, record: TodoItem) => (
        <span
          style={{
            display: "flex",
            alignItems: "center",
          }}
        >
          <Checkbox
            checked={record.done}
            onChange={async (e) => {
              const updatedDoneStatus = e.target.checked;
              await updateItemStatus(record.id, updatedDoneStatus);
            }}
          ></Checkbox>
        </span>
      ),
    },
    {
      title: "Todo Item Title",
      key: "title",
      render: (_: any, record: TodoItem) => (
        <span
          style={{
            display: "flex",
            alignItems: "center",
            textDecoration: record.done ? "line-through" : "none", // Add line-through if done
            fontWeight: record.done ? "normal" : "bold", // Make title bold if not done
            fontSize: "16px", // Set a larger font size for prominence
            color: record.done ? "#aaa" : "#000", // Change color when done (grayed out)
          }}
        >
          {record.title}
        </span>
      ),
    },
    {
      title: "Priority",
      key: "priority",
      render: (_: any, record: TodoItem) => (
        <span style={{ display: "flex", alignItems: "center" }}>
          {PriorityLevel[record.priority] || ""}
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
        pagination={{
          pageSize: 5,
          total: totalCount,
          onChange: (page, size) => {
            setPageNumber(page);
            setPageSize(size);
          },
        }}
        rowKey="id"
      />
      <TodoItemEdit visible={isModalVisible} onClose={handleCancel} />
    </>
  );
};

export default TodoItemList;
