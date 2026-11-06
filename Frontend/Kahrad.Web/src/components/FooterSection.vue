<script setup lang="ts">
const linkGroups = [
  {
    title: 'About us',
    links: [
      { label: 'about us', href: 'https://kahrad.com/about/' },
      { label: 'Production process', href: 'https://kahrad.com/productionprocess/' },
      { label: 'Frequently asked questions', href: 'https://kahrad.com/faq/' },
      { label: 'warranty', href: 'https://kahrad.com/warranty/' },
      { label: 'Team Kahrad', href: 'https://kahrad.com/team/' },
      { label: 'join us', href: 'https://kahrad.com/join-us/' },
    ],
  },
  {
    title: 'User guide videos',
    links: [
      { label: 'How to access the digital page of representatives', href: 'https://kahrad.com/access/' },
      {
        label: 'How to register the warranty in the name of the buyers',
        href: 'https://kahrad.com/warranty/',
      },
      { label: 'How to view product information', href: 'https://kahrad.com/information/' },
    ],
  },
  {
    title: 'Contact us',
    links: [
      { label: 'join us', href: 'https://kahrad.com/join-us/' },
      { label: 'contact', href: 'https://kahrad.com/contact/' },
    ],
  },
]
</script>

<template>
  <footer>
    <section class="kahrad_footer_section">
      <div class="container">
        <div class="kahrad_footer_section_div">
          <div class="logo">
            <img src="/Kahrad_files/logo.png" alt="Kahrad" />
          </div>
          <div class="details row">
            <div
              v-for="(group, index) in linkGroups"
              :key="group.title"
              class="col-12 col-md-4"
              :class="{ 'justify-content-between': index === 1 }"
            >
              <div v-if="index === 1" class="d-flex flex-column">
                <span>{{ group.title }}</span>
                <div class="custom-menu-class">
                  <ul class="menu">
                    <li v-for="link in group.links" :key="link.label">
                      <a :href="link.href">{{ link.label }}</a>
                    </li>
                  </ul>
                </div>
              </div>
              <template v-else>
                <span>{{ group.title }}</span>
                <div class="custom-menu-class">
                  <ul class="menu">
                    <li v-for="link in group.links" :key="link.label">
                      <a :href="link.href">{{ link.label }}</a>
                    </li>
                  </ul>
                </div>
              </template>
            </div>
          </div>
          <div class="under">
            <p>© 1399 Kahrad&nbsp;All rights reserved.&nbsp;ver(2.0.0)</p>
          </div>
        </div>
      </div>
    </section>
  </footer>
</template>
