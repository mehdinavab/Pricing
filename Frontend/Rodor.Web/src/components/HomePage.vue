<script setup>
import { ref } from 'vue'
import LegacyCarousel from './LegacyCarousel.vue'
import HeroSlider from './HeroSlider.vue'
const activeTab = ref('e2a21c4148')
const formMessage = ref('')
async function submitAppointment(event) {
 const form = event.target
 if (!form.reportValidity()) return
 try {
  const response = await fetch('https://rodorjewelry.com/wp-json/contact-form-7/v1/contact-forms/12981/feedback', { method: 'POST', body: new FormData(form) })
  const result = await response.json()
  formMessage.value = result.message
  if (result.status === 'mail_sent') form.reset()
 } catch { formMessage.value = 'Unable to reach the appointment service. Please contact info@rodorjewelry.com.' }
}

</script>
<template>
<div class="main-elementor-template-theme">
<div class="post-1853 page type-page status-publish hentry" id="post-1853">
<div class="elementor elementor-1853" data-elementor-id="1853" data-elementor-type="wp-page">
<div class="elementor-element elementor-element-3679304 e-con-full box-banner-slider-home1 e-flex e-con e-parent" data-element_type="container" data-id="3679304">
<div class="elementor-element elementor-element-f5efda3 elementor-widget elementor-widget-slider_revolution" data-element_type="widget" data-id="f5efda3" data-widget_type="slider_revolution.default">
<div class="wp-block-themepunch-revslider">
<HeroSlider />

</div>
</div>
</div>
<div class="elementor-element elementor-element-bec54eb e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="bec54eb" data-settings='{"background_background":"classic"}'>
<div class="e-con-inner">
<div class="elementor-element elementor-element-3482435 elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="3482435" data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">Product search to verify authenticity and register your product</h2> </div>
</div>
<div class="elementor-element elementor-element-0ba3362 elementor-button-warning elementor-widget elementor-widget-button" data-element_type="widget" data-id="0ba3362" data-widget_type="button.default">
<a class="elementor-button elementor-button-link elementor-size-sm" href="https://rodorjewelry.com/search-product/">
<span class="elementor-button-content-wrapper">
<span class="elementor-button-text">Search Product</span>
</span>
</a>
</div>
</div>
</div>
<div class="elementor-element elementor-element-c64af35 e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="c64af35">
<div class="e-con-inner">
<div class="elementor-element elementor-element-f4f8474 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="f4f8474">
<div class="e-con-inner">
<div class="elementor-element elementor-element-80e7581 e-con-full e-flex e-con e-child" data-element_type="container" data-id="80e7581">
<div class="elementor-element elementor-element-38fec29 elementor-widget elementor-widget-bzotech-heading-global animated fadeInDown" data-element_type="widget" data-id="38fec29" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">Categories<img alt="svg" class="elbzotech-heading-global__icon" decoding="async" src="/Rodor_files/STAR.svg"/></h2> </div>
</div>
</div>
<div class="elementor-element elementor-element-3f18690 e-con-full e-flex e-con e-child" data-element_type="container" data-id="3f18690">
<div class="elementor-element elementor-element-fc90b7f elementor-widget elementor-widget-bzotech-slider_global animated fadeInRight" data-element_type="widget" data-id="fc90b7f" data-settings='{"_animation":"fadeInRight"}' data-widget_type="bzotech-slider_global.default">
<div class="elementor-widget-container">
<div class="elbzotech-wrapper-slider-global elbzotech-wrapper-slider-global-style2 display-swiper-navi-group display-swiper-pagination- display-swiper-scrollbar- auto-show-scrollbar- slider-type-">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container slider-wrap popup-gallery" data-auto="" data-center="" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="1" data-items-mobile-extra="" data-items-tablet="" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-slidertype="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed="3000"> <div aria-live="off" class="swiper-wrapper" id="swiper-wrapper-21032c1f5547d2420"> <div aria-label="1 / 5" class="swiper-slide wslider-item elementor-repeater-item-099f3c7 bzotech-active-swiper swiper-slide-active" role="group"><div class="item-slider-global-style2"><div class="item-slider-global-style2-inner"><div class="cate-img"><a class="adv-thumb-link-cate adv-thumb-link" href="https://rodorjewelry.com/product-category/rings/"><img alt="" class="attachment-full size-full" decoding="async" height="1080" loading="lazy" sizes="(max-width: 1080px) 100vw, 1080px" src="/Rodor_files/IMG_20250731_130242_442-min.jpg" width="1080"/></a></div>
<div class="info"><h2 class="info-title color-title"><a href="https://rodorjewelry.com/product-category/rings/">Rings</a></h2><p class="count_html">13 products</p></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div aria-label="2 / 5" class="swiper-slide wslider-item elementor-repeater-item-5744141 bzotech-active-swiper swiper-slide-next" role="group"><div class="item-slider-global-style2"><div class="item-slider-global-style2-inner"><div class="cate-img"><a class="adv-thumb-link-cate adv-thumb-link" href="https://rodorjewelry.com/product-category/rings/"><img alt="" class="attachment-full size-full" decoding="async" height="1080" loading="lazy" sizes="(max-width: 1080px) 100vw, 1080px" src="/Rodor_files/IMG_20250731_130242_475-min.jpg" width="1080"/></a></div>
<div class="info"><h2 class="info-title color-title"><a href="https://rodorjewelry.com/product-category/rings/">Rings</a></h2><p class="count_html">13 products</p></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div aria-label="3 / 5" class="swiper-slide wslider-item elementor-repeater-item-7f3f344 bzotech-active-swiper" role="group"><div class="item-slider-global-style2"><div class="item-slider-global-style2-inner"><div class="cate-img"><a class="adv-thumb-link-cate adv-thumb-link" href="https://rodorjewelry.com/product-category/rings/"><img alt="" class="attachment-full size-full" decoding="async" height="1080" loading="lazy" sizes="(max-width: 1080px) 100vw, 1080px" src="/Rodor_files/IMG_20250731_130242_333-min.jpg" width="1080"/></a></div>
<div class="info"><h2 class="info-title color-title"><a href="https://rodorjewelry.com/product-category/rings/">Rings</a></h2><p class="count_html">13 products</p></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div aria-label="4 / 5" class="swiper-slide wslider-item elementor-repeater-item-749f7be" role="group"><div class="item-slider-global-style2"><div class="item-slider-global-style2-inner"><div class="cate-img"><a class="adv-thumb-link-cate adv-thumb-link" href="https://rodorjewelry.com/product-category/rings/"><img alt="" class="attachment-full size-full" decoding="async" height="1080" loading="lazy" sizes="(max-width: 1080px) 100vw, 1080px" src="/Rodor_files/IMG_20250731_130242_628-min.jpg" width="1080"/></a></div>
<div class="info"><h2 class="info-title color-title"><a href="https://rodorjewelry.com/product-category/rings/">Jewelry set</a></h2><p class="count_html">13 products</p></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div aria-label="5 / 5" class="swiper-slide wslider-item elementor-repeater-item-a791158" role="group"><div class="item-slider-global-style2"><div class="item-slider-global-style2-inner"><div class="cate-img"><a class="adv-thumb-link-cate adv-thumb-link" href="https://rodorjewelry.com/product-category/rings/"><img alt="" class="attachment-full size-full" decoding="async" height="1080" loading="lazy" sizes="(max-width: 1080px) 100vw, 1080px" src="/Rodor_files/IMG_20250731_130242_638-min.jpg" width="1080"/></a></div>
<div class="info"><h2 class="info-title color-title"><a href="https://rodorjewelry.com/product-category/rings/">Couple jewelry</a></h2><p class="count_html">13 products</p></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
<div class="mouse-cursor text-css-e">aaaaa</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-9bea0da e-con-full e-flex e-con e-parent animated fadeIn" data-element_type="container" data-id="9bea0da" data-settings='{"background_background":"classic","animation":"fadeIn"}'>
<div class="elementor-element elementor-element-fdffe04 e-con-full e-flex e-con e-child" data-element_type="container" data-id="fdffe04" data-settings='{"background_background":"classic"}'>
</div>
<div class="elementor-element elementor-element-ee8722b e-con-full e-flex e-con e-child" data-element_type="container" data-id="ee8722b">
<div class="elementor-element elementor-element-fde3d97 elementor-widget__width-initial elementor-widget elementor-widget-bzotech_info_box_global" data-element_type="widget" data-id="fde3d97" data-widget_type="bzotech_info_box_global.default">
<div class="elementor-widget-container">
<div class="flex-wrapper container-flex-e bzoteche-info-box-global- item-info-box-global">
<div class="list-text-info flex-wrapper info-container-flex-e"> <h2 class="list-text-info__item elementor-repeater-item-28f3d4d">Modern And Minimalist Jewelry</h2><p class="list-text-info__item elementor-repeater-item-0df742c">Fashion is about dressing according to what's fashionable. Style is more about being yourself. Design is a constant challenge to balance comfort with luxe...</p></div>
</div> </div>
</div>
<div class="elementor-element elementor-element-b189330 elementor-widget elementor-widget-bzotech-button-global" data-element_type="widget" data-id="b189330" data-widget_type="bzotech-button-global.default">
<div class="elementor-widget-container">
<div class="button-wrap-global">
<a class="button-inner icon-on elbzotech-bt-global-default elementor-animation- btn-flex-e image_hover-" href="https://rodorjewelry.com/shop/" rel="nofollow">
<span class="text-button">
        	Read more    	</span>
