<script setup>
  import { usePatientStore } from '@/stores/patientStore';
  import { storeToRefs } from 'pinia';
  import { formatDatumTijd } from '@/utils/datum';

  const store = usePatientStore();
  // oefeningenVandaag komt uit dezelfde state als de oefenpagina: afvinken daar is hier direct zichtbaar
  // samenvatting komt uit één database-aanroep (sp_DashboardSamenvatting)
  const { user, recentPains, oefeningenVandaag, samenvatting } = storeToRefs(store);
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
        <h2 class="text-base font-['Times_New_Roman']">Recente Pijnindicaties</h2>
      </div>
      <ul class="list-square list-inside mt-2">
        <li v-for="pain in recentPains" :key="pain.id" class="my-1">
          <p>{{ formatDatumTijd(pain.datumTijdRegistratie) }}: <span class="text-red-600 font-bold">{{ pain.pijnScore }}/10</span> - {{ pain.toelichting }}</p>
        </li>
      </ul>
      <p class="mt-2">
        <template v-if="samenvatting.aantalMetingen7Dagen > 0">
          <b>Gemiddeld de afgelopen 7 dagen:</b> {{ samenvatting.gemiddeldePijn7Dagen }}/10
          ({{ samenvatting.aantalMetingen7Dagen }} {{ samenvatting.aantalMetingen7Dagen === 1 ? 'meting' : 'metingen' }})
        </template>
        <span v-else class="italic">Je hebt de afgelopen 7 dagen geen pijn geregistreerd.</span>
      </p>
      <router-link to="/pain" class="inline-block mt-2 text-blue-700 underline hover:text-red-600">Pijn registreren</router-link>
    </section>

    <!-- Geplande Oefeningen -->
    <section class="border border-gray-400 mb-6 p-3 bg-gray-100">
      <div class="bg-[#003366] text-white px-3 py-1 border-b-2 border-gray-400 flex items-center">
        <h2 class="text-base font-['Times_New_Roman']">Geplande Oefeningen Vandaag</h2>
      </div>
      <ul v-if="oefeningenVandaag.length" class="list-square list-inside mt-2">
        <li v-for="exercise in oefeningenVandaag" :key="exercise.id" class="my-1">
          <p class="flex flex-row flex-wrap justify-between gap-2">
            <span class="text-red-600 font-bold" :class="{ 'line-through': exercise.isAfgerondVandaag }">{{ exercise.naam }}</span>
            <span>({{ exercise.frequentiePerDag }}x per dag, {{ exercise.sets }} sets, {{ exercise.herhalingen }} keer)</span>
            <span :class="exercise.isAfgerondVandaag ? 'text-green-800 font-semibold' : ''">
              {{ exercise.isAfgerondVandaag ? 'Afgerond' : `${exercise.keerGedaanVandaag} van ${exercise.frequentiePerDag} gedaan` }}
            </span>
          </p>
        </li>
      </ul>
      <p v-else class="italic mt-2">Vandaag staan er geen oefeningen gepland.</p>
      <router-link to="/exercises" class="inline-block mt-2 text-blue-700 underline hover:text-red-600">Naar mijn oefeningen</router-link>
    </section>

    <!-- Eerstvolgende Afspraak -->
    <section class="border border-gray-400 p-3 bg-gray-100">
      <div class="bg-[#003366] text-white px-3 py-1 border-b-2 border-gray-400 flex items-center">
        <h2 class="text-base font-['Times_New_Roman']">Eerstvolgende Afspraak</h2>
      </div>
      <div v-if="samenvatting.volgendeAfspraakDatum" class="mt-2">
        <p><b>Datum:</b> {{ formatDatumTijd(samenvatting.volgendeAfspraakDatum) }}</p>
        <p><b>Duur:</b> {{ samenvatting.volgendeAfspraakDuur }} minuten</p>
        <p><b>Type:</b> {{ samenvatting.volgendeAfspraakType }}</p>
        <p><b>Arts:</b> {{ samenvatting.volgendeAfspraakArts }}</p>
        <p><b>Status:</b> {{ samenvatting.volgendeAfspraakStatus }}</p>
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
