<script setup>
// Blok met een blauwe kopbalk in de RevaliNow-stijl. De kop is een echte <h2>,
// en de sectie is daaraan gekoppeld (aria-labelledby), zodat schermlezers de blokken als kopjes zien.
defineProps({
  titel: { type: String, required: true },
  kopId: { type: String, required: true }
});
</script>

<template>
  <section :aria-labelledby="kopId" class="border border-gray-400 bg-gray-100">
    <div class="bg-[#003366] text-white px-3 py-1.5 border-b-2 border-gray-400">
      <h2 :id="kopId" class="m-0 font-['Times_New_Roman'] text-lg font-bold">{{ titel }}</h2>
    </div>
    <div class="p-3">
      <slot />
    </div>
  </section>
</template>
