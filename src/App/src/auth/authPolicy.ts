import { useAuthStore } from "./authStore";
import { Roles } from "./RolesEnum";

export const useAuthPolicy = () => {
  const { user, roles } = useAuthStore();

  // Convenience methods
  const isUser = () => !!user;
  const isGuest = () => !user;
  const hasRole = (rolesToCheck: Roles[]) => {
    if (!user || !rolesToCheck || rolesToCheck.length === 0) return false;
    return roles.some((role) => rolesToCheck.includes(role));
  };

  return {
    isUser,
    isGuest,
    hasRole,
    user,
    roles,
  };
};
