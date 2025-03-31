import { useState } from "react";
import { Button, Card, Space } from "antd";
import TodoItemList from "./TodoItemList";
import TodoItemCreate from "./TodoItemCreate";

export const TodoItemDashboard = () => {
  const [isModalVisible, setIsModalVisible] = useState(false);

  const handleCancel = () => {
    setIsModalVisible(false);
  };

  return (
    <Card>
      <h1>Manage Todo Items</h1>

      <>
        <Space style={{ marginBottom: 16 }}>
          <Button
            type="primary"
            onClick={() => {
              setIsModalVisible(true);
            }}
          >
            Create Todo Item
          </Button>
        </Space>
        <TodoItemList />
        <TodoItemCreate visible={isModalVisible} onClose={handleCancel} />
      </>
    </Card>
  );
};
