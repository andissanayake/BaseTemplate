import { Card, Space, Typography } from "antd";

export const NotFoundPage = () => {
  return (
    <Card>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Not found
        </Typography.Title>
      </Space>
    </Card>
  );
};
