# AGENTS.md

# Product Pricing & Management System — Engineering Guidelines

## 1. Project Overview

This repository contains a product management, pricing, representative management, and sales-support system for two brands operating in Iran and international markets.

The system must support:

- Two independent brands with separate configurations.
- Product and product-item management.
- Product pricing and price recalculation.
- Representative management and representative-specific pricing.
- Product offer lists.
- Product sales registration.
- Barcode and product identity-card printing.
- User management and menu-based permissions.
- SMS notifications.
- Product images and videos.
- Integration with the accounting system.
- Price inquiry APIs for internal and external consumers.
- Future support for international deployments and independent regional read models.

The current product catalog contains approximately 35,000 products. Some existing product data requires correction and migration.

The architecture must prioritize maintainability, predictable behavior, operational simplicity, and performance.

The primary developer and maintainer is a single developer. Avoid unnecessary abstractions, infrastructure complexity, and overengineering.

---

## 2. Non-Negotiable Architecture Decisions

The following decisions are mandatory and must not be changed without explicit approval.

### Architecture

- Use **Clean Architecture**.
- Use a **Modular Monolith**.
- Use classic layered architecture with Service and Repository patterns.
- Apply DDD only where it provides real value, particularly in Pricing and Beauty Formula domains.
- Keep simple CRUD modules simple.
- Use separate projects for a module only when its complexity and boundaries justify them.
- Keep business logic out of controllers, infrastructure classes, and background job definitions.

### Explicitly prohibited

Do NOT introduce:

- Vertical Slice Architecture.
- CQRS.
- MediatR.
- Microservices.
- Event Sourcing.
- Docker.
- Kubernetes.
- Unnecessary generic repositories.
- Unnecessary interfaces and abstractions.
- Unnecessary domain events or messaging infrastructure.
- RabbitMQ or Kafka.
- Distributed infrastructure that is not justified by an actual requirement.

Do not replace the agreed architecture with a different architectural style because of personal preference or coding conventions.

### Decision-making rules

Before changing any agreed architectural decision:

1. Explain the problem with the current approach.
2. Describe the proposed alternative.
3. Explain its operational and maintenance impact.
4. Wait for explicit approval.

Do not silently change business rules, pricing formulas, module boundaries, database ownership, or infrastructure assumptions.

---

## 3. Technology Stack

Use the following technologies and packages as the project's standard stack.

| Area | Technology |
|---|---|
| Runtime | .NET / ASP.NET Core |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core |
| Performance-oriented queries | Dapper when justified |
| Background jobs | Hangfire |
| Distributed caching | Redis |
| Cache abstraction | Microsoft.Extensions.Caching.Hybrid |
| Resilience | Polly |
| Filtering, sorting, pagination | Sieve |
| API versioning | Asp.Versioning.Http |
| API documentation | Microsoft.AspNetCore.OpenApi |
| API documentation UI | Scalar.AspNetCore |
| Image processing | SixLabors.ImageSharp |
| SMS provider | IPE.SmsIr |
| Logging | Serilog |
| Object mapping | AutoMapper |
| Validation | FluentValidation |
| Health monitoring | ASP.NET Core Health Checks |
| Testing | xUnit |

### Package management

- Use Central Package Management through `Directory.Packages.props`.
- Keep package versions centralized.
- Select package versions compatible with the selected .NET target framework.
- Do not install duplicate or overlapping packages without a concrete reason.
- Do not upgrade major package versions without checking compatibility.
- Do not introduce alternative libraries for capabilities already covered by the approved stack without approval.

---

## 4. Solution Structure

Use the following structure as the architectural guideline.

```text
ProductPricing.sln

src/
  BuildingBlocks/
    Domain/
    Application/
    Infrastructure/

  Modules/
    Products/
      Products.Domain/
      Products.Application/
      Products.Infrastructure/

    Brands/
      Brands.Domain/
      Brands.Application/
      Brands.Infrastructure/

    Representatives/
      Representatives.Domain/
      Representatives.Application/
      Representatives.Infrastructure/

    Pricing/
      Pricing.Domain/
      Pricing.Application/
      Pricing.Infrastructure/
      Pricing.Engine/

  Hosts/
    ProductManagement.Api/
    PriceInquiry.Api/
    Pricing.Worker/

tests/
  UnitTests/
  IntegrationTests/
  ArchitectureTests/
```