<i class="icon-button-el las la-arrow-right"></i>
</a>
</div> </div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-88c75af e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="88c75af">
<div class="e-con-inner">
<div class="elementor-element elementor-element-3887648 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="3887648">
<div class="e-con-inner">
<div class="elementor-element elementor-element-ddd4d7f e-con-full e-flex e-con e-child animated fadeInLeft" data-element_type="container" data-id="ddd4d7f" data-settings='{"animation":"fadeInLeft"}'>
<div class="elementor-element elementor-element-2fc6260 elementor-widget elementor-widget-bzotech-heading-global animated fadeInDown" data-element_type="widget" data-id="2fc6260" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">Best Collection 2025</h2> </div>
</div>
</div>
<div class="elementor-element elementor-element-aca9a6a e-con-full e-flex e-con e-child animated fadeInRight" data-element_type="container" data-id="aca9a6a" data-settings='{"animation":"fadeInRight"}'>
<div class="elementor-element elementor-element-6b53992 elementor-widget elementor-widget-bzotech-products-global" data-element_type="widget" data-id="6b53992" data-widget_type="bzotech-products-global.default">
<div class="elementor-widget-container">
<div class="product-tab-wrap tab-wrap product-tab-style1">
<div class="product-tab-title">
<div class="inner">
<ul class="list-none nav nav-tabs" role="tablist">
<li :class="{ active: activeTab === 'e2a21c4148' }" class="tab-item-wrap">
<a :aria-selected="activeTab === 'e2a21c4148'" @click.prevent="activeTab = 'e2a21c4148'" aria-expanded="false" data-target="#e2a21c4148" data-toggle="tab" href="https://rodorjewelry.com/#e2a21c4148" role="tab">TOP RATING</a>
</li><li :class="{ active: activeTab === '5cd463a845' }" class="tab-item-wrap">
<a :aria-selected="activeTab === '5cd463a845'" @click.prevent="activeTab = '5cd463a845'" aria-expanded="false" data-target="#5cd463a845" data-toggle="tab" href="https://rodorjewelry.com/#5cd463a845" role="tab">FEATURED PRODUCTS</a>
</li><li :class="{ active: activeTab === '6963fda587' }" class="tab-item-wrap">
<a :aria-selected="activeTab === '6963fda587'" @click.prevent="activeTab = '6963fda587'" aria-expanded="false" data-target="#6963fda587" data-toggle="tab" href="https://rodorjewelry.com/#6963fda587" role="tab">BEST SELLERS</a>
</li>
</ul>
</div>
</div>
<div class="product-tab-content">
<div class="tab-content">
<div :class="{ active: activeTab === 'e2a21c4148' }" class="tab-pane" id="e2a21c4148" role="tabpanel" v-show="activeTab === 'e2a21c4148'"><div class="elbzotech-wrapper-slider elbzotech-wrapper-slider-product display-swiper-navi-group">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container" data-auto="" data-center="" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="1" data-items-mobile-extra="" data-items-tablet="2" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed=""> <div aria-live="polite" class="swiper-wrapper" id="swiper-wrapper-8393e3c2102172ef2"> <div aria-label="1 / 10" class="swiper-slide item-grid-product-style4 swiper-slide-active bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-10/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6139-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6139-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13075" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13075" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13075" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13075" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-10/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="2 / 10" class="swiper-slide item-grid-product-style4 swiper-slide-next bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-12/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151403_978-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151403_978-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="12657" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=12657" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="12657" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=12657" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-12/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="3 / 10" class="swiper-slide item-grid-product-style4 bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-11/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_536-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_536-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13026" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13026" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13026" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13026" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-11/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="4 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-9/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151428_080-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151428_080-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13087" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13087" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13087" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13087" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-9/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="5 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-8/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151329_902-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151329_902-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13092" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13092" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13092" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13092" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-8/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="6 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-7/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13097" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13097" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13097" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13097" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-7/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="7 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-2/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13176" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13176" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13176" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13176" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-2/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="8 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151323_915-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151323_915-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13177" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13177" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13177" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13177" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="9 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-6/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6138-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6138-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13098" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13098" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13098" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13098" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-6/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="10 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-5/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151354_970-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151354_970-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13101" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13101" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13101" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13101" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-5/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
</div></div><div :class="{ active: activeTab === '5cd463a845' }" class="tab-pane" id="5cd463a845" role="tabpanel" v-show="activeTab === '5cd463a845'"><div class="elbzotech-wrapper-slider elbzotech-wrapper-slider-product display-swiper-navi-group">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container" data-auto="" data-center="" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="1" data-items-mobile-extra="" data-items-tablet="2" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed=""> <div aria-live="polite" class="swiper-wrapper" id="swiper-wrapper-086dbdb1c629df20"> <div class="swiper-slide item-grid-product-style4 bzotech-active-swiper"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151323_915-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151323_915-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13177" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13177" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13177" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13177" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4 bzotech-active-swiper"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-2/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13176" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13176" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13176" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13176" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-2/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4 bzotech-active-swiper"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-4/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13102" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13102" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13102" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13102" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-4/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-7/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13097" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13097" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13097" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13097" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-7/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-13/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151334_458-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151334_458-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="12376" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=12376" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="12376" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=12376" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-13/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
</div></div><div :class="{ active: activeTab === '6963fda587' }" class="tab-pane" id="6963fda587" role="tabpanel" v-show="activeTab === '6963fda587'"><div class="elbzotech-wrapper-slider elbzotech-wrapper-slider-product display-swiper-navi-group">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container" data-auto="" data-center="" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="1" data-items-mobile-extra="" data-items-tablet="2" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed=""> <div aria-live="polite" class="swiper-wrapper" id="swiper-wrapper-833d780fff103ff94"> <div class="swiper-slide item-grid-product-style4 bzotech-active-swiper"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-12/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151403_978-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151403_978-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="12657" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=12657" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="12657" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=12657" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-12/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4 bzotech-active-swiper"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-5/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151354_970-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151354_970-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13101" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13101" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13101" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13101" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-5/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4 bzotech-active-swiper"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-11/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_536-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_536-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13026" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13026" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13026" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13026" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-11/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-2/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13176" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13176" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13176" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13176" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-2/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-4/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13102" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13102" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13102" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13102" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-4/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-3/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151358_538-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151358_538-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13175" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13175" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13175" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13175" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-3/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-9/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151428_080-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151428_080-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13087" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13087" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13087" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13087" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-9/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-10/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6139-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6139-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13075" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13075" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13075" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13075" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-10/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-8/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151329_902-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151329_902-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13092" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13092" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13092" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13092" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-8/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div class="swiper-slide item-grid-product-style4"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-7/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13097" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13097" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13097" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13097" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-7/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
</div></div>
</div>
</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-7f6aa14 e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="7f6aa14">
<div class="e-con-inner">
<div class="elementor-element elementor-element-41ed2eb e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="41ed2eb">
<div class="e-con-inner">
<div class="elementor-element elementor-element-7f8bd91 elementor-widget elementor-widget-bzotech-heading-global animated fadeInDown" data-element_type="widget" data-id="7f8bd91" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">Shop by Occasions<img alt="svg" class="elbzotech-heading-global__icon" decoding="async" src="/Rodor_files/STAR.svg"/></h2> </div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-6c79123 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="6c79123">
<div class="e-con-inner">
<div class="elementor-element elementor-element-8ea90a3 e-con-full e-flex e-con e-child" data-element_type="container" data-id="8ea90a3">
<div class="elementor-element elementor-element-ad0204a home1-bn-effect elementor-widget elementor-widget-bzotech-banner-info-global animated zoomIn" data-element_type="widget" data-id="ad0204a" data-settings='{"_animation":"zoomIn"}' data-widget_type="bzotech-banner-info-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-banner-info-global-wrap elbzotech-banner-info-global-style3">
<div class="elbzotech-banner-info-global-thumb">
<a href="https://rodorjewelry.com/shop/" rel="nofollow">
<img alt="" class="attachment-full size-full wp-image-19612" decoding="async" height="1280" loading="lazy" sizes="(max-width: 853px) 100vw, 853px" src="/Rodor_files/IMG_20250729_184948_350-min.jpg" width="853"/> </a>
</div>
<div class="info-banner text-center">
<h3 class="title-item">Birthday </h3> <div class="desc-item">Birthday gift for you, a meaningful and luxurious gift on the most meaningful day</div> <div><a class="elbzotech-bt-style3" href="https://rodorjewelry.com/shop/" rel="nofollow">SHOP NOW</a></div> </div>
</div> </div>
</div>
<div class="elementor-element elementor-element-45b4ed1 home1-bn-effect elementor-widget elementor-widget-bzotech-banner-info-global animated zoomIn" data-element_type="widget" data-id="45b4ed1" data-settings='{"_animation":"zoomIn"}' data-widget_type="bzotech-banner-info-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-banner-info-global-wrap elbzotech-banner-info-global-style3">
<div class="elbzotech-banner-info-global-thumb">
<a href="https://rodorjewelry.com/shop/" rel="nofollow">
<img alt="" class="attachment-full size-full wp-image-21299" decoding="async" height="1380" loading="lazy" sizes="(max-width: 1380px) 100vw, 1380px" src="/Rodor_files/143.jpg" width="1380"/> </a>
</div>
<div class="info-banner text-center">
<h3 class="title-item">Vacation Looks</h3> <div class="desc-item">Vivamus integer non suscipit taciti mus etiam at primis tempor sagittis sit</div> <div><a class="elbzotech-bt-style3" href="https://rodorjewelry.com/shop/" rel="nofollow">Shop Now</a></div> </div>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-5fe150c e-con-full e-flex e-con e-child" data-element_type="container" data-id="5fe150c">
<div class="elementor-element elementor-element-dc1bd14 home1-bn-effect elementor-widget elementor-widget-bzotech-banner-info-global animated zoomIn" data-element_type="widget" data-id="dc1bd14" data-settings='{"_animation":"zoomIn"}' data-widget_type="bzotech-banner-info-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-banner-info-global-wrap elbzotech-banner-info-global-style3">
<div class="elbzotech-banner-info-global-thumb">
<a href="https://rodorjewelry.com/shop/" rel="nofollow">
<img alt="" class="attachment-full size-full wp-image-21302" decoding="async" height="1380" loading="lazy" sizes="(max-width: 1380px) 100vw, 1380px" src="/Rodor_files/253.jpg" width="1380"/> </a>
</div>
<div class="info-banner text-center">
<h3 class="title-item">Weekend Staycation</h3> <div class="desc-item">Vivamus integer non suscipit taciti mus etiam at primis tempor sagittis sit</div> <div><a class="elbzotech-bt-style3" href="https://rodorjewelry.com/shop/" rel="nofollow">Shop Now</a></div> </div>
</div> </div>
</div>
<div class="elementor-element elementor-element-b89ac6f home1-bn-effect elementor-widget elementor-widget-bzotech-banner-info-global animated zoomIn" data-element_type="widget" data-id="b89ac6f" data-settings='{"_animation":"zoomIn"}' data-widget_type="bzotech-banner-info-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-banner-info-global-wrap elbzotech-banner-info-global-style3">
<div class="elbzotech-banner-info-global-thumb">
<a href="https://rodorjewelry.com/shop/" rel="nofollow">
<img alt="" class="attachment-full size-full wp-image-19614" decoding="async" height="1280" loading="lazy" sizes="(max-width: 853px) 100vw, 853px" src="/Rodor_files/IMG_20250729_184905_136-min.jpg" width="853"/> </a>
</div>
<div class="info-banner text-center">
<h3 class="title-item">Everyday Essentials</h3> <div class="desc-item">Vivamus integer non suscipit taciti mus etiam at primis tempor sagittis sit</div> <div><a class="elbzotech-bt-style3" href="https://rodorjewelry.com/shop/" rel="nofollow">Shop Now</a></div> </div>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-89cf877 e-con-full e-flex e-con e-child" data-element_type="container" data-id="89cf877">
<div class="elementor-element elementor-element-2b9c569 home1-bn-effect elementor-widget elementor-widget-bzotech-banner-info-global animated zoomIn" data-element_type="widget" data-id="2b9c569" data-settings='{"_animation":"zoomIn"}' data-widget_type="bzotech-banner-info-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-banner-info-global-wrap elbzotech-banner-info-global-style3">
<div class="elbzotech-banner-info-global-thumb">
<a href="https://rodorjewelry.com/shop/" rel="nofollow">
<img alt="" class="attachment-full size-full wp-image-19613" decoding="async" height="1280" loading="lazy" sizes="(max-width: 853px) 100vw, 853px" src="/Rodor_files/IMG_20250729_184927_653-min.jpg" width="853"/> </a>
</div>
<div class="info-banner text-center">
<h3 class="title-item">Appointment day</h3> <div class="desc-item">Vivamus integer non suscipit taciti mus etiam at primis tempor sagittis sit</div> <div><a class="elbzotech-bt-style3" href="https://rodorjewelry.com/shop/" rel="nofollow">Shop Now</a></div> </div>
</div> </div>
</div>
<div class="elementor-element elementor-element-2b0fed7 home1-bn-effect elementor-widget elementor-widget-bzotech-banner-info-global animated zoomIn" data-element_type="widget" data-id="2b0fed7" data-settings='{"_animation":"zoomIn"}' data-widget_type="bzotech-banner-info-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-banner-info-global-wrap elbzotech-banner-info-global-style3">
<div class="elbzotech-banner-info-global-thumb">
<a href="https://rodorjewelry.com/shop/" rel="nofollow">
<img alt="" class="attachment-full size-full wp-image-19610" decoding="async" height="1024" loading="lazy" sizes="(max-width: 1024px) 100vw, 1024px" src="/Rodor_files/IMG_20250731_161223_289-min.png" width="1024"/> </a>
</div>
<div class="info-banner text-center">
<h3 class="title-item">Wedding</h3> <div class="desc-item">Vivamus integer non suscipit taciti mus etiam at primis tempor sagittis sit</div> <div><a class="elbzotech-bt-style3" href="https://rodorjewelry.com/shop/" rel="nofollow">Shop Now</a></div> </div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-8ecf0d8 e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="8ecf0d8">
<div class="e-con-inner">
<div class="elementor-element elementor-element-0c0f6bb e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="0c0f6bb">
<div class="e-con-inner">
<div class="elementor-element elementor-element-0f4a5cc elementor-widget elementor-widget-bzotech-heading-global animated fadeInDown" data-element_type="widget" data-id="0f4a5cc" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">New Arrivals<img alt="svg" class="elbzotech-heading-global__icon" decoding="async" src="/Rodor_files/STAR.svg"/></h2> </div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-ecef6d4 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="ecef6d4">
<div class="e-con-inner">
<div class="elementor-element elementor-element-ad83b19 e-con-full e-flex e-con e-child animated fadeInLeft" data-element_type="container" data-id="ad83b19" data-settings='{"animation":"fadeInLeft"}'>
<div class="elementor-element elementor-element-540132e e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="540132e" data-settings='{"background_background":"classic"}'>
<div class="e-con-inner">
<div class="elementor-element elementor-element-18aff7b elementor-widget__width-initial elementor-widget elementor-widget-bzotech_info_box_global" data-element_type="widget" data-id="18aff7b" data-widget_type="bzotech_info_box_global.default">
<div class="elementor-widget-container">
<div class="flex-wrapper container-flex-e bzoteche-info-box-global- item-info-box-global">
<div class="list-text-info flex-wrapper info-container-flex-e"> <h2 class="list-text-info__item elementor-repeater-item-826f7c2">ring set</h2><p class="list-text-info__item elementor-repeater-item-8a151cd">Add our new arrivals to your weekly lineup.</p></div>
</div> </div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-7b95fc4 e-con-full e-flex e-con e-child animated fadeInRight" data-element_type="container" data-id="7b95fc4" data-settings='{"animation":"fadeInRight"}'>
<div class="elementor-element elementor-element-c8f7275 elementor-widget elementor-widget-bzotech-products-global" data-element_type="widget" data-id="c8f7275" data-widget_type="bzotech-products-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-wrapper-slider elbzotech-wrapper-slider-product display-swiper-navi-group">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container" data-auto="" data-center="" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="1" data-items-mobile-extra="2" data-items-tablet="" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed=""> <div aria-live="polite" class="swiper-wrapper" id="swiper-wrapper-7618d80104e8c50f3"> <div aria-label="1 / 10" class="swiper-slide item-grid-product-style4 swiper-slide-active bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-9/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151428_080-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151428_080-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13087" data-product_sku="TOY014-1-2" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-9/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13087" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13087" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13087" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13087" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-9/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="2 / 10" class="swiper-slide item-grid-product-style4 swiper-slide-next bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-8/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151329_902-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151329_902-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13092" data-product_sku="TOY014-1-2-1" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-8/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13092" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13092" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13092" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13092" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-8/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="3 / 10" class="swiper-slide item-grid-product-style4 bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-7/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13097" data-product_sku="TOY014-1-2-1-1" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-7/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13097" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13097" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13097" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13097" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-7/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="4 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-6/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6138-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6138-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13098" data-product_sku="TOY01443" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-6/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13098" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13098" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13098" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13098" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-6/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="5 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-5/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151354_970-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151354_970-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13101" data-product_sku="TOY014-1-2-1-4" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-5/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13101" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13101" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13101" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13101" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-5/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="6 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-4/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13102" data-product_sku="TOY0546" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-4/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13102" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13102" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13102" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13102" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-4/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="7 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-3/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151358_538-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151358_538-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13175" data-product_sku="TOY05543" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-3/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13175" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13175" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13175" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13175" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-3/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="8 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-2/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13176" data-product_sku="TOY0326" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-2/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13176" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13176" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13176" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13176" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-2/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="9 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-13/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151334_458-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151334_458-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="12376" data-product_sku="" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-13/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="12376" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=12376" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="12376" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=12376" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-13/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="10 / 10" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-11/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_536-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_536-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<input class="addtocart-ajax-nonce" name="addtocart-ajax-nonce" type="hidden" value="9ff2f4a290"/><a class="addcart-link case-cart-icon product_type_simple bzotech_ajax_add_to_cart product_type_simple" data-placement="top" data-product_id="13026" data-product_sku="TOY014-1" data-quantity="1" data-title="Read more" data-toggle="tooltip" href="https://rodorjewelry.com/product/ring-11/" rel="nofollow"><i class="las la-shopping-bag"></i></a>
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13026" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13026" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13026" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13026" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-11/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-919aa74 e-con-full home1-bn-full e-flex e-con e-parent animated fadeIn" data-element_type="container" data-id="919aa74" data-settings='{"background_background":"classic","animation":"fadeIn"}'>
<div class="elementor-element elementor-element-b153d62 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="b153d62">
<div class="e-con-inner">
<div class="elementor-element elementor-element-ece149d e-con-full e-flex e-con e-child" data-element_type="container" data-id="ece149d">
<div class="elementor-element elementor-element-fa2f82e elementor-widget elementor-widget-bzotech_info_box_global" data-element_type="widget" data-id="fa2f82e" data-widget_type="bzotech_info_box_global.default">
<div class="elementor-widget-container">
<div class="flex-wrapper container-flex-e bzoteche-info-box-global- item-info-box-global">
<div class="list-text-info flex-wrapper info-container-flex-e"> <h2 class="list-text-info__item elementor-repeater-item-1f50bcb">THE NEW</h2><h2 class="list-text-info__item elementor-repeater-item-8fa2671">COLLECTIONS</h2><p class="list-text-info__item elementor-repeater-item-abcd513">Look to our new season collection.</p></div>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-dca287d e-con-full e-flex e-con e-child" data-element_type="container" data-id="dca287d">
<div class="elementor-element elementor-element-c51ec33 elementor-widget elementor-widget-bzotech-banner-info-global" data-element_type="widget" data-id="c51ec33" data-widget_type="bzotech-banner-info-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-banner-info-global-wrap elbzotech-banner-info-global-elbzotech-bndf zoom-rotate elbzotech-hidden">
<div class="elbzotech-banner-info-global-thumb zoom-rotate elbzotech-hidden">
<a class="adv-thumb-link elementor-animation-" rel="nofollow">
<img alt="" class="attachment-full size-full wp-image-21923" decoding="async" height="1024" loading="lazy" sizes="(max-width: 1536px) 100vw, 1536px" src="/Rodor_files/ChatGPT-Image-Sep-27-2025-10_04_46-AM.png" width="1536"/> </a>
</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-d9a2a0b e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="d9a2a0b">
<div class="e-con-inner">
<div class="elementor-element elementor-element-76799ba e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="76799ba">
<div class="e-con-inner">
<div class="elementor-element elementor-element-7d4dffa elementor-widget elementor-widget-bzotech-heading-global animated fadeInDown" data-element_type="widget" data-id="7d4dffa" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">Shop By Trend<img alt="svg" class="elbzotech-heading-global__icon" decoding="async" src="/Rodor_files/STAR.svg"/></h2> </div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-8b93a50 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="8b93a50">
<div class="e-con-inner">
<div class="elementor-element elementor-element-74f4a86 e-con-full e-flex e-con e-child animated fadeInRight" data-element_type="container" data-id="74f4a86" data-settings='{"animation":"fadeInRight"}'>
<div class="elementor-element elementor-element-bd60212 elementor-widget elementor-widget-bzotech-products-global" data-element_type="widget" data-id="bd60212" data-widget_type="bzotech-products-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-wrapper-slider elbzotech-wrapper-slider-product display-swiper-navi-group">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container" data-auto="" data-center="" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="1" data-items-mobile-extra="2" data-items-tablet="" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed=""> <div aria-live="polite" class="swiper-wrapper" id="swiper-wrapper-cd228b619d3dbc0e"> <div aria-label="1 / 5" class="swiper-slide item-grid-product-style4 swiper-slide-active bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151323_915-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151323_915-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13177" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13177" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13177" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13177" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="2 / 5" class="swiper-slide item-grid-product-style4 swiper-slide-next bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-2/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_6140-min-300x300.png" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13176" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13176" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13176" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13176" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-2/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="3 / 5" class="swiper-slide item-grid-product-style4 bzotech-active-swiper" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-4/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_987-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13102" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13102" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13102" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13102" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-4/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="4 / 5" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-7/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151411_252-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="13097" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=13097" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="13097" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=13097" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-7/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> <div aria-label="5 / 5" class="swiper-slide item-grid-product-style4" role="group"> <div class="item-product"> <div class="product-thumb attribute-close product">

