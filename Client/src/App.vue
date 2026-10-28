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
  <NavigationBar v-if="route.name && route.name !== 'login' "/>

  <RouterView/>
</template>


<style scoped>
/* Material Icons import */
@import url('https://fonts.googleapis.com/icon?family=Material+Icons');
</style>
