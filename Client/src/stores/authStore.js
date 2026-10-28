// src/stores/authStore.js
import { defineStore } from 'pinia';
import api from '@/api';

function leesOpgeslagenGebruiker() {
  try {
    return JSON.parse(localStorage.getItem('user')) || null;
  } catch {
    return null;
  }
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: leesOpgeslagenGebruiker(),
    token: localStorage.getItem('token') || null,
    loading: false,
    error: null
  }),
  getters: {
    isIngelogd: (state) => !!state.token,
    patientId: (state) => state.user?.patientId ?? null
  },
  actions: {
    async login(username, password) {
      this.loading = true;
      this.error = null;
      try {
        const response = await api.post('/auth/login', { username, password });
        const { token, ...user } = response.data;
        this.token = token;
        this.user = user;
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
        return true;
      } catch (err) {
        this.error = err.response?.data?.message || "Inloggen mislukt";
        return false;
      } finally {
        this.loading = false;
      }
    },
    logout() {
      this.user = null;
      this.token = null;
      localStorage.removeItem('user');
      localStorage.removeItem('token');
    }
  }
});
