import { Card } from "antd";
import { TenantView } from "../features/Tenant/TenantView"; // Ensure named import if TenantView is not default export
import ItemList from "../features/Item/ItemList";

export const TenantViewPage = () => {
  return (
    <Card>
      <TenantView />
      <div className="mt-4">
        <ItemList />
      </div>
    </Card>
  );
};
