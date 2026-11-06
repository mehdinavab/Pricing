<script setup lang="ts">
const categories = [
  {
    label: 'For Men',
    href: 'https://kahrad.com/product-category/mens-products/',
    image: '/Kahrad_files/men.jpg',
  },
  {
    label: 'For Women',
    href: 'https://kahrad.com/product-category/womens-products/',
    image: '/Kahrad_files/women.jpg',
  },
  {
    label: 'Wedding Rings',
    href: 'https://kahrad.com/product-category/set-product/',
    image: '/Kahrad_files/ring.jpg',
  },
  {
    label: 'Necklace',
    href: 'https://kahrad.com/#',
    image: '/Kahrad_files/necklace.png',
  },
]
</script>

<template>
  <section aria-label="Product collections">
    <div class="kahrad_main_collection">
      <div class="container">
        <div class="kahrad_main_collection_row">
          <h2>
            Authenticate and Register Your Product
            <a target="_blank" rel="noreferrer" href="https://kahrad.com/en/search-page/">Here</a>
          </h2>
        </div>
      </div>
    </div>

    <div class="kahrad_main_under_collection d-flex align-items-center flex-wrap">
      <div v-for="category in categories" :key="category.label" class="col-12 col-lg-3">
        <a class="link" :href="category.href">
          {{ category.label }}
          <img src="/Kahrad_files/more_link.png" alt="" />
        </a>
        <img :src="category.image" :alt="category.label" />
      </div>
    </div>
  </section>
</template>
