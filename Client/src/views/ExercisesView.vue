<script setup>
import { ref, computed } from 'vue';
import { storeToRefs } from 'pinia';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import DataList from '@/components/DataList.vue';
import StatusMelding from '@/components/StatusMelding.vue';
import FilterTabs from '@/components/FilterTabs.vue';
import { formatDatum } from '@/utils/datum';

const store = usePatientStore();
const { oefenplannen, loading } = storeToRefs(store);

const statusFilter = ref('vandaag');
const filterOpties = [
  { waarde: 'vandaag', label: 'Vandaag' },
  { waarde: 'nog-te-doen', label: 'Nog te doen' },
  { waarde: 'afgerond', label: 'Afgerond' },
  { waarde: 'alle', label: 'Alle oefenplannen' }
];

const zichtbareOefeningen = computed(() => {
  switch (statusFilter.value) {
    case 'vandaag': return oefenplannen.value.filter((o) => o.isActief);
    case 'nog-te-doen': return oefenplannen.value.filter((o) => o.isActief && !o.isAfgerondVandaag);
    case 'afgerond': return oefenplannen.value.filter((o) => o.isAfgerondVandaag);
    default: return oefenplannen.value;
  }
});

const bezigMet = ref(null);
const { melding, toonSucces, toonFout, sluit } = useMelding();

async function afvinken(oefening, isAfgevinkt) {
  bezigMet.value = oefening.id;
  sluit();
  try {
    await store.afvinken(oefening.id, isAfgevinkt);
    toonSucces(isAfgevinkt ? `${oefening.naam} afgevinkt.` : `Afvinken van ${oefening.naam} ongedaan gemaakt.`);
  } catch (err) {
    toonFout(err);
  } finally {
    bezigMet.value = null;
  }
}
</script>

<template>
  <div class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    <h1 class="text-2xl font-bold mb-4 font-['Times_New_Roman']">Mijn Oefeningen</h1>

    <FilterTabs v-model="statusFilter" :opties="filterOpties" label="Filter oefeningen op status" />

    <!-- Bij afvinken laat de oefening zelf al zien wat er veranderde; de succesmelding is alleen voor schermlezers -->
    <StatusMelding
      :type="melding.type"
      :tekst="melding.tekst"
      :alleen-schermlezer="melding.type === 'succes'"
      @sluiten="sluit"
    />

    <DataList
      :items="zichtbareOefeningen"
      :laden="loading"
      label="Toegewezen oefeningen"
      leeg-tekst="Geen oefeningen in deze categorie."
    >
      <template #item="{ item }">
        <div :class="{ 'opacity-70': item.isAfgerondVandaag }">
          <h2 class="text-lg font-['Times_New_Roman'] font-bold text-[#900000]" :class="{ 'line-through': item.isAfgerondVandaag }">
            {{ item.naam }}
          </h2>
          <p class="mb-2">{{ item.beschrijving }}</p>
          <p class="mb-2 font-semibold">
            Herhalingen: {{ item.herhalingen }} | Sets: {{ item.sets }} | {{ item.frequentiePerDag }}x per dag
          </p>
          <p v-if="item.opmerkingen" class="mb-2 italic">Let op: {{ item.opmerkingen }}</p>
          <p class="mb-2 text-sm">Loopt van {{ formatDatum(item.startDatum) }} t/m {{ formatDatum(item.eindDatum) }}</p>
          <a
            v-if="item.instructieVideoUrl"
            :href="item.instructieVideoUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="text-blue-700 underline hover:text-red-700"
          >
            Bekijk instructievideo<span class="sr-only"> van {{ item.naam }} (opent in nieuw tabblad)</span>
          </a>

          <div v-if="item.isActief" class="mt-3 flex flex-wrap items-center gap-3">
            <span class="font-semibold" :class="item.isAfgerondVandaag ? 'text-green-800' : ''">
              {{ item.isAfgerondVandaag ? 'Afgerond voor vandaag' : `Vandaag ${item.keerGedaanVandaag} van ${item.frequentiePerDag} keer gedaan` }}
            </span>
            <button
              type="button"
              class="bg-[#003366] hover:bg-[#002244] text-white px-3 py-1 rounded disabled:opacity-50"
              :disabled="item.isAfgerondVandaag || bezigMet === item.id"
              @click="afvinken(item, true)"
            >
              Afvinken<span class="sr-only"> {{ item.naam }}</span>
            </button>
            <button
              v-if="item.keerGedaanVandaag > 0"
              type="button"
              class="border border-gray-500 px-3 py-1 rounded disabled:opacity-50"
              :disabled="bezigMet === item.id"
              @click="afvinken(item, false)"
            >
              Ongedaan maken<span class="sr-only"> {{ item.naam }}</span>
            </button>
          </div>
          <p v-else class="mt-3 italic">Dit oefenplan loopt nu niet.</p>
        </div>
      </template>
    </DataList>
  </div>
</template>
