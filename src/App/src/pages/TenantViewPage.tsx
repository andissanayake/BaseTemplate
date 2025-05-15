import { Card } from "antd";
import { TenantView } from "../features/Tenant/TenantView"; // Ensure named import if TenantView is not default export

export const TenantViewPage = () => {
  return (
    <Card>
      <TenantView />
    </Card>
  );
};
