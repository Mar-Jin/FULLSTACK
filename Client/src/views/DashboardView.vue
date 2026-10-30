<script setup>
  import { computed } from 'vue';
  import { storeToRefs } from 'pinia';
  import { usePatientStore } from '@/stores/patientStore';
  import { useMelding } from '@/composables/useMelding';
  import { useAfvinken } from '@/composables/useAfvinken';
  import KaderSectie from '@/components/KaderSectie.vue';
  import GedaanVandaag from '@/components/GedaanVandaag.vue';
  import StatusMelding from '@/components/StatusMelding.vue';
  import { formatVolledigeDatum, formatDagDatum, formatTijd, formatKorteDatum } from '@/utils/datum';

  const store = usePatientStore();
  // oefeningenVandaag komt uit dezelfde state als de oefenpagina: afvinken hier is daar direct zichtbaar (en andersom)
  // samenvatting komt uit één database-aanroep (sp_DashboardSamenvatting)
  const { user, recentPains, painData, oefeningenVandaag, samenvatting, loading } = storeToRefs(store);
  const { melding, toonSucces, toonFout, sluit } = useMelding();

  const { bezigMet, afvinken } = useAfvinken({ toonSucces, toonFout, sluit });

  const vandaag = formatVolledigeDatum(new Date());

  // Kleine trendlijn van de laatste 8 pijnscores (oudste links). y: 0 = onderaan, 10 = bovenaan.
  const trend = computed(() => {
    const punten = painData.value.slice(0, 8).reverse();
    if (punten.length < 2) return null;
    const stap = 232 / (punten.length - 1);
    return {
      punten: punten.map((p, i) => `${4 + i * stap},${58 - p.pijnScore * 5}`).join(' '),
      label: `Pijnverloop van ${formatDagDatum(punten[0].datumTijdRegistratie)} tot ${formatDagDatum(punten[punten.length - 1].datumTijdRegistratie)}: `
        + `van ${punten[0].pijnScore} naar ${punten[punten.length - 1].pijnScore}`
    };
  });
</script>

