import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { vereistLogin: true }
    },
    {
      path: '/exercises',
      name: 'exercises',
      component: () => import('../views/ExercisesView.vue'),
      meta: { vereistLogin: true }
    },
    {
      path: '/logbook',
      name: 'logbook',
      component: () => import('../views/LogbookView.vue'),
      meta: { vereistLogin: true }
    },
    {
      path: '/pain',
      name: 'pain',
      component: () => import('../views/PainView.vue'),
      meta: { vereistLogin: true }
    },
    {
      path: '/appointments',
      name: 'appointments',
      component: () => import('../views/AppointmentsView.vue'),
      meta: { vereistLogin: true }
    }
  ],
})

// Niet ingelogd? Dan naar de loginpagina. Al ingelogd? Dan niet nog een keer de loginpagina tonen.
// Dit is alleen gebruiksgemak: de echte beveiliging zit in de API, die elk verzoek zelf controleert.
router.beforeEach((to) => {
  const authStore = useAuthStore()

  if (to.meta.vereistLogin && !authStore.isIngelogd) {
    return { name: 'login' }
  }
  if (to.name === 'login' && authStore.isIngelogd) {
    return { name: 'dashboard' }
  }
})

export default router
