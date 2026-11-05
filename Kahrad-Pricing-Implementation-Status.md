# Kahrad Pricing ? implementation comparison

Comparison date: 2026-11-05. Reference: `Kahrad-Pricing-Master-Architecture.md` and repository `Agents.md`. This is an inspection of the current workspace, including earlier uncommitted work, not a production deployment audit. The solution has **32 source projects and 6 test projects**, rather than the historical estimate of 23 projects. A project folder or a successful build does not imply a working business capability.

## Current capability matrix

| Area / master sections | Verified code | Still required | Status |
| --- | --- | --- | --- |
| Modular monolith / Clean Architecture (?3) | Domain/Application/Infrastructure projects; explicit host composition; SQL module contexts; no CQRS/MediatR/brokers | Domain EF/Sieve coupling remains in Products, Brands, Guarantees and Identity. Several application services perform EF queries; catalog repository writes also contain orchestration that should move incrementally into Application. Identity uses framework user/role base types; preserve the approved authentication design while deciding that boundary. | Partial |
| Product catalog / localization (?4.1, ?7) | Products/groups/images, existing pricing factors; shared feature definitions, specifications with typed values, language registry, manual translations, localized POST search/details, fallback, admin edits and migration tests | Product-item input model, variants, publication workflow beyond IsActive/default translation, import/data-quality tools, barcode/identity-card printing. Legacy feature definitions are deliberately not merged by equal labels. | Working catalog; scope remains |
| Brands / configuration (?4.3) | Brand/store CRUD, brand-scoped key/value configurations and a manually set ActivePriceVersion field | Typed and audited price-policy configuration; approved brand/representative precedence; validate publication against a completed price run. A manually assigned version number is not a publishing pipeline. | Partial |
| Representatives (?4.4) | Three projects containing Class1 placeholders | Representatives/types, user ownership, business access scope, sales relationships and approved pricing/discount inputs. | Missing |
| Pricing.Engine (?5) | Empty Class1 library; dependency guard test | Actual approved input model, deterministic equation and rounding, golden examples, brand/representative cases and edge cases. Existing `ProductService.CalculateEstimatedPriceAsync` has a separate additive estimate with four-decimal rounding; it is not evidence of approval for the production formula and was not changed. | Missing; formula pending |
| Price runs / publishing (?5?6) | Worker hosts Hangfire for SMS; Brand.ActivePriceVersion exists | SQL recalculation requests/runs/precomputed ProductPrice rows, version allocation and validation, atomic per-brand publication, conflict prevention and safe retry/recovery. No Redis source-of-truth substitution. | Missing |
| Price Inquiry (?3.4, ?5.2) | Separate API host; safe health endpoints added in this pass | Published-price single/batch services, configurable batch limit, authorized representative context, SQL projections, HybridCache/Redis version keys and SQL fallback/outage tests. | Price delivery missing |
| Identity (?8) | GUID users/roles, password login, JWT, refresh rotation/family revocation, logout, user/role/permission management, audit persistence, explicit development SuperAdmin seed | OTP issuance/verification/replay protection/rate policy; database permission authorization policies on operations; full identity security tests; concurrent refresh/reuse handling and chosen access-token revocation policy. DTOs named RequestOtp/VerifyOtp are not a working OTP login flow. | Partial |
| SMS (?8, ?13) | Persisted message/log/provider/module configuration, SQL retry scheduling, SMS.ir/Kavenegar adapters, Hangfire outbox, immutable attempt outcomes and safe unknown-delivery behavior | Live-provider account smoke tests/credentials, approved authentication OTP integration and recovery operation for uncertain sends. Raw send-otp is now administrator-only. | Implemented delivery module; live integration pending |
| Guarantees | Vendor/plan/certificate services, API and migrations | Dedicated module regression coverage and actual workflow acceptance. This existing module is outside the master document's main module list; it was preserved. | Implemented code; acceptance not established |
| Media (?10) | Product image URL metadata/gallery; separate worker/library projects; Windows Service lifetime added | Real upload/content validation, storage adapter, image variants, video Range delivery/FFmpeg thumbnails, durable processing state, jobs, resource limits and storage/backup deployment. Media library projects are Class1 placeholders. | Missing pipeline |
| Accounting / legacy (?4.7, ?6.1) | No implemented accounting adapter/import/sync process found | Actual Mahak contract, canonical identifiers, ownership/conflict rules, idempotent imports and sales exports/outbox where necessary, dry-run and reconciliation for problematic data. 35,000/9,000 are requirements estimates, not measured database counts. | Missing; contracts pending |
| Sales / offers / international orders (?2) | No complete workflows found | Historical sale-price snapshots, finalized accounting requests, confirmed offer quantities/averaging/rounding, received-versus-confirmed international orders and reliable synchronization. | Missing; rules pending |
| Frontends (?11) | Two Angular NgModule starters and two Vue starters in the actual directories | API clients, login/permissions, admin CRUD and translation editors, representative views, localized storefronts, price read integration and business E2E tests. Source scan found no HttpClient/fetch/axios/API calls in any app. Preserve `Rodor.Web` until any rename to the documented Rodoro brand is approved. Vue starters currently use Cypress; do not silently replace it with Playwright just to match the illustrative stack. | Scaffolds only |
| Operations / API conventions (?9, ?12?14) | CPM already exists; JWT/OpenAPI/Scalar; Hangfire and Windows Service support; logging/health/correlation added below | Controlled module migration rollout, permission/rate policies, a compatible versioning plan, worker backlog/failure metrics, operational dashboards, backup/restore drills, deployment secrets and load/failure tests. Existing REST routes are preserved; new catalog actions use POST. | Partial |