<template>
  <div v-if="user?.voornaam" class="flex flex-col gap-6">

    <div class="flex flex-wrap justify-between items-end gap-2">
      <div>
        <h1 class="m-0 font-['Times_New_Roman'] text-4xl font-bold leading-tight">Welkom, {{ user.voornaam }}!</h1>
        <p class="mt-1 mb-0 text-gray-600 italic">Hier is je revalidatie-overzicht</p>
      </div>
      <p class="m-0 text-sm text-gray-700">{{ vandaag }}</p>
    </div>

    <!-- Bij afvinken laat de taak zelf al zien wat er veranderde; de succesmelding is alleen voor schermlezers -->
    <StatusMelding
      :type="melding.type"
      :tekst="melding.tekst"
      :alleen-schermlezer="melding.type === 'succes'"
      @sluiten="sluit"
    />

    <div class="flex flex-wrap gap-6 items-start">

      <KaderSectie titel="Vandaag te doen" kop-id="vandaag-kop" class="flex-[2_1_420px] min-w-0">
        <ul class="list-none m-0 p-0 flex flex-col gap-2">
          <!-- Per taak eerst naam en uitleg, daaronder status en acties (zelfde opbouw als 'Mijn oefeningen') -->
          <li v-for="oefening in oefeningenVandaag" :key="oefening.id" class="bg-white border border-gray-400 p-3 flex flex-col gap-2.5">
            <div>
              <p class="m-0 font-bold" :class="oefening.isAfgerondVandaag ? 'text-gray-600 line-through' : ''">{{ oefening.naam }}</p>
              <p class="mt-0.5 mb-0 text-sm text-gray-700">
                {{ oefening.frequentiePerDag }}x per dag · {{ oefening.sets }} sets · {{ oefening.herhalingen }} herhalingen
              </p>
            </div>
            <div class="flex flex-wrap items-center gap-x-3.5 gap-y-2.5 border-t border-gray-200 pt-2.5">
              <GedaanVandaag v-if="oefening.isAfgerondVandaag" />
              <template v-else>
                <span>{{ oefening.keerGedaanVandaag }} van {{ oefening.frequentiePerDag }} gedaan</span>
                <button
                  type="button"
                  class="knop-primair"
                  :disabled="bezigMet === oefening.id"
                  @click="afvinken(oefening, true)"
                >
                  Afvinken <span class="sr-only">{{ oefening.naam }}</span>
                </button>
              </template>
              <button
                v-if="oefening.keerGedaanVandaag > 0"
                type="button"
                class="knop-secundair"
                :disabled="bezigMet === oefening.id"
                @click="afvinken(oefening, false)"
              >
                Ongedaan maken <span class="sr-only">{{ oefening.naam }}</span>
              </button>
              <router-link
                :to="{ name: 'exercises', hash: `#oefenplan-${oefening.id}` }"
                class="text-sm text-blue-700 underline hover:text-red-700"
              >
                Bekijken <span class="sr-only">{{ oefening.naam }}</span>
              </router-link>
            </div>
          </li>

          <li class="bg-white border border-gray-400 p-3 flex flex-col gap-2.5">
            <div>
              <p class="m-0 font-bold" :class="samenvatting.pijnVandaag > 0 ? 'text-gray-600 line-through' : ''">Pijn doorgeven</p>
              <p class="mt-0.5 mb-0 text-sm text-gray-700">Hoe voel je je vandaag? Kost minder dan een minuut.</p>
            </div>
            <div class="flex flex-wrap items-center gap-x-3.5 gap-y-2.5 border-t border-gray-200 pt-2.5">
              <GedaanVandaag v-if="samenvatting.pijnVandaag > 0" />
              <span v-else>Nog niet vandaag</span>
              <router-link to="/pain" class="text-sm text-blue-700 underline hover:text-red-700">Pijn registreren</router-link>
            </div>
          </li>

          <li class="bg-white border border-gray-400 p-3 flex flex-col gap-2.5">
            <div>
              <p class="m-0 font-bold" :class="samenvatting.logboekVandaag > 0 ? 'text-gray-600 line-through' : ''">Activiteit loggen</p>
              <p class="mt-0.5 mb-0 text-sm text-gray-700">Wat heb je vandaag gedaan in huis of daarbuiten?</p>
            </div>
            <div class="flex flex-wrap items-center gap-x-3.5 gap-y-2.5 border-t border-gray-200 pt-2.5">
              <GedaanVandaag v-if="samenvatting.logboekVandaag > 0" />
              <span v-else>Nog niet vandaag</span>
              <router-link to="/logbook" class="text-sm text-blue-700 underline hover:text-red-700">Naar logboek</router-link>
            </div>
          </li>
        </ul>
        <p v-if="!oefeningenVandaag.length" class="mt-3 mb-0 italic">Vandaag staan er geen oefeningen gepland.</p>
      </KaderSectie>

      <div class="flex-[1_1_280px] min-w-0 flex flex-col gap-6">
        <KaderSectie titel="Volgende afspraak" kop-id="afspraak-kop">
          <div v-if="samenvatting.volgendeAfspraakDatum" class="flex flex-col gap-1">
            <p class="m-0 font-['Times_New_Roman'] text-2xl font-bold">{{ formatDagDatum(samenvatting.volgendeAfspraakDatum) }}</p>
            <p class="m-0">{{ formatTijd(samenvatting.volgendeAfspraakDatum) }} · {{ samenvatting.volgendeAfspraakDuur }} minuten</p>
            <p class="m-0">{{ samenvatting.volgendeAfspraakType }} bij {{ samenvatting.volgendeAfspraakArts }}</p>
          </div>
          <p v-else class="m-0 italic">Geen afspraken gepland.</p>
          <router-link to="/appointments" class="inline-block mt-2 text-sm text-blue-700 underline hover:text-red-700">Alle afspraken</router-link>
        </KaderSectie>

        <KaderSectie titel="Recente pijnindicaties" kop-id="pijn-kop">
          <div class="flex flex-col gap-2.5">
            <svg
              v-if="trend"
              role="img"
              :aria-label="trend.label"
              width="100%" height="64" viewBox="0 0 240 64" preserveAspectRatio="none"
              class="block bg-white border border-gray-400"
            >
              <polyline :points="trend.punten" fill="none" stroke="#003366" stroke-width="2.5"
                stroke-linejoin="round" stroke-linecap="round" vector-effect="non-scaling-stroke" />
            </svg>
            <!-- Kopje maakt duidelijk dat dit de laatste metingen zijn, ook als ze ouder zijn dan 7 dagen -->
            <h3 v-if="recentPains.length" id="laatste-metingen-kop" class="m-0 text-sm font-bold">Laatste metingen</h3>
            <ul v-if="recentPains.length" aria-labelledby="laatste-metingen-kop" class="list-none m-0 p-0 flex flex-col gap-2">
              <li v-for="pijn in recentPains" :key="pijn.id" class="flex gap-2.5 items-baseline">
                <span class="shrink-0 font-bold">{{ pijn.pijnScore }}/10</span>
                <span class="text-sm">
                  <span class="text-gray-700">{{ formatKorteDatum(pijn.datumTijdRegistratie) }}</span>
                  <template v-if="pijn.toelichting"> · {{ pijn.toelichting }}</template>
                </span>
              </li>
            </ul>
            <p v-else class="m-0 italic">Je hebt nog geen pijn geregistreerd.</p>
            <p class="m-0 text-sm text-gray-700">
              <template v-if="samenvatting.aantalMetingen7Dagen > 0">
                Gemiddeld de afgelopen 7 dagen: <b>{{ samenvatting.gemiddeldePijn7Dagen }}/10</b>
                ({{ samenvatting.aantalMetingen7Dagen }} {{ samenvatting.aantalMetingen7Dagen === 1 ? 'meting' : 'metingen' }})
              </template>
              <span v-else class="italic">Je hebt de afgelopen 7 dagen geen pijn geregistreerd.</span>
            </p>
            <router-link to="/pain" class="self-start text-sm text-blue-700 underline hover:text-red-700">Pijnverloop bekijken</router-link>
          </div>
        </KaderSectie>
      </div>
    </div>
  </div>

  <div v-else>
    <h1 class="m-0 font-['Times_New_Roman'] text-3xl font-bold">
      <!-- De foutmelding zelf staat boven de pagina (App.vue) -->
      {{ loading ? 'Bezig met laden...' : 'Dashboard' }}
    </h1>
  </div>
</template>