This is a logical reference structure, not a requirement to create every project immediately.

Do not create separate projects for simple modules when doing so adds no meaningful isolation or maintainability.

The Pricing module requires stronger separation because its calculation logic must remain independent of persistence, caching, HTTP, and infrastructure.

### Layer responsibilities

#### Domain

Contains:

- Entities.
- Value Objects.
- Domain Services.
- Business Rules.
- Domain Exceptions.
- Enums.

Rules:

- Must not depend on Infrastructure.
- Must not depend on ASP.NET Core.
- Must not depend on EF Core.
- Must not depend on Redis or external providers.
- Must contain business rules rather than database or transport concerns.

#### Application

Contains:

- Application Services.
- DTOs.
- Service Interfaces.
- Repository Interfaces.
- Validators.
- Mapping Profiles where appropriate.
- Application-level orchestration.

Rules:

- Coordinates business operations.
- Must not contain HTTP-specific implementation details.
- Must not directly depend on infrastructure implementations.
- Must not become a CQRS or MediatR implementation.

#### Infrastructure

Contains:

- EF Core DbContexts and configurations.
- Repository implementations.
- SQL Server persistence.
- Redis integration.
- External system integrations.
- SMS provider implementation.
- File storage implementations.
- Infrastructure-specific services.

#### Presentation / Hosts

Contains:

- Controllers or HTTP endpoints.
- Dependency injection composition.
- Middleware.
- Authentication and authorization configuration.
- API-specific request and response handling.
- Host-specific configuration.

#### Pricing.Engine

Contains pure pricing calculation logic.

It must remain independent of:

- SQL Server.
- EF Core.
- Redis.
- HybridCache.
- ASP.NET Core.
- HTTP.
- Hangfire.
- External services.
- Database repositories.

It must accept calculation inputs and return calculation results.

It must be deterministic and independently unit-testable.

---

## 5. Dependency Rules

Follow Clean Architecture dependency direction.

- Domain must not depend on Application or Infrastructure.
- Application may depend on Domain.
- Infrastructure may depend on Application and Domain.
- Presentation may depend on Application and register Infrastructure implementations.
- Pricing.Engine must not depend on infrastructure or presentation projects.

Avoid circular project references.

Do not introduce dependencies between modules merely to reuse a small helper.

Cross-module communication should use explicit application-level contracts or shared abstractions where justified.

Do not expose EF Core entities as public API contracts.

---

## 6. Business Modules

The system includes the following functional areas.

### Products

Responsibilities:

- Product creation and editing.
- Product item management.
- Product grouping.
- Product-related pricing factors.
- Product publishing.
- Product images and videos.
- Barcode printing.
- Product identity-card printing.

Product data must preserve the distinction between product information, product items, and factors affecting pricing.

### Brands

Responsibilities:

- Brand management.
- Independent brand pricing configurations.
- Brand-specific product and pricing settings.

Each brand must have independent pricing configuration.

Do not assume that configuration values are shared between brands.

### Representatives

Responsibilities:

- Representative management.
- Representative types.
- Representative-specific pricing.
- Representative-related sales operations.

The calculated price must be identifiable by at least:

`BrandId + ProductId + RepresentativeTypeId`

### Pricing

Responsibilities:

- Pricing configuration.
- Pricing calculation orchestration.
- Pricing Engine integration.
- Recalculation requests.
- Calculation runs.
- Price version management.
- Publishing calculated prices.
- Price inquiry support.

Pricing is a core domain. Keep its business rules explicit and testable.

### Offer Lists

Responsibilities:

- Creating lists of products offered together.
- Managing the products included in each list.
- Calculating the offer-list price.

The currently described business rule is that the offer-list price is based on the average product price multiplied by the quantity.

Do not implement this formula until the exact interpretation of quantities, averaging, and rounding has been confirmed with the business owner.

### Sales

Responsibilities:

