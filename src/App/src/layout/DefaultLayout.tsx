import { App, Layout } from "antd";
import { Content, Header } from "antd/es/layout/layout";
import { Outlet } from "react-router";
import { AppFooter } from "./AppFooter";
import { AppLogo } from "./AppLogo";
import { AppMenu } from "./AppMenu";

export const DefaultLayout = () => {
  return (
    <App>
      <Layout className="layout">
        <Header style={{ display: "flex", alignItems: "center" }}>
          <AppLogo />
          <AppMenu />
        </Header>
        <Content
          style={{ padding: "0 50px", minHeight: "calc(100vh - 64px - 70px)" }}
        >
          <Outlet />
        </Content>
      </Layout>
      <AppFooter />
    </App>
  );
};
