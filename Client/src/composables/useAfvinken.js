// src/composables/useAfvinken.js
// Afvinken en ongedaan maken van een oefening, met melding. Gedeeld door het dashboard en 'Mijn oefeningen',
// zodat beide pagina's precies hetzelfde doen en dezelfde teksten tonen.
import { ref } from 'vue';
import { usePatientStore } from '@/stores/patientStore';

export function useAfvinken({ toonSucces, toonFout, sluit }) {
  const store = usePatientStore();
  // Id van de oefening waarvoor een verzoek loopt: die knoppen staan dan uit, zodat je niet dubbel afvinkt
  const bezigMet = ref(null);

  async function afvinken(oefening, isAfgevinkt) {
    bezigMet.value = oefening.id;
    sluit();
    try {
      await store.afvinken(oefening.id, isAfgevinkt);
      toonSucces(isAfgevinkt ? `${oefening.naam} afgevinkt.` : `Afvinken van ${oefening.naam} ongedaan gemaakt.`);
    } catch (err) {
      toonFout(err);
    } finally {
      bezigMet.value = null;
    }
  }

  return { bezigMet, afvinken };
}
