import { useEffect } from "react";
import "./App.css";
import {
  handleLogin,
  handleLogout,
  onAuthStateChangedListener,
} from "./auth/firebase";
import { AppDispatch, RootState } from "./store";
import { useDispatch, useSelector } from "react-redux";
import { setUser } from "./auth/authSlice";
import { authPolicy } from "./auth/authPolicy";
import { Policies } from "./auth/PoliciesEnum";

export const App = () => {
  const dispatch = useDispatch<AppDispatch>();
  const user = useSelector((state: RootState) => state.auth.user);

  useEffect(() => {
    const unsubscribe = onAuthStateChangedListener((user) => {
      if (user) {
        dispatch(setUser(user));
      } else {
        dispatch(setUser(null));
      }
    }, false);

    return () => {
      unsubscribe();
    };
  }, [dispatch]);

  return (
    <>
      {authPolicy(Policies.User, user) ? (
        <>
          <h1>Welcome {user?.displayName}</h1>
          <a
            onClick={() => {
              handleLogout();
            }}
          >
            Login
          </a>
        </>
      ) : (
        <>
          <h1>Welcome Guest</h1>
          <a
            onClick={() => {
              handleLogin();
            }}
          >
            Login
          </a>
        </>
      )}
    </>
  );
};

export default App;
