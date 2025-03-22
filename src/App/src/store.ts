import { configureStore } from "@reduxjs/toolkit";
import authReducer, { AuthState } from "./auth/authSlice";

const defaultAuthState: AuthState = {
  user: null,
};

const loadAuthState = (): AuthState | undefined => {
  try {
    const serializedState = localStorage.getItem("auth");
    return serializedState ? JSON.parse(serializedState) : undefined;
  } catch (error) {
    console.error("Could not load auth state from localStorage", error);
    return undefined;
  }
};

const saveAuthState = (state: AuthState) => {
  try {
    const serializedState = JSON.stringify(state);
    localStorage.setItem("auth", serializedState);
  } catch (error) {
    console.error("Could not save auth state to localStorage", error);
  }
};

const persistedAuthState = loadAuthState() ?? defaultAuthState;

export const store = configureStore({
  reducer: {
    auth: authReducer,
  },
  preloadedState: {
    auth: persistedAuthState,
  },
});

store.subscribe(() => {
  saveAuthState(store.getState().auth);
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
