# Kahrad Pricing — Master Architecture & Decision Record

**Project:** KahradEnterprise / Kahrad.Pricing
**Document type:** Consolidated architecture, requirements, design decisions, and Codex implementation guidance
**Last consolidated:** 2026-11-05
**Status:** Living architecture baseline; confirm actual repository details before implementation
**Primary stack:** .NET 10, ASP.NET Core Web API, EF Core, SQL Server, Redis, Angular, Vue

> This document consolidates outcomes of earlier Kahrad discussions. It is not a claim that every component is already implemented. **[DECIDED]** means explicitly accepted or repeatedly specified; **[DESIGN DIRECTION]** is an agreed architectural direction whose exact implementation remains to be verified; **[PROPOSED]** is guidance, not an approved hard requirement; **[OPEN]** means a decision is still needed. **[REPO CHECK]** means inspect actual code before relying on a name, configuration, or behavior.

---

## 1. Executive summary

Build a **modular monolith with independently deployable hosts**, adhering to **Clean Architecture** within the solution and keeping business modules separated. Modules/engines are primarily libraries; do **not** create a standalone microservice for each module. Keep pricing computations separate from the public price-inquiry read path. Use **SQL Server as the authoritative store**, and **Redis as a derived low-latency cache**. Background workers calculate and publish prices, synchronize inputs, and process media. Admin and representative interfaces use **Angular**; storefront interfaces for **Kahrad and Rodoro** use **Vue**. Products, their reusable features, and their specifications are manually translatable to Persian, Arabic, English, and later languages; storefront APIs return localized data. An Identity module handles password and SMS OTP login, JWT, rotating refresh tokens, roles, permissions, and audit logging.

### Non-negotiable constraints

- **[DECIDED]** Use **Clean Architecture**, **not CQRS**, **not Vertical Slice**, and do not introduce MediatR merely to simulate either.
- **[DECIDED]** Use **SQL Server**, not PostgreSQL, for the Kahrad project.
- **[DECIDED]** Do not add **Kubernetes** or **Docker** as implementation prerequisites.
- **[DECIDED]** Preserve existing modules, names, conventions, and behavior; avoid a wholesale rewrite.
- **[DECIDED]** Prefer **POST** endpoints, including new API operations, according to the existing API direction.
- **[DECIDED]** Use manual content translations; do not introduce automatic translation services.
- **[DECIDED]** Protect production data: migrations, imports, and pricing changes require a controlled, testable rollout.

---

## 2. Business scope and constraints

### 2.1 Catalog and legacy systems

- **[DECIDED]** Target approximately **35,000 products**; an earlier broad estimate was 10,000–100,000 and should be regarded as capacity context, not the current count.
- **[DECIDED]** Products have a configurable/dynamic set of **product items** that drive pricing.
- **[DECIDED]** Products have **shared/reusable features** and **shared/reusable specification definitions**.
- **[DECIDED]** Existing inventory includes approximately **9,000 problematic legacy products** requiring careful cleanup/migration.
- **[DESIGN DIRECTION]** Integrate with the existing accounting/Mahak environment using APIs and a controlled synchronization flow; existing legacy data must not be silently discarded.
- **[OPEN]** Confirm canonical product identifiers, SKU/code schemes, variations, variants, category hierarchy, inventory semantics, accounting event contracts, and exact legacy migration mapping.

### 2.2 Brands, representatives, sales channels

- **[DECIDED]** Brands include **Kahrad** and **Rodoro**, with separate storefronts.
- **[DECIDED]** A brand can have a factor in the **1–10** range, and the design includes beauty coefficient, commerce percentage, sales percentage, and rounding.
- **[DECIDED]** Representatives are an explicit business concept; representative type can affect the calculation, and representative-specific discounts need to be supported as the business evolves.
- **[DECIDED]** Base/shared formulas should be reusable rather than duplicating a different pricing engine per brand or representative.
- **[DECIDED]** Rounding behavior is the same across brands at present, but should be modeled so it can be **configured per brand** later.
- **[OPEN]** Define the authoritative exact pricing equation, order of operations, tax/fees, discount application, minimums, precision, integer rounding mode, effective dates, and precedence of brand versus representative rules before coding a new algorithm.

### 2.3 Frontend and geographic deployment

- **[DECIDED]** Two Angular administration experiences: internal/admin pricing panel and representative panel.
- **[DECIDED]** Two brand storefronts using Vue: Kahrad and Rodoro. Separate domestic/international storefront deployments are possible; do not conflate a deployment with an entirely new business module.
- **[DECIDED]** Main operations and users are based in Iran; the architecture must continue to serve Iranian users during internet disruptions. External storefronts may serve cached content and accept incoming orders for later review in Iran, subject to business synchronization rules.
- **[OPEN]** Specify the actual order acceptance/replication model, consistency guarantees, offline queueing rules, payment dependencies, and disaster-recovery RPO/RTO for cross-border operation.

