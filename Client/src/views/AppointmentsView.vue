<script setup>
import { ref, computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import DataList from '@/components/DataList.vue';
import StatusMelding from '@/components/StatusMelding.vue';
import { formatVolledigeDatum, formatTijd } from '@/utils/datum';

const store = usePatientStore();
const { afspraken } = storeToRefs(store);

const laden = ref(true);
const { melding, toonFout, sluit } = useMelding();
const vandaag = formatVolledigeDatum(new Date());

onMounted(async () => {
  try {
    await store.fetchAfspraken();
  } catch (err) {
    toonFout(err, 'De afspraken konden niet worden geladen.');
  } finally {
    laden.value = false;
  }
});

// Eén tijdlijn (US6: toekomstige én eerdere afspraken te bekijken).
// Gepland: eerstvolgende bovenaan. Eerder: meest recente bovenaan, direct onder 'vandaag'.
const gepland = computed(() => afspraken.value.filter((a) => a.isToekomstig));
const eerder = computed(() => afspraken.value.filter((a) => !a.isToekomstig).reverse());

// Alleen weergave: een afspraak in het verleden die nog op 'Gepland' staat, is nooit afgemeld.
// De status in de database blijft ongewijzigd.
function toonStatus(afspraak) {
  return !afspraak.isToekomstig && afspraak.status === 'Gepland' ? 'Niet afgemeld' : afspraak.status;
}
</script>

<template>
  <div class="flex flex-col gap-4">
    <h1 class="m-0 text-3xl font-bold font-['Times_New_Roman']">Mijn afspraken</h1>

    <StatusMelding :type="melding.type" :tekst="melding.tekst" @sluiten="sluit" />

    <h2 class="mt-2 mb-0 text-xl font-bold font-['Times_New_Roman']">Gepland</h2>
    <DataList :items="gepland" :laden="laden" label="Geplande afspraken, eerstvolgende eerst" leeg-tekst="Je hebt geen geplande afspraken.">
      <template #item="{ item }">
        <div class="flex flex-wrap items-baseline gap-x-6 gap-y-1">
          <p class="m-0 basis-64 font-bold">{{ formatVolledigeDatum(item.datumTijdAfspraak) }}, {{ formatTijd(item.datumTijdAfspraak) }}</p>
          <p class="m-0 basis-24">{{ item.typeAfspraak }}</p>
          <p class="m-0 grow basis-40">{{ item.revalidatiearts }}</p>
          <p class="m-0 basis-20">{{ item.duurMinuten }} min</p>
          <p class="m-0 basis-28">{{ toonStatus(item) }}</p>
        </div>
      </template>
    </DataList>

    <!-- Markering 'vandaag': laat zien waar je staat in je traject -->
    <div class="flex items-center gap-3 my-2" role="separator" :aria-label="`Vandaag, ${vandaag}`">
      <div class="grow border-t-[3px] border-[#000080]"></div>
      <span class="font-bold" aria-hidden="true">vandaag, {{ vandaag }}</span>
      <div class="grow border-t-[3px] border-[#000080]"></div>
    </div>

    <h2 class="m-0 text-xl font-bold font-['Times_New_Roman']">Eerder</h2>
    <!-- Eerdere afspraken iets lichter (gray-700 blijft ruim boven contrast 4.5:1) -->
    <DataList :items="eerder" :laden="laden" label="Eerdere afspraken, meest recente eerst" leeg-tekst="Je hebt nog geen eerdere afspraken." :item-klasse="() => 'border-gray-300 bg-white'">
      <template #item="{ item }">
        <div class="flex flex-wrap items-baseline gap-x-6 gap-y-1 text-gray-700">
          <p class="m-0 basis-64 font-bold">{{ formatVolledigeDatum(item.datumTijdAfspraak) }}, {{ formatTijd(item.datumTijdAfspraak) }}</p>
          <p class="m-0 basis-24">{{ item.typeAfspraak }}</p>
          <p class="m-0 grow basis-40">{{ item.revalidatiearts }}</p>
          <p class="m-0 basis-20">{{ item.duurMinuten }} min</p>
          <p class="m-0 basis-28">{{ toonStatus(item) }}</p>
        </div>
      </template>
    </DataList>
  </div>
</template>
