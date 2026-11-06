<script setup>
const scrollToTop = () => window.scrollTo({ top: 0, behavior: 'smooth' })
import SiteHeader from './components/SiteHeader.vue'
import HomePage from './components/HomePage.vue'
import SiteFooter from './components/SiteFooter.vue'
</script>
<template><div class="wrap"><SiteHeader /><HomePage /><SiteFooter /><a class="scroll-progress-wrap scroll-top-style2 active-progress" href="#" aria-label="Back to top" @click.prevent="scrollToTop()"></a></div></template>