## Completed in this bounded pass

1. Added Serilog structured JSON logging for the API, Price Inquiry, pricing/SMS worker and media worker through `BuildingBlocks.Infrastructure/Hosting/HostOperations.cs`. HTTP logs contain method/path/status/elapsed time and a validated correlation ID, not request bodies, query strings, passwords, OTPs or tokens. Accepted X-Correlation-ID values are bounded to 64 ASCII letters/digits/underscore/hyphen; invalid input is replaced by the server trace ID.
2. Added anonymous `/health/live`, `/health/ready` and `/health` to both API hosts with no-store, status-only output. Liveness does not query SQL. Management readiness probes its configured SQL databases with bounded timeouts. These probes verify connectivity, not migration completeness or the completion of every business feature. Redis is not required by these checks.
3. Removed Price Inquiry's weather demo. Its readiness deliberately returns 503 until actual published-price delivery is configured; process liveness returns 200. No fabricated price endpoint was added.
4. Removed the pricing worker's one-second template log loop; its existing Hangfire SMS processing remains registered. Added Windows Service integration to Media.Worker and replaced its template spam with one explicit warning that no media pipeline is configured. Media processing is still incomplete.
5. Protected existing SuperAdmin targets from Admin-level profile changes, password resets, status changes and role assignment/removal, using an endpoint authorization filter. SuperAdmin access and Admin operations on ordinary accounts remain. Hardened existing checks against case-variant SuperAdmin role names.
6. Removed anonymous access to low-level `/api/sms/send-otp`. This endpoint accepts a caller-supplied code, so it is an administrative send primitive, not OTP authentication. Admin/SuperAdmin is now required; its audit description no longer includes the phone number. Future anonymous OTP login must generate, track and verify its own challenges with the approved rate/expiry policy.
7. Added four architecture tests for inward project dependencies, a cycle-free project graph, Pricing.Engine dependency isolation, centralized versions and prohibited messaging/CQRS packages. These tests do **not** claim the existing Domain EF coupling or repository orchestration debt has been fixed.
8. Added fifteen host/security tests, including the SuperAdmin cases, ordinary authorized operations, raw OTP 401/403, safe readiness output, correlation validation and Hangfire job serializer round-trip.
9. Pinned the worker's Newtonsoft.Json dependency to 13.0.4. Hangfire.Core 1.8.24 declares a compatible lower-bound range; the SMS job serialization round-trip passed. This addresses its previously resolved 11.0.1 dependency and the advisory affecting versions before 13.0.1: https://github.com/advisories/GHSA-5crp-9r3c-p9vr . No assertion is made that a complete current online dependency audit has passed.
10. Normalized source path casing in the solution. No business schemas, pricing formulas, production data or frontend applications were changed in this pass.

## Local database versus source code

Read-only inspection of local `localhost/KahradPricing` found only Identity (11 tables) and SMS (4 tables) installed, with these applied migrations:

- 20261103165252_InitialIdentitySchema
- 20261104112903_InitialSmsSchema
- 20261105122835_SeparateSmsDeliveryConfiguration
- 20261105130003_AddSmsLogsAndConfigurationEntities

Brands, Products/catalog and Guarantees code is therefore ahead of that database. Prepared idempotent deployment SQL:

- `artifacts/architecture-audit/brands.sql`
- `artifacts/architecture-audit/products.sql`
- `artifacts/architecture-audit/guarantees.sql`

