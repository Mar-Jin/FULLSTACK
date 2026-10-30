<script setup>
// Tabbladen volgens het WAI-ARIA 'tabs'-patroon: met Tab kom je op het actieve tabblad,
// met pijltje links/rechts (en Home/End) wissel je van tabblad. Zo is het filter ook zonder muis te bedienen.
import { ref } from 'vue';

const props = defineProps({
  opties: { type: Array, required: true }, // [{ waarde: 'alle', label: 'Alle' }, ...]
  label: { type: String, required: true }
});

const model = defineModel({ type: String, required: true });
const knoppen = ref([]);

function kies(index) {
  model.value = props.opties[index].waarde;
  knoppen.value[index]?.focus();
}

function onKeydown(event, index) {
  const laatste = props.opties.length - 1;
  const volgende = {
    ArrowRight: index === laatste ? 0 : index + 1,
    ArrowLeft: index === 0 ? laatste : index - 1,
    Home: 0,
    End: laatste
  }[event.key];

  if (volgende !== undefined) {
    event.preventDefault();
    kies(volgende);
  }
}
</script>

<template>
  <div role="tablist" :aria-label="label" class="flex flex-wrap gap-2">
    <button
      v-for="(optie, index) in opties"
      :key="optie.waarde"
      :ref="(el) => (knoppen[index] = el)"
      type="button"
      role="tab"
      :aria-selected="model === optie.waarde"
      :tabindex="model === optie.waarde ? 0 : -1"
      class="min-h-11 border-2 border-gray-500 px-3.5 font-['Verdana'] text-sm"
      :class="model === optie.waarde ? 'bg-[#003366] text-white font-bold' : 'bg-white text-[#000080]'"
      @click="kies(index)"
      @keydown="onKeydown($event, index)"
    >
      {{ optie.label }}
    </button>
  </div>
</template>
