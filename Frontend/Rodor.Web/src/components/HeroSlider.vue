<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
const slides = [{"title": "A special sparkle for special moments.", "text": "Each piece of our jewelry tells a story of elegance and grandeur.", "desktop": "https://rodorjewelry.com/wp-content/uploads/2025/09/Gemini_Generated_Image_w5er9iw5er9iw5er.png", "mobile": "https://rodorjewelry.com/wp-content/uploads/2025/08/IMG_20250729_184842_763.jpg"}, {"title": "Beauty is your choice.", "text": "Our jewelry is not just an accessory; it’s a reflection of your personality and taste.", "desktop": "https://rodorjewelry.com/wp-content/uploads/2025/09/rodor-s3.jpg", "mobile": "/Rodor_files/IMG_20250729_184905_136-min.jpg"}, {"title": "Strength and self-confidence lie in the details.", "text": "With jewelry designed to shine in a unique way.", "desktop": "https://rodorjewelry.com/wp-content/uploads/2025/09/rodor-s1.jpg", "mobile": "https://rodorjewelry.com/wp-content/uploads/2025/08/IMG_20250729_184845_904.jpg"}]
const current = ref(0)
const select = (index) => { current.value = (index + slides.length) % slides.length }
let timer
onMounted(() => { timer = setInterval(() => select(current.value + 1), 9000) })
onBeforeUnmount(() => clearInterval(timer))
</script>
<template>
 <section class="migration-hero" aria-roledescription="carousel" aria-label="Jewelry collections">
  <div v-for="(slide, index) in slides" :key="slide.title" v-show="index === current" class="migration-hero-slide" :style="{ '--desktop-image': `url('${slide.desktop}')`, '--mobile-image': `url('${slide.mobile}')` }">
   <div class="migration-hero-copy"><h2>{{ slide.title }}</h2><p>{{ slide.text }}</p></div>
  </div>
  <button class="migration-hero-prev" aria-label="Previous slide" @click="select(current - 1)"></button>
  <button class="migration-hero-next" aria-label="Next slide" @click="select(current + 1)"></button>
  <div class="migration-hero-dots"><button v-for="(slide, index) in slides" :key="slide.title" :aria-label="`Slide ${index + 1}`" :aria-current="current === index" :class="{ selected: current === index }" @click="select(index)"></button></div>
 </section>
</template>
