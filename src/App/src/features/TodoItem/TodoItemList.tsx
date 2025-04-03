/* eslint-disable @typescript-eslint/no-explicit-any */
// TodoItemList.tsx
import React, { useState, useEffect } from "react";
import {
  Table,
  Button,
  Space,
  notification,
  Popconfirm,
  Checkbox,
  Typography,
} from "antd";
import { EditOutlined, DeleteOutlined, PlusOutlined } from "@ant-design/icons";
import { useTodoItemStore } from "./todoItemStore";
import { useParams } from "react-router-dom";
import TodoItemCreate from "./TodoItemCreate";
import TodoItemEdit from "./TodoItemEdit";
import { PriorityLevel, TodoItem } from "./TodoItemModel";

const TodoItemList: React.FC = () => {
  const {
    todoItemList,
    loading,
    fetchTodoItems,
    totalCount,
    deleteTodoItem,
    currentPage,
    pageSize,
    setPagination,
    updateItemStatus,
  } = useTodoItemStore();
  const { listId } = useParams();
  const [isCreateModalVisible, setIsCreateModalVisible] = useState(false);
  const [isEditModalVisible, setIsEditModalVisible] = useState(false);
  const [currentTodoItem, setCurrentTodoItem] = useState<TodoItem | null>(null);

  if (!listId) throw "List Id is required.";
  // Fetch todo items when the listId changes
  useEffect(() => {
    if (listId) {
      fetchTodoItems(+listId);
    }
  }, [listId, fetchTodoItems, currentPage, pageSize]);

  const handleEdit = (item: TodoItem) => {
    setCurrentTodoItem(item);
    setIsEditModalVisible(true);
  };

  const handleCreate = () => {
    setIsCreateModalVisible(true);
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteTodoItem(id, +listId);
      notification.success({ message: "Item deleted successfully!" });
    } catch (error) {
      console.log(error);
      notification.error({ message: "Failed to delete item!" });
    }
  };

  const handlePaginationChange = (page: number, pageSize: number) => {
    setPagination(page, pageSize);
    fetchTodoItems(+listId);
  };

  const columns = [
    {
      title: "Done",
      render: (_: any, record: TodoItem) => (
        <Checkbox
          checked={record.done}
          onChange={async (e) => {
            const updatedDoneStatus = e.target.checked;
            await updateItemStatus(record.id, updatedDoneStatus, +listId);
          }}
        />
      ),
    },
    {
      title: "Title",
      render: (_: any, record: TodoItem) => (
        <span
          style={{
            textDecoration: record.done ? "line-through" : "none",
            fontWeight: record.done ? "normal" : "bold",
          }}
        >
          {record.title}
        </span>
      ),
    },
    {
      title: "Priority",
      render: (_: any, record: TodoItem) => PriorityLevel[record.priority],
    },
    {
      title: "Actions",
      render: (_: any, record: TodoItem) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          />
          <Popconfirm
            title="Are you sure to delete this todo item?"
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
          Todo Item List
        </Typography.Title>
        <Button
          type="primary"
          shape="circle"
          icon={<PlusOutlined />}
          onClick={handleCreate}
        />
      </Space>
      <Table
        columns={columns}
        dataSource={todoItemList}
        loading={loading}
        pagination={{
          current: currentPage,
          pageSize,
          total: totalCount,
          onChange: handlePaginationChange,
        }}
        rowKey="id"
      />
      <TodoItemCreate
        visible={isCreateModalVisible}
        onClose={() => setIsCreateModalVisible(false)}
      />
      <TodoItemEdit
        visible={isEditModalVisible}
        onClose={() => setIsEditModalVisible(false)}
        todoItem={currentTodoItem!}
      />
    </>
  );
};

export default TodoItemList;
