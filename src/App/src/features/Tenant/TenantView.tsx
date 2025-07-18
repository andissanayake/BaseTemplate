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
import { TenantService } from "./tenantService";
import { handleResult } from "../../common/handleResult";
import { handleServerError } from "../../common/serverErrorHandler";
import { useAsyncEffect } from "../../common/useAsyncEffect";

export const TenantView: React.FC = () => {
  const { currentTenant, setCurrentTenant, loading, setLoading } =
    useTenantStore();

  useAsyncEffect(async () => {
    setLoading(true);
    const response = await TenantService.fetchTenant();
    handleResult(response, {
      onSuccess: (data) => {
        if (data) {
          setCurrentTenant(data);
        } else {
          setCurrentTenant(null);
          notification.error({ message: "Tenant not found." });
        }
      },
      onServerError: (errors) => {
        setCurrentTenant(null);
        handleServerError(errors, "Failed to fetch tenant details.");
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [setCurrentTenant, setLoading]);

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
          <Link to={`/tenants/view/${currentTenant.id}/edit`}>
            <Button type="primary" shape="circle" icon={<EditOutlined />} />
          </Link>
        )}
      </Space>
      {tenantViewContent()}
    </>
  );
};
