# MAHAK Integration API — Implementation Report

## Delivered structure

```text
MAHAK.Integration.slnx
Src/MAHAK.Integration.Api/
  Configuration/   typed JSON options and startup validation
  Contracts/       stable request/response projections
  Data/            SQL connection factory, centralized SQL, specialized repositories
  Endpoints/       exactly four authenticated business routes
  Errors/          sanitized ProblemDetails mapping
  Observability/   correlation logging and SQL readiness
  Security/        constant-time API-key lookup and immutable brand context
  Services/        validation and orchestration
  Program.cs
  appsettings*.json
  README.md
Tests/MAHAK.Integration.Tests/
  Security/
  Unit/
  Integration/
  TestSupport/
```

The project is independent from `Kahrad.Pricing.slnx`. It targets .NET 10 and uses a single API project plus one xUnit test project. It references Dapper and Microsoft.Data.SqlClient only for persistence; no EF/EF Core package or stored procedure is used.

## Endpoint disposition

- Items — **partial**: the known `ReturnedItemsOnly` projection is implemented with an explicit scope. `AllItems` is blocked with safe `409 ProblemDetails` because item fields, price columns, filter, and unique ordering key are unknown.
- Representatives — **partial**: verified fields `code`, `Fax`, and `GrpCode` and the exact `LEN(Fax) > 1 AND GrpCode IN (4,5,6)` filter are implemented.
- Invoices — **partial**: the supplied anti-join, group filter, and strict `kala_code > 10000` projection are implemented. It is described only as the legacy non-returned projection, not a formally verified latest-owner algorithm.
- Sale prices — **operational after real configuration/schema verification**: parameterized update of `CostLevel_Sell_1` and `CostLevel_Sell_3` by `GoodCode`. A multi-row update is rolled back. No invoice history or exchange-rate calculation is touched.

Reads use `DISTINCT TOP (@Take)` and verified columns converted to bounded strings because source SQL types have not been supplied. This is an explicit deviation from legacy `SELECT *`. It prevents leaking unknown personal/accounting columns. Reads allow one bounded page only; cursor continuation is intentionally blocked until a unique stable key is confirmed.

## Security and isolation

- API keys and per-brand SQL configuration load from the same `Mahak:Brands` JSON tree.
- SHA-256 key digests are compared with `CryptographicOperations.FixedTimeEquals`.
- Active duplicate keys and duplicate KeyIds fail startup validation.
- The resolved brand/database is immutable request context; client database/brand fields are rejected as unmapped JSON.
- Connection strings are built only with `SqlConnectionStringBuilder.InitialCatalog` from trusted configuration.
- Both SQL password and Windows integrated authentication are supported.
- Production placeholders and absent enabled brands fail startup.
- Production and local secret JSON filenames are ignored by Git.
- Errors, health responses, and OpenAPI do not expose keys, passwords, or catalogs.
- Request bodies are limited to 64 KiB; business routes are rate-limited.
- Correlation, route, brand code, status, duration, and row count are logged without payloads/Fax/API keys.

## SQL used

- Items: `GoodInf` filtered with `EXISTS` against `BackFact_Detail`; this is explicitly returned-items-only.
- Representatives: `PerInf` with the exact Fax/group filters.
- Invoices: supplied `Fact_Fo_Detail` / `BackFact_Detail` / `Fact_Fo` / `PerInf` joins, consistent aliases, anti-join, and parameterized threshold `10000`.
- Prices: confirmed parameterized `GoodCostDiscount` update only.

No catalog names are embedded in SQL. No `NOLOCK`, `SELECT *`, dynamic client SQL, schema changes, indexes, Change Tracking, EF migrations, or stored procedures were introduced.

## Verification

Development verification after implementation:

```text
dotnet restore MAHAK.Integration.slnx       succeeded
dotnet build MAHAK.Integration.slnx         succeeded, 0 warnings, 0 errors
dotnet build MAHAK.Integration.slnx -c Release  succeeded, 0 warnings, 0 errors
dotnet test MAHAK.Integration.slnx -c Release   27 passed, 1 skipped, 0 failed
```

The skipped test requires `MAHAK_TEST_SQL_SERVER` pointing to a dedicated schema-compatible test database. It was not run because no authorized MAHAK test SQL Server was configured.

Covered tests include JSON loading, brand/catalog isolation, key rotation shape, wrong/disabled keys, duplicate keys, missing database settings, caller override rejection, secret-free errors/OpenAPI, SQL rules, product serial string concatenation, bounded pagination blocking, negative price validation, exact price pass-through, and multi-row integrity conflict.

## Open blockers

1. Real brand/database/key/credential mapping.
2. All-item `GoodInf` projection and price columns.
3. Verified table key types and unique stable pagination keys.
4. Additional representative projection fields.
5. Definitive return/resale/current-owner semantics.
6. Durable write ordering/idempotency and confirmed `GoodCode` uniqueness.
7. USD precision and zero-price policy.
8. Execution plans and DBA-approved tuning on representative data volumes.

The adapter must not be connected to production until these configuration and schema items are reviewed with the MAHAK operator/accountant.
