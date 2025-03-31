import { Card, Space, Typography } from "antd";
import { TodoGroupView } from "../features/TodoGroup/TodoGroupView";

export const TodoGroupViewPage = () => {
  return (
    <Card>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List View
        </Typography.Title>
      </Space>

      <TodoGroupView />
    </Card>
  );
};