---

## 3. System style, boundaries, and solution structure

### 3.1 Architectural style

**[DECIDED]** Start with a **modular monolith** with separately deployable host applications. Business modules should expose contracts/services, not direct access to one another's internal tables and DbContexts. An engine is a library or application component, not automatically a new HTTP service. Split into external services only when operational evidence warrants it.

### 3.2 Known solution layout

**[REPO CHECK]** Earlier discussions identified solution **`Kahrad.Pricing.slnx`**, root **`Src/`**, and roughly **23 projects** at the time of discussion. The known logical grouping is:

```text
KahradEnterprise/
├── Kahrad.Pricing.slnx
├── Directory.Build.props
├── Src/
│   ├── BuildingBlocks/                   # Shared primitives, abstractions, cross-cutting infrastructure
│   ├── Hosts/
│   │   ├── Kahrad.Pricing.Api/            # Administrative/application HTTP host (verify name)
│   │   ├── Kahrad.Pricing.PriceInquiry.Api/ # Public, high-throughput price-read host (verify name)
│   │   ├── Kahrad.Pricing.Worker/         # Pricing, sync, scheduled background processing
│   │   └── Kahrad.Pricing.Media.Worker/   # Media-processing background host
│   └── Modules/
│       ├── Pricing/
│       ├── Products/
│       ├── Brands/
│       ├── Representatives/
│       ├── Media/
│       └── Identity/                      # Discussed/under development; inspect repository
├── Frontend/                              # Verify actual folder names
│   ├── Admin/                             # Angular admin app
│   ├── Representatives/                   # Angular representative app
│   ├── Kahrad.Pricing.Web/                # Vue storefront
│   └── Rodoro.Web/                        # Vue storefront
└── tests/                                 # Follow existing test structure
```

The tree above is **illustrative, not a directive to rename or create folders**. Inspect the repository first. `Directory.Build.props` is used; lack of `Directory.Packages.props` was explicitly accepted. Do not force central package management into the project.

### 3.3 Within each module

**[DESIGN DIRECTION]** Keep Domain, Application, Infrastructure and relevant API composition clearly separated, matching the existing structure:

- **Domain:** entities, value objects, business invariants, pure domain services; no EF Core/HTTP coupling.
- **Application:** use-case services, interfaces/abstractions, DTOs, validators, authorization checks, transaction coordination.
- **Infrastructure:** EF Core DbContexts and mapping, external integrations, cache stores, file/media adapters, SMS adapters, background infrastructure.
- **Host/API:** controllers/endpoints, versioning, Swagger, authentication middleware, DI composition; no pricing formula logic in controllers.

Avoid direct references from Domain to Infrastructure and from one module into another module's internal implementation. Prefer explicitly defined inter-module contracts; use DI to wire them at the composition root.

### 3.4 Host responsibilities

| Host | Main responsibility | Explicit boundary |
|---|---|---|
| Admin/Application API | Product/brand/representative management, authentication, translation administration, config and operations | Not the bulk price-compute engine |
| Price Inquiry API | High-throughput read/lookup of published prices from Redis, with appropriate SQL fallback | Does not recompute a price on every read |
| Worker | Pricing refresh, SQL input loading, price computation, publishing, scheduled jobs, accounting sync orchestration | Avoid long-running synchronous admin requests |
| Media.Worker | Image/video processing, variants, metadata generation, delivery preparation | Offload CPU and I/O intensive work from HTTP APIs |

---

## 4. Core modules and responsibilities

### 4.1 Products/Catalog

- Products, categories and product-specific data; reusable feature/specification definitions and assignments.
- Product items used as pricing inputs.
- Localization of product and shared catalog display content, without changing numeric pricing.
- Product publishing, search, catalog projections, import/backfill and data-quality workflows.
- Retain existing keys and columns unless a safe migration demonstrates the need for change.

### 4.2 Pricing

- Reusable pricing formula engine and typed price calculation inputs.
- Brand factors and configurable price policy.
- Representative-sensitive formula inputs/discounts where rules are approved.
- Price publication, effective version, and recalculation trigger handling.
- Keep engine independent of localized product names, descriptions and media.

### 4.3 Brands

- Brand identity and brand-scoped configuration, including factor, commercial percentages, sales percentages, and configurable rounding policy.
- Brand association with products and storefront visibility.
- **[OPEN]** Whether a product belongs to one or multiple brands and how brand-specific availability and price records are modeled.

### 4.4 Representatives

