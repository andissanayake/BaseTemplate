import { authPolicy } from "../auth/authPolicy";
import { useAuthStore } from "../auth/authStore";
import { Policies } from "../auth/PoliciesEnum";
import { TodoGroupDashboard } from "../features/TodoGroup/TodoGroupDashboard";

export const HomePage = () => {
  const user = useAuthStore((state) => state.user);
  return (
    <div>
      <h1>Home Page</h1>

      {authPolicy(Policies.User, user) ? <TodoGroupDashboard /> : <></>}
    </div>
  );
};