- Recording products sold to representatives.
- Recording the relevant product price at the time of sale.
- Integrating finalized sales with the accounting system.

A historical sale must not change merely because the current product price changes.

### Users and Permissions

Responsibilities:

- User management.
- Authentication and authorization.
- Menu-based access permissions.

Do not treat frontend menu visibility as a replacement for backend authorization.

Every protected operation must enforce authorization on the server.

### SMS

Responsibilities:

- SMS configuration.
- SMS templates.
- Sending and tracking messages.
- Integration with SMS.ir.

---

## 7. Pricing Architecture

Pricing calculation must be separated from price delivery.

The system uses three main components:

1. Pricing.Engine.
2. Pricing Worker.
3. Price Inquiry API.

SQL Server remains the source of truth.

Redis is only a cache.

### 7.1 Pricing.Engine

Pricing.Engine is a pure calculation library.

Responsibilities:

- Accept normalized pricing inputs.
- Apply the approved pricing rules.
- Calculate the resulting price.
- Return calculation details and results.

It must not:

- Read from SQL Server.
- Write to SQL Server.
- Access Redis.
- Call external APIs.
- Load configuration from environment variables.
- Perform HTTP requests.
- Execute background jobs.

The engine must be testable without a database or external infrastructure.

Do not place business calculation formulas inside controllers, repositories, Hangfire jobs, or Redis services.

### 7.2 Pricing Worker

The Pricing Worker is responsible for preparing calculated prices.

Its workflow:

1. Retrieve pending recalculation work from SQL Server.
2. Load the required product data.
3. Load product items and pricing factors.
4. Load brand-specific pricing configuration.
5. Load representative-type pricing configuration.
6. Build calculation input models.
7. Invoke Pricing.Engine.
8. Persist calculated prices in SQL Server.
9. Validate the calculation run.
10. Publish the new price version only after successful completion.

The worker must not use Redis as a source of calculation data.

The worker must not require Redis availability to calculate or persist prices.

### 7.3 Price Inquiry API

The Price Inquiry API provides already-calculated prices.

It must not calculate prices during normal API requests.

Expected behavior:

1. Resolve the active price version for the requested brand.
2. Check HybridCache.
3. On a cache hit, return the cached result.
4. On a cache miss, read the price from SQL Server.
5. Populate the cache.
6. Return the result.

If Redis is unavailable, the API must fall back to SQL Server.

Redis failure must not cause incorrect pricing or prevent price inquiry when SQL Server is available.

### 7.4 SQL Server

SQL Server is the authoritative source for:

- Product data.
- Brand configuration.
- Representative configuration.
- Pricing configuration.
- Calculation requests.
- Calculation run status.
- Calculated prices.
- Active price versions.

Do not make Redis the only place where a calculated price or active version exists.

---

## 8. Price Versioning and Publishing

Use version-based price publishing.

Each brand must have an independent active price version.

Example:

- Brand A active version: 42.
- Brand B active version: 17.

When recalculation starts:

1. Create a new calculation run.
2. Allocate a new version for the relevant brand.
3. Calculate prices for that version.
4. Persist the results without making them active.
5. Validate completion and consistency.
6. Atomically update the active version pointer.

Only a successfully completed and validated version may become active.

If calculation fails:

- Keep the previous version active.
- Do not expose incomplete prices.
- Preserve failure information for investigation.
- Allow safe recovery or retry.

Suggested domain-level persistence concepts:

- `PriceCalculationRun`
- `ActivePriceVersion`
- `PriceRecalculationRequest`
- `ProductPrice`

These names are conceptual guidelines. Reuse existing project conventions where appropriate.

### Atomic publishing

The active version update must be atomic.

API consumers must never observe a partially calculated version.

The previous version must remain usable until the new version is successfully published.

### Versioned cache keys

Use version-based cache keys.

Example:

```text
price:v42:brand:1:product:1250:representative:3
price:active-version:brand:1
```

The active version must be resolved independently per brand.

When a new version is published, consumers should start using the new version's keys.

Do not require deleting every old cache entry during publication.

Old entries can expire according to their TTL.

Initial cache TTL may be 24 hours and must remain configurable.

