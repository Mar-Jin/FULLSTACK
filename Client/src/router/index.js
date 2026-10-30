import { nextTick } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
      meta: { titel: 'Inloggen' }
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { vereistLogin: true, titel: 'Dashboard' }
    },
    {
      path: '/exercises',
      name: 'exercises',
      component: () => import('../views/ExercisesView.vue'),
      meta: { vereistLogin: true, titel: 'Mijn oefeningen' }
    },
    {
      path: '/logbook',
      name: 'logbook',
      component: () => import('../views/LogbookView.vue'),
      meta: { vereistLogin: true, titel: 'Mijn logboek' }
    },
    {
      path: '/pain',
      name: 'pain',
      component: () => import('../views/PainView.vue'),
      meta: { vereistLogin: true, titel: 'Mijn pijnindicaties' }
    },
    {
      path: '/appointments',
      name: 'appointments',
      component: () => import('../views/AppointmentsView.vue'),
      meta: { vereistLogin: true, titel: 'Mijn afspraken' }
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

// Toegankelijkheid bij het wisselen van pagina (in een single-page app gebeurt dat niet vanzelf):
// - de titel van het tabblad zegt waar je bent (WCAG 2.4.2)
// - de focus gaat naar de kop van de nieuwe pagina, zodat een schermlezer die voorleest
//   en wie met het toetsenbord werkt niet bovenaan de vorige pagina blijft staan
router.afterEach((to, from) => {
  document.title = to.meta.titel ? `${to.meta.titel} - RevaliNow` : 'RevaliNow'

  if (from.name === undefined) return // eerste keer laden: de browser regelt de focus zelf
  nextTick(() => {
    const kop = document.querySelector('#inhoud h1')
    if (kop) {
      kop.setAttribute('tabindex', '-1')
      kop.focus()
    }
  })
})

export default router
