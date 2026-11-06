// https://on.cypress.io/api

describe('Rodor storefront', () => {
  it('loads the migrated home page', () => {
    cy.visit('/')
    cy.contains('Product search to verify authenticity')
    cy.contains('Categories')
  })
})
