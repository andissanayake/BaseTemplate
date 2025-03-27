import { useEffect } from "react";
import axiosInstance from "./axiosInstance";

export const Secure = () => {
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
    fetchData();
  }, []);

  return <p>Secure</p>;
};
