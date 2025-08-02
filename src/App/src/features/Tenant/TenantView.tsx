import React, { useCallback } from "react";
import { Link } from "react-router-dom";
import {
  Descriptions,
  notification,
  Space,
  Typography,
  Spin,
  Button,
} from "antd";
import { EditOutlined } from "@ant-design/icons";
import { useTenantStore } from "./tenantStore";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { Tenant } from "./TenantModel";

export const TenantView: React.FC = () => {
  const {
    currentTenant,
    setCurrentTenant,
    loading,
    setLoading,
    cleanCurrentTenant,
  } = useTenantStore();

  useAsyncEffect(async () => {
    setLoading(true);
    apiClient.get<Tenant>("/api/tenant", {
      onSuccess: (data) => {
        setCurrentTenant(data);
      },
      onServerError: () => {
        cleanCurrentTenant();
        notification.error({ message: "Failed to fetch tenant details." });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [setCurrentTenant, setLoading]);

  const tenantViewContent = useCallback(() => {
    if (loading) {
      return <Spin style={{ display: "block", marginTop: "20px" }} />;
    } else {
      return (
        <Descriptions column={1} bordered>
          <Descriptions.Item label="ID">{currentTenant.id}</Descriptions.Item>
          <Descriptions.Item label="Name">
            {currentTenant.name || "N/A"}
          </Descriptions.Item>
          <Descriptions.Item label="Address">
            {currentTenant.address || "N/A"}
          </Descriptions.Item>
        </Descriptions>
      );
    }
  }, [loading, currentTenant]);

  return (
    <>
      <Space
        className="mb-4"
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          Tenant View
        </Typography.Title>

        <Link to={`/tenants/edit`}>
          <Button type="primary" shape="circle" icon={<EditOutlined />} />
        </Link>
      </Space>
      {tenantViewContent()}
    </>
  );
};
