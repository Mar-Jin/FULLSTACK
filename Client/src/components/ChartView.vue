<script setup>
import { Line } from 'vue-chartjs';
import { computed } from 'vue';
import { formatDatum } from '@/utils/datum';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  CategoryScale,
  LinearScale,
  PointElement,
  TimeScale,
} from 'chart.js';
import 'chartjs-adapter-date-fns';
import { nl } from 'date-fns/locale';

ChartJS.register(
  Title, Tooltip, Legend,
  LineElement, CategoryScale, LinearScale, PointElement, TimeScale
);

// Generieke pijngrafiek: de parent zet de data om naar punten { datum, score, toelichting },
// zodat dezelfde component werkt voor losse metingen en voor weekgemiddelden.
const props = defineProps({
  punten: { type: Array, default: () => [] },
  label: { type: String, default: 'Pijnscore' },
  eenheid: { type: String, default: 'day' } // tijdseenheid van de x-as: 'day' of 'week'
});

// Een <canvas> is onzichtbaar voor een schermlezer: daarom een korte samenvatting als beschrijving,
// en dezelfde gegevens als tabel om uit te klappen (WCAG 1.1.1, niet-tekstuele content)
const samenvatting = computed(() => {
  if (props.punten.length === 0) return 'Er zijn nog geen pijnscores om te tonen.';
  const scores = props.punten.map((p) => Number(p.score));
  // Op datum sorteren: metingen komen nieuwste-eerst binnen, weekgemiddelden oudste-eerst
  const opDatum = [...props.punten].sort((a, b) => new Date(a.datum) - new Date(b.datum));
  const oudste = opDatum[0];
  const nieuwste = opDatum[opDatum.length - 1];
  return `${props.label} van ${formatDatum(oudste.datum)} tot ${formatDatum(nieuwste.datum)}: `
    + `${props.punten.length} waarden, laagste ${Math.min(...scores)}, hoogste ${Math.max(...scores)}, `
    + `meest recent ${nieuwste.score}.`;
});

// computed: de grafiek werkt zichzelf bij zodra de data binnenkomt of er een nieuwe registratie bij komt
const chartData = computed(() => ({
  labels: props.punten.map(p => p.datum),
  datasets: [
    {
      label: props.label,
      data: props.punten.map(p => p.score),
      fill: false,
      borderColor: '#003366',
      backgroundColor: '#003366',
      // Rechte lijnen: een vloeiende curve zou tussen twee metingen waarden tonen die nooit gemeten zijn
      tension: 0,
    }
  ]
}));


const chartOptions = computed(() => ({
  responsive: true,
  plugins: {
    tooltip: {
      callbacks: {
        label(context) {
          const toelichting = props.punten[context.dataIndex]?.toelichting;
          return `${props.label}: ${context.parsed.y}${toelichting ? ` (${toelichting})` : ''}`;
        }
      }
    }
  },
  scales: {
    x: {
      type: 'time',
      // Nederlandse datums op de as ('1 okt.' in plaats van 'Oct 1')
      adapters: { date: { locale: nl } },
      time: {
        unit: props.eenheid
      },
      title: {
        display: true,
        text: 'Datum'
      }
    },
    y: {
      min: 0,
      max: 10,
      title: {
        display: true,
        text: 'Pijnscore'
      }
    }
  }
}));
</script>

<template>
  <figure>
    <div role="img" :aria-label="samenvatting">
      <Line :data="chartData" :options="chartOptions" aria-hidden="true" />
    </div>
    <figcaption class="mt-2 text-sm">{{ samenvatting }}</figcaption>
    <details v-if="punten.length" class="mt-2">
      <summary class="cursor-pointer underline text-blue-700 hover:text-red-700">Toon de gegevens als tabel</summary>
      <table class="mt-2 border-collapse text-sm bg-white">
        <caption class="sr-only">{{ label }} per datum</caption>
        <thead>
          <tr>
            <th scope="col" class="border border-gray-400 px-2 py-1 text-left">Datum</th>
            <th scope="col" class="border border-gray-400 px-2 py-1 text-left">{{ label }}</th>
            <th scope="col" class="border border-gray-400 px-2 py-1 text-left">Toelichting</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(punt, index) in punten" :key="index">
            <td class="border border-gray-400 px-2 py-1">{{ formatDatum(punt.datum) }}</td>
            <td class="border border-gray-400 px-2 py-1">{{ punt.score }}</td>
            <td class="border border-gray-400 px-2 py-1">{{ punt.toelichting }}</td>
          </tr>
        </tbody>
      </table>
    </details>
  </figure>
</template>
