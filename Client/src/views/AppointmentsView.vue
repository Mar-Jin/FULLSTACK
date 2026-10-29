<script setup>
import { ref, computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import DataList from '@/components/DataList.vue';
import StatusMelding from '@/components/StatusMelding.vue';
import FilterTabs from '@/components/FilterTabs.vue';
import { formatDatumTijd } from '@/utils/datum';

const store = usePatientStore();
const { afspraken } = storeToRefs(store);

const laden = ref(true);
const { melding, toonFout, sluit } = useMelding();
const tabblad = ref('gepland');
const tabbladen = [
  { waarde: 'gepland', label: 'Geplande afspraken' },
  { waarde: 'eerder', label: 'Eerdere afspraken' }
];

onMounted(async () => {
  try {
    await store.fetchAfspraken();
  } catch (err) {
    toonFout(err, 'De afspraken konden niet worden geladen.');
  } finally {
    laden.value = false;
  }
});

// Alleen weergave: een afspraak in het verleden die nog op 'Gepland' staat, is nooit afgemeld.
// De status in de database blijft ongewijzigd.
function toonStatus(afspraak) {
  return !afspraak.isToekomstig && afspraak.status === 'Gepland' ? 'Niet afgemeld' : afspraak.status;
}

// Geplande: eerstvolgende bovenaan. Eerdere: meest recente bovenaan.
const zichtbareAfspraken = computed(() =>
  tabblad.value === 'gepland'
    ? afspraken.value.filter((a) => a.isToekomstig)
    : afspraken.value.filter((a) => !a.isToekomstig).reverse()
);
</script>

<template>
  <div class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    <h1 class="text-2xl font-bold mb-4 font-['Times_New_Roman']">Mijn Afspraken</h1>

    <FilterTabs v-model="tabblad" :opties="tabbladen" label="Kies welke afspraken je wilt zien" />

    <StatusMelding :type="melding.type" :tekst="melding.tekst" @sluiten="sluit" />

    <DataList
      :items="zichtbareAfspraken"
      :laden="laden"
      :label="tabblad === 'gepland' ? 'Geplande afspraken' : 'Eerdere afspraken'"
      :leeg-tekst="tabblad === 'gepland' ? 'Je hebt geen geplande afspraken.' : 'Je hebt nog geen eerdere afspraken.'"
    >
      <template #item="{ item }">
        <h2 class="text-lg font-bold font-['Times_New_Roman'] text-[#900000]">{{ item.typeAfspraak }}</h2>
        <p><b>Wanneer:</b> {{ formatDatumTijd(item.datumTijdAfspraak) }} ({{ item.duurMinuten }} minuten)</p>
        <p><b>Revalidatiearts:</b> {{ item.revalidatiearts }}</p>
        <p><b>Status:</b> {{ toonStatus(item) }}</p>
      </template>
    </DataList>
  </div>
</template>
