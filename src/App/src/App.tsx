import { useEffect } from "react";
import "./App.css";
import {
  handleLogin,
  handleLogout,
  onAuthStateChangedListener,
} from "./auth/firebase";
import { authPolicy } from "./auth/authPolicy";
import { Policies } from "./auth/PoliciesEnum";
import axiosInstance from "./auth/axiosInstance";
import { useAuthStore } from "./auth/authStore";

export const App = () => {
  const user = useAuthStore((state) => state.user);
  const setUser = useAuthStore((state) => state.setUser);

  useEffect(() => {
    const unsubscribe = onAuthStateChangedListener((user) => {
      if (user) {
        setUser(user);
      } else {
        setUser(null);
      }
    }, false);

    return () => {
      unsubscribe();
    };
  }, [setUser]);

  const fetchData = async () => {
    try {
      const response = await axiosInstance.get(
        "http://localhost:5001/api/test/secure"
      );
      console.log(response.data);
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  };

  useEffect(() => {
    if (user) {
      fetchData();
    }
    console.log(user);
  }, [user]);

  return (
    <>
      {authPolicy(Policies.User, user) ? (
        <>
          <h1>Welcome {user?.displayName}</h1>
          <a onClick={handleLogout}>Logout</a>
        </>
      ) : (
        <>
          <h1>Welcome Guest</h1>
          <a onClick={handleLogin}>Login</a>
        </>
      )}
    </>
  );
};

export default App;
