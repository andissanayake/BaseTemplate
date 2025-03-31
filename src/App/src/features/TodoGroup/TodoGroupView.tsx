import { useParams } from "react-router-dom";
import { useTodoGroupStore } from "./todoGroupStore";
import { Button, Descriptions, Space, Typography } from "antd";
import { useEffect, useState } from "react";
import { TodoGroupService } from "./todoGroupService";
import TodoItemList from "../TodoItem/TodoItemList";
import TodoItemCreate from "../TodoItem/TodoItemCreate";
import { PlusOutlined } from "@ant-design/icons";
import { useTodoItemStore } from "../TodoItem/todoItemStore";

export const TodoGroupView = () => {
  const { currentTodoGroup, setTodoGroupCurrent } = useTodoGroupStore();
  const { fetchTodoItems } = useTodoItemStore();
  const { listId } = useParams();
  const [isModalVisible, setIsModalVisible] = useState(false);

  if (!listId) throw new Error("listId is required");

  const handleCancel = async () => {
    setIsModalVisible(false);
    await fetchTodoItems(+listId, 1, 100);
  };

  useEffect(() => {
    TodoGroupService.fetchTodoGroupById(listId).then((res) =>
      setTodoGroupCurrent(res.data, null)
    );
  }, [listId, setTodoGroupCurrent]);

  return (
    <>
      {currentTodoGroup && (
        <>
          <Descriptions column={1} bordered>
            <Descriptions.Item label="Name">
              {currentTodoGroup.title}
            </Descriptions.Item>
            <Descriptions.Item label="Colour">
              <div
                style={{
                  display: "inline-block",
                  width: 24,
                  height: 24,
                  borderRadius: "50%",
                  backgroundColor: currentTodoGroup.colour,
                  border: "1px solid #ccc",
                }}
              />
            </Descriptions.Item>
          </Descriptions>
          <div className="mt-4">
            <Space className="mb-4">
              <Typography.Title level={3} style={{ margin: 0 }}>
                Todo Item List
              </Typography.Title>
              <Button
                type="primary"
                shape="circle"
                icon={<PlusOutlined />}
                onClick={() => setIsModalVisible(true)}
              />
            </Space>
            <TodoItemList />
          </div>
          <TodoItemCreate visible={isModalVisible} onClose={handleCancel} />
        </>
      )}
    </>
  );
};
