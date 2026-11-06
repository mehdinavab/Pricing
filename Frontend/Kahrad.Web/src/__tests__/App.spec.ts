import { describe, it, expect } from 'vitest'

import { mount } from '@vue/test-utils'
import App from '../App.vue'

describe('App', () => {
  it('renders the Kahrad storefront', () => {
    const wrapper = mount(App)
    expect(wrapper.text()).toContain('Authenticate and Register Your Product')
    expect(wrapper.text()).toContain('Warranty')
    expect(wrapper.find('header').exists()).toBe(true)
    expect(wrapper.find('footer').exists()).toBe(true)
  })
})