Cache DTOs must be immutable or treated as immutable.

Do not cache mutable EF Core entities.

---

## 9. Pricing Business Rules and Configuration

Each brand has independent pricing settings.

The known configuration categories include:

- Brand price multiplier, commonly between 1 and 10.
- Beauty coefficient.
- Commercial percentage.
- Sales percentage.
- Rounding formula.

Beauty, commercial, and sales coefficients must support configurable names and values per brand.

The rounding formula is currently expected to be shared, but the design must allow it to become brand-specific later.

### Critical business-rule restriction

The exact order and formula for applying coefficients, percentages, and rounding have not yet been finalized.

Do not invent or assume:

- The order of applying multipliers and percentages.
- Whether percentages are additive or sequential.
- Whether a coefficient applies to the base price or an adjusted price.
- Intermediate rounding behavior.
- Final rounding precision.
- Treatment of zero or null values.
- Whether a pricing factor is optional or mandatory.

Before implementing or changing pricing formulas, obtain explicit confirmation and at least one real calculation example with expected results.

Represent approved pricing rules explicitly rather than scattering constants and formulas throughout the codebase.

Do not hardcode brand-specific values in Pricing.Engine.

The engine must receive the required configuration as calculation input.

---

## 10. Recalculation and Background Processing

Use Hangfire for background job scheduling and execution.

### Host

Run Hangfire Server in the independent:

`Pricing.Worker`

Host.

The Worker must run as a Windows Service in production.

Do not put pricing calculation logic directly inside Hangfire job methods.

Hangfire jobs should invoke application services that coordinate the actual business operation.

### Hangfire responsibilities

Hangfire is responsible for:

- Scheduling jobs.
- Executing jobs.
- Retrying eligible failed jobs.
- Managing job execution.
- Running recurring jobs where required.

Hangfire is not the authoritative owner of business calculation state.

### Business state

Keep business-level status and recovery information in SQL Server domain tables, particularly:

`PriceRecalculationRequest`

and calculation-run records.

Hangfire storage and domain business state serve different purposes.

Do not replace business-level job state with Hangfire dashboard status.

### Job design

Jobs must be:

- Idempotent where possible.
- Safe to retry.
- Observable through structured logs.
- Designed to avoid conflicting executions.
- Resistant to partial failures.

Prevent conflicting simultaneous publication operations for the same brand.

Use SQL Server coordination or locking where required.

Do not introduce distributed messaging infrastructure.

### Recalculation scope

Full recalculation is acceptable for the initial implementation.

Do not introduce complex incremental recalculation infrastructure prematurely.

However, avoid designs that make future incremental recalculation unnecessarily difficult.

The exact behavior after a single product or pricing factor changes must follow the approved business requirement. Do not assume whether that change triggers a product-level or brand-wide recalculation.

---

## 11. API Design

Use ASP.NET Core APIs.

Use `Asp.Versioning.Http` for API versioning.

Prefer URL-segment versioning.

Examples:

```text
/api/v1/products
/api/v1/products/{id}
/api/v1/products/{id}/price
/api/v1/products/prices
```

### Price inquiry endpoints

Expected endpoints include:

```http
GET /api/v1/products/{id}/price
```

For batch requests:

```http
POST /api/v1/products/prices
```

Batch requests must have a configurable maximum number of product IDs.

Price inquiry must support the relevant brand and representative type.

The API must return prepared prices, not calculate prices on demand.

### API standards

- Use explicit request and response DTOs.
- Validate incoming requests.
- Use consistent HTTP status codes.
- Return consistent error responses.
- Avoid exposing database implementation details.
- Avoid returning EF Core entities.
- Support cancellation tokens for asynchronous operations.
- Apply pagination to collection endpoints.
- Apply authorization to protected endpoints.
- Keep controllers thin.

### API documentation

Use:

- `Microsoft.AspNetCore.OpenApi`
- `Scalar.AspNetCore`

Document:

- All public endpoints.
- Request and response models.
- Validation errors.
- Authentication requirements.
- API version information.
- Relevant status codes.

Do not leave new endpoints undocumented.

---

