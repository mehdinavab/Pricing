<script setup>
import { ref } from 'vue'
const menuOpen = ref(false)
const languageOpen = ref(false)
</script>
<template>
<div class="header-page bzotech-elementor-template bzotech-header-page-header-1-home-1" id="header" style="min-height: 90px;">
<div class="bzotech-container-width">
<div class="elementor elementor-12195" data-elementor-id="12195" data-elementor-type="wp-post">
<div class="elementor-element elementor-element-134a655 e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="134a655">
<div class="e-con-inner">
<div class="elementor-element elementor-element-3af1eb7 e-flex e-con-boxed e-con e-child" data-element_type="container" data-id="3af1eb7">
<div class="e-con-inner">
<div class="elementor-element elementor-element-644ddd5 e-con-full elementor-hidden-mobile e-flex e-con e-child" data-element_type="container" data-id="644ddd5">
</div>
<div class="elementor-element elementor-element-e9205fd e-con-full elementor-hidden-tablet elementor-hidden-mobile_extra elementor-hidden-mobile e-flex e-con e-child" data-element_type="container" data-id="e9205fd">
</div>
<div class="elementor-element elementor-element-7cbfb9d e-con-full elementor-hidden-mobile e-flex e-con e-child" data-element_type="container" data-id="7cbfb9d">
</div>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-7e0f2f1 main-header1 e-flex e-con-boxed e-con e-parent" data-element_type="container" data-id="7e0f2f1" data-settings='{"background_background":"classic"}'>
<div class="e-con-inner">
<div class="elementor-element elementor-element-25aa683 e-con-full e-flex e-con e-child" data-element_type="container" data-id="25aa683">
<div class="elementor-element elementor-element-1c06a25 elementor-widget elementor-widget-bzotech-logo-global" data-element_type="widget" data-id="1c06a25" data-widget_type="bzotech-logo-global.default">
<div class="elementor-widget-container">
<div class="elbzotech-logo-global-style-image">
<a href="/">
<img alt="" class="attachment-full size-full wp-image-21269" decoding="async" fetchpriority="high" height="169" sizes="(max-width: 600px) 100vw, 600px" src="/Rodor_files/logo.png" width="600"/> </a>
</div>
</div>
</div>
</div>
<div class="elementor-element elementor-element-facdf6e e-con-full e-flex e-con e-child" data-element_type="container" data-id="facdf6e">
<div class="elementor-element elementor-element-b6c1af1 elementor-widget elementor-widget-bzotech-menu-global" data-element_type="widget" data-id="b6c1af1" data-widget_type="bzotech-menu-global.default">
<div class="elementor-widget-container">
<div :class="{ 'active': menuOpen }" class="bzotech-menu-global-container bzotech_line_arrow menu-global-style-" data-megamenu-maxwidth="">
<a :aria-expanded="menuOpen" @click.prevent="menuOpen = !menuOpen" class="toggle-mobile-menu" href="https://rodorjewelry.com/#"><i class="white la la-reorder"></i></a>
<div :class="{ 'migration-menu-open': menuOpen }" class="bzotech-menu-inner"><ul class="bzotech-navbar-nav menu-sticky-on" id="menu-main-menu"><li class="main-menu-item menu-item-even menu-item-depth-0 menu-item menu-item-type-post_type menu-item-object-page menu-item-home current-menu-item page_item page-item-1853 current_page_item" id="nav-menu-item-19572"><a class="menu-link main-menu-link" href="/"><span>Home</span></a></li>
<li class="main-menu-item menu-item-even menu-item-depth-0 menu-item menu-item-type-post_type menu-item-object-page" id="nav-menu-item-21574"><a class="menu-link main-menu-link" href="https://rodorjewelry.com/shop/"><span>Shop</span></a></li>
<li class="main-menu-item menu-item-even menu-item-depth-0 menu-item menu-item-type-post_type menu-item-object-page" id="nav-menu-item-19580"><a class="menu-link main-menu-link" href="https://rodorjewelry.com/blog/"><span>Blog</span></a></li>
<li class="main-menu-item menu-item-even menu-item-depth-0 menu-item menu-item-type-post_type menu-item-object-page" id="nav-menu-item-19578"><a class="menu-link main-menu-link" href="https://rodorjewelry.com/about-page/"><span>About Us</span></a></li>
<li class="main-menu-item menu-item-even menu-item-depth-0 menu-item menu-item-type-post_type menu-item-object-page" id="nav-menu-item-19584"><a class="menu-link main-menu-link" href="https://rodorjewelry.com/contact-page/"><span>Contact Us</span></a></li>
<li class="main-menu-item menu-item-even menu-item-depth-0 menu-item menu-item-type-post_type menu-item-object-page" id="nav-menu-item-19582"><a class="menu-link main-menu-link" href="https://rodorjewelry.com/faqs-v2/"><span>FAQs</span></a></li>
</ul></div>
</div> </div>
</div>
</div>
<div class="elementor-element elementor-element-07a6e25 e-con-full right-main-header1 e-flex e-con e-child" data-element_type="container" data-id="07a6e25">
<div class="elementor-element elementor-element-6d8e1e5 elementor-widget elementor-widget-shortcode" data-element_type="widget" data-id="6d8e1e5" data-widget_type="shortcode.default">
<div class="elementor-shortcode">
<div class="wpml-ls-statics-shortcode_actions wpml-ls wpml-ls-legacy-dropdown js-wpml-ls-legacy-dropdown" id="lang_sel">
<ul>
<li class="wpml-ls-slot-shortcode_actions wpml-ls-item wpml-ls-item-en wpml-ls-current-language wpml-ls-item-legacy-dropdown" tabindex="0">
<a @click.prevent="languageOpen = !languageOpen" class="js-wpml-ls-item-toggle wpml-ls-item-toggle lang_sel_sel icl-en" href="https://rodorjewelry.com/#">
<img alt="" class="wpml-ls-flag iclflag" height="12" src="/Rodor_files/en.svg" width="18"/><span class="wpml-ls-native icl_lang_sel_native">English</span></a>
<ul :style="languageOpen ? { visibility: 'visible' } : {}" class="wpml-ls-sub-menu">
<li class="icl-ar wpml-ls-slot-shortcode_actions wpml-ls-item wpml-ls-item-ar wpml-ls-first-item">
<a class="wpml-ls-link" href="https://rodorjewelry.com/ar/">
<img alt="" class="wpml-ls-flag iclflag" height="12" src="/Rodor_files/ar.svg" width="18"/><span class="wpml-ls-native icl_lang_sel_native" lang="ar">العربية</span></a>
</li>
<li class="icl-fa wpml-ls-slot-shortcode_actions wpml-ls-item wpml-ls-item-fa wpml-ls-last-item">
<a class="wpml-ls-link" href="https://rodorjewelry.com/fa/">
<img alt="" class="wpml-ls-flag iclflag" height="12" src="/Rodor_files/fa.svg" width="18"/><span class="wpml-ls-native icl_lang_sel_native" lang="fa">فارسی</span></a>
</li>
</ul>
</li>
</ul>
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
