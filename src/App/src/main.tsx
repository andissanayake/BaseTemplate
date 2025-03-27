import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import { initializeFirebase } from "./auth/firebase.ts";
import { setupAxios } from "./auth/axiosInstance.ts";

const initializeAppWithAxios = async () => {
  try {
    await initializeFirebase();
    setupAxios();
    console.log("App initialized with Axios and Firebase");
  } catch (error) {
    console.error("Error during initialization:", error);
  }
};

initializeAppWithAxios();

createRoot(document.getElementById("root")!).render(<App />);