- Representatives/types, access scope, business adjustments and future representative-specific discounts.
- Price inquiry might depend on authorized representative context; avoid treating every price as a single anonymous public amount.
- **[OPEN]** Exact permissions, representative hierarchy, and calculation/discount precedence.

### 4.5 Identity and access control

- ASP.NET Core Identity users and roles with GUID keys; password and SMS OTP authentication.
- JWT access tokens, rotating refresh tokens, revocation, role- and permission-based authorization.
- Auditing for sensitive business activity without recording passwords, OTP values, tokens, credentials or sensitive payloads.
- SMS delivery behind a provider abstraction; keep SMS concerns in a separate module/adapter where appropriate.

### 4.6 Media

- Metadata, original objects, derivative images/videos, upload/processing state, product-media relationships and ordering.
- Processing happens through Media.Worker, not during price inquiry.
- **[OPEN]** Exact storage backend and actual image/video processing formats, CDN strategy, signing and cleanup policies.

### 4.7 Accounting and integration

- Establish an anti-corruption layer around the Mahak/legacy integration contracts.
- Authenticate external API calls, make import/update operations idempotent, preserve source identifiers, record sync outcomes and failures.
- **[PROPOSED]** Use a durable SQL outbox/inbox or job table for reliable retry before adding a separate message broker; this is not a decision to install RabbitMQ or Kafka.

---

## 5. Pricing architecture

### 5.1 Calculation concept

**[DECIDED]** Item-based prices are calculated using a **shared formula** with adjustments. Previously discussed conceptual processing chain:

```text
Product item inputs / item prices
        ↓
Shared beauty/coefficient logic
        ↓
Brand factor (configured; 1–10)
        ↓
Commerce percentage adjustment
        ↓
Sales percentage adjustment
        ↓
Representative type / representative-specific adjustment when applicable
        ↓
Brand-configurable rounding policy
        ↓
Published price
```

**IMPORTANT:** This is an **illustrative ordering, not a formally approved mathematical equation**. Confirm whether each component is a multiplier, additive percentage, markup/discount, or conditional rule. A developer/agent must not turn this outline into a new pricing formula without reviewing existing code and validated sample calculations.

### 5.2 Calculation and publication flow

**[DESIGN DIRECTION]** Previous architecture discussions selected:

```text
Accounting/Mahak API or manual changes
               ↓
        SQL Server inputs
               ↓
    Scheduled / event-triggered Worker
               ↓
         Pricing.Engine library
               ↓
       SQL Server published prices
               ↓
      Redis published-price cache
               ↓
        Price Inquiry API
               ↓
     Web / representative clients
```

- **SQL Server is authoritative** for product inputs, business configuration, and published prices.
- **Redis is a cache, not the source of truth.**
- Price Inquiry **reads Redis first**, with a **controlled SQL fallback** for absent/unavailable cache keys.
- Changes to input data/config should enqueue or schedule recomputation/publishing, not trigger every caller to calculate directly.
- Version/expiry/invalidation behavior must prevent prolonged stale prices; design cache keys for relevant price dimensions (brand, product, representative/type, currency or channel **only when applicable**).
- **[OPEN]** Exact freshness SLA, cache invalidation, reconciliation cadence, locking for concurrent calculations, partial failure handling and transactional cache publication.

### 5.3 Pricing correctness requirements

- Use `decimal`/precise SQL numeric types for monetary amounts; avoid float/double.
- Centralize rounding policy; specify precision and midpoint behavior.
- Validate brand multiplier and percent ranges; preserve explicit configuration precedence.
- Test known pricing examples with expected outcomes before optimization.
- Version or audit critical configuration changes and published prices.
- Never depend on localized product names for price calculations or cache identities.

---

## 6. SQL Server and data ownership

**[DECIDED]** SQL Server + EF Core are the persistence foundation.

- Prefer **module-owned entities and EF configurations**; determine the current approach to module-specific DbContexts versus shared DbContext by inspecting the solution.
- Use migrations safely; do not create unrelated schema changes in a feature PR.
- Explicitly model unique constraints, indexes, FK delete behavior, concurrency and transaction boundaries.
- Preserve identifiers needed for accounting/legacy synchronization and product references.
- Publish/calculate prices in the authoritative data store before (re)building Redis projections.
- **[PROPOSED]** Audit/price history and outbox events as separately queryable tables if not already covered.
- **[OPEN]** Actual SQL Server version, database schemas, tenancy/brand isolation, schema naming, and DbContext locations.

### 6.1 Legacy migration rules

