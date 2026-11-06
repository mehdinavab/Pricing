<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
const root = ref(null)
let slider
onMounted(() => {
 const el = root.value
 if (typeof window.Swiper !== 'function') return
 const d = el.dataset
 const group = el.parentElement
 slider = new window.Swiper(el, {
  slidesPerView: Number(d.itemsMobile || 1), spaceBetween: Number(d.spaceMobile || d.space || 30),
  speed: Number(d.speed || 300), observer: true, observeParents: true,
  breakpoints: {
   768: { slidesPerView: Number(d.itemsTablet || d.itemsMobileExtra || d.items || 3), spaceBetween: Number(d.spaceMobileExtra || d.space || 30) },
   1025: { slidesPerView: Number(d.items || 3), spaceBetween: Number(d.space || 30) }
  },
  navigation: { nextEl: group.querySelector('.swiper-button-next'), prevEl: group.querySelector('.swiper-button-prev') }
 })
})
onBeforeUnmount(() => slider?.destroy(true, true))
</script>
<template><div ref="root"><slot /></div></template>
