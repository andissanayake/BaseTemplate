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
import axiosInstance from "./auth/axiosInstance";

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

  const fetchData = async () => {
    try {
      const response = await axiosInstance.get(
        "http://localhost:6001/api/test/secure"
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
  }, [user]);
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
            Logout
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