- Inventory includes around **9,000 problem records** requiring classification and validation, not blind deletion.
- Dry-run and report counts; make changes repeatable/idempotent where practicable.
- Backup and verify recovery before risky production migrations.
- Log mapping decisions, per-record failures, and reconciliation counts.
- For old Mahak/product-code mapping, inspect actual `CodeId`, serial and item-key relationships; do not assume old scripts represent final production design.
- Use transactions for each logical unit of change; define rollback behavior.

---

## 7. Multilingual catalog — finalized design

### 7.1 Accepted requirements

**[DECIDED]**

1. Product names must support Persian `fa`, Arabic `ar`, English `en`, and future languages without schema changes per language.
2. Persian is the current **default and fallback** language.
3. All translations are manually entered/maintained by authorized users; no machine translation.
4. API storefront responses are **localized**, rather than returning every translation by default.
5. Select language using `Accept-Language`; use deterministic resolution and fallback.
6. Features and specifications are **shared definitions** reused across products, with their labels translated once.
7. A product's textual specification **value** may require translation; numeric/boolean/date values are language-neutral.
8. Localization must not change the pricing engine or published numeric price cache.

### 7.2 Recommended logical SQL schema

**[DESIGN DIRECTION]** Match existing actual table names, PK types and domain semantics; do not duplicate entities already in the repository.

| Table | Role | Important constraint |
|---|---|---|
| `Languages` | Data-driven language registry (`Code`, `Name`, `IsActive`, `IsDefault`) | `Code` unique; max one default |
| `Products` | Canonical product and nonlocalized product attributes | Preserve existing PK and business keys |
| `ProductTranslations` | Localized product `Name`, optional existing text fields | Unique `(ProductId, LanguageId)` |
| `Features` | Reusable feature definition | Unique stable code if supported |
| `FeatureTranslations` | Shared feature display name | Unique `(FeatureId, LanguageId)` |
| `ProductFeatures` | Product-to-feature assignments and sort ordering | Uniqueness consistent with actual feature semantics |
| `Specifications` | Reusable specification definition/type/unit | Unique stable code if supported |
| `SpecificationTranslations` | Shared specification display label | Unique `(SpecificationId, LanguageId)` |
| `ProductSpecifications` | Assigned specification and typed product-specific value | Respect multi-value/variant semantics |
| `ProductSpecificationTranslations` | Textual product-specific specification values | Unique `(ProductSpecificationId, LanguageId)` |

**[PROPOSED]** If specification values are reusable options already shared across products (e.g. material = wood), translate the **option dictionary** rather than duplicating the same value translation on each product assignment.

### 7.3 Relationship sketch

```text
Languages ───< ProductTranslations >─── Products
Languages ───< FeatureTranslations >─── Features
Products  ───< ProductFeatures >──────── Features
Languages ───< SpecificationTranslations >─── Specifications
Products  ───< ProductSpecifications >─── Specifications
Languages ───< ProductSpecificationTranslations >─── ProductSpecifications
```

- Restrict or otherwise safely guard deletion of a `Language` referenced by translations.
- Choose cascade behavior when deleting an owner according to existing soft-delete/hard-delete policy.
- Backfill existing Persian names into `ProductTranslations` before retiring any old display-name fields.
- A product should have a valid default translation before publication; migration must tolerate incomplete legacy records until repaired.

### 7.4 Localization resolution algorithm

**[DESIGN DIRECTION]**

1. Parse and validate `Accept-Language` including optional quality weights.
2. Try the exact active requested locale, e.g. `en-US`.
3. Try compatible neutral language, e.g. `en`.
4. Fall back to configured default `fa`.
5. If requested translation for an individual resource is missing, fall back to that resource's `fa` translation; define explicit missing-data behavior where no default exists.

Do not infer UI language from free text names or user location. Translate only display strings and textual values; keep IDs, SKU, numeric prices, codes, units and business invariants stable.

### 7.5 Public and administration endpoints

**[DECIDED]** New API operations follow the project's preference for **POST-style endpoints**. Exact route names are **[REPO CHECK]**; examples only:

```http
POST /api/products/search
Accept-Language: ar
Content-Type: application/json

{ "page": 1, "pageSize": 20, "searchTerm": "..." }
```

```http
POST /api/products/details
Accept-Language: en
Content-Type: application/json

{ "productId": "<existing-id>" }
```

Example localized shape (illustrative, not a mandatory replacement for established DTOs):

```json
{
  "id": "existing-product-id",
  "name": "Localized product name",
  "language": "en",
  "features": [{ "id": "feature-id", "name": "Localized feature" }],
  "specifications": [
    { "id": "specification-id", "name": "Material", "value": "Wood" }
  ]
}
```

Admin operations require authorization and may include translation upsert, missing-translation search, and activation controls. Prefer bounded, validated application services and follow the actual route naming, authorization and result conventions.

