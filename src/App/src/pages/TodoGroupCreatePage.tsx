import { Card, Space, Typography } from "antd";
import TodoGroupCreate from "../features/TodoGroup/TodoGroupCreate";

export const TodoGroupCreatePage = () => {
  return (
    <Card>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List Create
        </Typography.Title>
      </Space>

      <TodoGroupCreate />
    </Card>
  );
};
