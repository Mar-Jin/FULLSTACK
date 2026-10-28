<template>
    <header class="mt-8 mb-8 text-center font-['Verdana'] text-[#000080]">
        <div class="flex flex-col items-center border-2 border-gray-500 bg-white shadow-[4px_4px_0_#404040] w-4/5 mx-auto p-4 bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
        <!-- Navigatie -->
        <nav class="border border-gray-400 bg-gray-100 px-6 py-3 w-full flex justify-evenly space-x-8 shadow-inner">
            <router-link
            to="/dashboard"
            class="text-blue-700 underline hover:text-red-600 border-2 border-gray-400 rounded-sm px-2 py-1"
            >
            Dashboard
            </router-link>
            <router-link
            to="/exercises"
            class="text-blue-700 underline hover:text-red-600 border-gray-400 rounded-sm px-2 py-1"
            >
            Oefeningen
            </router-link>
            <router-link
            to="/logbook"
            class="text-blue-700 underline hover:text-red-600 border-gray-400 rounded-sm px-2 py-1"
            >
            Logboek
            </router-link>
            <router-link
            to="/pain"
            class="text-blue-700 underline hover:text-red-600 border-gray-400 rounded-sm px-2 py-1"
            >
            Pijnregistratie
            </router-link>
            <router-link
            to="/appointments"
            class="text-blue-700 underline hover:text-red-600 border-gray-400 rounded-sm px-2 py-1"
            >
            Afspraken
            </router-link>
            <button
            type="button"
            @click="uitloggen"
            class="text-blue-700 underline hover:text-red-600 border-gray-400 rounded-sm px-2 py-1"
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

const router = useRouter();
const authStore = useAuthStore();

function uitloggen() {
  authStore.logout();
  router.push({ name: 'login' });
}
</script>