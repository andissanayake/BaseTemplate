import { Avatar, Card, Descriptions, Space, Tag, Typography } from "antd";
import { useAuthStore } from "../auth/authStore";

export const ProfilePage = () => {
  const { user, roles } = useAuthStore((state) => state);
  if (!user) throw new Error("User not found");
  return (
    <Card>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          User Info
        </Typography.Title>
      </Space>
      <Descriptions column={1} bordered styles={{ label: { width: 120 } }}>
        <Descriptions.Item label="Name">
          {user.displayName || "-"}
        </Descriptions.Item>

        <Descriptions.Item label="Email">{user.email || "-"}</Descriptions.Item>

        <Descriptions.Item label="Photo">
          {user.photoURL ? (
            <Avatar src={user.photoURL} />
          ) : (
            <Avatar>{user.displayName?.[0] || "U"}</Avatar>
          )}
        </Descriptions.Item>

        <Descriptions.Item label="Roles">
          {roles && roles.length > 0 ? (
            <Space wrap>
              {roles.map((role) => (
                <Tag color="blue" key={role}>
                  {role}
                </Tag>
              ))}
            </Space>
          ) : (
            "No roles assigned"
          )}
        </Descriptions.Item>
        <Descriptions.Item label="UID">{user.uid}</Descriptions.Item>
        <Descriptions.Item label="Token">
          {user.token ? (
            <span style={{ wordBreak: "break-all" }}>{user.token}</span>
          ) : (
            "-"
          )}
        </Descriptions.Item>
      </Descriptions>
    </Card>
  );
};
