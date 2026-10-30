<script setup>
// Melding in de pagina (vervangt alert()). Groen bij succes, rood bij een fout, navy bij informatie
// (bijv. 'je sessie is verlopen': geen fout van de gebruiker, dus geen rood).
// Beide live regions staan altijd in de DOM: een schermlezer leest een melding alleen betrouwbaar voor
// als de region al bestond vóórdat de tekst verscheen. Succes/info = beleefd (status), fout = direct (alert).
defineProps({
  type: { type: String, default: 'succes', validator: (t) => ['succes', 'info', 'fout'].includes(t) },
  tekst: { type: String, default: '' },
  // Alleen voorlezen, niet tonen: voor acties waarbij het scherm zelf al laat zien wat er veranderde
  alleenSchermlezer: { type: Boolean, default: false }
});

const emit = defineEmits(['sluiten']);
</script>

<template>
  <!-- Een lege region staat buiten de opmaak (absolute), zodat hij geen ruimte inneemt in een flex-gap -->
  <div role="status" class="empty:absolute" :class="{ 'sr-only': alleenSchermlezer }">
    <div
      v-if="type === 'succes' && tekst"
      class="flex items-start justify-between gap-3 border-2 border-green-800 bg-green-50 text-green-900 px-3 py-2 shadow-[2px_2px_0_#404040]"
    >
      <p><span aria-hidden="true">✔ </span>{{ tekst }}</p>
      <button v-if="!alleenSchermlezer" type="button" class="underline" @click="emit('sluiten')">
        Sluiten <span class="sr-only">melding</span>
      </button>
    </div>
    <div
      v-if="type === 'info' && tekst"
      class="flex items-start justify-between gap-3 border-2 border-[#000080] bg-blue-50 text-[#000080] px-3 py-2 shadow-[2px_2px_0_#404040]"
    >
      <p><span aria-hidden="true">ℹ </span>{{ tekst }}</p>
      <button type="button" class="underline" @click="emit('sluiten')">
        Sluiten <span class="sr-only">melding</span>
      </button>
    </div>
  </div>
  <div role="alert" class="empty:absolute">
    <div
      v-if="type === 'fout' && tekst"
      class="flex items-start justify-between gap-3 border-2 border-red-700 bg-red-50 text-red-800 px-3 py-2 shadow-[2px_2px_0_#404040]"
    >
      <p><span aria-hidden="true">✖ </span>{{ tekst }}</p>
      <button type="button" class="underline" @click="emit('sluiten')">
        Sluiten <span class="sr-only">foutmelding</span>
      </button>
    </div>
  </div>
</template>
