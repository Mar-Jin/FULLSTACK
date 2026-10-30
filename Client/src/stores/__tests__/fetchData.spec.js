import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import api from '@/api'
import { usePatientStore } from '../patientStore'
import { useAuthStore } from '../authStore'

// De echte axios-instantie (en de router die api.js importeert) is niet nodig: we bepalen zelf wat de API teruggeeft
vi.mock('@/api', () => ({ default: { get: vi.fn() } }))

describe('fetchData', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    useAuthStore().user = { patientId: 229 }
    vi.mocked(api.get).mockReset()
  })

  it('geeft een duidelijke foutmelding als de gegevens niet geladen kunnen worden (bijv. API onbereikbaar)', async () => {
    vi.mocked(api.get).mockRejectedValue(new Error('Network Error'))
    const store = usePatientStore()

    await store.fetchData()

    expect(store.error).toBe('De gegevens konden niet worden geladen. Controleer je verbinding en probeer het opnieuw.')
    expect(store.loading).toBe(false)
    expect(store.oefenplannen).toEqual([])
  })

  it('wist een eerdere fout zodra het opnieuw proberen lukt', async () => {
    vi.mocked(api.get).mockRejectedValueOnce(new Error('Network Error'))
    const store = usePatientStore()
    await store.fetchData()

    vi.mocked(api.get).mockImplementation((url) =>
      Promise.resolve({
        data: url.endsWith('/dashboard')
          ? { user: { voornaam: 'Nout' }, recentPains: [], painData: [], samenvatting: {} }
          : [{ id: 1, naam: 'Calf Raises', isActief: true }]
      })
    )
    await store.fetchData()

    expect(store.error).toBeNull()
    expect(store.user.voornaam).toBe('Nout')
    expect(store.oefenplannen).toHaveLength(1)
  })
})
