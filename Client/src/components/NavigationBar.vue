<template>
    <header class="mt-8 mb-8 text-center font-['Verdana'] text-[#000080]">
        <div class="flex flex-col items-center border-2 border-gray-500 bg-white shadow-[4px_4px_0_#404040] w-4/5 mx-auto p-4 bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
        <!-- Navigatie -->
        <nav aria-label="Hoofdmenu" class="border border-gray-400 bg-gray-100 px-6 py-3 w-full flex flex-wrap justify-evenly gap-x-8 gap-y-2 shadow-inner">
            <!-- De rand komt alleen om de actieve pagina; router-link zet daar ook aria-current="page" op -->
            <router-link
            v-for="link in links"
            :key="link.to"
            :to="link.to"
            class="text-blue-700 underline hover:text-red-600 border-2 border-transparent rounded-sm px-2 py-1"
            active-class="!border-gray-400"
            >
            {{ link.label }}
            </router-link>
            <button
            type="button"
            @click="uitloggen"
            class="text-blue-700 underline hover:text-red-600 border-2 border-transparent rounded-sm px-2 py-1"
            >
            Uitloggen
            </button>
        </nav>
        </div>
    </header>
</template>

<script setup>
import { RouterLink, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

const links = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/exercises', label: 'Oefeningen' },
  { to: '/logbook', label: 'Logboek' },
  { to: '/pain', label: 'Pijnregistratie' },
  { to: '/appointments', label: 'Afspraken' }
];

const router = useRouter();
const authStore = useAuthStore();

function uitloggen() {
  authStore.logout();
  router.push({ name: 'login' });
}
</script>
