// src/composables/useMelding.js
// Eén melding per pagina, gebruikt samen met <StatusMelding>. Zo werkt feedback op elke pagina hetzelfde.
import { reactive } from 'vue';
import { foutmelding } from '@/stores/patientStore';

export function useMelding() {
  const melding = reactive({ type: 'succes', tekst: '' });

  function toonSucces(tekst) {
    melding.type = 'succes';
    melding.tekst = tekst;
  }

  // Accepteert een tekst of een API-fout (dan wordt de melding van de server gebruikt)
  function toonFout(foutOfTekst, standaard) {
    melding.type = 'fout';
    melding.tekst = typeof foutOfTekst === 'string' ? foutOfTekst : foutmelding(foutOfTekst, standaard);
  }

  function sluit() {
    melding.tekst = '';
  }

  return { melding, toonSucces, toonFout, sluit };
}