## 12. Sieve: Filtering, Sorting, and Pagination

Use Sieve for filtering, sorting, and pagination where appropriate.

Rules:

- Apply filtering and pagination at database level.
- Do not load all records into memory before filtering.
- Whitelist allowed filter and sort properties.
- Set reasonable pagination limits.
- Do not expose sensitive or internal database properties.
- Use stable sorting when pagination requires deterministic ordering.

Do not use Sieve for operations where a simple explicit query is clearer and more appropriate.

---

## 13. Caching Strategy

Use:

`Microsoft.Extensions.Caching.Hybrid`

as the application-level caching abstraction.

Use Redis as the distributed cache provider.

### Rules

- SQL Server is the source of truth.
- Redis is a performance optimization.
- Cache misses must be recoverable from SQL Server.
- Redis outages must not break business correctness.
- Cache only data that is safe to reuse.
- Use version-based cache keys for calculated prices.
- Keep TTL values configurable.
- Avoid unnecessary cache invalidation complexity.

Do not implement a second caching abstraction alongside HybridCache without a clear technical reason.

Do not store critical business state exclusively in Redis.

---

## 14. Resilience and Polly

Use Polly for resilience around external and unreliable operations.

Potential use cases:

- Accounting integration.
- SMS provider calls.
- Other external HTTP integrations.

Use appropriate:

- Timeouts.
- Retry policies.
- Circuit breakers.
- Rate limiting where justified.

### Retry restrictions

Do not blindly retry non-idempotent operations.

Before enabling retries, determine whether the operation can safely be repeated.

For SMS and accounting operations, prevent duplicate side effects where possible through idempotency or provider-supported mechanisms.

Do not add retry policies to every method by default.

Keep resilience configuration centralized and observable.

Avoid duplicate resilience implementations for the same integration.

---

## 15. Accounting Integration

Accounting integration must be isolated behind a dedicated integration boundary.

The accounting system may provide input data used in pricing calculations.

The system also needs to register finalized sales and relevant prices in accounting.

### Inbound integration

Expected flow:

1. Receive accounting data.
2. Validate and normalize it.
3. Persist accepted data.
4. Trigger the appropriate recalculation workflow when necessary.

### Outbound integration

Expected flow:

1. Finalize the sale in the application.
2. Persist the sale and its accounting integration request reliably.
3. Process the integration asynchronously.
4. Retry eligible failures safely.
5. Record the outcome.

Use a transactional outbox where necessary to ensure that a committed business operation is not lost before integration.

Use idempotency to prevent duplicate accounting records.

### Important restrictions

The actual accounting integration mechanism is not yet confirmed.

It may be an API or database-based integration.

Do not assume the accounting system's API, schema, authentication mechanism, or data contract.

Do not directly couple business services to accounting database tables.

Keep accounting-specific code inside Infrastructure.

---

## 16. SMS Integration

Use the `IPE.SmsIr` package for SMS.ir integration.

The SMS provider SDK must only be used in Infrastructure.

Application services should depend on an application-level abstraction such as:

`ISmsSender`

The implementation should encapsulate SMS.ir-specific behavior.

### Requirements

- Keep provider credentials in secure configuration.
- Never hardcode API keys.
- Support the required transactional and template SMS scenarios.
- Record relevant sending attempts and provider responses.
- Use timeouts and appropriate resilience policies.
- Avoid duplicate messages when retrying.
- Keep message templates configurable where required.

Do not call the SMS provider directly from controllers or Domain entities.

Use Hangfire for deferred SMS processing when asynchronous delivery is required.

Do not assume that every SMS failure is safe to retry.

---

## 17. Logging with Serilog

Use Serilog for structured logging in all application hosts.

Logging should support:

- Request correlation.
- Error investigation.
- Background job tracing.
- Pricing calculation diagnostics.
- External integration diagnostics.

Include relevant identifiers where available:

- Correlation ID.
- Brand ID.
- Product ID.
- Calculation Run ID.
- Price Version.
- Hangfire Job ID.

### Logging restrictions

Never log:

- Passwords.
- API credentials.
- Authentication tokens.
- OTP codes.
- Unnecessary personal information.
- Sensitive request payloads.

