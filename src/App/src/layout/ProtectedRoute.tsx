import React, { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuthPolicy } from "../auth/authPolicy";
import { Policies } from "../auth/PoliciesEnum";
import { Roles } from "../auth/RolesEnum";

interface ProtectedRouteProps {
  policy: Policies;
  roles?: Roles[];
  children: ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  policy,
  roles,
  children,
}) => {
  const { isUser, isGuest, hasRole } = useAuthPolicy();

  const hasAccess = () => {
    switch (policy) {
      case Policies.User:
        return isUser();
      case Policies.Guest:
        return isGuest();
      case Policies.Role:
        return roles ? hasRole(roles) : false;
      default:
        return false;
    }
  };

  return hasAccess() ? <>{children}</> : <Navigate to="/no-access" />;
};

export { ProtectedRoute };
