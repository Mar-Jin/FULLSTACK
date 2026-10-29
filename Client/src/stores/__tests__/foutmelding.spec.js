import { describe, it, expect } from 'vitest'
import { foutmelding } from '../patientStore'

// Zet een fout van de API om naar een melding voor de gebruiker
describe('foutmelding', () => {
  it('gebruikt de titel uit ProblemDetails (zoals FoutAfhandeling die teruggeeft)', () => {
    const fout = { response: { data: { title: 'Deze oefening is vandaag al vaak genoeg gedaan.' } } }

    expect(foutmelding(fout)).toBe('Deze oefening is vandaag al vaak genoeg gedaan.')
  })

  it('voegt validatiefouten van [ApiController] samen', () => {
    const fout = { response: { data: { title: 'One or more validation errors occurred.', errors: {
      PijnScore: ['De pijnscore moet tussen 0 en 10 liggen.'],
      Toelichting: ['De toelichting mag maximaal 500 tekens zijn.']
    } } } }

    expect(foutmelding(fout)).toBe('De pijnscore moet tussen 0 en 10 liggen. De toelichting mag maximaal 500 tekens zijn.')
  })

  it('valt terug op de standaardtekst als de server niets bruikbaars stuurt (bijv. geen verbinding)', () => {
    expect(foutmelding(new Error('Network Error'), 'Het logboek kon niet worden geladen.'))
      .toBe('Het logboek kon niet worden geladen.')
  })
})
