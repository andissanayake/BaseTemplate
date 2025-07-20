import { Card, Typography, Space, Button, Spin } from "antd";
import { LogoutOutlined, HomeOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router";
import { handleLogout } from "../auth/firebase";
import { useEffect, useState } from "react";
import { useAuthStore } from "../auth/authStore";

const { Title, Text } = Typography;

export const LogoutPage = () => {
  const navigate = useNavigate();
  const { user } = useAuthStore((state) => state);
  const [isLoggingOut, setIsLoggingOut] = useState(false);

  useEffect(() => {
    // If user is not logged in, redirect to home
    if (!user) {
      navigate("/");
      return;
    }

    // Automatically logout when the page loads
    const performLogout = async () => {
      setIsLoggingOut(true);
      try {
        await handleLogout();
        // Small delay to show the logout message
        setTimeout(() => {
          navigate("/");
        }, 1500);
      } catch (error) {
        console.error("Logout failed:", error);
        setIsLoggingOut(false);
      }
    };

    performLogout();
  }, [user, navigate]);

  const handleManualLogout = async () => {
    setIsLoggingOut(true);
    try {
      await handleLogout();
      navigate("/");
    } catch (error) {
      console.error("Logout failed:", error);
      setIsLoggingOut(false);
    }
  };

  const handleGoHome = () => {
    navigate("/");
  };

  return (
    <div
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        minHeight: "100vh",
        background: "#f0f2f5",
      }}
    >
      <Card style={{ width: 400, textAlign: "center" }}>
        <Space direction="vertical" size="large" style={{ width: "100%" }}>
          <div>
            <Title level={2}>Logging Out</Title>
            {isLoggingOut ? (
              <div>
                <Spin size="large" />
                <br />
                <Text type="secondary">Signing you out...</Text>
              </div>
            ) : (
              <Text type="secondary">
                You have been successfully logged out
              </Text>
            )}
          </div>

          {!isLoggingOut && (
            <Space>
              <Button
                type="primary"
                icon={<LogoutOutlined />}
                onClick={handleManualLogout}
              >
                Logout Again
              </Button>
              <Button icon={<HomeOutlined />} onClick={handleGoHome}>
                Go Home
              </Button>
            </Space>
          )}
        </Space>
      </Card>
    </div>
  );
};
