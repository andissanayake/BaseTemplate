import { useAuthStore } from "../auth/authStore";

export const ProfilePage = () => {
  const user = useAuthStore((state) => state.user);

  return <div>{user?.displayName}</div>;
};
