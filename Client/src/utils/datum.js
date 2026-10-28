// src/utils/datum.js
// Datums uit de API leesbaar maken in het Nederlands, bijv. "ma 9 november 2026, 16:30"
const datumTijd = new Intl.DateTimeFormat('nl-NL', {
  weekday: 'short', day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit'
});
const datum = new Intl.DateTimeFormat('nl-NL', { day: 'numeric', month: 'long', year: 'numeric' });

export const formatDatumTijd = (waarde) => (waarde ? datumTijd.format(new Date(waarde)) : '');
export const formatDatum = (waarde) => (waarde ? datum.format(new Date(waarde)) : '');
