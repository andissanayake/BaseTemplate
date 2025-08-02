import { useAuthStore } from "./authStore";
import { Roles } from "./RolesEnum";

export const useAuthPolicy = () => {
  const { user, roles, tenant, staffInvitation } = useAuthStore();

  // Convenience methods
  const isUser = () => !!user;
  const isGuest = () => !user;
  const hasTenant = () => tenant && tenant?.id > 0;
  const hasInvitation = () => staffInvitation && staffInvitation?.id > 0;
  const hasRole = (rolesToCheck: Roles[]) => {
    if (!user || !rolesToCheck || rolesToCheck.length === 0) return false;
    return roles.some((role) => rolesToCheck.includes(role));
  };

  return {
    isUser,
    isGuest,
    hasTenant,
    hasInvitation,
    hasRole,
    user,
    roles,
  };
};
