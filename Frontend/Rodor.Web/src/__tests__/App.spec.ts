import { describe, it, expect } from 'vitest'

import { mount } from '@vue/test-utils'
import App from '../App.vue'

describe('App', () => {
  it('renders the Rodor storefront', () => {
    const wrapper = mount(App)
    expect(wrapper.text()).toContain('Product search to verify authenticity')
    expect(wrapper.text()).toContain('Categories')
    expect(wrapper.find('header, #header').exists()).toBe(true)
    expect(wrapper.find('footer, #footer').exists()).toBe(true)
  })
})
