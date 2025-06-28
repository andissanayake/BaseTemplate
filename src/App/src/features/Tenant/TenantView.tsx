import React, { useCallback } from "react";
import { Link, useParams } from "react-router-dom";
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
import { TenantService } from "./tenantService";
import { handleResult } from "../../common/handleResult";
import { useAsyncEffect } from "../../common/useAsyncEffect";

export const TenantView: React.FC = () => {
  const { currentTenant, setCurrentTenant, loading, setLoading } =
    useTenantStore();
  const { tenantId } = useParams<{ tenantId: string }>();

  useAsyncEffect(async () => {
    if (!tenantId) {
      notification.error({ message: "Tenant ID is required to view details." });
      // Consider navigating away or showing a specific error component
      return;
    }
    setLoading(true);
    const response = await TenantService.fetchTenantById(tenantId);
    handleResult(response, {
      onSuccess: (data) => {
        if (data) {
          setCurrentTenant(data);
        } else {
          setCurrentTenant(null);
          notification.error({ message: "Tenant not found." });
        }
      },
      onServerError: () => {
        setCurrentTenant(null);
        notification.error({ message: "Failed to fetch tenant details." });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [tenantId, setCurrentTenant, setLoading]);

  const tenantViewContent = useCallback(() => {
    if (loading) {
      return <Spin style={{ display: "block", marginTop: "20px" }} />;
    } else if (!currentTenant) {
      return <Typography.Text>Tenant details are unavailable.</Typography.Text>;
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

        {currentTenant && (
          <Link to={`/tenants/edit/${currentTenant.id}`}>
            <Button type="primary" shape="circle" icon={<EditOutlined />} />
          </Link>
        )}
      </Space>
      {tenantViewContent()}
    </>
  );
};
