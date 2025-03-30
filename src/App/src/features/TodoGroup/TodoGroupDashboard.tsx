import { useState } from "react";
import { Button, Card, Space } from "antd";
import { useTodoGroupStore } from "./todoGroupStore";
import { TodoGroupEdit } from "./TodoGroupEdit";
import TodoGroupList from "./TodoGroupList";
import TodoGroupCreate from "./TodoGroupCreate";
export const TodoGroupDashboard = () => {
  const { editTodoGroup } = useTodoGroupStore();
  const [isModalVisible, setIsModalVisible] = useState(false);

  const handleCancel = () => {
    setIsModalVisible(false);
  };

  return (
    <Card>
      <h1>Manage Todo Lists</h1>
      {editTodoGroup ? (
        <TodoGroupEdit />
      ) : (
        <>
          <Space style={{ marginBottom: 16 }}>
            <Button
              type="primary"
              onClick={() => {
                setIsModalVisible(true);
              }}
            >
              Create Todo List
            </Button>
          </Space>
          <TodoGroupList />
          <TodoGroupCreate visible={isModalVisible} onClose={handleCancel} />
        </>
      )}
    </Card>
  );
};
