# Rodor.Web

Vue 3 and Vite implementation of the Rodor public storefront.

The migrated page components live in `src/components`. Static brand styles, images, icons, and the required Swiper runtime are served from `public/Rodor_files`. Product, language, media, and form links currently point to the existing Rodor website.

## Development

```sh
npm install
npm run dev
```

## Verification

```sh
npm run build
npm run test:unit -- --run
```

Use `npm run test:e2e` when the Cypress browser binary is available.
