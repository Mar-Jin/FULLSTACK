<script setup>
import { Line } from 'vue-chartjs';
import 'chartjs-adapter-date-fns';
import { ref, watch } from 'vue';
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

const props = defineProps({
  painData: Array
});

const chartData = ref({
  labels: props.painData.map(d => d.datumTijdRegistratie),
  datasets: [
    {
      label: 'Pijnscore',
      data: props.painData.map(d => d.pijnScore),
      fill: false,
      borderColor: 'rgb(75, 192, 192)',
      tension: 0.3,
    }
  ]
});


const chartOptions = {
  responsive: true,
  plugins: {
    tooltip: {
      callbacks: {
        label(context) {
          const index = context.dataIndex;
          const toelichting = props.painData[index].toelichting || '';
          return `Pijnscore: ${context.parsed.y} (${toelichting})`;
        }
      }
    }
  },
  scales: {
    x: {
      type: 'time',
      time: {
        unit: 'day'
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
};
</script>

<template>
  <div>
    <Line :data="chartData" :options="chartOptions" />
  </div>
</template>