### 7.6 Translation caching

- Localized catalog response caches **must include resolved language** in keys.
- Changing a translation invalidates only affected localized display projections or equivalent cached responses.
- **Do not invalidate or recompute numeric published prices solely because a name changed.**
- Redis must not be added just to implement localization if the relevant cache abstractions are not available; preserve the established host/cache boundaries.

### 7.7 Tests and migration

- Language matching and fallback: exact, neutral, default, missing per-item translation, inactive language, invalid header.
- Shared feature/specification translations across multiple products.
- Textual versus numeric/boolean specification values.
- Uniqueness constraints, foreign keys, authorization, manual update, and cache invalidation.
- SQL Server migration/backfill from original Persian values; no loss of price/product identity.
- Integration tests against the chosen SQL Server-compatible strategy; don't silently substitute SQLite when SQL Server semantics matter.

For the dedicated Codex implementation spec, use the companion file **`Kahrad-Multilingual-Products-Codex.md`**. This master document supersedes it only where a newer architecture-wide decision is explicitly marked.

---

## 8. Identity architecture — login, roles, permissions, auditing

### 8.1 Requirements

**[DECIDED]**

- ASP.NET Core Identity with **GUID** user IDs and role IDs.
- Password-based login **and** SMS OTP login.
- Separate SMS service/provider abstraction; OTP issuance, delivery and verification should not be hardwired to one SMS gateway.
- JWT access tokens, **refresh-token rotation**, logout/revocation, refresh flow.
- Role-based access control (RBAC) combined with **database-managed permissions**.
- Centralized, reusable audit logging for authentication-sensitive and business changes; exclude secrets.
- .NET 10, EF Core, Clean Architecture and existing controller conventions.

### 8.2 Known application-level files/contracts

Earlier solution inspection showed an Identity Application project with paths akin to:

```text
Modules/Identity/Kahrad.Pricing.Identity.Application/
├── Abstractions/Persistence/
│   ├── IAuditLogRepository.cs
│   ├── IPermissionRepository.cs
│   └── IRefreshTokenRepository.cs
├── Abstractions/Services/
│   ├── IAuditService.cs
│   ├── IAuthenticationService.cs
│   ├── IJwtTokenGenerator.cs
│   ├── IPermissionService.cs
│   ├── IRoleService.cs
│   └── IUserService.cs
├── Auditing/AuditService.cs
├── Authentication/AuthenticationService.cs
└── Authorization/
```

**[REPO CHECK]** These were observed at an earlier point; inspect current files before modifying them.

### 8.3 Suggested endpoint surface

**[DESIGN DIRECTION]** POST operations with actual existing naming conventions:

| Area | Candidate POST action | Notes |
|---|---|---|
| Auth | LoginWithPassword | Credentials → access/refresh tokens |
| Auth | RequestOtp | Rate-limited; generic responses to avoid account enumeration |
| Auth | VerifyOtp / LoginWithOtp | One-time, expiry-bound OTP; protected against replay |
| Auth | RefreshToken | Rotate refresh token atomically |
| Auth | Logout / RevokeToken | Revoke session/token |
| Users | Create / Update / Search / Details | Permissions and audit |
| Roles | Create / Update / AssignToUser / Search | Permission enforcement |
| Permissions | Search / AssignToRole | Database permissions and policy mapping |
| Audit | Search / Details | Restricted read access, safe redaction |

Do not implement route strings blindly; inspect real API controllers, ASP.NET Core Identity integration, authorization policy naming, API response envelope, and DTO standards. Define OTP attempt limits, expiry, resend cooldown and replay protection in configuration and tests; exact values remain **[OPEN]**.

### 8.4 Auditing and security

- Record actor ID, operation, target, timestamp, outcome and correlation identifier when available.
- Never log passwords, OTPs, JWT/refresh tokens, signing keys, personal secrets or raw sensitive requests.
- Grant least-privilege permissions; include automated forbidden/unauthorized tests.
- **[PROPOSED]** Add refresh-token family/reuse detection and account lockout/rate limiting if missing; confirm desired security policy.

---

## 9. API conventions and documentation

**[DECIDED]** ASP.NET Core Web APIs, .NET 10, POST-oriented endpoints, Swagger/OpenAPI.

- Keep controllers thin and delegate work to application services.
- Authentication with JWT Bearer; separate anonymous endpoints from permission-protected administration endpoints.
- Document DTOs, response envelopes, validation errors, authorization, locale behavior, pagination and versioning.
- Support existing API versioning (`Asp.Versioning.Http` was among discussed libraries) **if present/approved**.
- Group Swagger actions consistently using controller/action descriptor data. Earlier discussion included `TagActionsBy` so names like `StoresBaseController` appear under **`Stores`** rather than confusing default tags.
- An earlier configuration used `ResolveConflictingActions(apiDescriptions => apiDescriptions.First())`; do not use that to silently hide genuine route collisions. Fix conflicting routes/signatures when possible.
- **[OPEN]** Canonical route prefix, exact API versioning scheme, stable shared response/error envelope and swagger documentation hosting.