They were generated and inspected, not applied to the application database. The catalog script preserves legacy columns, backfills confirmed Persian content and fills feature-definition references. Deployment must target the intended database and use the existing reviewed rollout procedure. No production database was accessed. SQL regression tests used an isolated catalog database and a rolled-back SMS transaction.

## Dependencies and decision boundaries

The master document describes itself as a living reference and explicitly disallows treating it as authorization for a single massive rewrite (?17). Its statement that centralized package management need not be forced is historical; the current repository already uses Directory.Packages.props and Agents.md explicitly requires it. It was retained. Likewise, the master leaves API versioning details OPEN while Agents.md specifies URL-segment versioning; introduce compatible aliases/deprecation only in a reviewed API change, not a mass route rewrite during this audit.

The user confirmed during this comparison that pricing details will be provided later. Do not implement a new equation or turn the existing illustrative chain/estimate into an approved production algorithm.

Required inputs for subsequent bounded implementations:

| Priority | Work package | Concrete decision/input required |
| --- | --- | --- |
| P0 | Install existing completed module schemas in the intended environment | Explicit rollout target and migration review; scripts above are prepared |
| P1 | Identity permissions and OTP login | Permission-to-operation matrix; OTP expiry, attempts, resend/rate limits and delivery templates; access-token invalidation policy |
| P1 | Product items and representatives | Canonical item/variant schema, source/accounting IDs, ownership and representative/type hierarchy/scope |
| P1 | Production pricing and SQL publication | Approved equation and at least one real expected-price example, rounding precision/mode, recalculation scope; reviewed run/version/store design |
| P2 | Price inquiry cache/read path | The published-price contract, authorized dimensions and freshness/active-version policy; Redis configuration. It must not depend on the formula engine at request time |
| P2 | Media pipeline | Storage root/backend and delivery topology, allowed formats/size limits and approved processing profiles |
| P2 | Application frontends | Approved screen/workflow scope and API contracts; storefront SEO/SSR and authentication/order choices |
| P3 | Accounting / legacy correction | Mahak API/database contract, record mapping, ownership and idempotency/reconciliation strategy |
| P3 | Sales / offers / regional intake | Historical price/finalization rules, offer arithmetic example, received/confirmed validation and cross-border failure/recovery semantics |
| P3 | Operations and boundary cleanup | Deployment resources, backup RPO/RTO, secrets, monitored SLAs; isolated incremental EF metadata/application-query/repository-orchestration cleanup preserving current models |

These are actual remaining deliverables. The operational/security foundation completed here does not make Pricing, Representatives, Media or frontends complete.

## Validation executed

```powershell
dotnet restore Kahrad.Pricing.slnx -p:NuGetAudit=false -v quiet
dotnet build Kahrad.Pricing.slnx --no-restore -v quiet
dotnet test Tests/ArchitectureTests/Kahrad.Pricing.ArchitectureTests --no-build --no-restore -v minimal
dotnet test Tests/IntegrationTests/Kahrad.Pricing.Hosts.IntegrationTests --no-build --no-restore -v minimal
dotnet test Tests/UnitTests/Kahrad.Pricing.Products.Tests --no-build --no-restore -v quiet
dotnet test Tests/UnitTests/Kahrad.Pricing.Sms.Tests --no-build --no-restore -v quiet
$env:CATALOG_TEST_SQL_SERVER = 'Server=localhost;Database=master;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5'
dotnet test Tests/IntegrationTests/Kahrad.Pricing.Products.IntegrationTests --no-build --no-restore -v minimal
$env:SMS_TEST_CONNECTION = 'Server=localhost;Database=KahradPricing;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5'
dotnet test Tests/IntegrationTests/Kahrad.Pricing.Sms.IntegrationTests --no-build --no-restore -v minimal
```

Build passed with zero warnings/errors after the serializer pin. **62 tests passed:** architecture 4, host/security 15, catalog unit 20, SMS unit 21, catalog SQL/API 1, SMS SQL 1. The catalog SQL test creates/deletes only a generated KahradCatalogTests_{guid} database; the SMS SQL test rolls back. Host authorization tests validate real JWT middleware with isolated service doubles; they do not modify the seeded account.

A live Price Inquiry host was started for a local smoke test and stopped afterward: `/health/live` 200 with `{"status":"Healthy"}`, `/health/ready` 503, removed `/weatherforecast` 404. Output is under `artifacts/architecture-audit/price-inquiry.*.log`.

`git diff --check` passed. Frontend build/E2E, live SMS-provider calls, Redis outage/load tests, production restore exercises and real accounting integration tests were not run; those capabilities are not implemented/configured. Pricing golden tests remain blocked on the forthcoming approved rules.
