<script setup lang="ts">
defineProps({
  visible: {
    type: Boolean,
    required: true,
  },
})

defineEmits(['close'])

const menuLinks = [
  { label: 'Home', href: 'https://kahrad.com/' },
  { label: 'about us', href: 'https://kahrad.com/about/' },
  { label: 'contact', href: 'https://kahrad.com/contact/' },
  { label: 'join us', href: 'https://kahrad.com/join-us/' },
  { label: 'Team Kahrad', href: 'https://kahrad.com/team/' },
  { label: 'Production process', href: 'https://kahrad.com/productionprocess/' },
  { label: 'FAQs', href: 'https://kahrad.com/faq/' },
]
</script>

<template>
  <div
    class="war_hamburger_top"
    :class="{ open: visible }"
    :inert="!visible"
  >
    <button
      type="button"
      class="close_hamburger"
      aria-label="Close menu"
      @click="$emit('close')"
    >
      <i aria-hidden="true">+</i>
    </button>
    <div class="container">
      <nav class="menu_items" aria-label="Main menu">
        <div class="custom-menu-class">
          <ul id="menu-header-en" class="menu">
            <li v-for="link in menuLinks" :key="link.label">
              <a :href="link.href" @click="$emit('close')">{{ link.label }}</a>
            </li>
          </ul>
        </div>
      </nav>
    </div>
  </div>
</template>

<style scoped>
.war_hamburger_top.open {
  display: block;
}

.close_hamburger {
  border: 0;
  padding: 0;
  background: transparent;
}
</style>