---

## 10. Media and storage

### 10.1 Scale estimates

**[DECIDED]** Approximate input assumptions:

- **35,000** products.
- **5 images** per product at around **2 MB** each → ~**350 GB** of image originals.
- **1–2 videos** per product at around **50 MB** each → ~**1.75–3.5 TB** of video originals.
- Raw originals combined: approximately **2.1–3.85 TB**, before derived sizes, redundancy, backups or delivery copies.

These are capacity **estimates**, not measured production storage volumes. Design storage and bandwidth plans for the full size rather than assuming everything fits on the application server's system disk.

### 10.2 Design direction

- Separate **media service/worker workload** from pricing computation and price inquiry.
- Store metadata in SQL Server; store large binary payloads in a dedicated media storage/server arrangement.
- Worker generates image renditions/metadata and any needed video derivatives asynchronously.
- Preserve stable URLs/identifiers during migration and consider domestic versus international delivery realities.
- Avoid making external object stores (e.g. Cloudflare R2 or MinIO) mandatory; these were discussed as unavailable/not desired in the deployment environment.
- **[OPEN]** Storage implementation, resizing/transcoding profiles, lifecycle cleanup, secure upload authorization, backup strategy for media and cache/CDN delivery topology.

---

## 11. Frontend stack and application separation

### 11.1 Angular applications

**[DECIDED]** Separate Angular applications for internal administration and representatives. Angular is used for admin experiences, not the two brand storefronts. The user previously preferred a **non-standalone / NgModule-oriented** Angular organization for the admin app; preserve its chosen current setup instead of forcing a framework restructuring.

- Shared access control, authentication integration, validation and API client conventions where appropriate.
- Keep representative-facing permissions and data scope distinct from internal administrator capabilities.
- The environment previously used **Angular CLI 22.2.2** and **Node 24.21.0**; verify installed versions rather than hardcoding old toolchain requirements.

### 11.2 Vue storefronts

**[DECIDED]** Two separate blank/initial frontend apps: **`Kahrad.Pricing.Web`** and **`Rodoro.Web`**, using **Vue + TypeScript**, Vue Router, Pinia, Vitest, Playwright, ESLint, Prettier.

- Brand-specific themes/content; reuse nonbranding utilities where useful.
- API content localized by request language; do not translate prices as strings or recalculate them in the browser.
- Support loading, cache failure and stale-data scenarios safely.
- Historical WordPress showcases are intended to be rewritten as modern storefront frontends; migrate content/URLs carefully.
- **[OPEN]** SSR/SEO strategy, component design system, storefront authentication/order flow, actual deployment count and release pipeline.

---

## 12. Hosting, network topology and operational requirements

### 12.1 Deployment principles

**[DECIDED / DESIGN DIRECTION]**

- Primary workloads and SQL storage in **Iran** due to local users and connectivity resilience requirements.
- Prefer **Windows cloud/application hosting** with **SQL Server Standard** where licensing/capacity permit.
- Separate **application/SQL/media** responsibilities when possible; private data services should not be directly public.
- API/edge HTTPS required; auth endpoints, admin endpoints and public price inquiries should have suitable rate limits and network controls.
- Domestic and external storefronts may use different delivery paths; maintain an explicit sync boundary where cross-border connectivity is unreliable.
- Container/Kubernetes dependencies are **not desired**.

### 12.2 Earlier capacity discussion

- Earlier server idea: **32 cores, 64 GB RAM, approximately 300 GB SSD**, subject to budget and actual workload.
- Media server/delivery budget discussed near **$100/month**; this is a planning target, not a verified market quote or sufficient storage budget.
- The large media estimate is **far beyond 300 GB**; distinguish SQL/application disks from media originals and backups.
- **[OPEN]** Actual CPU cores, SQL licensing, disk IOPS, RAM split, bandwidth, storage growth, availability requirements and provider quotation.

### 12.3 Logical network view

```text
         Domestic / International Storefronts (Vue)
                            │
                       HTTPS / edge
                            │
                 Public API / Price Inquiry
                            │
                      Redis Price Cache
                            │ miss / recovery
                      SQL Published Prices

  Angular Admin ── HTTPS ── Admin API ── Modules / SQL Server
                           │
              Pricing Worker / Integration Worker
                           │
                 Mahak / accounting system

     Upload API ─── Media metadata / job queue
                           │
                      Media Worker
                           │
                  Media storage / delivery
```

