import React, { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { authPolicy } from "../auth/authPolicy";
import { Policies } from "../auth/PoliciesEnum";
import { useAuthStore } from "../auth/authStore";

interface ProtectedRouteProps {
  policy: Policies;
  children: ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  policy,
  children,
}) => {
  const user = useAuthStore((state) => state.user);
  return authPolicy(policy, user) ? <>{children}</> : <Navigate to="/" />;
};

export default ProtectedRoute;
