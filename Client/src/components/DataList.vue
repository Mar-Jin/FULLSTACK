<script setup>
// Generieke lijst: de parent bepaalt via de slot #item hoe één item eruitziet,
// deze component regelt de gedeelde dingen (laden, lege lijst, opmaak, toegankelijkheid).
// Gebruikt voor oefeningen, logboek en afspraken.
defineProps({
  items: { type: Array, required: true },
  itemKey: { type: String, default: 'id' },
  label: { type: String, required: true },
  leegTekst: { type: String, default: 'Er is nog niets om te tonen.' },
  laden: { type: Boolean, default: false },
  // Optioneel: extra opmaak per item, bijv. een lichtere kaart voor eerdere afspraken
  itemKlasse: { type: Function, default: () => 'border-gray-400 bg-gray-100' }
});
</script>

<template>
  <p v-if="laden" class="italic" role="status">Bezig met laden...</p>
  <p v-else-if="items.length === 0" class="italic">{{ leegTekst }}</p>
  <ul v-else :aria-label="label" class="list-none p-0">
    <li
      v-for="item in items"
      :key="item[itemKey]"
      class="mb-3 border p-3"
      :class="itemKlasse(item)"
    >
      <slot name="item" :item="item" />
    </li>
  </ul>
</template>
