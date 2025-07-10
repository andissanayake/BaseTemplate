/* eslint-disable @typescript-eslint/no-explicit-any */
import { Avatar, Menu } from "antd";
import { ItemType, MenuItemType } from "antd/es/menu/interface";
import { useCallback, useEffect, useState } from "react";
import {
  onAuthStateChangedListener,
  handleLogout,
  handleLogin,
} from "../auth/firebase";
import { useLocation, useNavigate } from "react-router";
import { useAuthStore } from "../auth/authStore";
import { userService } from "../auth/userService";
import { handleResult } from "../common/handleResult";
import { useTenantStore } from "../features/Tenant/tenantStore";

export const AppMenu = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const {
    user,
    setUser,
    setRoles,
    setTenantId,
    setTenantName,
    tenantId,
    tenantName,
    roles,
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
        const res = await userService.details();
        handleResult(res, {
          onSuccess: (data) => {
            setRoles(data?.roles ?? []);
            setTenantId(data?.tenantId ?? null);
            setTenantName(data?.tenantName ?? null);
          },
          onServerError: () => {
            console.error("Failed to fetch roles!");
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
    setTenantId,
    setTenantName,
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
      menuItems.push({
        key: "/todo-list",
        label: <span>Todo List</span>,
        onClick: (e: any) => {
          handleClick(e.key);
        },
      });

      menuItems.push({
        key: "/items",
        label: <span>Items</span>,
        onClick: (e: any) => {
          handleClick(e.key);
        },
      });

      if (tenantId) {
        menuItems.push({
          key: "/tenants/view/" + tenantId,
          label: <span>{tenantName}</span>,
          onClick: (e: any) => {
            handleClick(e.key);
          },
        });

        // Add Staff Requests menu item for tenant owners
        if (roles.includes("TenantOwner")) {
          menuItems.push({
            key: "/tenants/view/" + tenantId + "/staff-requests",
            label: <span>Staff Requests</span>,
            onClick: (e: any) => {
              handleClick(e.key);
            },
          });
        }
      }

      if (!tenantId) {
        menuItems.push({
          key: "/tenants/create",
          label: "Become a Tenant",
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
        onClick: () => {
          handleLogout();
          navigate("/");
        },
      });
    } else {
      menuItems.push({
        key: "/login",
        label: "Login",
        onClick: () => {
          handleLogin();
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
    tenantId,
    tenantName,
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
