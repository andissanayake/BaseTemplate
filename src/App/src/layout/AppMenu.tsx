/* eslint-disable @typescript-eslint/no-explicit-any */
import { Avatar, Menu } from "antd";
import { ItemType, MenuItemType } from "antd/es/menu/interface";
import { useCallback, useEffect, useState } from "react";
import { onAuthStateChangedListener } from "../auth/firebase";
import { useLocation, useNavigate } from "react-router";
import { useAuthStore } from "../auth/authStore";
import { apiClient } from "../common/apiClient";
import { useTenantStore } from "../features/Tenant/tenantStore";

export const AppMenu = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const {
    user,
    setUser,
    setRoles,
    tenant,
    setTenant,
    setStaffInvitation,
    staffInvitation,
  } = useAuthStore((state) => state);
  const { currentTenant } = useTenantStore((state) => state);
  const [current, setCurrent] = useState(
    location.pathname === "/" || location.pathname === ""
      ? "/"
      : location.pathname
  );

  const [items, setItems] = useState<ItemType<MenuItemType>[]>([]);

  const handleClick = useCallback(
    (key: string) => {
      navigate(key);
    },
    [navigate]
  );

  useEffect(() => {
    const unsubscribe = onAuthStateChangedListener(async (user) => {
      if (user) {
        setUser(user);
        apiClient.post<any>("/api/user/user-details", undefined, {
          onSuccess: (data) => {
            setRoles(data?.roles ?? []);
            setTenant(data?.tenant ?? null);
            setStaffInvitation(data?.staffInvitation ?? null);
          },
          onServerError: () => {
            // Silently handle server error for user details
          },
        });
      } else {
        setUser(null);
        setRoles([]);
      }
    }, false);

    return () => {
      unsubscribe();
    };
  }, [
    setUser,
    setTenant,
    setStaffInvitation,
    setRoles,
    currentTenant?.id,
    currentTenant?.name,
  ]);

  useEffect(() => {
    if (location) {
      if (current !== location.pathname) {
        setCurrent(location.pathname);
      }
    }
  }, [current, location]);

  useEffect(() => {
    const menuItems = [];

    // Left side menu items
    menuItems.push({
      key: "/",
      label: "Home",
      onClick: (e: any) => {
        handleClick(e.key);
      },
    });

    if (user) {
      if (tenant?.id) {
        menuItems.push({
          key: "/tenants/view",
          label: <span>{tenant.name}</span>,
          onClick: (e: any) => {
            handleClick(e.key);
          },
        });
        menuItems.push({
          key: "/tenants/view/staff-invitations",
          label: <span>Staff Invitation</span>,
          onClick: (e: any) => {
            handleClick(e.key);
          },
        });
        menuItems.push({
          key: "/tenants/view/staff",
          label: <span>Staff</span>,
          onClick: (e: any) => {
            handleClick(e.key);
          },
        });
      }

      menuItems.push({
        key: "/items",
        label: "Item",
        onClick: (e: any) => {
          handleClick(e.key);
        },
      });
      menuItems.push({
        key: "/item-attribute-types",
        label: "Attribute Type",
        onClick: (e: any) => {
          handleClick(e.key);
        },
      });
      menuItems.push({
        key: "/specifications",
        label: "Specification",
        onClick: (e: any) => {
          handleClick(e.key);
        },
      });

      if (!tenant?.id && !staffInvitation?.id) {
        menuItems.push({
          key: "/tenants/create",
          label: "Become a Tenant",
          onClick: (e: any) => {
            handleClick(e.key);
          },
        });
      }
      if (!tenant?.id && staffInvitation?.id) {
        menuItems.push({
          key: "/staff-invitations/respond",
          label: "Join a Tenant",
          onClick: (e: any) => {
            handleClick(e.key);
          },
        });
      }
      // Right side menu items with auto margin
      menuItems.push({
        key: "/profile",
        label: (
          <span>
            <Avatar src={user.photoURL} style={{ marginRight: 8 }} />
            {user.displayName}
          </span>
        ),
        onClick: (e: any) => {
          handleClick(e.key);
        },
        style: { marginLeft: "auto" },
      });

      menuItems.push({
        key: "/logout",
        label: "Logout",
        onClick: (e: any) => {
          handleClick(e.key);
        },
      });
    } else {
      menuItems.push({
        key: "/login",
        label: "Login",
        onClick: (e: any) => {
          handleClick(e.key);
        },
        style: { marginLeft: "auto" },
      });
    }

    setItems(menuItems);
  }, [
    user,
    navigate,
    current,
    location.pathname,
    handleClick,
    tenant?.id,
    tenant?.name,
    staffInvitation?.id,
  ]);

  return (
    <Menu
      theme="dark"
      mode="horizontal"
      defaultSelectedKeys={["/"]}
      selectedKeys={[current]}
      items={items}
      style={{
        flex: 1,
        minWidth: 0,
        display: "flex",
        alignItems: "center",
      }}
    />
  );
};
