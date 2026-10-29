// src/stores/patientStore.js
// Eén store voor de gegevens van de ingelogde patiënt. Meerdere views gebruiken dezelfde state:
// vink je een oefening af op de oefenpagina, dan is dat direct zichtbaar op het dashboard.
import { defineStore } from 'pinia'
import api from '@/api'
import { useAuthStore } from '@/stores/authStore'

function basisUrl() {
  // Altijd de patiënt van de ingelogde gebruiker, nooit een vast ID
  return `/patienten/${useAuthStore().patientId}`
}

export const usePatientStore = defineStore('patient', {
  state: () => ({
    user: {},
    recentPains: [],
    painData: [],
    pijnPerWeek: [],
    samenvatting: {},
    oefenplannen: [],
    logboek: [],
    afspraken: [],
    loading: false,
    error: null
  }),

  getters: {
    // De oefeningen die vandaag gedaan moeten worden (dashboard)
    oefeningenVandaag: (state) => state.oefenplannen.filter((o) => o.isActief)
  },

  actions: {
    async fetchData() {
      if (!useAuthStore().patientId) return

      this.loading = true
      this.error = null
      try {
        const [dashboard, oefenplannen] = await Promise.all([
          api.get(`${basisUrl()}/dashboard`),
          api.get(`${basisUrl()}/oefenplannen`)
        ])

        this.user = dashboard.data.user
        this.recentPains = dashboard.data.recentPains
        this.painData = dashboard.data.painData
        this.samenvatting = dashboard.data.samenvatting
        this.oefenplannen = oefenplannen.data
      } catch (err) {
        console.error(err)
        this.error = foutmelding(err, 'Fout bij het ophalen van data')
      } finally {
        this.loading = false
      }
    },

    async registreerPijn(pijnScore, toelichting) {
      await api.post(`${basisUrl()}/pijnindicaties`, { pijnScore, toelichting })
      await Promise.all([this.fetchData(), this.fetchPijnPerWeek()])
    },

    async fetchPijnPerWeek() {
      const response = await api.get(`${basisUrl()}/pijnindicaties/per-week`)
      this.pijnPerWeek = response.data
    },

    // Afvinken (true) of ongedaan maken (false); de server geeft de bijgewerkte voortgang terug
    async afvinken(oefenplanId, isAfgevinkt) {
      const response = await api.post(`${basisUrl()}/oefenplannen/${oefenplanId}/afvinkingen`, { isAfgevinkt })
      const index = this.oefenplannen.findIndex((o) => o.id === oefenplanId)
      if (index !== -1) this.oefenplannen[index] = response.data
    },

    async fetchLogboek() {
      const response = await api.get(`${basisUrl()}/logboek`)
      this.logboek = response.data
    },

    async logboekToevoegen(beschrijving) {
      const response = await api.post(`${basisUrl()}/logboek`, { beschrijving })
      this.logboek.unshift(response.data)
    },

    async logboekBewerken(id, beschrijving) {
      const response = await api.put(`${basisUrl()}/logboek/${id}`, { beschrijving })
      const index = this.logboek.findIndex((l) => l.id === id)
      if (index !== -1) this.logboek[index] = response.data
    },

    async logboekVerwijderen(id) {
      await api.delete(`${basisUrl()}/logboek/${id}`)
      this.logboek = this.logboek.filter((l) => l.id !== id)
    },

    async fetchAfspraken() {
      const response = await api.get(`${basisUrl()}/afspraken`)
      this.afspraken = response.data
    }
  }
});

// Haalt de nette foutmelding uit een API-fout (ProblemDetails of validatiefouten)
export function foutmelding(err, standaard = 'Er ging iets mis.') {
  const data = err.response?.data
  if (data?.errors) return Object.values(data.errors).flat().join(' ')
  return data?.title || data?.message || standaard
}
