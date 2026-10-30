<script setup>
import { ref, computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { usePatientStore } from '@/stores/patientStore';
import { useMelding } from '@/composables/useMelding';
import DataList from '@/components/DataList.vue';
import StatusMelding from '@/components/StatusMelding.vue';
import BevestigDialoog from '@/components/BevestigDialoog.vue';
import { formatTijd, dagLabel } from '@/utils/datum';

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

// Logboekregels per dag ('Vandaag', 'Gisteren', 'maandag 26 oktober'); de lijst is al nieuwste-eerst
const perDag = computed(() => {
  const groepen = [];
  for (const regel of logboek.value) {
    const sleutel = new Date(regel.datumTijdActiviteit).toDateString();
    let groep = groepen.find((g) => g.sleutel === sleutel);
    if (!groep) {
      groep = { sleutel, label: dagLabel(regel.datumTijdActiviteit), regels: [] };
      groepen.push(groep);
    }
    groep.regels.push(regel);
  }
  return groepen.map((g) => ({ ...g, sleutel: g.sleutel.replace(/\s/g, '-') }));
});

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
  <div class="flex flex-col gap-5">
    <h1 class="m-0 text-3xl font-bold font-['Times_New_Roman']">Mijn logboek</h1>

    <StatusMelding :type="melding.type" :tekst="melding.tekst" @sluiten="sluit" />

    <form class="border border-gray-400 bg-gray-100 p-4 flex flex-col gap-2" novalidate @submit.prevent="toevoegen">
      <label for="nieuweActiviteit" class="font-bold">Wat heb je gedaan?</label>
      <textarea
        id="nieuweActiviteit"
        v-model="nieuweBeschrijving"
        rows="3"
        :maxlength="MAX_LENGTE"
        aria-describedby="nieuweActiviteitHulp"
        class="invoer"
        placeholder="Bijv. boodschappen gedaan, wandeling van 20 minuten"
      ></textarea>
      <p id="nieuweActiviteitHulp" class="m-0 text-sm text-gray-700">De datum en tijd worden automatisch vastgelegd.</p>
      <button type="submit" class="knop-primair self-start">
        Toevoegen
      </button>
    </form>

    <h2 ref="lijstKop" tabindex="-1" class="m-0 text-xl font-bold font-['Times_New_Roman'] focus:outline-none">Eerdere activiteiten</h2>

    <!-- Leeg of aan het laden: één lijst met de juiste melding -->
    <DataList v-if="laden || !logboek.length" :items="[]" :laden="laden" label="Logboekregels" leeg-tekst="Je logboek is nog leeg. Wat heb je vandaag gedaan?" />

    <!-- Gegroepeerd per dag, nieuwste eerst: snel terug te vinden -->
    <template v-else>
    <section v-for="dag in perDag" :key="dag.sleutel" :aria-labelledby="`dag-${dag.sleutel}`" class="flex flex-col gap-2">
      <h3 :id="`dag-${dag.sleutel}`" class="m-0 text-base font-bold border-b-2 border-gray-300 pb-1">{{ dag.label }}</h3>
      <DataList :items="dag.regels" :label="`Activiteiten van ${dag.label.toLowerCase()}`">
      <template #item="{ item }">
        <form v-if="bewerkId === item.id" class="flex flex-col gap-2" novalidate @submit.prevent="opslaan(item.id)">
          <label :for="'bewerk-' + item.id" class="text-sm font-semibold">
            {{ formatTijd(item.datumTijdActiviteit) }} · Beschrijving aanpassen
          </label>
          <textarea
            :id="'bewerk-' + item.id"
            v-model="bewerkTekst"
            rows="2"
            :maxlength="MAX_LENGTE"
            class="invoer"
          ></textarea>
          <div class="flex flex-wrap gap-2">
            <button type="submit" class="knop-primair">Opslaan</button>
            <button type="button" class="knop-secundair" @click="bewerkId = null">Annuleren</button>
          </div>
        </form>

        <div v-else class="flex flex-wrap items-center justify-between gap-2">
          <p class="m-0 flex gap-3 items-baseline min-w-0">
            <span class="shrink-0 text-sm font-semibold">{{ formatTijd(item.datumTijdActiviteit) }}</span>
            <span class="break-words min-w-0">{{ item.beschrijving }}</span>
          </p>
          <div class="flex gap-2">
            <button type="button" class="knop-secundair" @click="startBewerken(item)">
              Bewerken <span class="sr-only">{{ item.beschrijving }}</span>
            </button>
            <button type="button" class="knop-secundair" @click="teVerwijderen = item">
              Verwijderen <span class="sr-only">{{ item.beschrijving }}</span>
            </button>
          </div>
        </div>
      </template>
      </DataList>
    </section>
    </template>

    <BevestigDialoog
      :open="teVerwijderen !== null"
      titel="Logboekregel verwijderen?"
      bevestig-tekst="Ja, verwijderen"
      @bevestig="verwijderen"
      @annuleer="teVerwijderen = null"
    >
      <p>Weet je zeker dat je deze activiteit wilt verwijderen? Dit kun je niet ongedaan maken.</p>
      <p v-if="teVerwijderen" class="mt-2 mb-0 italic">"{{ teVerwijderen.beschrijving }}"</p>
    </BevestigDialoog>
  </div>
</template>
