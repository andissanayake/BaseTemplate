import { Card, Space, Typography } from "antd";
import { TodoGroupEdit } from "../features/TodoGroup/TodoGroupEdit";

export const TodoGroupEditPage = () => {
  return (
    <Card>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List Edit
        </Typography.Title>
      </Space>

      <TodoGroupEdit />
    </Card>
  );
};
