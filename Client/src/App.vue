<script setup>
import { computed, watch } from 'vue';
import { RouterView } from 'vue-router';
import { usePatientStore } from '@/stores/patientStore';
import { useAuthStore } from '@/stores/authStore';
import NavigationBar from '@/components/NavigationBar.vue';
import StatusMelding from '@/components/StatusMelding.vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const store = usePatientStore();
const authStore = useAuthStore();

// De loginpagina heeft geen zijbalk
const toonMenu = computed(() => route.name && route.name !== 'login');

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

  <!-- Zijbalk + inhoud naast elkaar; in een smal venster of bij inzoomen komt de zijbalk erboven (reflow) -->
  <div v-if="toonMenu" class="mx-auto max-w-[1240px] flex flex-wrap items-start gap-6">
    <NavigationBar class="w-full md:w-auto md:flex-[0_1_240px]" />
    <main
      id="inhoud"
      tabindex="-1"
      class="paneel flex-[999_1_560px] min-w-0 p-6 font-['Verdana'] text-[#000080] focus:outline-none"
    >
      <!-- Mislukt het ophalen van de gegevens, dan zeggen we dat op elke pagina (in plaats van een lege lijst) -->
      <div v-if="store.error" class="mb-6 flex flex-col items-start gap-2">
        <StatusMelding type="fout" :tekst="store.error" @sluiten="store.error = null" />
        <button type="button" class="knop-secundair" :disabled="store.loading" @click="store.fetchData()">
          {{ store.loading ? 'Bezig met laden...' : 'Opnieuw proberen' }}
        </button>
      </div>
      <RouterView />
    </main>
  </div>
  <main v-else id="inhoud" tabindex="-1" class="focus:outline-none">
    <RouterView />
  </main>
</template>

