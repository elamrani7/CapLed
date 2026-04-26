import axios from 'axios';

const httpClient = axios.create({
  baseURL: 'https://capled-api.onrender.com',
  timeout: 8000,
  headers: {
    'Content-Type': 'application/json',
  },
});

httpClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    console.error('API Error:', error.response || error.message);
    return Promise.reject(error);
  }
);

export default httpClient;
