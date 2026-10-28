<script setup>
  import { usePatientStore } from '@/stores/patientStore';
  import { storeToRefs } from 'pinia';

  const store = usePatientStore();
  const { user, recentPains, upcomingExercises, nextAppointment, loading } = storeToRefs(store);
</script>

<template>
  <div v-if="user?.voornaam" class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    
    <!-- Welkomsttekst -->
    <div class="mb-4">
      <h1 class="text-3xl font-bold text-[#000080] mb-1 font-['Times_New_Roman']">
        <div>Welkom, {{ user?.voornaam }}!</div>
      </h1>
      <p class="text-gray-600 italic">Hier is je revalidatie-overzicht</p>
    </div>

    <!-- Recente Pijnindicaties -->
    <section class="border border-gray-400 mb-6 p-3 bg-gray-100">
      <div class="bg-[#003366] text-white px-3 py-1 border-b-2 border-gray-400 flex items-center">
        <span class="text-sm">★</span>
        <h2 class="ml-2 text-base font-['Times_New_Roman']">Recente Pijnindicaties</h2>
      </div>
      <ul class="list-square list-inside mt-2">
        <li v-for="pain in recentPains" :key="pain.id" class="my-1">
          <p>{{ pain.datumTijdRegistratie }}: <span class="text-red-600 font-bold">{{ pain.pijnScore }}/10</span> - {{ pain.toelichting }}</p>
        </li>
      </ul>
    </section>

    <!-- Geplande Oefeningen -->
    <section class="border border-gray-400 mb-6 p-3 bg-gray-100">
      <div class="bg-[#003366] text-white px-3 py-1 border-b-2 border-gray-400 flex items-center">
        <span class="text-sm">💪</span>
        <h2 class="ml-2 text-base font-['Times_New_Roman']">Geplande Oefeningen Vandaag</h2>
      </div>
      <ul class="list-square list-inside mt-2 w-1/2">
        <li v-for="exercise in upcomingExercises" :key="exercise.id" class="my-1">
          <p class="flex flex-row justify-between"><span class="text-red-600 font-bold">{{ exercise.oefening.naam }}</span> <span>({{ exercise.frequentiePerDag }}x per dag, {{ exercise.sets }} sets, {{ exercise.herhalingen }} keer)</span></p>
        </li>
      </ul>
    </section>

    <!-- Eerstvolgende Afspraak -->
    <section class="border border-gray-400 p-3 bg-gray-100">
      <div class="bg-[#003366] text-white px-3 py-1 border-b-2 border-gray-400 flex items-center">
        <span class="text-sm">📅</span>
        <h2 class="ml-2 text-base font-['Times_New_Roman']">Eerstvolgende Afspraak</h2>
      </div>
      <div v-if="nextAppointment" class="mt-2">
        <p><b>Datum:</b> {{ nextAppointment.datumTijdAfspraak }}</p>
        <p><b>Duur:</b> {{ nextAppointment.duurMinuten }}</p>
        <p><b>Type:</b> {{ nextAppointment.typeAfspraak }}</p>
        <p><b>Arts:</b> {{ nextAppointment.revalidatiearts?.voornaam }} {{ nextAppointment.revalidatiearts?.achternaam }}</p>
        <p><b>Status:</b> {{ nextAppointment.status }}</p>
      </div>
      <div v-else class="italic mt-2">
        Geen afspraken gepland.
      </div>
    </section>
  </div>
  <div v-else class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    <!-- Welkomsttekst -->
    <div class="mb-4">
      <h1 class="text-3xl font-bold text-[#000080] mb-1 font-['Times_New_Roman']">
        <div>Geen gebruiker gevonden!</div>
      </h1>
    </div>
  </div>
</template>