<a class="product-thumb-link rotate-thumb" href="https://rodorjewelry.com/product/ring-13/">
<img alt="" class="attachment-300x300 size-300x300 wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151334_458-min-300x300.jpg" width="300"/>
<img alt="" class="image-hover wp-post-image" decoding="async" height="300" loading="lazy" src="/Rodor_files/IMG_20250802_151334_458-min-300x300.jpg" width="300"/>
</a>
<div class="product-extra-link">
<a class="add_to_wishlist wishlist-link" data-placement="top" data-product-id="12376" data-product-title="ring" data-toggle="tooltip" href="https://rodorjewelry.com/?add_to_wishlist=12376" rel="nofollow" title=""><i class="la-heart lar"></i><span></span></a> <a class="product-compare compare compare-link" data-placement="top" data-product_id="12376" data-toggle="tooltip" href="https://rodorjewelry.com/?action=yith-woocompare-add-product&amp;id=12376" title=""><i class="las la-exchange-alt"></i><span></span></a> </div>
</div>
<div class="product-info">
<h3 class="title16 product-title font-semibold text-center">
<a class="color-title" href="https://rodorjewelry.com/product/ring-13/" title="ring">ring</a>
</h3>
<div class="product-price-rating js-product-attr flex-wrapper justify_content-center">
<div class="product-price price"></div> </div>
</div>
</div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-cfb3028 e-con-full elementor-hidden-desktop elementor-hidden-laptop elementor-hidden-tablet_extra elementor-hidden-tablet elementor-hidden-mobile_extra elementor-hidden-mobile e-flex  e-con e-parent" data-element_type="container" data-id="cfb3028" data-settings='{"background_background":"classic","animation":"fadeIn"}'>
<div class="elementor-element elementor-element-bad91a3 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="bad91a3">
<div class="e-con-inner">
<div class="elementor-element elementor-element-19b1fde e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="19b1fde">
<div class="e-con-inner">
<div class="elementor-element elementor-element-2bc042b  elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="2bc042b" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">Customer Reviews<img alt="svg" class="elbzotech-heading-global__icon" decoding="async" src="/Rodor_files/STAR.svg"/></h2> </div>
</div>
<div class="elementor-element elementor-element-30c70ad  elementor-widget elementor-widget-bzotech-slider_global" data-element_type="widget" data-id="30c70ad" data-settings='{"_animation":"fadeInUp"}' data-widget_type="bzotech-slider_global.default">
<div class="elementor-widget-container">
<div class="elbzotech-wrapper-slider-global elbzotech-wrapper-slider-global-style4 display-swiper-navi-group display-swiper-pagination- display-swiper-scrollbar- auto-show-scrollbar- slider-type-">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container slider-wrap popup-gallery" data-auto="" data-center="" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="1" data-items-mobile-extra="2" data-items-tablet="" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-slidertype="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed=""> <div aria-live="polite" class="swiper-wrapper" id="swiper-wrapper-02105a636102fe64a1"> <div class="swiper-slide wslider-item elementor-repeater-item-e5d33d9 bzotech-active-swiper"><div class="item-slider-global-style4"><div class="item-slider-global-style4-inner"><div class="image-wrap zoom-image elementor-animation-"><a class="img-wrap adv-thumb-link"><img alt="" class="attachment-full size-full wp-image-12933" decoding="async" height="150" loading="lazy" src="/Rodor_files/h1-client-2.jpg" width="150"/></a></div><div class="client-info"><h3 class="title18 font-semibold">Kathryn Murphy</h3><p class="desc title14">Designer</p><div class="product-rate"><div class="product-rating number-star_3"></div></div><div class="content-slider-custom client-say"><p>Excellent merchandise with a good 40 percent off, rare to see such lovely jewelry at such attractive prices. I had been looking for a mexican fire opal for a few years and they had a lovely range to choose from.</p></div></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div class="swiper-slide wslider-item elementor-repeater-item-aef9faf bzotech-active-swiper"><div class="item-slider-global-style4"><div class="item-slider-global-style4-inner"><div class="image-wrap zoom-image elementor-animation-"><a class="img-wrap adv-thumb-link"><img alt="" class="attachment-full size-full wp-image-12934" decoding="async" height="151" loading="lazy" src="/Rodor_files/h1-client-3.jpg" width="150"/></a></div><div class="client-info"><h3 class="title18 font-semibold">Jerome Bell</h3><p class="desc title14">Performer</p><div class="product-rate"><div class="product-rating number-star_5"></div></div><div class="content-slider-custom client-say"><p>Lorem Ipsum has been the industry’s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries</p></div></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div class="swiper-slide wslider-item elementor-repeater-item-5bd774c bzotech-active-swiper"><div class="item-slider-global-style4"><div class="item-slider-global-style4-inner"><div class="image-wrap zoom-image elementor-animation-"><a class="img-wrap adv-thumb-link"><img alt="" class="attachment-full size-full wp-image-12932" decoding="async" height="151" loading="lazy" src="/Rodor_files/h1-client-1.jpg" width="150"/></a></div><div class="client-info"><h3 class="title18 font-semibold">Bessie Cooper</h3><p class="desc title14">Manager</p><div class="product-rate"><div class="product-rating number-star_5"></div></div><div class="content-slider-custom client-say"><p>It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p></div></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div class="swiper-slide wslider-item elementor-repeater-item-907df50"><div class="item-slider-global-style4"><div class="item-slider-global-style4-inner"><div class="image-wrap zoom-image elementor-animation-"><a class="img-wrap adv-thumb-link"><img alt="" class="attachment-full size-full wp-image-12935" decoding="async" height="150" loading="lazy" src="/Rodor_files/h1-client-4.jpg" width="150"/></a></div><div class="client-info"><h3 class="title18 font-semibold">Eleanor Pena</h3><p class="desc title14">Doctor</p><div class="product-rate"><div class="product-rating number-star_5"></div></div><div class="content-slider-custom client-say"><p>Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock</p></div></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div class="swiper-slide wslider-item elementor-repeater-item-b6cf994"><div class="item-slider-global-style4"><div class="item-slider-global-style4-inner"><div class="image-wrap zoom-image elementor-animation-"><a class="img-wrap adv-thumb-link"><img alt="" class="attachment-full size-full wp-image-12942" decoding="async" height="150" loading="lazy" src="/Rodor_files/h1-client-5.jpg" width="150"/></a></div><div class="client-info"><h3 class="title18 font-semibold">Kristin Watson</h3><p class="desc title14">Teacher</p><div class="product-rate"><div class="product-rating number-star_5"></div></div><div class="content-slider-custom client-say"><p> Looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source</p></div></div></div><div class="star-icon"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-d555a81 e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="d555a81">
<div class="e-con-inner">
<div class="elementor-element elementor-element-e3062f9 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="e3062f9">
<div class="e-con-inner">
<div class="elementor-element elementor-element-9376e13 e-con-full e-flex e-con e-child animated fadeInDown" data-element_type="container" data-id="9376e13" data-settings='{"animation":"fadeInDown"}'>
<div class="elementor-element elementor-element-461ece1 elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="461ece1" data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">About Us<img alt="svg" class="elbzotech-heading-global__icon" decoding="async" src="/Rodor_files/STAR.svg"/></h2> </div>
</div>
<div class="elementor-element elementor-element-2f374a2 elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="2f374a2" data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">EXCELLENCE IN EVERY DETAILS</h2> </div>
</div>
<div class="elementor-element elementor-element-4ea8829 elementor-widget__width-initial elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="4ea8829" data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-text-editor-global text-css-e"><p>To achieve our goals, we assembled a team of individuals with many years of experience in jewelry design and craftsmanship and decided to create a new path that answers all questions—a path where mass production has no place and is replaced by unique, handcrafted pieces.<br/>Our belief has always been that the authenticity and quality of a piece begin from the very first step of its creation, and the condition for the permanence of an original <strong>Maik</strong> is non-negotiable.<br/>For this reason, from the skill of master craftsmen to the selection of high-quality, standard raw materials, as well as premium gemstones, everything had to be executed with meticulous precision.<br/>After completing all these steps, it is time for specialists in professional laboratories to issue a dedicated certificate for each product, ensuring not only the quality of the piece but also its authenticity, uniqueness, and ownership for our valued customers.<br/>In fact, each piece is not merely a ring or jewelry, but the result of years of experience, deep understanding of the jewelry world, and a passion for art.</p></div> </div>
</div>
<div class="elementor-element elementor-element-4d3240e elementor-widget elementor-widget-bzotech-button-global" data-element_type="widget" data-id="4d3240e" data-widget_type="bzotech-button-global.default">
<div class="elementor-widget-container">
<div class="button-wrap-global">
<a class="button-inner icon-off elbzotech-bt-global-default elementor-animation- btn-flex-e image_hover-" href="https://rodorjewelry.com/about-page/" rel="nofollow">
<span class="text-button">
        	Read more    	</span>
