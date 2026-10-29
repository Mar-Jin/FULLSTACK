import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import FilterTabs from '../FilterTabs.vue'

const opties = [
  { waarde: 'a', label: 'Eerste' },
  { waarde: 'b', label: 'Tweede' },
  { waarde: 'c', label: 'Derde' }
]

function maak(modelValue = 'a') {
  return mount(FilterTabs, {
    props: {
      opties,
      label: 'Testfilter',
      modelValue,
      'onUpdate:modelValue': (waarde) => wrapper.setProps({ modelValue: waarde })
    },
    attachTo: document.body
  })
}
let wrapper

describe('FilterTabs', () => {
  it('markeert alleen het actieve tabblad als geselecteerd en focusbaar', () => {
    wrapper = maak('b')
    const tabs = wrapper.findAll('[role="tab"]')

    expect(tabs.map((t) => t.attributes('aria-selected'))).toEqual(['false', 'true', 'false'])
    // Alleen het actieve tabblad zit in de Tab-volgorde; de rest bereik je met de pijltjes
    expect(tabs.map((t) => t.attributes('tabindex'))).toEqual(['-1', '0', '-1'])
  })

  it('pijltje rechts kiest het volgende tabblad en zet de focus erop', async () => {
    wrapper = maak('a')

    await wrapper.findAll('[role="tab"]')[0].trigger('keydown', { key: 'ArrowRight' })

    expect(wrapper.props('modelValue')).toBe('b')
    expect(document.activeElement.textContent.trim()).toBe('Tweede')
  })

  it('pijltje links op het eerste tabblad springt naar het laatste', async () => {
    wrapper = maak('a')

    await wrapper.findAll('[role="tab"]')[0].trigger('keydown', { key: 'ArrowLeft' })

    expect(wrapper.props('modelValue')).toBe('c')
  })

  it('Home en End springen naar het eerste en laatste tabblad', async () => {
    wrapper = maak('b')
    const tabs = wrapper.findAll('[role="tab"]')

    await tabs[1].trigger('keydown', { key: 'End' })
    expect(wrapper.props('modelValue')).toBe('c')

    await tabs[2].trigger('keydown', { key: 'Home' })
    expect(wrapper.props('modelValue')).toBe('a')
  })
})
