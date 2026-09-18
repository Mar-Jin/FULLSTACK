import { createRouter, createWebHistory } from 'vue-router'

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
      component: () => import('../views/DashboardView.vue')
    },
    {
      path: '/exercises',
      name: 'exercises',
      component: () => import('../views/ExercisesView.vue')
    },
    {
      path: '/logbook',
      name: 'logbook',
      component: () => import('../views/LogbookView.vue')
    },
    {
      path: '/pain',
      name: 'pain',
      component: () => import('../views/PainView.vue')
    },
    {
      path: '/appointments',
      name: 'appointments',
      component: () => import('../views/AppointmentsView.vue')
    }
  ],
})

export default router
