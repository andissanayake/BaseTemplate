import { useAuthStore } from "../auth/authStore";
import { Secure } from "../auth/Secure";

export const ProfilePage = () => {
  const user = useAuthStore((state) => state.user);

  return (
    <div>
      {user?.displayName}
      <Secure />
    </div>
  );
};
