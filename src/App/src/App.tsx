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
import { TenantCreatePage } from "./pages/TenantCreatePage";
import { TenantEditPage } from "./pages/TenantEditPage";
import { TenantViewPage } from "./pages/TenantViewPage";
import { StaffRequestManagementPage } from "./pages/StaffRequestManagementPage";
import { StaffRequestResponsePage } from "./pages/StaffRequestResponsePage";
import { StaffManagementPage } from "./pages/StaffManagementPage";
import { ItemCreatePage } from "./pages/ItemCreatePage";
import { ItemEditPage } from "./pages/ItemEditPage";
import { ItemViewPage } from "./pages/ItemViewPage";
import { ItemListPage } from "./pages/ItemListPage";

export const App = () => {
  return (
    <>
      <Routes>
        <Route path="/" element={<DefaultLayout />}>
          <Route index element={<HomePage />} />
          <Route path="/todo-list" element={<TodoGroupListPage />} />

          <Route path="/todo-list/create" element={<TodoGroupCreatePage />} />
          <Route
            path="todo-list/edit/:listId"
            element={<TodoGroupEditPage />}
          />
          <Route
            path="todo-list/view/:listId"
            element={<TodoGroupViewPage />}
          />

          <Route
            path="/items"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemListPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/items/create"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemCreatePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/items/edit/:itemId"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/items/view/:itemId"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemViewPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/tenants/create"
            element={
              <ProtectedRoute policy={Policies.User}>
                <TenantCreatePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tenants/view/:tenantId/edit"
            element={
              <ProtectedRoute policy={Policies.User}>
                <TenantEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tenants/view/:tenantId"
            element={
              <ProtectedRoute policy={Policies.User}>
                <TenantViewPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tenants/view/:tenantId/staff-requests"
            element={
              <ProtectedRoute policy={Policies.User}>
                <StaffRequestManagementPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/staff-request/respond"
            element={
              <ProtectedRoute policy={Policies.User}>
                <StaffRequestResponsePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tenants/view/:tenantId/staff"
            element={
              <ProtectedRoute policy={Policies.User}>
                <StaffManagementPage />
              </ProtectedRoute>
            }
          />

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
