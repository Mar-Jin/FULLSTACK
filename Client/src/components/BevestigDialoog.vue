<script setup>
// Bevestigingsvenster (vervangt confirm()), gebouwd op het native <dialog>-element.
// showModal() regelt zelf de toegankelijkheid: focus blijft in het venster, Escape sluit het,
// de rest van de pagina is niet bedienbaar, en na het sluiten gaat de focus terug naar de knop die het opende.
import { ref, watch, nextTick } from 'vue';

const props = defineProps({
  open: { type: Boolean, required: true },
  titel: { type: String, required: true },
  bevestigTekst: { type: String, default: 'Bevestigen' }
});

const emit = defineEmits(['bevestig', 'annuleer']);

const dialoog = ref(null);
const annuleerKnop = ref(null);

watch(() => props.open, async (open) => {
  if (open) {
    dialoog.value.showModal();
    await nextTick();
    // Veilige keuze als startpunt: Enter verwijdert dan niet per ongeluk iets
    annuleerKnop.value.focus();
  } else if (dialoog.value?.open) {
    dialoog.value.close();
  }
});
</script>

<template>
  <!-- @cancel = Escape-toets -->
  <dialog
    ref="dialoog"
    aria-labelledby="bevestigTitel"
    aria-describedby="bevestigTekst"
    class="m-auto max-w-md border-2 border-gray-500 bg-white p-4 shadow-[4px_4px_0_#404040] font-['Verdana'] text-[#000080] backdrop:bg-black/40"
    @cancel.prevent="emit('annuleer')"
  >
    <h2 id="bevestigTitel" class="text-xl font-bold mb-2 font-['Times_New_Roman']">{{ titel }}</h2>
    <div id="bevestigTekst" class="mb-4">
      <slot />
    </div>
    <div class="flex justify-end gap-2">
      <button ref="annuleerKnop" type="button" class="border border-gray-500 px-3 py-1 rounded" @click="emit('annuleer')">
        Annuleren
      </button>
      <button type="button" class="bg-red-700 hover:bg-red-800 text-white px-3 py-1 rounded" @click="emit('bevestig')">
        {{ bevestigTekst }}
      </button>
    </div>
  </dialog>
</template>
