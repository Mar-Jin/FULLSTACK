<template>
  <aside class="paneel p-4 flex flex-col gap-4 font-['Verdana'] text-[#000080]">
    <p class="m-0 font-['Times_New_Roman'] text-2xl font-bold">RevaliNow</p>

    <nav aria-label="Hoofdmenu" class="flex flex-col gap-1 bg-gray-100 border border-gray-400 p-2">
      <!-- De rand komt alleen om de actieve pagina; router-link zet daar ook aria-current="page" op -->
      <router-link
        v-for="link in links"
        :key="link.to"
        :to="link.to"
        class="block px-2.5 py-2 border-2 border-transparent text-blue-700 underline hover:text-red-700"
        active-class="!border-gray-400 bg-white font-bold"
      >
        {{ link.label }}
      </router-link>
    </nav>

    <div class="border-t border-gray-400 pt-3 flex flex-col items-start gap-1.5">
      <p v-if="naam" class="m-0 text-sm text-gray-700">Ingelogd als {{ naam }}</p>
      <button
        type="button"
        @click="uitloggen"
        class="py-1 text-blue-700 underline hover:text-red-700"
      >
        Uitloggen
      </button>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { usePatientStore } from '@/stores/patientStore';

const links = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/exercises', label: 'Oefeningen' },
  { to: '/logbook', label: 'Logboek' },
  { to: '/pain', label: 'Pijnindicaties' },
  { to: '/appointments', label: 'Afspraken' }
];

const router = useRouter();
const authStore = useAuthStore();
const patientStore = usePatientStore();

const naam = computed(() =>
  patientStore.user?.voornaam ? `${patientStore.user.voornaam} ${patientStore.user.achternaam}` : authStore.user?.voornaam
);

function uitloggen() {
  authStore.logout();
  router.push({ name: 'login' });
}
</script>
