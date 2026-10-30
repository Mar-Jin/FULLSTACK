<script setup>
import ChartView from '@/components/ChartView.vue';
import FilterTabs from '@/components/FilterTabs.vue';
import { ref, computed, onMounted } from 'vue';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import StatusMelding from '@/components/StatusMelding.vue';
import KaderSectie from '@/components/KaderSectie.vue';
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
  <div class="flex flex-col gap-5">
    <h1 class="m-0 text-3xl font-bold font-['Times_New_Roman']">Mijn pijnindicaties</h1>

    <StatusMelding :type="melding.type" :tekst="melding.tekst" @sluiten="sluit" />

    <!-- Formulier en grafiek naast elkaar: je nieuwe score zie je meteen in het verloop -->
    <div class="flex flex-wrap gap-6 items-start">
    <KaderSectie titel="Pijn registreren" kop-id="pijn-invoer-kop" class="flex-[1_1_320px] min-w-0">
    <form @submit.prevent="submitPijnindicatie" novalidate>
      <div class="mb-6">
        <label for="painScore" class="block mb-2 font-bold">Hoeveel pijn heb je nu?</label>
        <input
          id="painScore"
          type="range"
          v-model="painScore"
          min="0"
          max="10"
          step="1"
          class="w-full min-h-11 accent-[#003366]"
          :aria-valuetext="`${painScore} van 10`"
          aria-describedby="painScoreUitleg"
        />
        <p id="painScoreUitleg" class="flex justify-between text-sm"><span>0 = geen pijn</span><span>10 = ondraaglijke pijn</span></p>
        <div class="text-center mt-2 font-['Times_New_Roman'] font-bold text-3xl" aria-hidden="true">{{ painScore }}</div>
        <p v-if="error" class="text-red-700 mt-1" role="alert">{{ error }}</p>
      </div>

      <div class="mb-4">
        <label for="toelichting" class="block mb-2 font-bold">Toelichting (optioneel)</label>
        <textarea
          id="toelichting"
          v-model="toelichting"
          rows="3"
          class="invoer"
          placeholder="Bijv. pijn na het traplopen"
        ></textarea>
        <p class="mt-1 mb-0 text-sm text-gray-700">De datum en tijd worden automatisch vastgelegd.</p>
      </div>

      <button
        type="submit"
        class="knop-primair"
      >
        Opslaan
      </button>
    </form>
    </KaderSectie>

    <KaderSectie titel="Verloop" kop-id="pijn-verloop-kop" class="flex-[1.4_1_420px] min-w-0">
      <template v-if="painData.length">
        <FilterTabs class="mb-4" v-model="weergave" :opties="weergaveOpties" label="Kies hoe de grafiek de pijnscores toont" />
        <ChartView
          :punten="grafiekPunten"
          :label="weergave === 'week' ? 'Gemiddelde pijnscore' : 'Pijnscore'"
          :eenheid="weergave === 'week' ? 'week' : 'day'"
        />
      </template>
      <!-- Lege toestand: uitleg in plaats van een lege grafiek -->
      <p v-else class="m-0 italic">Je hebt nog geen pijn geregistreerd. Je eerste score zie je hier meteen terug.</p>
    </KaderSectie>
    </div>
  </div>
</template>