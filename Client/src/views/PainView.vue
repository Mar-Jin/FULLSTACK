<script setup>
import ChartView from '@/components/ChartView.vue';
import FilterTabs from '@/components/FilterTabs.vue';
import { ref, computed, onMounted } from 'vue';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import StatusMelding from '@/components/StatusMelding.vue';
import { storeToRefs } from 'pinia';

const store = usePatientStore();
const { painData, pijnPerWeek } = storeToRefs(store);
const { melding, toonSucces, toonFout, sluit } = useMelding();

// Grafiek: elke meting apart, of het weekgemiddelde (berekend in de database)
const weergave = ref('metingen');
const weergaveOpties = [
  { waarde: 'metingen', label: 'Per meting' },
  { waarde: 'week', label: 'Weekgemiddelde' }
];

onMounted(async () => {
  try {
    await store.fetchPijnPerWeek();
  } catch (err) {
    toonFout(err, 'Het weekoverzicht kon niet worden geladen.');
  }
});

const grafiekPunten = computed(() =>
  weergave.value === 'week'
    ? pijnPerWeek.value.map((w) => ({
        datum: w.weekStart,
        score: w.gemiddeldePijn,
        toelichting: `${w.aantalMetingen} metingen, laagste ${w.laagstePijn}, hoogste ${w.hoogstePijn}`
      }))
    : painData.value.map((p) => ({ datum: p.datumTijdRegistratie, score: p.pijnScore, toelichting: p.toelichting }))
);
const painScore = ref(5);
const toelichting = ref('');
const error = ref('');

async function submitPijnindicatie() {
  if (painScore.value < 0 || painScore.value > 10) {
    error.value = 'Voer een waarde in tussen 0 en 10.';
    return;
  }
  error.value = '';

  try {
    // De patiënt en de datum/tijd bepaalt de server zelf
    await store.registreerPijn(Number(painScore.value), toelichting.value || null)
    toonSucces(`Pijnscore ${painScore.value}/10 geregistreerd.`);
    painScore.value = 5;
    toelichting.value = '';
  }
  catch (err) {
    toonFout(err, 'Er is een fout opgetreden bij het opslaan.');
  };
}
</script>

<template>
  <div class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    <h1 class="text-3xl font-bold mb-4 font-['Times_New_Roman']">Pijnindicatie Registreren</h1>

    <StatusMelding :type="melding.type" :tekst="melding.tekst" @sluiten="sluit" />

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
          :aria-valuetext="`${painScore} van 10`"
          aria-describedby="painScoreUitleg"
        />
        <p id="painScoreUitleg" class="flex justify-between text-sm"><span>0 = geen pijn</span><span>10 = ondraaglijke pijn</span></p>
        <div class="text-center mt-2 font-bold text-lg" aria-hidden="true">{{ painScore }}</div>
        <p v-if="error" class="text-red-700 mt-1" role="alert">{{ error }}</p>
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
    <FilterTabs v-model="weergave" :opties="weergaveOpties" label="Kies hoe de grafiek de pijnscores toont" />
    <ChartView
      :punten="grafiekPunten"
      :label="weergave === 'week' ? 'Gemiddelde pijnscore' : 'Pijnscore'"
      :eenheid="weergave === 'week' ? 'week' : 'day'"
      class="mt-4"
    />
  </div>
</template>