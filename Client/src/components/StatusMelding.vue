<script setup>
// Melding in de pagina (vervangt alert()). Groen bij succes, rood bij een fout.
// Beide live regions staan altijd in de DOM: een schermlezer leest een melding alleen betrouwbaar voor
// als de region al bestond vóórdat de tekst verscheen. Succes = beleefd (status), fout = direct (alert).
defineProps({
  type: { type: String, default: 'succes', validator: (t) => ['succes', 'fout'].includes(t) },
  tekst: { type: String, default: '' },
  // Alleen voorlezen, niet tonen: voor acties waarbij het scherm zelf al laat zien wat er veranderde
  alleenSchermlezer: { type: Boolean, default: false }
});

const emit = defineEmits(['sluiten']);
</script>

<template>
  <div role="status" :class="{ 'sr-only': alleenSchermlezer }">
    <div
      v-if="type === 'succes' && tekst"
      class="mb-4 flex items-start justify-between gap-3 border-2 border-green-800 bg-green-50 text-green-900 px-3 py-2 shadow-[2px_2px_0_#404040]"
    >
      <p><span aria-hidden="true">✔ </span>{{ tekst }}</p>
      <button v-if="!alleenSchermlezer" type="button" class="underline" @click="emit('sluiten')">
        Sluiten<span class="sr-only"> melding</span>
      </button>
    </div>
  </div>
  <div role="alert">
    <div
      v-if="type === 'fout' && tekst"
      class="mb-4 flex items-start justify-between gap-3 border-2 border-red-700 bg-red-50 text-red-800 px-3 py-2 shadow-[2px_2px_0_#404040]"
    >
      <p><span aria-hidden="true">✖ </span>{{ tekst }}</p>
      <button type="button" class="underline" @click="emit('sluiten')">
        Sluiten<span class="sr-only"> foutmelding</span>
      </button>
    </div>
  </div>
</template>