Use appropriate log levels.

Do not use excessive informational logging inside loops or high-frequency operations.

Exceptions should retain sufficient diagnostic details without exposing secrets.

---

## 18. AutoMapper

Use AutoMapper for appropriate object-to-object mapping.

Organize mapping profiles by module or functional area.

Rules:

- Keep mapping configuration understandable.
- Do not put business logic inside mapping profiles.
- Do not hide complex domain transformations inside AutoMapper.
- Use explicit code when transformation behavior is complex.
- Do not allow request-to-entity mapping to overwrite protected or sensitive fields.
- Review collection mapping behavior carefully.

Pricing.Engine must not depend on AutoMapper.

Do not use mapping as a substitute for business validation.

---

## 19. Validation

Use FluentValidation for request and application-level validation.

Rules:

- Validate user input at application boundaries.
- Keep business invariants in the appropriate Domain layer.
- Reuse validation rules where appropriate.
- Avoid duplicating the same validation in controllers and services.
- Return consistent validation errors.
- Do not trust frontend validation.

Validation must not depend on hidden assumptions about business rules.

---

## 20. Image and Video Management

Media files must not be stored directly in SQL Server.

Store metadata and storage references in SQL Server.

Use `SixLabors.ImageSharp` for image processing.

### Images

Support:

- Original image storage.
- Medium image variants, approximately 800×800.
- Thumbnail variants, approximately 200×200.
- WebP or AVIF output where supported and appropriate.
- Resizing and optimization.
- Dimension validation.

Validate actual file content.

Do not trust file extensions or user-provided MIME types.

Apply reasonable limits for:

- File size.
- Image dimensions.
- Processing resources.
- Supported formats.

Do not allow untrusted image files to consume unlimited CPU or memory.

### Videos

Store video files outside SQL Server.

Support HTTP Range requests and partial content responses where required.

Use MP4 as the initial standard delivery format.

Generate thumbnails using FFmpeg when needed.

Do not introduce HLS unless an actual streaming requirement justifies it.

### Storage architecture

Media storage should be independent of application database storage.

The initial infrastructure may use:

- Linux filesystem with Nginx.
- NAS storage.
- Appropriate internal network storage.

Do not introduce MinIO, Cloudflare R2, or unnecessarily complex object-storage infrastructure.

Media storage must be considered separately for Iran and international deployments.

A CDN cache alone is not a replacement for a required regional media replica.

RAID is not a backup strategy.

---

## 21. Health Checks

Use ASP.NET Core Health Checks through `AddHealthChecks()`.

Provide the following endpoints where appropriate:

```text
/health
/health/ready
/health/live
```

### Liveness

Liveness must determine whether the process is running and responsive.

It should not fail simply because an optional external dependency is unavailable.

### Readiness

Readiness should check dependencies required for the host to perform its intended role.

Potential checks include:

- SQL Server.
- Redis.
- Hangfire storage.
- Other mandatory external dependencies.

### Redis-specific rule

Redis is an optimization layer.

If Price Inquiry API can successfully serve requests from SQL Server without Redis, Redis failure must not automatically make the API unready.

### Security

Never expose:

- Connection strings.
- Credentials.
- Internal infrastructure details.
- Sensitive exception data.

in public health-check responses.

---

## 22. Database and Persistence

Use SQL Server as the primary database.

Use EF Core for standard persistence operations.

Use Dapper only when a measurable performance or query-complexity benefit justifies it.

### General rules

- Keep database access inside Infrastructure.
- Use asynchronous database operations.
- Pass cancellation tokens.
- Avoid N+1 queries.
- Avoid unnecessary eager loading.
- Use projections for read-heavy queries.
- Use transactions for operations that require atomicity.
- Create appropriate indexes for frequently queried columns.
- Review query execution plans for performance-sensitive operations.
- Avoid loading large datasets into application memory.

### Production safety

Never execute destructive production SQL operations without explicit authorization.

Do not issue UPDATE, DELETE, or INSERT against production application tables unless the user explicitly authorizes the operation.

For production investigation, prefer read-only SELECT queries.

### Data migration

