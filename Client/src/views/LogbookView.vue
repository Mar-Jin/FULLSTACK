<script setup>
import { ref, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import DataList from '@/components/DataList.vue';
import StatusMelding from '@/components/StatusMelding.vue';
import BevestigDialoog from '@/components/BevestigDialoog.vue';
import { formatDatumTijd } from '@/utils/datum';

const MAX_LENGTE = 500;

const store = usePatientStore();
const { logboek } = storeToRefs(store);
const { melding, toonSucces, toonFout, sluit } = useMelding();

const laden = ref(true);
const nieuweBeschrijving = ref('');
const bewerkId = ref(null);
const bewerkTekst = ref('');
const teVerwijderen = ref(null);
const lijstKop = ref(null);

onMounted(async () => {
  try {
    await store.fetchLogboek();
  } catch (err) {
    toonFout(err, 'Het logboek kon niet worden geladen.');
  } finally {
    laden.value = false;
  }
});

function controleer(tekst) {
  if (!tekst.trim()) return 'Beschrijf de activiteit.';
  if (tekst.length > MAX_LENGTE) return `De beschrijving mag maximaal ${MAX_LENGTE} tekens zijn.`;
  return '';
}

async function toevoegen() {
  const fout = controleer(nieuweBeschrijving.value);
  if (fout) return toonFout(fout);
  try {
    await store.logboekToevoegen(nieuweBeschrijving.value);
    nieuweBeschrijving.value = '';
    toonSucces('Activiteit toegevoegd aan je logboek.');
  } catch (err) {
    toonFout(err);
  }
}

function startBewerken(invoer) {
  bewerkId.value = invoer.id;
  bewerkTekst.value = invoer.beschrijving;
  sluit();
}

async function opslaan(id) {
  const fout = controleer(bewerkTekst.value);
  if (fout) return toonFout(fout);
  try {
    await store.logboekBewerken(id, bewerkTekst.value);
    bewerkId.value = null;
    toonSucces('Logboekregel bijgewerkt.');
  } catch (err) {
    toonFout(err);
  }
}

async function verwijderen() {
  const invoer = teVerwijderen.value;
  teVerwijderen.value = null;
  try {
    await store.logboekVerwijderen(invoer.id);
    toonSucces('Logboekregel verwijderd.');
    // De knop waarmee je verwijderde bestaat niet meer: zet de focus op een logische plek in plaats van bovenaan de pagina
    lijstKop.value?.focus();
  } catch (err) {
    toonFout(err);
  }
}
</script>

<template>
  <div class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    <h1 class="text-2xl font-bold mb-4 font-['Times_New_Roman']">Mijn Logboek</h1>

    <StatusMelding :type="melding.type" :tekst="melding.tekst" @sluiten="sluit" />

    <form class="mb-6" novalidate @submit.prevent="toevoegen">
      <label for="nieuweActiviteit" class="block mb-2 font-semibold font-['Times_New_Roman']">Wat heb je gedaan?</label>
      <textarea
        id="nieuweActiviteit"
        v-model="nieuweBeschrijving"
        rows="2"
        :maxlength="MAX_LENGTE"
        aria-describedby="nieuweActiviteitHulp"
        class="w-full px-3 py-2 border border-gray-400 rounded"
        placeholder="Bijv. boodschappen gedaan, wandeling van 20 minuten"
      ></textarea>
      <p id="nieuweActiviteitHulp" class="text-sm mb-2">De datum en tijd worden automatisch vastgelegd.</p>
      <button type="submit" class="bg-[#003366] hover:bg-[#002244] text-white font-semibold px-4 py-2 rounded">
        Toevoegen
      </button>
    </form>

    <h2 ref="lijstKop" tabindex="-1" class="text-xl font-bold mb-2 font-['Times_New_Roman'] focus:outline-none">Eerdere activiteiten</h2>
    <DataList :items="logboek" :laden="laden" label="Logboekregels, nieuwste eerst" leeg-tekst="Je logboek is nog leeg.">
      <template #item="{ item }">
        <p class="text-sm font-semibold">{{ formatDatumTijd(item.datumTijdActiviteit) }}</p>

        <form v-if="bewerkId === item.id" class="mt-2" novalidate @submit.prevent="opslaan(item.id)">
          <label :for="'bewerk-' + item.id" class="sr-only">Beschrijving aanpassen</label>
          <textarea
            :id="'bewerk-' + item.id"
            v-model="bewerkTekst"
            rows="2"
            :maxlength="MAX_LENGTE"
            class="w-full px-3 py-2 border border-gray-400 rounded"
          ></textarea>
          <div class="flex gap-2 mt-2">
            <button type="submit" class="bg-[#003366] text-white px-3 py-1 rounded">Opslaan</button>
            <button type="button" class="border border-gray-500 px-3 py-1 rounded" @click="bewerkId = null">Annuleren</button>
          </div>
        </form>

        <template v-else>
          <p class="my-1">{{ item.beschrijving }}</p>
          <div class="flex gap-2 mt-2">
            <button type="button" class="border border-gray-500 px-3 py-1 rounded" @click="startBewerken(item)">
              Bewerken<span class="sr-only"> {{ item.beschrijving }}</span>
            </button>
            <button type="button" class="border border-red-700 text-red-700 px-3 py-1 rounded" @click="teVerwijderen = item">
              Verwijderen<span class="sr-only"> {{ item.beschrijving }}</span>
            </button>
          </div>
        </template>
      </template>
    </DataList>

    <BevestigDialoog
      :open="teVerwijderen !== null"
      titel="Logboekregel verwijderen?"
      bevestig-tekst="Ja, verwijderen"
      @bevestig="verwijderen"
      @annuleer="teVerwijderen = null"
    >
      <p>Weet je zeker dat je deze activiteit wilt verwijderen? Dit kun je niet ongedaan maken.</p>
      <p v-if="teVerwijderen" class="mt-2 italic">"{{ teVerwijderen.beschrijving }}"</p>
    </BevestigDialoog>
  </div>
</template>
