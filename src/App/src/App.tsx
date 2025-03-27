import { useEffect } from "react";
import "./App.css";
import {
  handleLogin,
  handleLogout,
  onAuthStateChangedListener,
} from "./auth/firebase";
import { authPolicy } from "./auth/authPolicy";
import { Policies } from "./auth/PoliciesEnum";
import { useAuthStore } from "./auth/authStore";
import { Secure } from "./auth/Secure";

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

  return (
    <>
      {authPolicy(Policies.User, user) ? (
        <>
          <h1>Welcome {user?.displayName}</h1>
          <a onClick={handleLogout}>Logout</a>
          <Secure />
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
