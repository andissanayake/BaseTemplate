import { Button, Card, Space, Typography } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import TodoGroupList from "../features/TodoGroup/TodoGroupList";

export const TodoGroupListPage = () => {
  const navigate = useNavigate();

  return (
    <Card>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo Lists
        </Typography.Title>
        <Button
          type="primary"
          shape="circle"
          icon={<PlusOutlined />}
          onClick={() => navigate("create")}
        />
      </Space>

      <TodoGroupList />
    </Card>
  );
};