</a>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-65c4b3f e-con-full e-flex e-con e-child animated fadeInUp" data-element_type="container" data-id="65c4b3f" data-settings='{"animation":"fadeInUp"}'>
<div class="elementor-element elementor-element-6855ec1 elementor-widget elementor-widget-image" data-element_type="widget" data-id="6855ec1" data-widget_type="image.default">
<img alt="" class="attachment-large size-large wp-image-21417" decoding="async" height="640" loading="lazy" sizes="(max-width: 640px) 100vw, 640px" src="/Rodor_files/IMG_20250729_184934_824-min.jpg" width="640"/> </div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-3b6cc18 elementor-hidden-desktop elementor-hidden-laptop elementor-hidden-tablet_extra elementor-hidden-tablet elementor-hidden-mobile_extra elementor-hidden-mobile e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="3b6cc18">
<div class="e-con-inner">
<div class="elementor-element elementor-element-087d432 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="087d432">
<div class="e-con-inner">
<div class="elementor-element elementor-element-9b66d58 e-con-full elementor-hidden-mobile_extra elementor-hidden-mobile e-flex  e-con e-child" data-element_type="container" data-id="9b66d58" data-settings='{"animation":"fadeInLeft","background_background":"classic"}'>
<div class="elementor-element elementor-element-f9c5d78 e-con-full e-flex e-con e-child" data-element_type="container" data-id="f9c5d78">
<div class="elementor-element elementor-element-62d8a73 elementor-widget elementor-widget-image" data-element_type="widget" data-id="62d8a73" data-widget_type="image.default">
<img alt="" class="attachment-large size-large wp-image-21458" decoding="async" height="640" loading="lazy" sizes="(max-width: 640px) 100vw, 640px" src="/Rodor_files/21-min.jpg" width="640"/> </div>
</div>
</div>
<div class="elementor-element elementor-element-d6dfebc e-con-full e-flex  e-con e-child" data-element_type="container" data-id="d6dfebc" data-settings='{"animation":"fadeInRight"}'>
<div class="elementor-element elementor-element-baba9fc elementor-widget__width-initial elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="baba9fc" data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">MAKE AN APPOINTMENT</h2> </div>
</div>
<div class="elementor-element elementor-element-2e07843 elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="2e07843" data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-text-editor-global text-css-e"><p>Make an appointment to see the products in person at the store</p></div> </div>
</div>
<div class="elementor-element elementor-element-acdb09b box-contact-form elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="acdb09b" data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-text-editor-global text-css-e">
<div class="wpcf7 js" data-wpcf7-id="12981" dir="ltr" id="wpcf7-f12981-p1853-o1" lang="en-US">
<div class="screen-reader-response"><p aria-atomic="true" aria-live="polite" role="status"></p> <ul></ul></div>
<form @submit.prevent="submitAppointment" action="https://rodorjewelry.com/#wpcf7-f12981-p1853-o1" aria-label="Contact form" class="wpcf7-form init" data-status="init" method="post">
<fieldset class="hidden-fields-container"><input name="_wpcf7" type="hidden" value="12981"/><input name="_wpcf7_version" type="hidden" value="6.1.2"/><input name="_wpcf7_locale" type="hidden" value="en_US"/><input name="_wpcf7_unit_tag" type="hidden" value="wpcf7-f12981-p1853-o1"/><input name="_wpcf7_container_post" type="hidden" value="1853"/><input name="_wpcf7_posted_data_hash" type="hidden" value=""/>
</fieldset>
<div class="bzotech-row">
<div class="bzotech-col-sm-6 bzotech-col-sx-12">
<p><span class="wpcf7-form-control-wrap" data-name="your-name"><input aria-invalid="false" aria-required="true" autocomplete="name" class="wpcf7-form-control wpcf7-text wpcf7-validates-as-required" maxlength="400" name="your-name" placeholder="Your name*" required size="40" type="text" value=""/></span>
</p>
</div>
<div class="bzotech-col-sm-6 bzotech-col-sx-12">
<p><span class="wpcf7-form-control-wrap" data-name="your-email"><input aria-invalid="false" aria-required="true" autocomplete="email" class="wpcf7-form-control wpcf7-email wpcf7-validates-as-required wpcf7-text wpcf7-validates-as-email" maxlength="400" name="your-email" placeholder="Your email*" required size="40" type="email" value=""/></span>
</p>
</div>
<div class="bzotech-col-sm-6 bzotech-col-sx-12">
<p><span class="wpcf7-form-control-wrap" data-name="your-phone"><input aria-invalid="false" aria-required="true" class="wpcf7-form-control wpcf7-tel wpcf7-validates-as-required wpcf7-text wpcf7-validates-as-tel" maxlength="400" name="your-phone" placeholder="Phone number*" required size="40" type="tel" value=""/></span>
</p>
</div>
<div class="bzotech-col-sm-6 bzotech-col-sx-12">
<p><span class="wpcf7-form-control-wrap" data-name="your-subject"><input aria-invalid="false" class="wpcf7-form-control wpcf7-text" maxlength="400" name="your-subject" placeholder="Subject" size="40" type="text" value=""/></span>
</p>
</div>
</div>
<p><span class="wpcf7-form-control-wrap" data-name="your-message"><textarea aria-invalid="false" class="wpcf7-form-control wpcf7-textarea" cols="40" maxlength="2000" name="your-message" placeholder="Your message" rows="10"></textarea></span>
</p>
<div class="submit-appoint">
<p><input class="wpcf7-form-control wpcf7-submit has-spinner elbzotech-bt-default" type="submit" value="Submit"/><span class="wpcf7-spinner"></span>
</p>
</div><div aria-hidden="false" class="wpcf7-response-output" role="status" v-if="formMessage">{{ formMessage }}</div>
</form>
</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-e1d5857 e-con-full e-flex e-con e-parent animated fadeIn" data-element_type="container" data-id="e1d5857" data-settings='{"background_background":"classic","animation":"fadeIn"}'>
<div class="elementor-element elementor-element-9633a48 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="9633a48">
<div class="e-con-inner">
<div class="elementor-element elementor-element-d137867 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="d137867">
<div class="e-con-inner">
<div class="elementor-element elementor-element-8d37f6a elementor-widget elementor-widget-bzotech-heading-global animated fadeInDown" data-element_type="widget" data-id="8d37f6a" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">FOLLOW US ON INSTAGRAM<img alt="svg" class="elbzotech-heading-global__icon" decoding="async" src="/Rodor_files/STAR.svg"/></h2> </div>
</div>
<div class="elementor-element elementor-element-154ba24 elementor-widget elementor-widget-bzotech-instagram-global animated fadeInUp" data-element_type="widget" data-id="154ba24" data-settings='{"_animation":"fadeInUp"}' data-widget_type="bzotech-instagram-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-instagram elbzotech-wrapper-slider display-swiper-navi-group">
<LegacyCarousel class="elbzotech-swiper-slider swiper-container action-type-popup" data-column="1" data-items="3" data-items-custom="" data-items-laptop="" data-items-mobile="2" data-items-mobile-extra="" data-items-tablet="" data-items-tablet-extra="" data-items-widescreen="" data-loop="" data-navigation="group" data-pagination="" data-space="30" data-space-laptop="" data-space-mobile="" data-space-mobile-extra="20" data-space-tablet="" data-space-tablet-extra="" data-space-widescreen="" data-speed=""><div aria-live="polite" class="swiper-wrapper" id="swiper-wrapper-b43dfbe5ac7312a3"><div aria-label="1 / 4" class="swiper-slide elementor-repeater-item-878f338 swiper-slide-active bzotech-active-swiper" role="group"><div class="item-instagram item-instagram-global-" href="/Rodor_files/13.jpg"><div class="item-instagram-inner"><a class="img-wrap"><img alt="" class="attachment-full size-full wp-image-21152" decoding="async" height="1380" loading="lazy" sizes="(max-width: 1380px) 100vw, 1380px" src="/Rodor_files/13.jpg" width="1380"/><div class="instagram-text-follow"><i class="lab la-instagram title50"></i><span class="text">@giftxtore</span></div></a></div><div class="icon_star text-center"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div aria-label="2 / 4" class="swiper-slide elementor-repeater-item-5f12072 swiper-slide-next bzotech-active-swiper" role="group"><div class="item-instagram item-instagram-global-" href="/Rodor_files/14.jpg"><div class="item-instagram-inner"><a class="img-wrap"><img alt="" class="attachment-full size-full wp-image-21155" decoding="async" height="1380" loading="lazy" sizes="(max-width: 1380px) 100vw, 1380px" src="/Rodor_files/14.jpg" width="1380"/><div class="instagram-text-follow"><i class="lab la-instagram title50"></i><span class="text">@giftxtore</span></div></a></div><div class="icon_star text-center"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div aria-label="3 / 4" class="swiper-slide elementor-repeater-item-4be501b bzotech-active-swiper" role="group"><div class="item-instagram item-instagram-global-" href="/Rodor_files/13.jpg"><div class="item-instagram-inner"><a class="img-wrap"><img alt="" class="attachment-full size-full wp-image-21152" decoding="async" height="1380" loading="lazy" sizes="(max-width: 1380px) 100vw, 1380px" src="/Rodor_files/13.jpg" width="1380"/><div class="instagram-text-follow"><i class="lab la-instagram title50"></i><span class="text">@giftxtore</span></div></a></div><div class="icon_star text-center"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div><div aria-label="4 / 4" class="swiper-slide elementor-repeater-item-7326430" role="group"><div class="item-instagram item-instagram-global-" href="/Rodor_files/14.jpg"><div class="item-instagram-inner"><a class="img-wrap"><img alt="" class="attachment-full size-full wp-image-21155" decoding="async" height="1380" loading="lazy" sizes="(max-width: 1380px) 100vw, 1380px" src="/Rodor_files/14.jpg" width="1380"/><div class="instagram-text-follow"><i class="lab la-instagram title50"></i><span class="text">@giftxtore</span></div></a></div><div class="icon_star text-center"><svg fill="none" height="40" viewbox="0 0 40 40" width="40" xmlns="http://www.w3.org/2000/svg">
<g id="STAR">
<path d="M-1.96127e-08 19.9995C-3.0444e-08 20.2473 8.9543 20.4481 20 20.4481C31.0457 20.4481 40 20.2473 40 19.9995C40 19.7517 31.0457 19.5508 20 19.5508C8.9543 19.5508 -8.78138e-09 19.7517 -1.96127e-08 19.9995Z" fill="#2E2E2E" id="Vector"></path>
<path d="M19.5519 20C19.5519 31.0457 19.7528 40 20.0005 40C20.2483 40 20.4492 31.0457 20.4492 20C20.4492 8.95432 20.2483 -8.78067e-09 20.0005 -1.9612e-08C19.7528 -3.04433e-08 19.5519 8.95432 19.5519 20Z" fill="#2E2E2E" id="Vector_2"></path>
<path d="M20.2332 20.2327C14.5324 25.9334 9.80825 30.4517 9.68156 30.3251C9.55488 30.1984 14.0732 25.4742 19.7739 19.7734C25.4746 14.0727 30.1988 9.55438 30.3255 9.68107C30.4522 9.80775 25.9339 14.532 20.2332 20.2327Z" fill="#2E2E2E" id="Vector_3"></path>
<path d="M20.2332 19.7673C25.9339 25.4681 30.4522 30.1923 30.3255 30.3189C30.1988 30.4456 25.4746 25.9273 19.7739 20.2266C14.0732 14.5258 9.55488 9.80165 9.68156 9.67496C9.80825 9.54828 14.5324 14.0666 20.2332 19.7673Z" fill="#2E2E2E" id="Vector_4"></path>
<path d="M20.305 20.1204C17.3808 27.6369 14.8788 33.6702 14.7099 33.6068C14.541 33.5435 16.7738 27.3941 19.6927 19.8829C22.617 12.3664 25.119 6.33315 25.2879 6.39649C25.4568 6.45983 23.224 12.6092 20.305 20.1204Z" fill="#2E2E2E" id="Vector_5"></path>
<path d="M20.1225 19.693C27.639 22.6173 33.6722 25.1192 33.6089 25.2881C33.5455 25.4571 27.3962 23.2243 19.8849 20.3053C12.3684 17.381 6.3352 14.8791 6.39854 14.7102C6.46188 14.5412 12.6113 16.774 20.1225 19.693Z" fill="#2E2E2E" id="Vector_6"></path>
<path d="M20.137 20.3005C12.7947 23.6312 6.7826 26.2018 6.7087 26.0382C6.6348 25.8746 12.5255 23.0401 19.8678 19.7041C27.2101 16.3734 33.2223 13.8028 33.2962 13.9664C33.3701 14.13 27.4794 16.9645 20.137 20.3005Z" fill="#2E2E2E" id="Vector_7"></path>
<path d="M20.3018 19.863C23.6325 27.2053 26.2031 33.2174 26.0395 33.2913C25.8758 33.3652 23.0413 27.4745 19.7054 20.1322C16.3747 12.7899 13.8041 6.77771 13.9677 6.70381C14.1313 6.62991 16.9658 12.5206 20.3018 19.863Z" fill="#2E2E2E" id="Vector_8"></path>
</g>
</svg></div></div></div> </div>
</LegacyCarousel>
<div class="bzotech-swiper-navi">
<div aria-label="Next slide" class="swiper-button-nav swiper-button-next" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-right"></i></div>
<div aria-label="Previous slide" class="swiper-button-nav swiper-button-prev" role="button" tabindex="0"><i aria-hidden="true" class="las la-long-arrow-alt-left"></i></div>
</div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-de999c0 elementor-hidden-desktop elementor-hidden-laptop elementor-hidden-tablet_extra elementor-hidden-tablet elementor-hidden-mobile_extra elementor-hidden-mobile e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="de999c0">
<div class="e-con-inner">
<div class="elementor-element elementor-element-86e8bac e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="86e8bac">
<div class="e-con-inner">
<div class="elementor-element elementor-element-d0628b5  elementor-widget elementor-widget-bzotech-heading-global" data-element_type="widget" data-id="d0628b5" data-settings='{"_animation":"fadeInDown"}' data-widget_type="bzotech-heading-global.default">
<div class="elementor-widget-container">
<h2 class="container-flex-e elbzotech-heading-global font-title text-css-e font-medium text-uppercase title34 color-title elbzotech-heading-global-style1">LET’S GO SHOPPING</h2> </div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-fe7bc54 e-flex e-con-boxed  e-con e-child" data-element_type="container" data-id="fe7bc54" data-settings='{"animation":"fadeInLeft"}'>
<div class="e-con-inner">
<div class="elementor-element elementor-element-3185626 e-con-full e-flex e-con e-child" data-element_type="container" data-id="3185626">
<div class="elementor-element elementor-element-c6b6581 elementor-widget elementor-widget-bzotech_info_box_global" data-element_type="widget" data-id="c6b6581" data-widget_type="bzotech_info_box_global.default">
<div class="elementor-widget-container">
<div class="flex-wrapper container-flex-e bzoteche-info-box-global- item-info-box-global">
<div class="info-box-icon item-image-icon-e elementor-animation-"><a><img alt="" class="attachment-full size-full wp-image-12323" decoding="async" height="60" loading="lazy" src="/Rodor_files/h1-icon.png" width="60"/></a></div><div class="list-text-info flex-wrapper info-container-flex-e"> <h2 class="list-text-info__item elementor-repeater-item-8b2af2d">FREE SHIPPING</h2><p class="list-text-info__item elementor-repeater-item-f1c7bff">Capped At $105 Per Order</p><a class="list-text-info__item elementor-repeater-item-e2fc2b3" href="https://rodorjewelry.com/shop/">SHOP NOW</a></div>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-1deb17d e-con-full e-flex e-con e-child" data-element_type="container" data-id="1deb17d">
<div class="elementor-element elementor-element-2f0fd5e elementor-widget elementor-widget-bzotech_info_box_global" data-element_type="widget" data-id="2f0fd5e" data-widget_type="bzotech_info_box_global.default">
<div class="elementor-widget-container">
<div class="flex-wrapper container-flex-e bzoteche-info-box-global- item-info-box-global">
<div class="info-box-icon item-image-icon-e elementor-animation-"><a><img alt="" class="attachment-full size-full wp-image-13028" decoding="async" height="60" loading="lazy" src="/Rodor_files/h1-icon2.png" width="60"/></a></div><div class="list-text-info flex-wrapper info-container-flex-e"> <h2 class="list-text-info__item elementor-repeater-item-8b2af2d">FREE DISCOUNT CODE</h2><p class="list-text-info__item elementor-repeater-item-f1c7bff">Weekly Offer</p><a class="list-text-info__item elementor-repeater-item-e2fc2b3" href="https://rodorjewelry.com/shop/">SHOP NOW</a></div>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-9ec8515 e-con-full e-flex e-con e-child" data-element_type="container" data-id="9ec8515">
<div class="elementor-element elementor-element-c74e76e elementor-widget elementor-widget-bzotech_info_box_global" data-element_type="widget" data-id="c74e76e" data-widget_type="bzotech_info_box_global.default">
<div class="elementor-widget-container">
<div class="flex-wrapper container-flex-e bzoteche-info-box-global- item-info-box-global">
<div class="info-box-icon item-image-icon-e elementor-animation-"><a><img alt="" class="attachment-full size-full wp-image-13029" decoding="async" height="60" loading="lazy" src="/Rodor_files/h1-icon3-1.png" width="60"/></a></div><div class="list-text-info flex-wrapper info-container-flex-e"> <h2 class="list-text-info__item elementor-repeater-item-8b2af2d">CONVENIENT PAYMENT</h2><p class="list-text-info__item elementor-repeater-item-f1c7bff">Fast And Safe</p><a class="list-text-info__item elementor-repeater-item-e2fc2b3" href="https://rodorjewelry.com/shop/">SHOP NOW</a></div>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-25be34f e-con-full e-flex e-con e-child" data-element_type="container" data-id="25be34f">
<div class="elementor-element elementor-element-0ef0f9d elementor-widget elementor-widget-bzotech_info_box_global" data-element_type="widget" data-id="0ef0f9d" data-widget_type="bzotech_info_box_global.default">
<div class="elementor-widget-container">
<div class="flex-wrapper container-flex-e bzoteche-info-box-global- item-info-box-global">
<div class="info-box-icon item-image-icon-e elementor-animation-"><a><img alt="" class="attachment-full size-full wp-image-13030" decoding="async" height="60" loading="lazy" src="/Rodor_files/h1-icon4.png" width="60"/></a></div><div class="list-text-info flex-wrapper info-container-flex-e"> <h2 class="list-text-info__item elementor-repeater-item-8b2af2d">7-DAY RETURN</h2><p class="list-text-info__item elementor-repeater-item-f1c7bff">Shop With Confidence</p><a class="list-text-info__item elementor-repeater-item-e2fc2b3" href="https://rodorjewelry.com/shop/">SHOP NOW</a></div>
</div> </div>
</div>
</div>
</div>
</div>
</div>
</div>
</div>
</div>
</div>
</template>
