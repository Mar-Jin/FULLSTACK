<script setup>
import { onMounted } from 'vue';
import { RouterView } from 'vue-router';
import { usePatientStore } from '@/stores/patientStore';
import NavigationBar from '@/components/NavigationBar.vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const store = usePatientStore();

onMounted(async () => {
  console.log("Fetching data...");
  await store.fetchData();
});

</script>

<template>
  <NavigationBar v-if="route.name && route.name !== 'login' "/>

  <RouterView/>
</template>


<style scoped>
/* Material Icons import */
@import url('https://fonts.googleapis.com/icon?family=Material+Icons');
</style>
