# Host operations

## Logging

All four hosts use Serilog 10 with structured JSON console output. Minimum level defaults to Information, with Microsoft framework logs at Warning, and can be adjusted through the standard `Serilog` configuration section. Add an operational sink through reviewed configuration if durable aggregation is required; console output alone is not log retention.

The shared host support is in BuildingBlocks.Infrastructure. Because it includes ASP.NET Core operations support, framework-dependent deployments of the workers also require the matching ASP.NET Core shared runtime; alternatively publish the deployment with its appropriate self-contained runtime. No container is needed.

HTTP request logs omit bodies, query strings and authorization headers. X-Correlation-ID accepts 1?64 ASCII letters/digits/underscore/hyphen, otherwise the generated server trace ID is used. The selected ID is included in response headers, structured logs and the existing audit correlation path. Health successes log at Debug to avoid high-volume Information noise. Existing logging sites must still be reviewed as new integrations are added; shared logging does not automatically redact arbitrary values supplied by application code.

## API health

- GET /health/live: process responsiveness, independent of SQL/Redis/provider availability.
- GET /health/ready: required-role dependencies, 200 or 503.
- GET /health: same readiness aggregate.

Responses expose only `status`, never connection strings, individual dependency names or exceptions, and disable caching. These operational GET routes are intentional probes and do not change business API POST conventions.

The management API checks configured IdentityDb, BrandsDb, ProductsDb, GuaranteesDb and SmsDb connections, each falling back to DefaultConnection, deduplicating identical connections. Each SQL probe bounds connection and command waits and executes SELECT 1. Apply and verify migrations separately: connectivity is not a schema-deployment check. Redis is not required by these probes.

Price Inquiry deliberately fails readiness while its published-price read service is missing. Its liveness succeeds. The former weather demo was removed; no computed/fake price route is substituted. Replace the explicit incomplete-role check only when SQL-backed published prices and the cache fallback are actually functional and tested.

## Workers

Kahrad.Pricing.Worker runs Hangfire and the existing SMS outbox; it no longer starts a template one-second heartbeat loop. Pricing and accounting jobs are still missing. Windows Service lifetime is enabled with service name `Kahrad Pricing Worker`.

Media.Worker has Windows Service lifetime under `Kahrad Media Worker`; it logs once that processing is unconfigured and waits for shutdown. It does not process media. Neither worker opens a new HTTP health port in this change. Monitor Windows Service lifetime and Hangfire as appropriate; backlog/failed-job operational monitoring is still required.

## Administrative protections

Admin may manage ordinary users, but only SuperAdmin can mutate an existing SuperAdmin's profile, password, status or roles through the API. New SuperAdmin-role assignment checks are case-insensitive. Raw POST /api/sms/send-otp requires Admin/SuperAdmin; clients needing anonymous OTP authentication must use the future authenticated-challenge flow, not a caller-selected-code send primitive.

Full database permission policies, OTP challenges/rate limiting and immediate access-token revocation rules are separate pending Identity work. The new guards do not claim those are complete.

## Migration artifacts and state

See the root Kahrad-Pricing-Implementation-Status.md. Idempotent Brands, Products and Guarantees SQL scripts are prepared in artifacts/architecture-audit and have not been applied to the local application database during the comparison pass. Use the reviewed deployment process and a verified target/backup before rollout.
