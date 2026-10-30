<template>
  <div class="flex items-center justify-center min-h-screen px-6 py-12 font-['Verdana'] text-[#000080]">
    <div class="paneel w-full max-w-[420px] p-8 flex flex-col gap-5">
      <h1 class="m-0 font-['Times_New_Roman'] text-3xl font-bold">Inloggen RevaliNow</h1>

      <form class="flex flex-col gap-5" @submit.prevent="handleLogin">
        <div class="flex flex-col gap-1.5">
          <label for="gebruikersnaam" class="text-sm font-bold">Gebruikersnaam (je e-mailadres)</label>
          <input id="gebruikersnaam" v-model="username" type="text" autocomplete="username" class="invoer min-h-11" required>
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="wachtwoord" class="text-sm font-bold">Wachtwoord</label>
          <input id="wachtwoord" v-model="password" type="password" autocomplete="current-password" class="invoer min-h-11" required>
        </div>

        <button type="submit" :disabled="authStore.loading" class="knop-primair text-base">
          {{ authStore.loading ? 'Bezig met inloggen...' : 'Inloggen' }}
        </button>
      </form>

      <!-- Meldingen onder de knop: daar kijk je na het klikken -->
      <StatusMelding type="info" :tekst="sessieMelding" @sluiten="sessieMelding = ''" />
      <StatusMelding type="fout" :tekst="authStore.error || ''" @sluiten="authStore.error = null" />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import StatusMelding from '@/components/StatusMelding.vue';
import { useRoute, useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();

const username = ref('');
const password = ref('');

// Na automatisch uitloggen (token verlopen) zegt de pagina waarom je opnieuw moet inloggen
const sessieMelding = ref(route.query.sessie === 'verlopen' ? 'Je sessie is verlopen. Log opnieuw in om verder te gaan.' : '');

const handleLogin = async () => {
  sessieMelding.value = '';
  const success = await authStore.login(username.value, password.value);
  if (success) {
    router.push('/dashboard');
  }
};
</script>
