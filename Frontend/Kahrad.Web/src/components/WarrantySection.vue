<script setup lang="ts">
const details = [
  {
    title: 'Warranty',
    icon: '/Kahrad_files/medal_black.png',
    image: '/Kahrad_files/warranty.png',
    description:
      "With the assurance of the quality of its products and for the convenience of customers' purchases, Kahrad offers its products with a 2-year and lifetime warranty.",
    reverse: false,
  },
  {
    title: 'Digital ID',
    icon: '/Kahrad_files/qr_code_scan_black.png',
    image: '/Kahrad_files/id.png',
    description:
      'All kahrad jewels are checked by experts in the laboratories of the collection to be presented with authentic birth certificates. This ensures the purchase and certifies the authenticity of jewelry. Each product has a unique digital ID, which is visible to the buyer on the product manufacturing site, stones and other product specifications.',
    reverse: true,
  },
]
</script>

<template>
  <section class="kahrad_main_warranty" aria-label="Warranty and product identification">
    <div class="container">
      <div class="row flex-wrap">
        <div
          v-for="detail in details"
          :key="detail.title"
          class="col-12 d-flex kahrad_row flex-wrap"
          :class="{ 'flex-wrap-reverse': detail.reverse }"
        >
          <template v-if="!detail.reverse">
            <div class="col-12 col-lg-6 d-flex flex-column align-items-center align-items-lg-end first">
              <div class="title first">
                <span>
                  <img :src="detail.icon" alt="" />
                </span>
                {{ detail.title }}
              </div>
              <p>{{ detail.description }}</p>
            </div>
            <div class="col-12 col-lg-6">
              <img :src="detail.image" :alt="detail.title" />
            </div>
          </template>
          <template v-else>
            <div class="col-12 col-lg-6">
              <img :src="detail.image" :alt="detail.title" />
            </div>
            <div class="col-12 col-lg-6 second d-flex flex-column align-items-center align-items-lg-start">
              <div class="title second">
                <span>
                  <img :src="detail.icon" alt="" />
                </span>
                {{ detail.title }}
              </div>
              <p>{{ detail.description }}</p>
            </div>
          </template>
        </div>
      </div>
    </div>
  </section>
</template>