Data migration and cleanup must be treated as explicit project activities.

Do not silently modify legacy business data during ordinary application startup.

Profile and report data-quality problems before implementing automated cleanup.

---

## 23. Frontend and Regional Deployment

The backend must support multiple frontend applications.

Iran deployment includes:

- Product pricing administration panel.
- Representative administration panel.
- First brand website.
- Second brand website.

International deployment includes:

- First brand website.
- Second brand website.

The administration panels are planned with Angular.

Brand websites may use React or Vue and may be developed separately.

Backend APIs must provide stable, documented contracts for all consumers.

### Regional considerations

The international environment must not depend on uninterrupted connectivity to Iran.

The architecture should support a local product and price read model for international price inquiries.

Order intake from international systems must distinguish between:

- Received.
- Confirmed.

An international system must not mark an order as finally confirmed before the required validation in Iran.

Use reliable synchronization, idempotency, and transactional outbox patterns where required.

Do not introduce distributed infrastructure without a demonstrated need.

---

## 24. Authentication and Authorization

Use the project's approved authentication mechanism.

Apply authorization at the backend endpoint or application-service boundary as appropriate.

Rules:

- Do not rely on frontend-only permission checks.
- Protect administrative operations.
- Enforce menu permissions consistently.
- Avoid exposing sensitive user information.
- Keep secrets outside source control.
- Follow least-privilege principles.

Do not introduce a new identity provider or authentication architecture without approval.

---

## 25. Performance Requirements

The system must support approximately 35,000 products and daily price updates.

Price inquiry requests are expected to be moderate in volume.

The design must:

- Avoid calculation during normal price inquiry requests.
- Use precomputed prices.
- Use Redis through HybridCache for frequently accessed prices.
- Fall back to SQL Server when Redis is unavailable.
- Support batch price inquiries.
- Limit batch request size.
- Use database-level filtering and pagination.
- Avoid unnecessary full-table scans.
- Avoid loading the entire catalog into memory without a justified reason.

A few minutes of delay for daily price updates is acceptable.

Do not introduce premature distributed scaling or complicated infrastructure to solve hypothetical traffic problems.

---

## 26. Testing Strategy

Use xUnit.

Tests should cover:

### Unit tests

- Pricing.Engine calculations.
- Pricing business rules.
- Validation rules.
- Domain behavior.
- Important application-service logic.

Pricing.Engine tests must not require SQL Server, Redis, or external services.

### Integration tests

- SQL Server persistence.
- Repository behavior.
- API endpoints.
- Authentication and authorization.
- Hangfire integration where necessary.
- Redis cache behavior.
- Accounting integration boundaries.
- SMS integration boundaries.

Use test-specific infrastructure and configuration.

Do not make unit tests dependent on production services.

### Pricing test requirements

Every approved pricing formula must have tests with explicit input and expected output.

Include edge cases such as:

- Zero values.
- Null values where permitted.
- Large values.
- Decimal precision.
- Rounding boundaries.
- Different brands.
- Different representative types.

Do not write tests that merely repeat an unverified assumption about the pricing formula.

---

## 27. Code Quality and Implementation Rules

- Use clear, descriptive names.
- Prefer simple and readable implementations.
- Follow the existing project naming conventions.
- Keep methods focused.
- Avoid unnecessary abstraction layers.
- Avoid excessive inheritance.
- Avoid unnecessary generic frameworks.
- Use dependency injection appropriately.
- Use `async`/`await` for asynchronous I/O.
- Pass cancellation tokens through application and infrastructure calls.
- Handle exceptions at appropriate boundaries.
- Do not swallow exceptions.
- Do not use exceptions for normal business control flow.
- Do not put business logic in controllers.
- Do not put business logic in repository implementations.
- Do not put pricing formulas in background job classes.
- Do not use static mutable state for application business data.
- Do not hardcode environment-specific values.
- Keep configuration strongly typed and validated.

Do not refactor unrelated code merely because it could be stylistically improved.

When fixing a bug, keep the change focused unless a broader correction is necessary and approved.

---

## 28. Configuration and Secrets

Use standard .NET configuration mechanisms.

