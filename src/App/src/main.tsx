import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import { initializeFirebase } from "./auth/firebase.ts";
import { setupAxios } from "./auth/axiosInstance.ts";
import { BrowserRouter } from "react-router";
import "normalize.css";
import { StrictMode } from "react";
const renderApp = () => {
  const root = createRoot(document.getElementById("root")!);
  root.render(
    <StrictMode>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </StrictMode>
  );
};

const initializeAppWithAxios = async () => {
  try {
    await initializeFirebase();
    setupAxios();
  } catch (error) {
    console.error("Error during initialization:", error);
  } finally {
    renderApp();
  }
};

initializeAppWithAxios();
