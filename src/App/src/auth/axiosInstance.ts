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
      if (
        (error.response?.status === 401 || error.response?.status === 403) &&
        !originalRequest._retry
      ) {
        originalRequest._retry = true;
        return axiosInstance(originalRequest);
      }
      return Promise.reject(error);
    }
  );
};

export default axiosInstance;
