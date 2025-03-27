import axios from "axios";
import { getToken } from "./firebase";

const axiosInstance = axios.create({
  baseURL: "/",
});

export const setupAxios = () => {
  axiosInstance.interceptors.request.use(
    async (config) => {
      const token = await getToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => {
      console.error("Request Error", error);
      Promise.reject(error);
    }
  );
  axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
      const originalRequest = error.config;
      if (error.response?.status === 500) {
        console.error("Internal Server Error", error);
      } else if (error.response?.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true;
        return axiosInstance(originalRequest);
      } else {
        console.error("Response Error", error);
      }
      return Promise.reject(error);
    }
  );
};

export default axiosInstance;
