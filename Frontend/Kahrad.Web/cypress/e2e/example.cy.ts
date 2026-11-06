// https://on.cypress.io/api

describe('Kahrad storefront', () => {
  it('loads the migrated home page', () => {
    cy.visit('/')
    cy.contains('Authenticate and Register Your Product')
    cy.contains('Warranty')
  })
})
