<script setup>
import { ref, computed } from 'vue';
import { usePatientStore } from '@/stores/patientStore';

const store = usePatientStore();

const statusFilter = ref('');

function toggleStatus(item) {
  // Placeholder for toggling logic, e.g. API call
  alert(`Toggle status voor oefening ${item.id}`);
}

const filteredExercises = computed(() => {
  if (!statusFilter.value) return store.exercises;
  return store.exercises.filter(item => 
      statusFilter.value === 'Afgerond' ? item.isAfgerond : !item.isAfgerond
  );
});

</script>

<template>
  <div class="w-4/5 mx-auto my-6 bg-white border-2 border-gray-500 shadow-[4px_4px_0_#404040] p-4 font-['Verdana'] text-[#000080] bg-[url('https://www.transparenttextures.com/patterns/grey-wash-wall.png')]">
    <h2 class="text-2xl font-bold mb-4 font-['Times_New_Roman']">Mijn Oefeningen</h2>

    <div class="mb-4">
      <label class="mr-2 font-['Times_New_Roman'] font-semibold">Filter op status:</label>
      <select v-model="statusFilter" class="border border-gray-400 rounded px-2 py-1 font-['Verdana']">
        <option value="">Alle</option>
        <option value="Nog te doen">Nog te doen</option>
        <option value="Afgerond">Afgerond</option>
      </select>
    </div>

    <ul class="list-square list-inside">
      <li v-for="item in filteredExercises" :key="item.id" class="mb-6 border border-gray-400 p-3 bg-gray-100 rounded">
        <h3 class="text-lg font-['Times_New_Roman'] font-bold text-[#900000]">{{ item.oefening.naam }}</h3>
        <p class="mb-2">{{ item.oefening.beschrijving }}</p>
        <p class="mb-2 font-semibold">Herhalingen: {{ item.herhalingen }} | Sets: {{ item.sets }} | {{ item.frequentiePerDag }}x per dag</p>

        <div class="flex items-center">
          <input type="checkbox" :id="'exercise-' + item.id" :checked="item.isAfgerond" @change="toggleStatus(item)" class="mr-2" />
          <label :for="'exercise-' + item.id" class="cursor-pointer select-none font-['Verdana'] text-[#000080]">
            {{ item.isAfgerond ? 'Afgerond' : 'Nog te doen' }}
          </label>
        </div>
      </li>
    </ul>
  </div>
</template>
