# Kahrad.Web

Vue 3 and Vite implementation of the Kahrad public storefront.

The migrated page components live in `src/components`. Static brand styles, images, icons, and fonts are served from `public/Kahrad_files`. Product and informational links currently point to the existing Kahrad website.

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
