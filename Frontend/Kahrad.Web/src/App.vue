<script setup lang="ts">
import AppHeader from './components/AppHeader.vue'
import CategorySection from './components/CategorySection.vue'
import FooterSection from './components/FooterSection.vue'
import HeroSection from './components/HeroSection.vue'
import HighlightsSection from './components/HighlightsSection.vue'
import VideoBand from './components/VideoBand.vue'
import WarrantySection from './components/WarrantySection.vue'
</script>

<template>
  <AppHeader />

  <main class="elementor elementor-15771">
    <div
      class="elementor-element elementor-element-53d85ee e-con-full e-flex e-con e-parent e-lazyloaded"
      data-id="53d85ee"
      data-element_type="container"
    >
      <div
        class="elementor-element elementor-element-720f2e8 top_video_sections elementor-widget elementor-widget-first_video"
        data-id="720f2e8"
        data-element_type="widget"
      >
        <div class="elementor-widget-container">
          <HeroSection />
        </div>
      </div>

      <div
        class="elementor-element elementor-element-d196e50 elementor-widget elementor-widget-collection"
        data-id="d196e50"
        data-element_type="widget"
      >
        <div class="elementor-widget-container">
          <CategorySection />
        </div>
      </div>

      <div
        class="elementor-element elementor-element-2f63884 elementor-widget elementor-widget-special_product"
        data-id="2f63884"
        data-element_type="widget"
      >
        <div class="elementor-widget-container">
          <HighlightsSection />
        </div>
      </div>

      <div
        class="elementor-element elementor-element-2d1e89d elementor-widget elementor-widget-second_video"
        data-id="2d1e89d"
        data-element_type="widget"
      >
        <div class="elementor-widget-container">
          <VideoBand source="https://kahrad.com/wp-content/uploads/2024/07/d.mp4" />
        </div>
      </div>
    </div>

    <div
      class="elementor-element elementor-element-3ee916f e-con-full e-flex e-con e-parent"
      data-id="3ee916f"
      data-element_type="container"
    >
      <div
        class="elementor-element elementor-element-c3d4e1e elementor-widget elementor-widget-warranty"
        data-id="c3d4e1e"
        data-element_type="widget"
      >
        <div class="elementor-widget-container">
          <WarrantySection />
        </div>
      </div>
    </div>

    <div
      class="elementor-element elementor-element-845520a e-con-full e-flex e-con e-parent"
      data-id="845520a"
      data-element_type="container"
    >
      <div
        class="elementor-element elementor-element-53dd82d elementor-widget elementor-widget-third_video"
        data-id="53dd82d"
        data-element_type="widget"
      >
        <div class="elementor-widget-container">
          <div class="kahrad_main_top_footer">
            <VideoBand source="https://kahrad.com/wp-content/uploads/2024/07/mail2.mp4" />
          </div>
        </div>
      </div>
    </div>
  </main>

  <FooterSection />
</template>
