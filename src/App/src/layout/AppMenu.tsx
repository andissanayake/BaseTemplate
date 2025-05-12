/* eslint-disable @typescript-eslint/no-explicit-any */
import { Avatar, Menu } from "antd";
import { ItemType, MenuItemType } from "antd/es/menu/interface";
import { useCallback, useEffect, useState } from "react";
import {
  onAuthStateChangedListener,
  handleLogout,
  handleLogin,
} from "../auth/firebase";
//import { authPolicy } from "../auth/authPolicy";
//import { Policies } from "../auth/PoliciesEnum";
import { useLocation, useNavigate } from "react-router";
import { useAuthStore } from "../auth/authStore";
import { UserService } from "../auth/userService";
import { handleResult } from "../common/handleResult";

export const AppMenu = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, setUser, setRoles } = useAuthStore((state) => state);

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
        const res = await UserService.fetchRoles();
        handleResult(res, {
          onSuccess: (data) => {
            setRoles(data ?? []);
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
  }, [setUser]);

  useEffect(() => {
    if (location) {
      if (current !== location.pathname) {
        setCurrent(location.pathname);
      }
    }
  }, [current, location]);

  useEffect(() => {
    const menuItems = [];
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
      });
    }
    setItems(menuItems);
  }, [user, navigate, current, location.pathname, handleClick]);

  return (
    <Menu
      theme="dark"
      mode="horizontal"
      defaultSelectedKeys={["/"]}
      selectedKeys={[current]}
      items={items}
      style={{ flex: 1, minWidth: 0 }}
    />
  );
};
