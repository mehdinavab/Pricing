<script setup lang="ts">
const features = [
  {
    icon: '/Kahrad_files/qr-code-scan.png',
    description: 'Unique hand-made products with Digital ID',
    spacing: '',
  },
  {
    icon: '/Kahrad_files/magicpen.png',
    description: 'More than 40 years of experience.',
    spacing: '',
  },
  {
    icon: '/Kahrad_files/award.png',
    description:
      'A pioneer in designing and producing hand-made silver products with original jewelry',
    spacing: 'mt-4 mb-4 m-lg-0',
  },
  {
    icon: '/Kahrad_files/verify.png',
    description: 'Equipped with gemological laboratory under expert supervision',
    spacing: 'mt-4 mb-4 m-lg-0',
  },
]

const recognition =
  "With the help of skilled crafts and arts specialists and 40 years of experience in the production of all kinds of silver rings, the Kahrad Collection has succeeded in obtaining approval from France's Reliance and IRA's Gold and Jewelry Commission."
</script>

<template>
  <section class="kahrad_main_special_product" aria-label="About Kahrad products">
    <div class="container">
      <div class="row">
        <div class="col-12 col-lg-8 d-flex detail flex-wrap">
          <div class="col-12 col-lg-8 d-flex flex-wrap justify-content-between align-items-end">
            <div
              v-for="feature in features"
              :key="feature.description"
              class="box col-6"
              :class="feature.spacing"
            >
              <div class="icon">
                <img :src="feature.icon" alt="" />
              </div>
              <p>{{ feature.description }}</p>
            </div>
          </div>
          <div
            class="col-12 col-lg-4 d-flex align-items-center justify-content-center align-items-lg-end justify-content-lg-end"
          >
            <div class="full_box">
              <div class="icon">
                <img src="/Kahrad_files/medal.png" alt="" />
              </div>
              <p>{{ recognition }}</p>
            </div>
          </div>
        </div>
        <div class="col-12 col-lg-4 special_product">
          <img src="/Kahrad_files/wedding_ring.png" alt="Hand-crafted wedding ring" />
        </div>
      </div>
    </div>
  </section>
</template>
