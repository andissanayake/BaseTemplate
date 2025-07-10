import React from "react";
import { StaffRequestList } from "../features/StaffRequest/StaffRequestList";
import { Card } from "antd";

export const StaffRequestManagementPage: React.FC = () => {
  return (
    <Card>
      <StaffRequestList />
    </Card>
  );
};
