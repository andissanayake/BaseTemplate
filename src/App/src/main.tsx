import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import { Provider } from "react-redux";
import { store } from "./store.ts";
import { initializeFirebase } from "./auth/firebase.ts";
import { setupAxios } from "./auth/axiosInstance.ts";

const initializeAppWithAxios = async () => {
  try {
    await initializeFirebase();
    setupAxios();
  } catch (error) {
    console.error("Error during initialization:", error);
  }
};

initializeAppWithAxios();

createRoot(document.getElementById("root")!).render(
  <Provider store={store}>
    <App />
  </Provider>
);
