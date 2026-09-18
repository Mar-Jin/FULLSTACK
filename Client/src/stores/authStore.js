// src/stores/authStore.js
import { defineStore } from 'pinia';
import axios from 'axios';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    loading: false,
    error: null
  }),
  actions: {
    async login(username, password) {
      this.loading = true;
      this.error = null;
      try {
        const response = await axios.post('/api/auth/login', { username, password });
        this.user = response.data;
        localStorage.setItem('user', JSON.stringify(this.user));
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
      localStorage.removeItem('user');
    }
  }
});