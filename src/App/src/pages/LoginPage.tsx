import { Button, Card, Typography, Space, Spin } from "antd";
import { GoogleOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router";
import { handleLogin } from "../auth/firebase";
import { useEffect, useState } from "react";
import { useAuthStore } from "../auth/authStore";

const { Title, Text } = Typography;

export const LoginPage = () => {
  const navigate = useNavigate();
  const { user } = useAuthStore((state) => state);
  const [isLoggingIn, setIsLoggingIn] = useState(false);

  useEffect(() => {
    // If user is already logged in, redirect to home
    if (user) {
      navigate("/");
      return;
    }

    // Automatically trigger login when page loads
    const autoLogin = async () => {
      setIsLoggingIn(true);
      try {
        await handleLogin();
        // Navigation will be handled by the useEffect above when user state changes
      } catch (error) {
        console.error("Auto-login failed:", error);
        setIsLoggingIn(false);
      }
    };

    autoLogin();
  }, [user, navigate]);

  const handleLoginClick = async () => {
    try {
      await handleLogin();
      // Navigation will be handled by the useEffect above when user state changes
    } catch (error) {
      console.error("Login failed:", error);
    }
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
            <Title level={2}>Welcome Back</Title>
            {isLoggingIn ? (
              <div>
                <Spin size="large" />
                <br />
                <Text type="secondary">Signing you in...</Text>
              </div>
            ) : (
              <Text type="secondary">Sign in to your account to continue</Text>
            )}
          </div>

          {!isLoggingIn && (
            <Button
              type="primary"
              size="large"
              icon={<GoogleOutlined />}
              onClick={handleLoginClick}
              style={{ width: "100%" }}
            >
              Sign in with Google
            </Button>
          )}

          <Text type="secondary" style={{ fontSize: "12px" }}>
            By signing in, you agree to our terms of service and privacy policy
          </Text>
        </Space>
      </Card>
    </div>
  );
};
