/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState, useEffect, useCallback } from "react";
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
import { TodoItemService } from "./todoItemService";
import { handleResult } from "../../common/handleResult";

const TodoItemList: React.FC = () => {
  const {
    todoItemList,
    loading,
    totalCount,
    currentPage,
    pageSize,
    setPagination,
    setLoading,
    setTotalCount,
    setTodoItemList,
    setCurrentPage,
  } = useTodoItemStore();
  const { listId } = useParams();
  const [isCreateModalVisible, setIsCreateModalVisible] = useState(false);
  const [isEditModalVisible, setIsEditModalVisible] = useState(false);
  const [currentTodoItem, setCurrentTodoItem] = useState<TodoItem | null>(null);

  if (!listId) throw "List Id is required.";

  const loadTodoItems = useCallback(async () => {
    setLoading(true);
    const response = await TodoItemService.fetchTodoItems(
      +listId,
      currentPage,
      pageSize
    );
    handleResult(response, {
      onSuccess: (data) => {
        setTotalCount(data?.totalCount || 0);
        setTodoItemList(data?.items || []);
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [
    listId,
    currentPage,
    pageSize,
    setLoading,
    setTotalCount,
    setTodoItemList,
  ]);

  useEffect(() => {
    loadTodoItems();
  }, [loadTodoItems]);

  const handleEdit = (item: TodoItem) => {
    setCurrentTodoItem(item);
    setIsEditModalVisible(true);
  };

  const handleCreate = () => {
    setIsCreateModalVisible(true);
  };

  const handleDelete = async (id: number) => {
    setLoading(true);
    const response = await TodoItemService.deleteTodoItem(id);
    handleResult(response, {
      onSuccess: () => {
        const newTotalCount = totalCount - 1;
        const lastPage = Math.ceil(newTotalCount / pageSize);
        if (currentPage > lastPage && lastPage > 0) {
          setCurrentPage(lastPage);
        }
        notification.success({ message: "Todo Item deleted successfully!" });
        loadTodoItems();
      },
      onServerError: () => {
        notification.error({ message: "Failed to delete todo item!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const handlePaginationChange = (page: number, pageSize: number) => {
    setPagination(page, pageSize);
    loadTodoItems();
  };

  const updateItemStatus = async (id: number, done: boolean) => {
    setLoading(true);
    const response = await TodoItemService.updateTodoItemStatus({ id, done });
    handleResult(response, {
      onSuccess: () => {
        notification.success({
          message: "Todo Item status updated successfully!",
        });
        loadTodoItems();
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };
  const columns = [
    {
      title: "Done",
      render: (_: any, record: TodoItem) => (
        <Checkbox
          checked={record.done}
          onChange={async (e) => {
            const updatedDoneStatus = e.target.checked;
            await updateItemStatus(record.id, updatedDoneStatus);
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
            fontSize: 18,
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
        onClose={() => {
          setIsCreateModalVisible(false);
        }}
        refresh={loadTodoItems}
      />
      <TodoItemEdit
        visible={isEditModalVisible}
        onClose={() => {
          setIsEditModalVisible(false);
        }}
        todoItem={currentTodoItem!}
        refresh={loadTodoItems}
      />
    </>
  );
};

export default TodoItemList;
