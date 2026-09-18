<script setup>
import ChartView from '@/components/ChartView.vue';
import { ref } from 'vue';
import { usePatientStore } from '@/stores/patientStore';
import { storeToRefs } from 'pinia';
import axios from 'axios';

const store = usePatientStore();
const painData = store.painData || [];
const painScore = ref(5);
const toelichting = ref('');
const error = ref('');
const patientId = painData[0]?.patientId;

async function submitPijnindicatie() {
  if (painScore.value < 0 || painScore.value > 10) {
    error.value = 'Voer een waarde in tussen 0 en 10.';
    return;
  }
  error.value = '';

  // Maak het object dat je wil opsturen
  const pijnIndicatieData = {
    patientId: patientId,
    datumTijdRegistratie: new Date().toISOString(),
    pijnScore: painScore.value,
    toelichting: toelichting.value
  };

  try {
    await axios.post('/api/pijnindicatie', pijnIndicatieData)
    alert(`Pijnscore geregistreerd: ${painScore.value}\nToelichting: ${toelichting.value}`);
    painScore.value = 5;
    toelichting.value = '';
  }
  catch {
    error.value = 'Er is een fout opgetreden bij het opslaan.';
  };
}
</script>

<template>
  <div class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    <h2 class="text-3xl font-bold mb-4 font-['Times_New_Roman']">Pijnindicatie Registreren</h2>

    <form @submit.prevent="submitPijnindicatie" novalidate>
      <div class="mb-6">
        <label for="painScore" class="block mb-2 font-semibold font-['Times_New_Roman']">Pijnscore (0-10)*</label>
        <input
          id="painScore"
          type="range"
          v-model="painScore"
          min="0"
          max="10"
          step="1"
          class="w-full"
          aria-valuemin="0"
          aria-valuemax="10"
          aria-valuenow="painScore"
        />
        <div class="text-center mt-2 font-bold text-lg">{{ painScore }}</div>
        <p v-if="error" class="text-red-600 mt-1">{{ error }}</p>
      </div>

      <div class="mb-4">
        <label for="toelichting" class="block mb-2 font-semibold font-['Times_New_Roman']">Toelichting (optioneel)</label>
        <textarea
          id="toelichting"
          v-model="toelichting"
          rows="3"
          class="w-full px-3 py-2 border border-gray-400 rounded font-['Verdana']"
          placeholder="Bijv. pijn na het traplopen"
        ></textarea>
      </div>

      <button
        type="submit"
        class="bg-[#003366] hover:bg-[#002244] text-white font-semibold px-4 py-2 rounded font-['Verdana']"
      >
        Pijnindicatie Opslaan
      </button>
    </form>

    <h2 class="text-3xl font-bold mt-10 mb-4 font-['Times_New_Roman']">Eerdere pijnindicaties</h2>
    <ChartView :pain-data="painData" class="mt-8" />
  </div>
</template>