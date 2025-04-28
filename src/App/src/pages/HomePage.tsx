import { Card, Space, Typography } from "antd";

export const HomePage = () => {
  return (
    <Card>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Home Page
        </Typography.Title>
      </Space>
    </Card>
  );
};