Use observability (structured logs, health checks, metrics), encrypted backups and restore tests. Keep secrets outside checked-in appsettings and restrict SQL/Redis/media interfaces to appropriate networks.

---

## 13. Supporting libraries and cross-cutting capabilities

Previously discussed/requested libraries and capabilities include the following; **check installed package versions and actual repository usage** before adding anything:

| Technology | Intended role | Status |
|---|---|---|
| Serilog | Structured logging | Requested |
| Polly | Resilience policies, retries/circuit breakers where appropriate | Requested |
| Sieve | Filtering/sorting/pagination where appropriate | Requested |
| `Microsoft.Extensions.Caching.Hybrid` | Optional hybrid cache abstraction | Requested; evaluate per host |
| Redis | Published pricing cache | Architectural direction |
| `Asp.Versioning.Http` | HTTP API versioning | Requested |
| Swagger/OpenAPI | HTTP API docs and JWT Bearer definition | Required |
| ImageSharp | Image media processing | Requested |
| Hangfire | Background jobs | Requested; verify chosen worker design |
| `IPE.SmsIr` | SMS provider adapter | Requested; abstract provider contract |
| AutoMapper | Mapping where appropriate | Requested |
| `AddHealthChecks` | Availability/readiness diagnostics | Requested |
| EF Core SQL Server provider | Durable persistence | Required |

Avoid introducing a package just because it appears on this list. Verify licensing, .NET 10 compatibility, actual need, and coherent ownership. Do not install infrastructure (e.g., a message broker) merely to align with a hypothetical future architecture.

---

## 14. Quality attributes and test strategy

### 14.1 Correctness

- Pricing golden-sample tests for every meaningful formula and policy input.
- Validate representative/brand access scope; no cross-scope data leaks.
- SQL transaction boundaries for published prices, refresh token rotations, and imports.
- Localized API language fallback, non-lossy Persian migration and uniqueness enforcement.

### 14.2 Performance and availability

- Test representative high-throughput price lookup across Redis hit, cache miss, and Redis outage paths.
- Use pagination/projections for large product listings; prevent N+1 queries across translation tables.
- Observe worker backlog, failed price computations, stale published prices and failed media jobs.
- Keep accounting integration delays isolated from browsing/price inquiry.

### 14.3 Security and privacy

- Verify authorization on every admin/representative mutation.
- Rate limit OTP and authentication, protect sensitive audit metadata, store secrets securely.
- Test invalid/forged token, revoked refresh token, replay, and unauthorized translation administration.

### 14.4 Maintainability

- Use unit tests for pure business logic and integration tests for EF/SQL/cache behavior.
- Respect module boundaries and existing code style; refactor incrementally.
- Include migration and rollback notes for any schema change.

---

## 15. Consolidated decision log

| ID | Area | Outcome | State |
|---|---|---|---|
| ADR-001 | Style | Modular monolith, separate deployable hosts | DECIDED |
| ADR-002 | Architecture | Clean Architecture; reject CQRS and Vertical Slice | DECIDED |
| ADR-003 | Runtime | .NET 10 / ASP.NET Core | DECIDED |
| ADR-004 | Database | SQL Server + EF Core | DECIDED |
| ADR-005 | Caching | Redis price-read cache; SQL is source of truth | DESIGN DIRECTION |
| ADR-006 | Processing | Pricing.Engine library; worker calculates and publishes | DESIGN DIRECTION |
| ADR-007 | APIs | POST-oriented endpoints, Swagger/OpenAPI | DECIDED |
| ADR-008 | Identity | GUID Identity users/roles, password + SMS OTP | DECIDED |
| ADR-009 | Authorization | JWT + rotating refresh, RBAC + permissions, auditing | DECIDED |
| ADR-010 | Catalog languages | `fa`, `ar`, `en`, extensible; default `fa` | DECIDED |
| ADR-011 | Translation | Manually managed; public API localized by `Accept-Language` | DECIDED |
| ADR-012 | Catalog schema | Shared features/specifications and translation tables | DECIDED design, schema adaptable |
| ADR-013 | Pricing | Shared formulas; item-based; brand/representative adjustments | DECIDED concepts |
| ADR-014 | Rounding | Currently common; configurable per brand | DECIDED |
| ADR-015 | Frontends | Angular internal/rep panels; Vue Kahrad/Rodoro sites | DECIDED |
| ADR-016 | Media | Separate media processing worker and storage planning | DESIGN DIRECTION |
| ADR-017 | Operations | Iran-primary deployment; no Docker/Kubernetes prerequisite | DECIDED |
| ADR-018 | Legacy | Incremental, safe product/accounting migrations | DESIGN DIRECTION |
| ADR-019 | Formula math | Exact formula/order/rounding values | OPEN |
| ADR-020 | Repository schema | Actual types, names, DbContexts, routes and existing modules | REPO CHECK |
| ADR-021 | Cross-border orders | Offline/sync, acceptance/review and finalization | OPEN |

