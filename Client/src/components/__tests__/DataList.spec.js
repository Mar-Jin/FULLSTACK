import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import DataList from '../DataList.vue'

describe('DataList', () => {
  it('toont elk item via de slot van de parent', () => {
    const wrapper = mount(DataList, {
      props: { items: [{ id: 1, naam: 'Kniebuiging' }, { id: 2, naam: 'Plank' }], label: 'Oefeningen' },
      slots: { item: '<template #item="{ item }"><span class="naam">{{ item.naam }}</span></template>' }
    })

    expect(wrapper.findAll('li')).toHaveLength(2)
    expect(wrapper.findAll('.naam').map((n) => n.text())).toEqual(['Kniebuiging', 'Plank'])
    expect(wrapper.find('ul').attributes('aria-label')).toBe('Oefeningen')
  })

  it('toont de leeg-tekst als er geen items zijn', () => {
    const wrapper = mount(DataList, {
      props: { items: [], label: 'Logboek', leegTekst: 'Je logboek is nog leeg.' }
    })

    expect(wrapper.text()).toBe('Je logboek is nog leeg.')
    expect(wrapper.find('ul').exists()).toBe(false)
  })

  it('toont een laadmelding voor schermlezers tijdens het laden', () => {
    const wrapper = mount(DataList, { props: { items: [], label: 'Afspraken', laden: true } })

    expect(wrapper.find('[role="status"]').text()).toBe('Bezig met laden...')
  })
})
