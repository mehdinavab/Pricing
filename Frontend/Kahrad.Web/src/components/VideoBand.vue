<script setup lang="ts">
defineProps({
  source: {
    type: String,
    required: true,
  },
})
</script>

<template>
  <video
    autoplay
    muted
    loop
    playsinline
    preload="metadata"
    aria-hidden="true"
    style="height: auto; vertical-align: middle; width: 100%"
  >
    <source :src="source" type="video/mp4" />
  </video>
</template>
