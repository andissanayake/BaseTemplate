/* eslint-disable @typescript-eslint/no-explicit-any */
import { Breadcrumb } from "antd";
import { Link, useLocation, matchPath } from "react-router-dom";

export const AppBreadcrumb = () => {
  const breadcrumbRoutes: Record<string, string | ((params: any) => string)> = {
    "/": "Home",
    "/todo-list": "Todo List",
    "/todo-list/create": "Create",
    "/todo-list/edit/:listId": (params: any) => `Edit #${params.listId}`,
    "/todo-list/view/:listId": (params: any) => `View #${params.listId}`,
    "/profile": "Profile",
    "/tenants/create": "Create Tenant",
    "/tenants/view/:tenantId": (params: any) =>
      ` Tenant View #${params.tenantId}`,
    "/tenants/view/:tenantId/edit": (params: any) =>
      ` Tenant Edit #${params.tenantId}`,

    "/tenants/view/:tenantId/items/create": "Create Item",
    "/tenants/view/:tenantId/items/edit/:itemId": (params: any) =>
      `Edit Item #${params.itemId}`,
    "/tenants/view/:tenantId/items/view/:itemId": (params: any) =>
      `View Item #${params.itemId}`,
  };

  const location = useLocation();
  const pathSnippets = location.pathname.split("/").filter((i) => i);

  // Always start with root "/"
  const paths = ["/"];
  for (let i = 0; i < pathSnippets.length; i++) {
    const next = `/${pathSnippets.slice(0, i + 1).join("/")}`;
    paths.push(next);
  }

  const items = paths
    .map((path) => {
      // Match the most specific route
      const matchKey = Object.keys(breadcrumbRoutes)
        .sort((a, b) => b.length - a.length) // longest first
        .find((key) => matchPath({ path: key, end: true }, path));

      if (!matchKey) return null;

      const labelGetter = breadcrumbRoutes[matchKey];
      const label =
        typeof labelGetter === "function"
          ? labelGetter(
              matchPath({ path: matchKey!, end: true }, path)?.params ?? {}
            )
          : labelGetter;

      return {
        key: path,
        title: <Link to={path}>{label}</Link>,
      };
    })
    .filter((item) => item !== null); // Ensure no `null` values are included

  return (
    <Breadcrumb
      style={{ margin: "16px 0" }}
      items={items.length > 0 ? items : [{ title: "Not Found" }]}
    />
  );
};
