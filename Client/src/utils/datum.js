// src/utils/datum.js
// Datums uit de API leesbaar maken in het Nederlands, bijv. "ma 9 november 2026, 16:30"
const datumTijd = new Intl.DateTimeFormat('nl-NL', {
  weekday: 'short', day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit'
});
const datum = new Intl.DateTimeFormat('nl-NL', { day: 'numeric', month: 'long', year: 'numeric' });
const dagDatum = new Intl.DateTimeFormat('nl-NL', { weekday: 'long', day: 'numeric', month: 'long' });
const volledigeDatum = new Intl.DateTimeFormat('nl-NL', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
const korteDatum = new Intl.DateTimeFormat('nl-NL', { weekday: 'short', day: 'numeric', month: 'short' });
const tijd = new Intl.DateTimeFormat('nl-NL', { hour: '2-digit', minute: '2-digit' });

export const formatDatumTijd = (waarde) => (waarde ? datumTijd.format(new Date(waarde)) : '');
export const formatDatum = (waarde) => (waarde ? datum.format(new Date(waarde)) : '');
// "maandag 9 november"
export const formatDagDatum = (waarde) => (waarde ? dagDatum.format(new Date(waarde)) : '');
// "donderdag 29 oktober 2026"
export const formatVolledigeDatum = (waarde) => (waarde ? volledigeDatum.format(new Date(waarde)) : '');
// "do 15 okt"
export const formatKorteDatum = (waarde) => (waarde ? korteDatum.format(new Date(waarde)) : '');
// "16:30"
export const formatTijd = (waarde) => (waarde ? tijd.format(new Date(waarde)) : '');

// Is deze datum vandaag (in de tijdzone van de gebruiker)?
export function isVandaag(waarde) {
  return new Date(waarde).toDateString() === new Date().toDateString();
}

// Label voor een dag in een lijst: "Vandaag", "Gisteren" of "ma 26 oktober"
export function dagLabel(waarde) {
  const dag = new Date(waarde);
  const gisteren = new Date();
  gisteren.setDate(gisteren.getDate() - 1);
  if (isVandaag(dag)) return 'Vandaag';
  if (dag.toDateString() === gisteren.toDateString()) return 'Gisteren';
  return formatDagDatum(dag);
}
