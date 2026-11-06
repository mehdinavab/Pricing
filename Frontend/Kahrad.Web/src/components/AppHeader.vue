<script setup lang="ts">
import { ref } from 'vue'
import MenuOverlay from './MenuOverlay.vue'
import SearchOverlay from './SearchOverlay.vue'

const menuOpen = ref(false)
const searchOpen = ref(false)
const languagesOpen = ref(false)
const menuButton = ref<HTMLButtonElement | null>(null)
const searchButton = ref<HTMLButtonElement | null>(null)

const languages = [
  { label: 'العربية', href: 'https://kahrad.com/ar/', flag: '/Kahrad_files/ar.svg', alt: 'العربية' },
  { label: 'English', href: 'https://kahrad.com/', flag: '/Kahrad_files/en.svg', alt: 'English' },
  { label: 'فارسی', href: 'https://kahrad.com/fa/', flag: '/Kahrad_files/fa.svg', alt: 'فارسی' },
]

function closeMenu() {
  menuButton.value?.focus()
  menuOpen.value = false
}

function closeSearch() {
  searchButton.value?.focus()
  searchOpen.value = false
}
</script>

<template>
  <header>
    <div class="kahrad_header">
      <div class="container">
        <div class="row">
          <div class="kahrad_header_row d-flex align-items-center justify-content-between">
            <div class="kahrad_header_row_icons d-flex align-items-center justify-content-between">
              <button
                ref="menuButton"
                type="button"
                class="header-control"
                aria-label="Open menu"
                :aria-expanded="menuOpen"
                @click="menuOpen = true"
              >
                <img src="/Kahrad_files/menu.png" class="hamburger_icon" alt="" />
              </button>
              <button
                ref="searchButton"
                type="button"
                class="header-control"
                aria-label="Open search"
                :aria-expanded="searchOpen"
                @click="searchOpen = true"
              >
                <img class="nirwebIcon search_icon" src="/Kahrad_files/search.png" alt="" />
              </button>
            </div>
            <a href="https://kahrad.com/" class="icon" aria-label="Kahrad home">
              <img src="/Kahrad_files/Group-7.png" alt="Kahrad" />
            </a>
            <div class="kahrad_header_row_change_language_div">
              <button
                type="button"
                class="kahrad_header_row_change_language d-flex align-items-center"
                :class="{ active: languagesOpen }"
                :aria-expanded="languagesOpen"
                aria-controls="kahrad-languages"
                @click="languagesOpen = !languagesOpen"
              >
                <img src="/Kahrad_files/language.png" alt="" />
                <span>Language</span>
              </button>
              <div
                id="kahrad-languages"
                class="kahrad_header_languages"
                :class="{ open: languagesOpen }"
              >
                <a v-for="language in languages" :key="language.label" :href="language.href">
                  <img :src="language.flag" :alt="language.alt" />
                  {{ language.label }}
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </header>

  <MenuOverlay :visible="menuOpen" @close="closeMenu" />
  <SearchOverlay :visible="searchOpen" @close="closeSearch" />
</template>

<style scoped>
.header-control,
.kahrad_header_row_change_language {
  border: 0;
  padding: 0;
  background: transparent;
  color: inherit;
  font: inherit;
}

.header-control {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.kahrad_header_row_change_language {
  display: flex;
}

.kahrad_header_languages.open {
  display: block;
}
</style>