Keep environment-specific configuration outside source code.

Never commit:

- Passwords.
- API keys.
- Connection strings containing credentials.
- Private certificates.
- Production secrets.

Use secure production configuration.

Validate required configuration during application startup.

Keep brand-specific business settings in the database rather than hardcoding them in application configuration.

---

## 29. Infrastructure and Deployment Constraints

The initial production environment is based on Windows Server and SQL Server.

Expected deployment:

| Component | Deployment |
|---|---|
| ProductManagement.Api | IIS |
| PriceInquiry.Api | IIS |
| Pricing.Worker | Windows Service |
| SQL Server | Native Windows installation |
| Hangfire Storage | SQL Server |
| Redis | Linux native installation or Memurai on Windows |
| Media Storage | Linux filesystem/Nginx or NAS |

### Explicit restrictions

- Do not use Docker.
- Do not use Kubernetes.
- Do not require container-based deployment.
- Do not introduce infrastructure that requires a dedicated operations team.
- Do not install Redis on the SQL Server host by default without a justified infrastructure constraint.

Prefer a separate Linux Redis host on the internal network.

If Windows-native Redis compatibility is required, evaluate Memurai.

Do not assume that Redis officially supports native Windows deployment.

---

## 30. Development Workflow for AI Agents

AI agents working in this repository must follow these rules.

### Before implementation

1. Inspect the existing solution structure.
2. Read the relevant project files.
3. Identify the affected modules and layers.
4. Check existing conventions and reusable implementations.
5. Understand the requested behavior.
6. Identify missing business requirements.
7. Explain any architectural impact before making significant changes.

### When requirements are unclear

Ask a focused question instead of guessing.

This is particularly important for:

- Pricing formulas.
- Rounding rules.
- Brand-specific behavior.
- Representative-specific behavior.
- Offer-list calculations.
- Accounting data contracts.
- Recalculation scope.
- Regional order confirmation.

Do not invent business requirements to make implementation easier.

### During implementation

- Follow the agreed architecture.
- Make the smallest reasonable change.
- Preserve existing behavior unless a change is requested.
- Do not introduce prohibited patterns or technologies.
- Do not modify unrelated files unnecessarily.
- Do not add packages without justification.
- Do not change database schemas without understanding their impact.

### Before completion

1. Build affected projects.
2. Run relevant tests.
3. Fix compilation errors introduced by the change.
4. Review dependency direction.
5. Review security and validation.
6. Review database query performance where relevant.
7. Report changed files.
8. Report tests and build results.
9. Explicitly mention any remaining assumptions or unresolved requirements.

Never claim that a build or test passed unless it was actually executed successfully.

### Approval requirement

For significant changes to architecture, database design, pricing rules, infrastructure, or public API contracts:

- Present the proposed change.
- Explain the reason and impact.
- Wait for explicit approval before proceeding.

Do not interpret a request to investigate or propose a solution as authorization to implement it.

---

## 31. Current Open Business Decisions

The following matters require explicit business confirmation before their behavior is finalized.

1. Exact order of applying brand multiplier, beauty coefficient, commercial percentage, and sales percentage.
2. Exact rounding formula and intermediate rounding behavior.
3. Whether a change to one product triggers recalculation for that product or the entire brand.
4. Exact offer-list calculation, including quantity handling and rounding.
5. Accounting integration mechanism and data contracts.
6. Whether accounting provides raw material/item prices or final product cost data.
7. Exact international order validation and confirmation workflow.

Do not silently resolve these decisions in code.

When one of these decisions becomes relevant, ask for the required clarification and record the approved rule in the appropriate project documentation.

---

## 32. Final Engineering Principle

Prefer a system that is:

- Correct.
- Maintainable.
- Understandable by one developer.
- Predictable under failure.
- Efficient for the expected workload.
- Easy to deploy and operate.
- Extensible without unnecessary complexity.

Do not optimize for architectural fashion.

Do not add complexity without a real requirement.

**Preserve the agreed Clean Architecture + Modular Monolith design, keep Pricing.Engine pure, keep SQL Server authoritative, use Redis only as a cache, and never invent unapproved business rules.**