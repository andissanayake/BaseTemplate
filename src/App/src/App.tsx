import "./App.css";
import { Policies } from "./auth/PoliciesEnum";
import { Routes, Route } from "react-router";
import { DefaultLayout } from "./layout/DefaultLayout";
import ProtectedRoute from "./layout/ProtectedRoute";
import { HomePage } from "./pages/HomePage";
import { NotFoundPage } from "./pages/NotFoundPage";
import { ProfilePage } from "./pages/ProfilePage";
import { TodoGroupListPage } from "./pages/TodoGroupListPage";
import { TodoGroupCreatePage } from "./pages/TodoGroupCreatePage";
import { TodoGroupEditPage } from "./pages/TodoGroupEditPage";
import { TodoGroupViewPage } from "./pages/TodoGroupViewPage";

export const App = () => {
  return (
    <>
      <Routes>
        <Route path="/" element={<DefaultLayout />}>
          <Route index element={<HomePage />} />
          <Route path="/todo-list" element={<TodoGroupListPage />} />

          <Route path="/todo-list/create" element={<TodoGroupCreatePage />} />
          <Route path="todo-list/edit/:id" element={<TodoGroupEditPage />} />
          <Route path="todo-list/view/:id" element={<TodoGroupViewPage />} />

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
