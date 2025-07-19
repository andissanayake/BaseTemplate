import { useParams } from "react-router-dom";
import { useTodoGroupStore } from "./todoGroupStore";
import { Descriptions, Space, Typography, notification } from "antd";
import TodoItemList from "../TodoItem/TodoItemList";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { TodoGroup } from "./TodoGroupModel";
import { apiClient } from "../../common/apiClient";
import { useState } from "react";

export const TodoGroupView = () => {
  const [currentTodoGroup, setCurrentTodoGroup] = useState<TodoGroup | null>(
    null
  );
  const { setLoading } = useTodoGroupStore();
  const { listId } = useParams();

  if (!listId) throw new Error("listId is required");

  useAsyncEffect(async () => {
    setLoading(true);
    apiClient.get<TodoGroup>(`/api/todoLists/${listId}`, {
      onSuccess: (data) => {
        setCurrentTodoGroup(data ? data : null);
      },
      onServerError: () => {
        notification.error({ message: "Failed to fetch todo list item!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [listId, setLoading]);

  return (
    <>
      {currentTodoGroup && (
        <>
          <Space className="mb-4">
            <Typography.Title level={3} style={{ margin: 0 }}>
              Todo List View
            </Typography.Title>
          </Space>
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
            <TodoItemList />
          </div>
        </>
      )}
    </>
  );
};
