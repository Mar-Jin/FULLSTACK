<template>
  <div class="login-container flex items-center justify-center min-h-screen bg-gray-100">
    <div class="p-8 bg-white border-2 border-navy shadow-lg w-96">
      <h2 class="text-2xl font-bold text-navy mb-6">Inloggen RevaliNow</h2>
      
      <form @submit.prevent="handleLogin">
        <div class="mb-4">
          <label class="block text-sm font-bold mb-2">Gebruikersnaam</label>
          <input v-model="username" type="text" autocomplete="username" class="w-full p-2 border border-gray-400" required>
        </div>

        <div class="mb-6">
          <label class="block text-sm font-bold mb-2">Wachtwoord</label>
          <input v-model="password" type="password" class="w-full p-2 border border-gray-400" required>
        </div>

        <button :disabled="authStore.loading" class="bg-navy text-white w-full py-2 font-bold hover:bg-blue-900 transition">
          {{ authStore.loading ? 'Bezig met inloggen...' : 'Inloggen' }}
        </button>

        <p v-if="authStore.error" class="text-red-600 mt-4 text-sm italic">{{ authStore.error }}</p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const username = ref('');
const password = ref('');

const handleLogin = async () => {
  const success = await authStore.login(username.value, password.value);
  if (success) {
    router.push('/dashboard');
  }
};
</script>

<style scoped>
.text-navy { color: #000080; }
.border-navy { border-color: #000080; }
.bg-navy { background-color: #000080; }
</style>