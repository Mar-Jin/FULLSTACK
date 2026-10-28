// src/stores/patientStore.js
import { defineStore } from 'pinia'
import api from '@/api'
import { useAuthStore } from '@/stores/authStore'

export const usePatientStore = defineStore('patient', {
  state: () => ({
    user: {},
    recentPains: [],
    painData: [],
    upcomingExercises: [],
    nextAppointment: null,
    exercises: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchData() {
      // Altijd de patiënt van de ingelogde gebruiker, nooit een vast ID
      const patientId = useAuthStore().patientId
      if (!patientId) return

      this.loading = true
      this.error = null
      try {
        const response = await api.get(`/patienten/${patientId}/dashboard`)

        this.user = response.data.user
        this.recentPains = response.data.recentPains
        this.painData = response.data.painData
        this.upcomingExercises = response.data.upcomingExercises
        this.nextAppointment = response.data.nextAppointment
        this.exercises = response.data.exercises
      } catch (err) {
        console.error(err)
        this.error = err.response?.data?.message || err.message || 'Fout bij het ophalen van data'
      } finally {
        this.loading = false
      }
    },

    async registreerPijn(pijnScore, toelichting) {
      const patientId = useAuthStore().patientId
      await api.post(`/patienten/${patientId}/pijnindicaties`, { pijnScore, toelichting })
      await this.fetchData()
    }
  }
});
