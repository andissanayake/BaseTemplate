import "./App.css";
import { Policies } from "./auth/PoliciesEnum";
import { Routes, Route } from "react-router";
import { DefaultLayout } from "./layout/DefaultLayout";
import ProtectedRoute from "./layout/ProtectedRoute";
import { HomePage } from "./pages/HomePage";
import { NotFoundPage } from "./pages/NotFoundPage";
import { ProfilePage } from "./pages/ProfilePage";
import { TodoGroupPage } from "./pages/TodoGroupPage";

export const App = () => {
  return (
    <>
      <Routes>
        <Route path="/" element={<DefaultLayout />}>
          <Route index element={<HomePage />} />
          <Route path="/todo-list/*" element={<TodoGroupPage />} />
          <Route
            path="/profile"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ProfilePage />
              </ProtectedRoute>
            }
          />
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </>
  );
};

export default App;
