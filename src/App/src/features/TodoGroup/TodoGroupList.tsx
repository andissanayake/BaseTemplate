/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect } from "react";
import { Table, Button, Space, notification, Popconfirm } from "antd";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { useTodoGroupStore } from "./todoGroupStore";
import { TodoGroup, TodoGroupService } from "./todoGroupService";

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
    const data = await TodoGroupService.deleteTodoGroup(values);
    console.log("Todo group deleted:", data);
    notification.success({ message: "Operation successful!" });
  };

  const columns = [
    {
      title: "Todo Group Name",
      dataIndex: "title",
      key: "title",
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