---

## 16. Open questions — do not silently invent answers

1. Exact pricing formulas with numeric worked examples and sequencing of brand, beauty, commerce, sales and representative adjustments.
2. Definitive product and product-item schema, variants, stock constraints and relationships to brands.
3. Representative hierarchy, price visibility, authorization boundaries, default/overridden discount rules.
4. SQL Server database layout: one DbContext or module-specific contexts, schemas, migrations, key types, indexes, temporal/history requirements.
5. Cache key structure, price publication consistency model, invalidation SLA, Redis disaster behavior and reconciliation schedule.
6. Existing controller and API naming/versioning conventions and exact DTO/response envelope.
7. Accounting API contract, ownership of inventory changes, synchronization conflict resolution and integration error policy.
8. Shipping/order, external order acceptance during Iran outages, payment flow, queue durability and eventual reconciliation.
9. Media storage/delivery choice, image/video derivative standards, upload constraints, backup/restore needs.
10. Which requested packages are already installed, and whether Hangfire/SMS provider/HybridCache is operational.
11. Environment-specific production resources, SQL license, detailed security and backup plan.

Until answered, preserve current behavior and isolate new code behind clear interfaces/configuration. An **OPEN** item is not permission to install an arbitrary framework or invent business logic.

---

## 17. Codex / agent working contract

> The master document is an **architecture reference**, not authorization for a single massive rewrite. Implement in bounded changes, based on the real repository.

### 17.1 Discovery first

1. Read this master document and `Kahrad-Multilingual-Products-Codex.md` when working on localization.
2. Inspect `Kahrad.Pricing.slnx`, `Src/`, existing modules/hosts, current migrations and tests; identify discrepancies.
3. Report current implementation status separately from these architecture decisions.
4. Propose the smallest safe task-specific implementation plan and highlight dependencies.
5. Do not modify unrelated modules or scaffold duplicate projects.
6. If the operator says **“wait until I say START”**, perform planning only; do not edit files, install dependencies or migrate databases until START.

### 17.2 Implementation rules

- Follow existing Domain/Application/Infrastructure/Host layering and code conventions.
- Prefer application services/interfaces; do not generate CQRS commands/handlers or Vertical Slice folders.
- Keep `Pricing.Engine` pure and deterministic; keep HTTP/SQL/Redis outside the formula core.
- Preserve SQL Server migrations and existing data. No destructive cleanup by default.
- Maintain POST API conventions and Swagger documentation.
- Validate authentication/authorization, input and ownership scopes.
- Include tests for all new business invariants and data access behavior.
- Separate localization changes from numeric pricing changes.
- Keep configuration/environment secrets out of source control.
- Do not treat proposed diagrams, names, or formula sequences as fact when repository code disagrees.

### 17.3 Suggested order of work (planning guidance, not a rewrite mandate)

1. Map repository structure and shared conventions; establish build/tests and architectural boundary checks.
2. Finish Identity, permissions, auditing and JWT/OTP contract as a bounded module.
3. Stabilize Products, Brands, Representatives and pricing configuration models.
4. Implement multilingual catalog incrementally, with translation migrations and localized public APIs.
5. Complete price engine golden tests, worker publication, SQL durable store and Redis read path.
6. Complete Media.Worker/upload pipeline and storage capacity model.
7. Wire Angular admin/representative and Vue storefronts to agreed API contracts.
8. Implement/incrementally migrate accounting integration and legacy product correction.
9. Harden logging, health, backups, operational runbooks, security and load tests.

Actual order should follow dependencies in the current repository; do not replace working functionality.

### 17.4 Definition of done for each bounded implementation

- Solution builds with existing .NET 10 target.
- Relevant unit/integration tests pass and new tests cover critical cases.
- Migrations are safe, reviewable and documented.
- API docs and authorization are updated.
- There is no unintended contract/price/legacy data change.
- A concise final report lists changed files, decisions, assumptions, tests run, and any outstanding blockers.

---

## 18. Sources of consolidation / scope note

Prepared from the user's prior Kahrad architecture conversations (including the November 2–5, 2026 solution, Identity, Swagger and multilingual product discussions) and the existing companion Markdown task. It records past outcomes and reasonable design guidance; **it is not an inspection of the live GitHub repository or current production deployment**. Do not claim components are implemented until verified in code.
