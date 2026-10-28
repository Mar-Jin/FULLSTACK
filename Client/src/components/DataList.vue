<script setup>
// Generieke lijst: de parent bepaalt via de slot #item hoe één item eruitziet,
// deze component regelt de gedeelde dingen (laden, lege lijst, opmaak, toegankelijkheid).
// Gebruikt voor oefeningen, logboek en afspraken.
defineProps({
  items: { type: Array, required: true },
  itemKey: { type: String, default: 'id' },
  label: { type: String, required: true },
  leegTekst: { type: String, default: 'Er is nog niets om te tonen.' },
  laden: { type: Boolean, default: false }
});
</script>

<template>
  <p v-if="laden" class="italic" role="status">Bezig met laden...</p>
  <p v-else-if="items.length === 0" class="italic">{{ leegTekst }}</p>
  <ul v-else :aria-label="label" class="list-none p-0">
    <li
      v-for="item in items"
      :key="item[itemKey]"
      class="mb-4 border border-gray-400 p-3 bg-gray-100 rounded"
    >
      <slot name="item" :item="item" />
    </li>
  </ul>
</template>
