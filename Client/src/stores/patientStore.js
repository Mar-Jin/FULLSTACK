// src/stores/revaliStore.js
import { defineStore } from 'pinia'
import axios from 'axios'

export const usePatientStore = defineStore('patient', {
  state: () => ({
    user: {},
    recentPains: [],
    painData: [],
    upcomingExercises: [],
    nextAppointment: {},
    exercises: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchData() {
      try {
        const response = await axios.get("/api/patient/1/dashboard")

        this.user = response.data.user
        this.recentPains = response.data.recentPains
        this.painData = response.data.painData
        this.upcomingExercises = response.data.upcomingExercises
        this.nextAppointment = response.data.nextAppointment
        this.exercises = response.data.exercises
      } catch (err) {
        console.error(err)
        this.error = err.message || 'Fout bij het ophalen van data'
      } finally {
        this.loading = false
      }
    }
  }
});
