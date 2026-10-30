import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import StatusMelding from '../StatusMelding.vue'

describe('StatusMelding', () => {
  it('toont een succesmelding in de status-region (rustig voorgelezen)', () => {
    const wrapper = mount(StatusMelding, { props: { type: 'succes', tekst: 'Opgeslagen.' } })

    expect(wrapper.find('[role="status"]').text()).toContain('Opgeslagen.')
    expect(wrapper.find('[role="alert"]').text()).toBe('')
  })

  it('toont een fout in de alert-region (direct voorgelezen)', () => {
    const wrapper = mount(StatusMelding, { props: { type: 'fout', tekst: 'Er ging iets mis.' } })

    expect(wrapper.find('[role="alert"]').text()).toContain('Er ging iets mis.')
    expect(wrapper.find('[role="status"]').text()).toBe('')
  })

  it('toont een informatiemelding rustig in de status-region, niet als fout', () => {
    const wrapper = mount(StatusMelding, { props: { type: 'info', tekst: 'Je sessie is verlopen.' } })

    expect(wrapper.find('[role="status"]').text()).toContain('Je sessie is verlopen.')
    expect(wrapper.find('[role="alert"]').text()).toBe('')
  })

  it('beide live regions bestaan ook zonder melding (anders lezen schermlezers de eerste melding niet voor)', () => {
    const wrapper = mount(StatusMelding, { props: { tekst: '' } })

    expect(wrapper.find('[role="status"]').exists()).toBe(true)
    expect(wrapper.find('[role="alert"]').exists()).toBe(true)
  })

  it('de knop Sluiten geeft het event sluiten door', async () => {
    const wrapper = mount(StatusMelding, { props: { type: 'fout', tekst: 'Fout' } })

    await wrapper.find('button').trigger('click')

    expect(wrapper.emitted('sluiten')).toHaveLength(1)
  })
})
