<script setup>
import { Line } from 'vue-chartjs';
import 'chartjs-adapter-date-fns';
import { computed } from 'vue';
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

// computed: de grafiek werkt zichzelf bij zodra de data binnenkomt of er een nieuwe registratie bij komt
const chartData = computed(() => ({
  labels: props.punten.map(p => p.datum),
  datasets: [
    {
      label: props.label,
      data: props.punten.map(p => p.score),
      fill: false,
      borderColor: 'rgb(75, 192, 192)',
      tension: 0.3,
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
  <div>
    <Line :data="chartData" :options="chartOptions" />
  </div>
</template>
