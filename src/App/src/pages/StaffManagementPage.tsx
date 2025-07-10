import React from "react";
import { Card } from "antd";
import StaffList from "../features/Staff/StaffList";

export const StaffManagementPage: React.FC = () => {
  return (
    <Card>
      <StaffList />
    </Card>
  );
};
