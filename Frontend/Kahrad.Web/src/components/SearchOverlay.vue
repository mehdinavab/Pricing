<script setup lang="ts">
import { ref } from 'vue'

defineProps({
  visible: {
    type: Boolean,
    required: true,
  },
})

defineEmits(['close'])

const searchText = ref('')
</script>

<template>
  <div
    class="war_search_top"
    :class="{ open: visible }"
    :inert="!visible"
    role="dialog"
    aria-label="Product search"
  >
    <button
      type="button"
      class="colse_search"
      aria-label="Close search"
      @click="$emit('close')"
    >
      <i aria-hidden="true">+</i>
    </button>
    <div class="container">
      <form class="box_input_search" role="search" @submit.prevent>
        <input
          id="s"
          v-model="searchText"
          type="text"
          name="s"
          placeholder="جستجو ..."
          aria-label="Search products"
        />
        <span class="icon_search" aria-hidden="true">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
            <g transform="translate(1.889 1.102)">
              <rect
                width="27.922"
                height="27.922"
                rx="13.226"
                fill="none"
                stroke="#222628"
                stroke-width="2.3"
              />
              <path
                d="M27.039,31.329a10.3,10.3,0,0,1-.85-1.843,1.617,1.617,0,0,1,2.053-1.8,10.3,10.3,0,0,1,1.716,1.082h0a4.446,4.446,0,0,1,.676.511,1.617,1.617,0,0,1,.122,2.059,4.466,4.466,0,0,1-.611.587h0a4.468,4.468,0,0,1-.661.529,1.617,1.617,0,0,1-2.026-.389,4.463,4.463,0,0,1-.419-.737Z"
                transform="translate(-2.939 -2.939)"
                fill="none"
                stroke="#222628"
                stroke-width="2.3"
              />
            </g>
          </svg>
        </span>
        <ul class="result_ajax_search_nirweb" aria-live="polite"></ul>
      </form>
    </div>
  </div>
</template>

<style scoped>
.war_search_top.open {
  display: block;
}

.colse_search {
  border: 0;
  padding: 0;
  background: transparent;
}
</style>
