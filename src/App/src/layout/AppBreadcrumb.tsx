/* eslint-disable @typescript-eslint/no-explicit-any */
import { Breadcrumb } from "antd";
import { Link, useLocation, matchPath } from "react-router-dom";

export const AppBreadcrumb = () => {
  const breadcrumbRoutes: Record<string, string | ((params: any) => string)> = {
    //application
    "/": "Home",
    "/tenants/create": "Create Tenant",
    "/tenants/view": "Tenant View",
    "/tenants/edit": "Tenant Edit",
    "/tenants/view/staff-requests": "Staff Requests",

    "/items": "Items",
    "/items/create": "Create Item",
    "/items/edit/:itemId": (params: any) => `Edit Item #${params.itemId}`,
    "/items/view/:itemId": (params: any) => `View Item #${params.itemId}`,
    "/item-attribute-types": "Attribute Types",
    "/item-attribute-types/create": "Create Attribute Type",
    "/item-attribute-types/edit/:itemAttributeTypeId": (params: any) =>
      `Edit Attribute Type #${params.itemAttributeTypeId}`,
    "/item-attribute-types/view/:itemAttributeTypeId": (params: any) =>
      `View Attribute Type #${params.itemAttributeTypeId}`,
    "/profile": "Profile",

    "/tenants/view/:tenantId/staff": "Staff Management",
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
    .filter(
      (item): item is { key: string; title: JSX.Element } => item !== null
    ); // Ensure no `null` values are included

  return (
    <Breadcrumb
      style={{ margin: "16px 0" }}
      items={items.length > 0 ? items : [{ title: "Not Found" }]}
    />
  );
};
