import axios from 'axios';

export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'https://capled-api.onrender.com';

const httpClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 8000,
  headers: {
    'Content-Type': 'application/json',
  },
});

httpClient.interceptors.request.use((config) => {
  try {
    const savedAuth = localStorage.getItem('capled_auth');
    if (savedAuth) {
      const auth = JSON.parse(savedAuth);
      if (auth?.token) {
        config.headers.Authorization = `Bearer ${auth.token}`;
      }
    }
  } catch {
    localStorage.removeItem('capled_auth');
  }

  return config;
});

httpClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    console.error('API Error:', error.response || error.message);
    return Promise.reject(error);
  }
);

export default httpClient;
