// src/api.js
// Eén axios-instantie voor de hele app: stuurt automatisch het JWT-token mee
// en logt de gebruiker uit als het token verlopen of ongeldig is (401).
import axios from 'axios';
import router from '@/router';
import { useAuthStore } from '@/stores/authStore';

const api = axios.create({ baseURL: '/api' });

api.interceptors.request.use((config) => {
  const authStore = useAuthStore();
  if (authStore.token) {
    config.headers.Authorization = `Bearer ${authStore.token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401 && !error.config.url.includes('/auth/login')) {
      useAuthStore().logout();
      // De loginpagina legt uit waarom je opnieuw moet inloggen
      router.push({ name: 'login', query: { sessie: 'verlopen' } });
    }
    return Promise.reject(error);
  }
);

export default api;
