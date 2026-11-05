# Manually managed multilingual catalog

## Discovery and implementation plan

The existing module is `Src/Modules/Products`, using .NET 10 / EF Core 10.0.14 and SQL Server. Products, ProductGroups, ProductFeatures and ProductImages already exist in schema `products`. Keys are bigint with PublicId, audit fields, rowversion and soft deletion. Product name length is 250, feature name 150, and existing feature values are untyped strings of length 500. ProductFeatures are individual assignments carrying values, ordering and pricing effects; there were no shared definitions or specifications. Matching legacy feature labels do not prove shared identity. No localization, catalog cache, or localization invalidation hooks existed.

Existing API routes use `/api/products`, GET reads and REST writes, JWT authentication, Admin/SuperAdmin roles for writes, audit services and ProblemDetails errors. The new storefront and admin actions use POST under the same prefix; existing endpoints remain. Tests follow the repository's module-specific xUnit project convention, with opt-in SQL tests and no Docker.

The implementation adds domain entities, application contracts/validation/services, SQL persistence mappings/repositories, an incremental migration, localized public projections and protected manual administration. Pricing formulas, pricing-engine projects, assignments' financial fields, media and other modules are not changed by this task. Earlier uncommitted SMS work remains in the workspace.

## Data model and compatibility

New tables:

- Languages: data-driven BCP-47 code (16 characters), administration name, active/default flags. Codes are canonicalized (`en-US`) and case-insensitively unique. A filtered unique index permits at most one default; a check requires the default to be active and not deleted. Administration cannot remove the last default; switching requires translations for every active product, inside a transaction.
- ProductTranslations: ProductId + LanguageId unique; Name (250), LocalizedDescription (1000). No speculative slug or short-description fields.
- Features and FeatureTranslations: reusable definition with stable code, default label and one label per language. ProductFeatures gains nullable FeatureId with restricted deletion of an assigned definition.
- Specifications and SpecificationTranslations: reusable definition with stable code, value type and neutral unit, plus translated labels.
- ProductSpecifications and ProductSpecificationTranslations: one assignment per product/specification in this new domain; separate text/decimal/boolean/date/code slots and textual translations. A check enforces one stored value. Numbers use decimal(18,4); excess precision/range is rejected rather than rounded. Dates reject time-of-day data. Numeric, boolean, date and code slots cannot be translated.

All new entities follow existing audit/concurrency/soft-delete conventions. Unique entity/language pairs and language FKs are SQL-enforced. Language deletion is restricted. Product-owned data cascades with its owner; shared definition deletion is restricted by assignments. No new SQL attributes or infrastructure dependencies were introduced on the new domain entities.

Migration: `20261105132353_AddMultilingualCatalog`, added after the existing initial Products migration; already-applied migrations are unchanged. The owner explicitly confirmed that historical product and feature labels are Persian. The migration seeds fa/ar/en, makes fa the default and set-wise copies valid existing names/descriptions into Persian translations. Existing columns are retained, never cleared or dropped. Blank legacy names are preserved in place and skipped for translation; the missing-translation report exposes the need for manual correction.

Each legacy feature assignment receives its own reusable definition, with code LEGACY-{assignmentId}. This avoids guessing that equal labels have equal meaning or collapsing legitimate duplicate rows. Values, IDs, order and pricing effects remain intact. Future assignments explicitly reuse FeatureId through the existing feature-assignment API. There is no new uniqueness restriction on existing ProductFeatures. There were no historical specifications to backfill, and feature values are not reinterpreted as specifications.

Legacy create/update Name and Description inputs remain manually supplied default-language content and are saved atomically with default translations. Default translation edits synchronize legacy columns. Activation requires a valid default product translation. Existing authenticated admin reads remain compatible; the new storefront endpoints provide localized content. Shared-label edits affect every product using the definition. In particular, editing Name through the existing UpdateFeature endpoint updates that definition's default label, so shared assignments see the same label.

## Resolution and public API

Accept-Language is parsed with the framework's quality-aware parser. Preferences are ordered by q and header order. Resolution tries active exact tags, then neutral parents, then subsequent candidates. q=0 candidates are excluded while acceptable alternatives exist; wildcard is supported. If no acceptable active language remains, the configured default is served rather than 406, even if explicitly excluded. Invalid or oversized headers safely select the default. Inactive languages are visible to editors but cannot be publicly selected.

Every name, product description and textual specification value independently falls back from resolved locale to default locale, then preserved legacy content. No arbitrary third language is selected and no translation is generated. Missing required default product translations produce an aggregate diagnostic warning, without product content. All new/updated content validates nonblank labels.

Public, anonymous POST actions:

- `/api/products/search`: CatalogSearchRequest; page/pageSize, optional brandId/groupId/code/name and sortByName. Searches and sorts the resolved/fallback name in SQL, counts products before pagination and uses Id as tie-breaker. PageSize <= 100.
- `/api/products/details`: `{ "productId": 123 }`; returns an active product or 404.

Both return one localized representation, with existing product/group, pricing-factor, gallery and feature fields, plus typed specifications and resolved Language. Content-Language and Vary: Accept-Language are returned. Database projections fetch only selected/fallback text and use bounded split queries for collections, without loading every translation or issuing per-product queries. No catalog cache exists, so edits are visible on the next read; numeric price caches are neither invalidated nor recalculated.

