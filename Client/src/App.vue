<script setup>
import { watch } from 'vue';
import { RouterView } from 'vue-router';
import { usePatientStore } from '@/stores/patientStore';
import { useAuthStore } from '@/stores/authStore';
import NavigationBar from '@/components/NavigationBar.vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const store = usePatientStore();
const authStore = useAuthStore();

// Haal de gegevens op zodra er iemand is ingelogd (ook na een refresh van de pagina),
// en wis ze weer bij uitloggen zodat de volgende gebruiker ze niet ziet.
watch(() => authStore.patientId, async (patientId) => {
  if (patientId) {
    await store.fetchData();
  } else {
    store.$reset();
  }
}, { immediate: true });

</script>

<template>
  <!-- Skip-link: wie met het toetsenbord werkt, hoeft niet op elke pagina eerst door het menu te tabben -->
  <a
    href="#inhoud"
    class="sr-only focus:not-sr-only focus:fixed focus:top-2 focus:left-2 focus:z-50 bg-white border-2 border-[#000080] px-3 py-2 font-['Verdana'] text-[#000080]"
  >
    Naar de inhoud
  </a>

  <NavigationBar v-if="route.name && route.name !== 'login' "/>

  <main id="inhoud" tabindex="-1" class="focus:outline-none">
    <RouterView/>
  </main>
</template>


<style scoped>
/* Material Icons import */
@import url('https://fonts.googleapis.com/icon?family=Material+Icons');
</style>
