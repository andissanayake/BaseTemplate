import "./App.css";
import { Policies } from "./auth/PoliciesEnum";
import { Routes, Route } from "react-router";
import { DefaultLayout } from "./layout/DefaultLayout";
import ProtectedRoute from "./layout/ProtectedRoute";
import { HomePage } from "./pages/HomePage";
import { NotFoundPage } from "./pages/NotFoundPage";
import { ProfilePage } from "./pages/ProfilePage";
import { TenantCreatePage } from "./pages/TenantCreatePage";
import { TenantEditPage } from "./pages/TenantEditPage";
import { TenantViewPage } from "./pages/TenantViewPage";
import { StaffInvitationManagementPage } from "./pages/StaffInvitationManagementPage";
import { StaffInvitationResponsePage } from "./pages/StaffInvitationResponsePage";
import { StaffManagementPage } from "./pages/StaffManagementPage";
import { ItemCreatePage } from "./pages/ItemCreatePage";
import { ItemEditPage } from "./pages/ItemEditPage";
import { ItemViewPage } from "./pages/ItemViewPage";
import { ItemListPage } from "./pages/ItemListPage";
import ItemAttributeTypeCreatePage from "./pages/ItemAttributeTypeCreatePage";
import ItemAttributeTypeEditPage from "./pages/ItemAttributeTypeEditPage";
import ItemAttributeTypeViewPage from "./pages/ItemAttributeTypeViewPage";
import ItemAttributeTypeListPage from "./pages/ItemAttributeTypeListPage";
import SpecificationListPage from "./pages/SpecificationListPage";
import SpecificationCreatePage from "./pages/SpecificationCreatePage";
import SpecificationEditPage from "./pages/SpecificationEditPage";
import SpecificationViewPage from "./pages/SpecificationViewPage";
import { LoginPage } from "./pages/LoginPage";
import { LogoutPage } from "./pages/LogoutPage";
import NoAccessPage from "./pages/NoAccessPage";

export const App = () => {
  return (
    <>
      <Routes>
        <Route path="/" element={<DefaultLayout />}>
          <Route index element={<HomePage />} />

          <Route path="/login" element={<LoginPage />} />
          <Route path="/logout" element={<LogoutPage />} />

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
            path="/item-attribute-types"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemAttributeTypeListPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/item-attribute-types/create"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemAttributeTypeCreatePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/item-attribute-types/edit/:itemAttributeTypeId"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemAttributeTypeEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/item-attribute-types/view/:itemAttributeTypeId"
            element={
              <ProtectedRoute policy={Policies.User}>
                <ItemAttributeTypeViewPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/specifications"
            element={
              <ProtectedRoute policy={Policies.User}>
                <SpecificationListPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/specifications/create"
            element={
              <ProtectedRoute policy={Policies.User}>
                <SpecificationCreatePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/specifications/edit/:specificationId"
            element={
              <ProtectedRoute policy={Policies.User}>
                <SpecificationEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/specifications/view/:specificationId"
            element={
              <ProtectedRoute policy={Policies.User}>
                <SpecificationViewPage />
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
            path="/tenants/edit"
            element={
              <ProtectedRoute policy={Policies.User}>
                <TenantEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tenants/view"
            element={
              <ProtectedRoute policy={Policies.User}>
                <TenantViewPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tenants/view/staff-invitations"
            element={
              <ProtectedRoute policy={Policies.User}>
                <StaffInvitationManagementPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/staff-invitations/respond"
            element={
              <ProtectedRoute policy={Policies.User}>
                <StaffInvitationResponsePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tenants/view/staff"
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
          <Route path="/no-access" element={<NoAccessPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </>
  );
};

export default App;