Example:

```http
POST /api/products/details
Accept-Language: en-US,en;q=0.9,fa;q=0.5
Content-Type: application/json

{ "productId": 123 }
```

Relevant response fields (other existing fields remain):

```json
{
  "id": 123,
  "code": "KH-001",
  "name": "Example product",
  "language": "en",
  "features": [{ "id": 10, "featureId": 5, "name": "Waterproof", "value": "yes" }],
  "specifications": [{ "id": 20, "specificationId": 7, "name": "Material", "valueType": 1, "textValue": "Wood", "numberValue": null }]
}
```

## Manual administration

All routes below are POST under `/api/products/admin`, require an Admin/SuperAdmin JWT, and use the existing audit service for mutations:

| Action suffix | Request |
| --- | --- |
| languages/list | no body required |
| languages/upsert | code, name, isActive, isDefault, rowVersion on edits |
| products/translations/upsert and products/translations/list | TranslationRequest / TranslationListRequest |
| features/translations/upsert and features/translations/list | TranslationRequest / TranslationListRequest |
| specifications/translations/upsert and specifications/translations/list | TranslationRequest / TranslationListRequest |
| product-specifications/translations/upsert and product-specifications/translations/list | TranslationRequest / TranslationListRequest; textual values only |
| features/create and features/list | DefinitionRequest / DefinitionListRequest |
| specifications/create and specifications/list | DefinitionRequest / DefinitionListRequest |
| product-specifications/upsert and product-specifications/list | SpecificationAssignmentRequest / SpecificationAssignmentsRequest |
| translations/missing | target (1 product, 2 feature, 3 specification, 4 textual value), languageCode, page, pageSize |

TranslationRequest: entityId, languageCode, text, optional product description, rowVersion for existing translations. Lists return all supplied translations, including inactive languages, with rowVersion. Language lists and assignment lists also provide rowVersion. Editing without the current version returns 409; SQL unique races also return safe conflicts. Domain validation uses 400, missing products 404, anonymous writes 401 and insufficient roles 403. No translation-delete action is exposed, so required default translations cannot be removed through this API.

The existing POST `/api/products/{productId}/features` accepts FeatureId with Value and pricing/order fields; Name is optional when referencing a definition and is required for a new definition. Definitions are not duplicated per language. New languages require rows and translations, without a schema change. There is no AI/external translation integration.

Both `/swagger/v1/swagger.json` and `/openapi/v1.json` describe the POST routes, DTOs, Accept-Language behavior, responses and public/admin security. Scalar uses the built-in document.

## Validation and rollout

Verified commands:

```powershell
dotnet build Kahrad.Pricing.slnx --no-restore -v quiet
dotnet test Tests/UnitTests/Kahrad.Pricing.Products.Tests --no-build -v quiet
dotnet test Tests/UnitTests/Kahrad.Pricing.Sms.Tests --no-build -v quiet
$env:CATALOG_TEST_SQL_SERVER = 'Server=localhost;Database=master;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5'
dotnet test Tests/IntegrationTests/Kahrad.Pricing.Products.IntegrationTests --no-build --no-restore -v minimal
dotnet ef migrations has-pending-model-changes --no-build --project Src/Modules/Products/Kahrad.Pricing.Products.Infrastructure --startup-project Src/Modules/Products/Kahrad.Pricing.Products.Infrastructure --context ProductsDbContext
```

Results: solution build passed, 20 catalog unit tests passed, 21 existing SMS unit tests passed, and the SQL/API integration scenario passed. The integration test creates a unique KahradCatalogTests_{guid} database, migrates a populated legacy schema, validates projections/constraints/admin flows/JWT 401 and 403/default switching/OpenAPI, then deletes only that database. It also verifies existing product create/update and the unchanged price estimate. Audit persistence is stubbed in HTTP tests to keep unrelated Identity tables outside this test; production actions use the existing audit service. Without CATALOG_TEST_SQL_SERVER the SQL test is explicitly skipped.

The generated model matches its migration. Idempotent deployment SQL is at `artifacts/catalog/migrate.sql`. No application or production catalog database was migrated in this task. Review and back up the target database, profile blank legacy labels, then apply the reviewed incremental migration through the existing deployment process before starting the updated API. Verify seeded language/default state and translation counts. Rollback removes the newly introduced translations/definitions, so preserve manual translations before rollback. Legacy source names and financial fields remain.


## Integration test follow-up

The OmidNicClub-style API suite is documented in `Tests/IntegrationTests/Kahrad.Pricing.Api.IntegrationTests/README.md`.
It tests real Unicode, audited HTTP writes and SQL queries with a shared isolated fixture. It discovered and fixed SQL translation of the missing-translations report.
Incremental migration `20261105143057_RestoreCatalogLanguageDisplayNames` corrects only the known question-mark fa/ar seed display labels; manually edited names and historical product content are preserved. Earlier migrations remain unchanged. Review and deploy this follow-up migration with the catalog rollout; it was tested in disposable databases only.
