import { initializeApp } from "firebase/app";
import {
  browserLocalPersistence,
  getAuth,
  GoogleAuthProvider,
  setPersistence,
  signInWithPopup,
} from "firebase/auth";
import { AppUser } from "./authStore";

const firebaseConfig = {
  apiKey: "AIzaSyBbB_fo_pPycsI517HpetHULiOwil8I-jM",
  authDomain: "basetemplate-fb892.firebaseapp.com",
  projectId: "basetemplate-fb892",
  storageBucket: "basetemplate-fb892.firebasestorage.app",
  messagingSenderId: "1035461240297",
  appId: "1:1035461240297:web:321342a17d2bc5a9afd778",
  measurementId: "G-8YY98L08WC",
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

// Initialize Firebase Authentication
const auth = getAuth(app);

const handleLogin = async () => {
  const provider = new GoogleAuthProvider();
  try {
    await setPersistence(auth, browserLocalPersistence);
    const result = await signInWithPopup(auth, provider);
    console.log("User Info:", result.user);
  } catch (error) {
    console.error("Error signing in:", error);
  }
};

const handleLogout = () => {
  auth.signOut();
};

const onAuthStateChangedListener = (
  callback: (user: AppUser | null) => void,
  forceRefresh: boolean
) => {
  return auth.onAuthStateChanged(async (newUser) => {
    if (newUser) {
      const idTokenResult = await newUser.getIdTokenResult(forceRefresh);
      const plainUserObject: AppUser = {
        uid: newUser.uid,
        email: newUser.email,
        displayName: newUser.displayName,
        photoURL: newUser.photoURL,
        token: idTokenResult.token,
      };
      callback(plainUserObject);
    } else {
      callback(null);
    }
  });
};
const getToken = async () => {
  if (!auth.currentUser) {
    console.warn("No authenticated user.");
    return null;
  }
  const idTokenResult = await auth.currentUser?.getIdTokenResult();
  return idTokenResult?.token;
};
const initializeFirebase = (): Promise<void> => {
  return new Promise((resolve) => {
    onAuthStateChangedListener(() => {
      // Firebase Auth is now initialized and ready
      resolve();
    }, false);
  });
};
export {
  auth,
  handleLogin,
  handleLogout,
  onAuthStateChangedListener,
  getToken,
  initializeFirebase,
};
