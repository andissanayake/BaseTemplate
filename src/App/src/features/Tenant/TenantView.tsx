import React from "react";
import { useParams } from "react-router-dom";
import { Descriptions, notification, Space, Typography, Spin } from "antd";
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

  if (loading) {
    return (
      <Spin
        tip="Loading tenant details..."
        style={{ display: "block", marginTop: "20px" }}
      />
    );
  }

  if (!currentTenant) {
    // This message can be shown if loading is false and tenant is still null
    // (e.g., after an error or if tenantId was invalid)
    return <Typography.Text>Tenant details are unavailable.</Typography.Text>;
  }

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Tenant View
        </Typography.Title>
      </Space>
      <Descriptions column={1} bordered>
        <Descriptions.Item label="ID">{currentTenant.id}</Descriptions.Item>
        <Descriptions.Item label="Name">
          {currentTenant.name || "N/A"}
        </Descriptions.Item>
        <Descriptions.Item label="Address">
          {currentTenant.address || "N/A"}
        </Descriptions.Item>
        {/* Add other tenant properties here as needed */}
      </Descriptions>
    </>
  );
};

// Note: Exporting as default if it's the main export, or named if preferred.
// For consistency with other feature components, using named export.
// export default TenantView;
