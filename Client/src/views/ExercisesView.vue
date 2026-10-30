<script setup>
import { ref, computed, watch, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import { useAfvinken } from '@/composables/useAfvinken';
import DataList from '@/components/DataList.vue';
import FilterTabs from '@/components/FilterTabs.vue';
import GedaanVandaag from '@/components/GedaanVandaag.vue';
import StatusMelding from '@/components/StatusMelding.vue';
import { formatDatum } from '@/utils/datum';

const store = usePatientStore();
const route = useRoute();
const { oefenplannen, loading, error } = storeToRefs(store);

// Statusfilter uit user story 2. Begrippen (zie het design):
// - Nog te doen: het plan loopt vandaag en is nog niet vaak genoeg gedaan (ook 1 van 2)
// - Afgerond: vandaag zo vaak gedaan als de frequentie vraagt
// - Een plan dat niet loopt is geen van beide en staat alleen onder 'Alle'
const statusFilter = ref('alle');
const filterOpties = [
  { waarde: 'alle', label: 'Alle' },
  { waarde: 'nog-te-doen', label: 'Nog te doen' },
  { waarde: 'afgerond', label: 'Afgerond' }
];

// De API geeft de plannen al in een logische volgorde: lopende eerst, daarna op startdatum
const zichtbareOefeningen = computed(() => {
  switch (statusFilter.value) {
    case 'nog-te-doen': return oefenplannen.value.filter((o) => o.isActief && !o.isAfgerondVandaag);
    case 'afgerond': return oefenplannen.value.filter((o) => o.isAfgerondVandaag);
    default: return oefenplannen.value;
  }
});

function looptijd(oefening) {
  const tekst = `${formatDatum(oefening.startDatum)} t/m ${formatDatum(oefening.eindDatum)}`;
  if (oefening.isActief) return `Loopt van ${tekst}`;
  return new Date(oefening.startDatum) > new Date() ? `Begint op ${formatDatum(oefening.startDatum)}` : `Liep van ${tekst}`;
}

const { melding, toonSucces, toonFout, sluit } = useMelding();
const { bezigMet, afvinken } = useAfvinken({ toonSucces, toonFout, sluit });

// 'Bekijken' op het dashboard linkt naar #oefenplan-<id>: scroll daarheen, zet de focus op de kop
// (zodat een schermlezer meldt waar je bent) en markeer de oefening kort.
watch(
  () => [route.hash, oefenplannen.value.length],
  async ([hash]) => {
    if (!hash.startsWith('#oefenplan-')) return;
    statusFilter.value = 'alle';
    await nextTick();
    // Even wachten tot de router de focus op de paginakop heeft gezet, daarna verplaatsen we hem
    setTimeout(() => {
      const kaart = document.querySelector(hash);
      if (!kaart) return;
      kaart.scrollIntoView({ block: 'center' });
      kaart.querySelector('h2')?.focus();
      kaart.classList.remove('gemarkeerd');
      void kaart.offsetWidth; // animatie opnieuw starten
      kaart.classList.add('gemarkeerd');
    }, 50);
  },
  { immediate: true }
);
</script>

<template>
  <div class="flex flex-col gap-5">
    <h1 class="m-0 text-3xl font-bold font-['Times_New_Roman']">Mijn oefeningen</h1>

    <FilterTabs v-model="statusFilter" :opties="filterOpties" label="Filter oefeningen op status" />

    <!-- Bij afvinken laat de oefening zelf al zien wat er veranderde; de succesmelding is alleen voor schermlezers -->
    <StatusMelding
      :type="melding.type"
      :tekst="melding.tekst"
      :alleen-schermlezer="melding.type === 'succes'"
      @sluiten="sluit"
    />

    <!-- Bij een laadfout staat de melding al boven de pagina; een lege lijst zou dan misleiden -->
    <DataList
      v-if="!error"
      :items="zichtbareOefeningen"
      :laden="loading"
      label="Toegewezen oefeningen"
      leeg-tekst="Geen oefeningen in deze categorie."
      :item-klasse="(o) => (o.isActief ? 'border-gray-400 bg-gray-100' : 'border-dashed border-gray-400 bg-white')"
    >
      <template #item="{ item }">
        <article :id="`oefenplan-${item.id}`" class="flex flex-col gap-2 scroll-mt-6">
          <h2
            tabindex="-1"
            class="m-0 text-xl font-['Times_New_Roman'] font-bold"
            :class="item.isAfgerondVandaag ? 'text-gray-600 line-through' : 'text-[#000080]'"
          >
            {{ item.naam }}
          </h2>
          <p class="m-0">{{ item.beschrijving }}</p>
          <p class="m-0 font-semibold">Herhalingen: {{ item.herhalingen }} · Sets: {{ item.sets }} · {{ item.frequentiePerDag }}x per dag</p>
          <p v-if="item.opmerkingen" class="m-0 italic">Let op: {{ item.opmerkingen }}</p>
          <p class="m-0 text-sm">{{ looptijd(item) }}</p>
          <a
            v-if="item.instructieVideoUrl"
            :href="item.instructieVideoUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="self-start text-blue-700 underline hover:text-red-700"
          >
            Bekijk instructievideo <span class="sr-only">van {{ item.naam }} (opent in nieuw tabblad)</span>
          </a>

          <!-- Status en acties onder de oefening, net als op het dashboard -->
          <div v-if="item.isActief" class="mt-1 flex flex-wrap items-center gap-x-3.5 gap-y-2.5 border-t border-gray-300 pt-2.5">
            <GedaanVandaag v-if="item.isAfgerondVandaag" />
            <template v-else>
              <span class="font-semibold">Vandaag {{ item.keerGedaanVandaag }} van {{ item.frequentiePerDag }} keer gedaan</span>
              <button
                type="button"
                class="knop-primair"
                :disabled="bezigMet === item.id"
                @click="afvinken(item, true)"
              >
                Afvinken <span class="sr-only">{{ item.naam }}</span>
              </button>
            </template>
            <button
              v-if="item.keerGedaanVandaag > 0"
              type="button"
              class="knop-secundair"
              :disabled="bezigMet === item.id"
              @click="afvinken(item, false)"
            >
              Ongedaan maken <span class="sr-only">{{ item.naam }}</span>
            </button>
          </div>
          <p v-else class="mt-1 mb-0 italic border-t border-gray-300 pt-2.5">Dit oefenplan loopt nu niet.</p>
        </article>
      </template>
    </DataList>
  </div>
</template>
