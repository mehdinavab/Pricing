# MAHAK Integration API — Implementation Specification for Codex

> **Purpose:** Create a **new, standalone .NET 10 MAHAK Integration API project from scratch**. This service runs near the existing MAHAK accounting SQL Server and exposes four brand-scoped operations to KahradEnterprise/Kahrad Pricing. **It is not a rewrite of the existing KahradEnterprise solution.**
>
> **Instruction for Codex:** Read this document completely before creating or editing files. Implement confirmed requirements, explicitly track unknown accounting-schema details, and never invent business rules, table columns, API responses, or MAHAK behaviors. Favor a working, secure, testable vertical path over decorative scaffolding.

## 0. Mandatory working instructions

1. Before writing code, inspect the current working directory and detect any existing project. Do not overwrite another repository. If this is a clean directory, initialize a new solution.
2. Target **.NET 10**, **ASP.NET Core Minimal API**, **Dapper**, and **Microsoft.Data.SqlClient**.
3. Use a **lightweight, simplified N-layer structure within one API project** (Endpoints → Services → Repositories → SQL Server). A separate test project is required. Avoid CQRS, MediatR, Vertical Slice infrastructure, elaborate Clean Architecture multi-project scaffolding, Kafka, RabbitMQ, Kubernetes, Docker, and other unnecessary machinery.
4. **Absolutely no Entity Framework/EF Core, no stored procedures, and no dynamic raw SQL received from clients.** Use parameterized inline SQL.
5. Database isolation must be **derived from the authenticated API key**. **Store per-brand API keys and database settings in JSON configuration** as specified in Section 4A. Never trust a `BrandId`, database name, or connection string supplied in an incoming request.
6. Preserve all known MAHAK database objects and semantics. Do not change the accounting database schema, add indexes, alter tables, or enable SQL Server Change Tracking without explicit permission.
7. Build and run tests after each meaningful phase. Report changes, verification, and open integration blockers in `IMPLEMENTATION_REPORT.md`.
8. Produce code, not only a plan. Implement everything possible without undocumented MAHAK schema; leave narrow, explicit and safe integration blockers for unknowns. Never silently return fabricated data.
9. Keep **exchange-rate configuration and IRR→USD conversion outside this API**. That is a KahradEnterprise responsibility. The MAHAK API receives both prices from its authorized caller.
10. API should serve its four business operations efficiently, but speed and consistency are both required: no `WITH (NOLOCK)`, invented indexes, `SELECT *` in finalized public DTOs, or unbounded reads.

## 1. Confirmed business facts

- There are multiple brands (including **Kahrad** and **Rodor/Rodoro**). Each has its own **MAHAK SQL Server database**, including its own `GoodInf` and other accounting tables.
- Each brand has its **own API key**. MAHAK maps a validated key to a **fixed server-side brand configuration and database connection**.
- **MAHAK SQL Server** is on a strong machine (reportedly 32 CPU cores / 64 GB RAM); still tune based on measured SQL plans, network latency, indexing, and payload size.
- Four external operations are required:
  1. Read accounting items (and their prices when source columns are confirmed).
  2. Read representatives.
  3. Read invoices / current invoice detail state.
  4. Update a product's consumer prices in MAHAK.
- **Every Kahrad/Rodor product is a unique physical unit and exists in quantity one.** A product may be sold, returned, then sold to a different representative.
- `Fact_Fo_Detail.kala_code` identifies that unit. `GoodCostDiscount.GoodCode` is **the same identifier**. Product serial in Kahrad is the **string concatenation** `kala_code + "19"` (e.g. `12050` → `"1205019"`). Do not numerically add 19. Preserve exact identifier formatting.
- Invoice details with `kala_code > 10000` refer to Kahrad/Rodor products; `<= 10000` are unrelated. One legacy query uses `> 100000`: this is a **known discrepancy**, and `> 10000` is the clarified intended threshold for our products.
- In accounting `PerInf`, **the `Fax` convention is the only contractual discriminator between ordinary customers and representatives**: `LEN(Fax) > 1` in the supplied representative SQL. `GrpCode IN (4,5,6)` is an additional filter, **not an alternative discriminator**.
- `BackFact_Detail` contains **returns**. The user's legacy invoice query is reported to reflect the latest/current valid invoice state in practice, but its precise semantics for multiple sales/returns have **not yet been verified**; do not equate an absent row with a confirmed return without additional evidence.
- Existing MAHAK update operation sets `GoodCostDiscount.CostLevel_Sell_1` (consumer price **IRR**) and `CostLevel_Sell_3` (consumer price **USD**), matched by `GoodCode`.
- Historical sale price snapshots and buyer/warranty handling belong to **KahradEnterprise**, not this API. Do not overwrite past invoice amounts or pretend that changing `GoodCostDiscount` changes historical invoices.
- No EF Core and no stored procedures on the MAHAK side, including migrations or stored-procedure-based pagination.

## 2. Architectural decision

Use a small solution:

```text
MAHAK.Integration.slnx
src/
  MAHAK.Integration.Api/
    MAHAK.Integration.Api.csproj
    Program.cs
    Endpoints/
      ItemEndpoints.cs
      RepresentativeEndpoints.cs
      InvoiceEndpoints.cs
      SalePriceEndpoints.cs
    Contracts/
      Common/
      Items/
      Representatives/
      Invoices/
      Prices/
    Services/
      ItemService.cs
      RepresentativeService.cs
      InvoiceService.cs
      SalePriceService.cs
    Data/
      ISqlConnectionFactory.cs
      SqlConnectionFactory.cs
      Queries/
        ItemSql.cs
        RepresentativeSql.cs
        InvoiceSql.cs
        SalePriceSql.cs
      Repositories/
        ItemRepository.cs
        RepresentativeRepository.cs
        InvoiceRepository.cs
        SalePriceRepository.cs
    Security/
      ApiKeyAuthenticationHandler.cs
      BrandContext.cs
      BrandConnectionResolver.cs
    Configuration/
      MahakOptions.cs
      BrandConnectionOptions.cs
      BrandApiKeyOptions.cs
      SqlServerBrandOptions.cs
    Errors/
      ApiExceptionMapping.cs
    Observability/
    appsettings.json
    appsettings.Development.json
    appsettings.Production.example.json
    README.md

tests/
  MAHAK.Integration.Tests/
    MAHAK.Integration.Tests.csproj
    Unit/
    Integration/
    Security/

IMPLEMENTATION_REPORT.md
```

Adjust filenames to the actual minimal design but preserve responsibilities. Use DI; never make endpoints or business services execute SQL directly. SQL remains in repositories and is centralized for auditable query review. Avoid generic base repositories, generic UnitOfWork, needless reflection, or controllers unless a measured need arises.

### Dependency direction

```text
Minimal API endpoints
   -> application services (validation/orchestration)
     -> specialized repositories (Dapper + parameterized SQL)
       -> SQL Server for authenticated brand
```

DTOs are not MAHAK table entities; expose stable, minimal response contracts. Do not leak accounting internal columns (especially personal information) through `SELECT *` responses.

## 3. HTTP routes and contracts

Use exactly **four business endpoints** (health/readiness and OpenAPI metadata are supporting routes, not business operations). All are POST, matching the selected integration style:

```text
POST /api/v1/items/sync
POST /api/v1/representatives/sync
POST /api/v1/invoices/sync
POST /api/v1/sales/prices
```

Every call requires `X-API-Key: <key>` via HTTPS. Authentication resolves `BrandContext` with immutable `BrandId`, `BrandCode`, and server-side connection configuration. Do not accept external `BrandId` or catalog/database names as selectors. A brand key cannot query another brand.

### Shared pagination contract (provisional)

For read requests, use a maximum page size and a **server-generated opaque cursor** when source keys permit deterministic keyset pagination:

```json
{
  "cursor": null,
  "pageSize": 500
}
```

```json
{
  "items": [],
  "nextCursor": null,
  "hasMore": false
}
```

The actual cursor must be keyed on **real verified unique, stable MAHAK columns** (for example `Code` only if unique and sortable); never assume uniqueness merely from a name. If a safe cursor cannot be implemented with the known schema, avoid inventing one: implement a bounded, deterministic fallback only if verifiable and document the remaining contract blocker. Maximum page size should be configured (e.g. 1000) and validated. Incremental sync is an **extension**, not a confirmed ability: do **not** invent an `UpdatedAt` field, SQL change tracking, or tombstone column. Full reconciliation may be necessary until source capabilities are confirmed.

Return typed `ProblemDetails` on errors. Avoid returning SQL errors, connection strings, API keys, or MAHAK credentials to callers.

## 4. API key security and tenant isolation

1. **The source of truth for brand keys and database configuration is JSON configuration.** Define `Mahak:Brands` in `appsettings.json` and override actual secrets in a protected, environment-specific JSON file such as `appsettings.Production.json`; see Section 4A. Do **not** require environment variables, a database table, or a secret manager to resolve keys. Environment overrides may be supported as an optional operational enhancement, but JSON must work on its own.
2. Each brand JSON entry has **one or more API keys** (for controlled rotation) and its **own SQL Server settings, including database name**. No runtime database selection from request content is allowed.
3. API-key comparisons must avoid timing leakage where practical. Prefer a dedicated authentication handler/`AuthenticationScheme` plus authorization policy for all business endpoints rather than scattered checks.
4. Resolve the authenticated brand **once per request** from validated JSON-backed configuration. Create the SQL connection using `SqlConnectionStringBuilder` and the configured database as `Initial Catalog`; **never concatenate database names into SQL queries**.
5. Never log API keys, database passwords, full connection strings, or personal identifying data. Mask potentially sensitive representatives/invoices in structured logs.
6. Reject missing, unknown, revoked, or disabled keys (`401` / `403` as appropriate). Use HTTPS in production. Apply request size limits and IP allowlist/firewall rules if operationally available.
7. Add tests demonstrating that Brand A's key cannot access Brand B's database, even when a request body tries to pass `brandId`, `databaseName`, or similar fields.
8. Consider separate SQL credentials for read operations and the price-update operation. Scope permissions narrowly to required tables/columns where deployment permits; do not change MAHAK schema.
9. Protect real JSON credentials at the OS/deployment level and **never commit actual keys/passwords**. Only commit safe templates with unmistakably fake values.

**Brand database names:** An example query used `[mahak2]` for Rodor; another used `[mahak3]`. Do **not** infer the definitive brand-to-database mapping from this alone. It must come from environment-specific configuration confirmed by the operator.


## 4A. Mandatory JSON configuration: API keys and per-brand SQL Server databases

**User requirement (confirmed):** Codex MUST implement **JSON-configured API keys and their associated brand/database configuration**. This is not an optional future feature. The API must be fully operable using JSON files alone, without a key database, EF Core, or stored procedures.

### Configuration files and deployment rules

- Commit a **safe** `appsettings.json` with ordinary defaults and an example brand configuration containing only obvious placeholders; alternatively keep placeholders in `appsettings.Production.example.json`. These are examples, not working credentials.
- Actual brand API keys and SQL Server credentials must be stored in an environment-specific JSON configuration file such as **`appsettings.Production.json`** on the MAHAK server. ASP.NET Core loads this convention by default when `ASPNETCORE_ENVIRONMENT=Production`. The secure JSON must be installed by the deployment operator and **excluded from Git**. Production does not require a cloud secret store or environment variables for its keys.
- Add to `.gitignore` at minimum `appsettings.Production.json`, `appsettings.Local.json` (if supported), and any other JSON used for live secrets. Commit `appsettings.Production.example.json` with placeholders for reproducible setup. Protect the real JSON with restricted NTFS permissions to the application identity and administrators, restrict backup access, and do not serve it via static files.
- If adding `appsettings.Local.json`, register it explicitly via `builder.Configuration.AddJsonFile(...)`; do not assume ASP.NET Core loads an arbitrary filename automatically. Respect JSON configuration precedence and document it.
- On startup, fail fast with actionable **non-secret** diagnostics if an enabled brand lacks a usable API key, SQL Server/database setting, valid authentication options, or required SQL credentials; reject obvious placeholder values in Production. The app must not silently fall back to another brand's database.

### Required configuration shape

Use a stable dictionary `Mahak:Brands`, keyed by **internal configured brand code**. Every entry carries `Enabled`, `ApiKeys[]`, and `SqlServer`. The following is a **template**, not real credentials or an assertion of actual database mappings:

```json
{
  "Mahak": {
    "Brands": {
      "Kahrad": {
        "Enabled": true,
        "ApiKeys": [
          {
            "KeyId": "kahrad-v1",
            "Key": "REPLACE_WITH_A_LONG_RANDOM_KAHRAD_API_KEY",
            "Enabled": true
          }
        ],
        "SqlServer": {
          "DataSource": "YOUR_MAHAK_SQL_SERVER",
          "Database": "CONFIRM_KAHRAD_DATABASE_NAME",
          "AuthenticationMode": "SqlPassword",
          "UserId": "YOUR_SQL_LOGIN",
          "Password": "REPLACE_WITH_SQL_PASSWORD",
          "Encrypt": true,
          "TrustServerCertificate": false,
          "ConnectTimeoutSeconds": 5,
          "MaxPoolSize": 30
        },
        "CommandTimeoutSeconds": 30
      },
      "Rodor": {
        "Enabled": true,
        "ApiKeys": [
          {
            "KeyId": "rodor-v1",
            "Key": "REPLACE_WITH_A_LONG_RANDOM_RODOR_API_KEY",
            "Enabled": true
          }
        ],
        "SqlServer": {
          "DataSource": "YOUR_MAHAK_SQL_SERVER",
          "Database": "CONFIRM_RODOR_DATABASE_NAME",
          "AuthenticationMode": "SqlPassword",
          "UserId": "YOUR_SQL_LOGIN",
          "Password": "REPLACE_WITH_SQL_PASSWORD",
          "Encrypt": true,
          "TrustServerCertificate": false,
          "ConnectTimeoutSeconds": 5,
          "MaxPoolSize": 30
        },
        "CommandTimeoutSeconds": 30
      }
    }
  }
}
```

**Important:** `[mahak2]` appeared in one *Rodor* legacy query and `[mahak3]` in other examples, but the actual deployment mapping must be provided/verified. **Do not fill real database names by guessing.** The server may be shared while databases are separate.

The sample chooses SQL username/password as an illustration. Also support Windows Integrated Security **if required by the actual MAHAK deployment**; when enabled, do not require `UserId`/`Password`, and use the application pool/service identity appropriately. Implement this through typed configuration and `SqlConnectionStringBuilder`, not SQL string interpolation. Do not add EF or stored procedures.

### Implementation contract for Codex

1. Create strongly typed `MahakOptions`, `BrandConnectionOptions`, `BrandApiKeyOptions`, and `SqlServerBrandOptions`. Use the **Options pattern** with startup validation; do not instantiate config classes via scattered `GetSection` calls throughout endpoints.
2. Build an immutable/validated in-memory lookup from configured key values to internal brand code/context. Keys are **not** the dictionary's public brand codes. Reject the same active key assigned to multiple brands; reject blank/duplicate `KeyId` values and disabled brand configurations with inconsistent settings as appropriate.
3. The API receives `X-API-Key` via HTTPS. Validate the supplied key against configured enabled keys with **constant-time comparisons** of fixed-length hashes (for example, SHA-256 digests plus `CryptographicOperations.FixedTimeEquals`), not normal `==` string checks. Treat API keys as high-entropy secrets; never include them in response bodies/logs.
4. For a valid key, resolve the configured brand, configured `DataSource`, and configured `Database`. Ignore/reject caller-supplied `BrandId`, database name or connection string. Brand isolation is enforced by database routing, not by a user-controlled filter.
5. Construct a new `SqlConnection` for the authenticated brand via `SqlConnectionStringBuilder`, setting `DataSource`, `InitialCatalog`, encryption/authentication options, connection timeout, and pool size from **that brand's** validated JSON settings. Open connection only when required and dispose correctly; pooling is handled by SqlClient.
6. Supply the same trusted brand context to all four endpoints/repositories, including the **write** endpoint. Never share mutable per-request brand state across requests; do not cache/reuse an open `SqlConnection` globally.
7. Key rotation should be manageable by deploying a new JSON with both old/new enabled keys, then disabling the old one in a subsequent deployment. Document whether a restart is required (acceptable for v1); do not claim live key reload without implementing and testing it.
8. Add tests that load **temporary sample JSON** (without real credentials) proving: Kahrad key resolves only Kahrad database; Rodor key resolves only Rodor database; wrong/disabled keys rejected; duplicate keys across brands cause startup validation failure; missing database settings fail startup; a request's `database` / `brandId` cannot override JSON routing; no secrets appear in logs, exception pages or OpenAPI.
9. Include README steps for generating strong random API keys, filling real `appsettings.Production.json`, setting environment to Production, NTFS file permission restrictions, configuring SQL credentials, starting the API, verifying with the brand's `X-API-Key`, and rotating a key safely.

### Mandatory acceptance criteria for JSON configuration

- [ ] Brand API key(s) stored and loaded from JSON settings.
- [ ] Each brand's SQL Server / database name and connection settings stored and loaded from the **same JSON configuration tree**.
- [ ] No hard-coded API keys, database names or SQL credentials in C#.
- [ ] Production credentials are in protected JSON excluded from source control; checked-in templates contain placeholders only.
- [ ] Wrong/disabled key never connects to SQL Server; Kahrad key cannot access Rodor database and vice versa.
- [ ] Valid startup checks, safe rotations, and JSON-driven isolation tests pass.

## 5. SQL rules common to all repositories

- Prefer Dapper (`QueryAsync`, `QuerySingleOrDefaultAsync`, `ExecuteAsync`) and `Microsoft.Data.SqlClient` for connection creation and transactions.
- Use `CommandDefinition` with cancellation tokens and configured command timeouts.
- Use SQL parameters for values. Do not interpolate `GoodCode`, `GrpCode`, threshold, identifiers supplied by a caller, or cursor values into SQL text.
- All queries are executed against the selected database's `dbo` schema without hard-coded `[mahak2]` / `[mahak3]` qualifiers.
- Use projections selecting **only verified columns** in production DTOs. The supplied legacy `SELECT *` is context and a starting point for schema discovery, not a final API contract.
- Evaluate query execution plans and existing indexes before claiming an optimization. Do not create indexes without DBA approval.
- Maintain database consistency; avoid `NOLOCK` and accidental long-running transactions.
- Use short-lived connections obtained via `SqlConnectionFactory`. Rely on built-in SqlClient pooling.
- Keep write operations fully parameterized and validate affected row counts.
- Log elapsed time, request correlation ID, brand code, row count, and sanitized error category. No key/PII/full SQL-parameter dumps.

## 6. Endpoint 1 — accounting items

### Confirmed legacy SQL

```sql
SELECT *
FROM GoodInf
WHERE code IN (
    SELECT kala_code
    FROM [mahak3].[dbo].[BackFact_Detail]
);
```

Every brand has its own `GoodInf` and `BackFact_Detail`. After connecting to that brand's database, a semantically equivalent baseline query is:

```sql
SELECT g.*
FROM dbo.GoodInf AS g
WHERE EXISTS (
    SELECT 1
    FROM dbo.BackFact_Detail AS d
    WHERE d.kala_code = g.code
);
```

**Critical caveat:** Despite the legacy property name `GetReturnedItems`, this query only returns items appearing in **returns** (`BackFact_Detail`). It does **not** necessarily return **all accounting items**, and the user's broader requirement is to sync all brand items and item prices. **Do not silently ship the `EXISTS` filter as an all-items synchronization API.**

Implement a clearly named query strategy with a documented decision point:

- `AllItems`: intended sync of all brand items, subject to confirmation of expected source table, price columns, filtering, and pagination.
- `ReturnedItemsOnly`: matches supplied legacy query exactly and is usable only if explicitly selected/approved for that purpose.

Because the complete `GoodInf` schema and exact price source columns have **not** been provided, do not invent columns such as `UnitPrice`, `Title`, `ChangedAt` or fake conversions. Implement the repository abstraction and safe known-field mapping (`code` is known) and record missing schema requirements in `README.md` / `IMPLEMENTATION_REPORT.md`. Do not mark item-price sync complete until a real price column and example rows are confirmed.

**Tests:** per-brand isolation, code normalization, no duplicate item records for repeated return rows (`EXISTS` semantics), page boundaries once an ordering key is verified, correct behavior when no items are found.

## 7. Endpoint 2 — representatives

### Confirmed Rodor legacy SQL

```sql
SELECT *
FROM [mahak2].[dbo].[PerInf]
WHERE LEN(Fax) > 1
  AND GrpCode IN (4, 5, 6);
```

After resolving the authenticated database:

```sql
SELECT p.*
FROM dbo.PerInf AS p
WHERE LEN(p.Fax) > 1
  AND p.GrpCode IN (4, 5, 6);
```

**Rules:**

- `Fax` is the only contractual representative discriminator in accounting. Do not equate `GrpCode` alone to representative status.
- The extra `GrpCode IN (4,5,6)` restriction is agreed for this query; do not remove it without approval.
- `PerInf.code`, `Fax`, `GrpCode` are known column names. Other representative name/contact fields are **unknown**. Do not invent them.
- Returned records will be imported/mapped within KahradEnterprise; MAHAK API merely exposes the authorized accounting data. Do not introduce local representative business entities in MAHAK.
- If the operator requests normalized trimming, verify how `LEN(Fax) > 1` and `LEN(LTRIM(RTRIM(Fax))) > 1` behave on real records before altering semantics.

**Tests:** Fax null/empty/length-one/two, group outside 4–6, brand isolation, duplicate codes, no unintended leakage of all `PerInf` attributes.

## 8. Endpoint 3 — invoices and return-aware current state

### Known tables/links

```text
Fact_Fo.Code                  = Fact_Fo_Detail.code
Fact_Fo.Shakhs_Code           = PerInf.code
BackFact_Detail.sanad_code    = Fact_Fo_Detail.code
BackFact_Detail.kala_code     = Fact_Fo_Detail.kala_code
```

`Fact_Fo` is an invoice header, `Fact_Fo_Detail` contains its product lines, `PerInf` identifies the account/person, and `BackFact_Detail` contains returns.

### Confirmed legacy query reported to show the current, non-returned invoice lines

```sql
SELECT
    PRF.Fax,
    FM.Shakhs_Code,
    f.kala_code,
    f.code
FROM dbo.Fact_Fo_Detail AS f
LEFT JOIN dbo.BackFact_Detail AS bf
    ON bf.sanad_code = f.code
   AND bf.kala_code = f.kala_code
LEFT JOIN dbo.Fact_Fo AS FM
    ON FM.Code = f.code
LEFT JOIN dbo.PerInf AS PRF
    ON PRF.code = FM.Shakhs_Code
WHERE bf.sanad_code IS NULL
  AND PRF.GrpCode IN (4, 5, 6)
  AND f.kala_code > 10000;
```

Original code used `F.code` / `F.kala_code` despite alias `f`. Use consistent alias `f` to support case-sensitive collations.

**Semantics and constraints:**

1. `f.kala_code > 10000` is the agreed product-code threshold, strictly greater than (not >=).
2. The anti-join removes detail records with a matching return row. The operator states this query has reliably reflected the current invoice state; **do not claim it is a formally validated last-sale algorithm**.
3. In particular, a unique product may be sold → returned → resold to another representative; one of the old queries may not capture all complexities. Do not automatically mark absent products as returned or delete historical sales on an absent row.
4. The original invoice query uses `GrpCode IN (4,5,6)` but **does not check Fax length**. Since Fax is the contract for distinguishing representatives, the invoice filter policy needs explicit reconciliation; do not secretly change the proven legacy query. If needed, expose a separately named representative-only query mode after authorization and acceptance tests.
5. Provide both `Shakhs_Code` (actual PerInf identity) and `Fax` as **separate fields**. Never alias `Fax AS Shakhs_Code`.
6. Retain `InvoiceCode`, `kala_code`, and identifier values faithfully. Treat product codes as identifiers, not measures; prevent leading-zero loss if possible.
7. Other legacy variants are background evidence, **not necessarily required endpoints**:

```sql
-- Headers (unfiltered):
SELECT * FROM dbo.Fact_Fo;

-- Detail rows without matching return:
SELECT f.*
FROM dbo.Fact_Fo_Detail AS f
LEFT JOIN dbo.BackFact_Detail AS bf
    ON bf.sanad_code = f.code AND bf.kala_code = f.kala_code
WHERE bf.sanad_code IS NULL;

-- Returns linked to sale invoice, legacy threshold differs:
SELECT
    BD.kala_code,
    FM.Code AS fact_code,
    PIF.Fax AS Shakhs_Code
FROM dbo.BackFact_Detail AS BD
INNER JOIN dbo.Fact_Fo_Detail AS FD
    ON FD.kala_code = BD.kala_code AND BD.sanad_code = FD.code
INNER JOIN dbo.Fact_Fo AS FM
    ON FM.Code = FD.code
LEFT JOIN dbo.PerInf AS PIF
    ON PIF.code = FM.Shakhs_Code
WHERE BD.kala_code > 100000
  AND LEN(LTRIM(RTRIM(PIF.Fax))) > 1;
```

The user clarified the intended inclusion cutoff `>10000`, so the `>100000` in the old diagnostic query is **not** a basis for excluding 10001–100000 from a new general product sync.

**Required protections:**

- This endpoint is a **bounded read-only synchronization projection**, not a local sale-finalization command.
- Use stable pagination after checking available unique ordering keys; if a combination of columns is not unique, identify a real detail-row ID rather than fabricating uniqueness.
- Do not rely on list order to infer current owner. Provide enough identifiers to KahradEnterprise for its reconciliation workflow.
- Document unknown return-header/date/source-link semantics. If available later, add explicit return records or a separate optional projection within this same business operation; do not create a fifth unapproved business endpoint.

**Tests:** a unique unit sold once; sale+return; sale+return+resale; two brands with overlapping product codes; `kala_code=10000` excluded; `10001` included; null Fax; duplicate join effects; no false confirmation on absent rows.

## 9. Endpoint 4 — set consumer prices in MAHAK

### Confirmed SQL (do not substitute an UPDATE of invoice history)

```sql
UPDATE dbo.GoodCostDiscount
SET
    CostLevel_Sell_1 = @ConsumerPrice,
    CostLevel_Sell_3 = @ConsumerPriceDollar
WHERE GoodCode = @GoodCode;
```

- `GoodCode = Fact_Fo_Detail.kala_code` is explicitly confirmed.
- `CostLevel_Sell_1` is **IRR consumer price**.
- `CostLevel_Sell_3` is the **USD equivalent consumer price**.
- The caller (KahradEnterprise) calculates and sends both values. **Never query an exchange-rate service or calculate exchange rates inside MAHAK.**
- The user says the price update occurs after selling a product; this writes the **current product price record**, not the historical invoice's price. Treat these as distinct concepts.
- Write SQL with parameterized Dapper/SqlClient, short bounded command timeout and cancellation.

### Proposed request (contract can evolve without guessing schema)

```json
{
  "goodCode": "12050",
  "consumerPrice": 150000000,
  "consumerPriceDollar": 100,
  "operationId": "37a09c7d-c8ed-4f69-b470-e11c9d167582"
}
```

`goodCode` must be validated and converted to the actual MAHAK SQL type only after inspecting the schema; do not cast strings unsafely or assume MAHAK keys are `int`. Use `decimal` for prices, never `double` or `float`. Do not invent currency formatting / USD rounding rules. Avoid exposing `operationId` in MAHAK SQL unless a durable idempotency design is explicitly approved.

### Persistence, idempotency, and ordering — critical

- Repeating **the exact same** `UPDATE` with identical values is intrinsically state-idempotent. It does **not** guard against **stale requests overwriting newer prices**.
- Strong ordering/durable idempotency needs a trusted monotonic version and persisted operation state or a coordinated single-writer strategy. MAHAK's schema modification permissions are unknown.
- Do **not** claim guaranteed protection against out-of-order delivery without a verified design. KahradEnterprise owns its sales/outbox ordering; document the unresolved ordering contract in `IMPLEMENTATION_REPORT.md`.
- Check affected row count: `0` → `404` (no matching product) unless agreed otherwise; `1` → successful update; `>1` → data-integrity violation, raise alarm and determine remediation (a rowcount check alone occurs after an UPDATE; if uniqueness is not guaranteed, ensure transactional preflight or a safe strategy to avoid partial/unintended multiple-row changes).
- SQL Server `UPDATE` auto-transacts one statement; use explicit transaction only when additional atomic operations require it.
- Validate finite, nonnegative decimal prices, API key, and request size. Determine legitimate zero price policy with owner rather than assuming.
- Do not alter `Fact_Fo` or `Fact_Fo_Detail` records, or historical sale snapshots.

### Proposed response

```json
{
  "goodCode": "12050",
  "updated": true,
  "affectedRows": 1
}
```

No conversion logic or business-price formula belongs here.

## 10. Performance and scalability expectations

Design for low-latency, safe synchronization against a busy 32-core/64GB SQL Server, without asserting hardware alone makes queries fast.

- `AsNoTracking` does not apply (we are **not** using EF).
- Keep SQL projection small. Avoid `SELECT *` for public output.
- Prefer keyset pagination over large `OFFSET` scans when verified keys exist.
- If incremental sync becomes possible through verified source change columns, design cursor and deletion handling carefully; do not assume such fields exist.
- Measure query duration, SQL plan, logical reads, CPU impact, and response payload size. Produce a benchmark checklist; do not create unsupported performance claims.
- Consider HTTP response compression for large reads after measuring CPU/network impact; never compress secrets in ways that expose them in shared context.
- Limit `pageSize`, concurrency per brand, connection pool usage, and timeout budgets. Don't open a large number of parallel full-table queries.
- Consider `ETag`, response caching, or Redis only if measurements and consistency requirements justify them. Do not introduce caching by default.
- Index recommendations must be **advisory**, based on plans and DBA permission. Candidate columns to inspect: `GoodInf.code`, `BackFact_Detail.kala_code`, `BackFact_Detail.sanad_code`, `PerInf.code`, `PerInf.GrpCode`, `Fact_Fo.Code`, `Fact_Fo_Detail.code`, `Fact_Fo_Detail.kala_code`, `GoodCostDiscount.GoodCode`.

## 11. Observability, readiness, and error behavior

- Add `/health/live` and `/health/ready` as non-business endpoints. Liveness should not fail just because MAHAK SQL Server is temporarily unavailable; readiness must reflect ability to reach configured dependencies safely without leaking credentials.
- OpenAPI/Swagger available in development, appropriately secured or disabled in production.
- Include `X-Correlation-ID` / trace ID in structured logs and error responses when appropriate.
- Log request duration, brand, endpoint, row count, SQL timeout category, and redacted failures. Avoid logging names, Fax, keys, DB credentials, invoice contents, or full payloads by default.
- Map `400` validation, `401` authentication, `403` inactive brand, `404` missing product on update, `409` verified conflict, `429` rate limits, `503` transient unavailable, `500` unexpected.
- Configured timeouts and cancellation propagation; retries only for safe reads/transient faults with bounded retry and jitter. **Do not blindly retry price writes** unless the end-to-end idempotency/ordering contract is settled.

## 12. Tests and verification

Unit tests:

- API-key authentication and brand isolation, key rotation behavior, disabled key handling.
- Brand context not alterable by request payload; correct catalog routing.
- Product serial mapping: `"12050" + "19" == "1205019"` (mapping can be documented as external contract; no need to add business logic to MAHAK unless actually used).
- Represented persons: Fax length and GrpCode combination.
- `kala_code > 10000` boundary; `10000` excluded, `10001` included.
- Legacy invoice anti-join semantics, with warning on resale/return uncertainty.
- Price update request validation, parameter binding, affected-row outcomes, and no USD conversion.
- No secret values in logs/errors.

Integration tests (only against a **dedicated test SQL Server/database** with seeded `GoodInf`, `PerInf`, `Fact_Fo`, `Fact_Fo_Detail`, `BackFact_Detail`, `GoodCostDiscount` compatible **minimal test fixtures**, never the production MAHAK database):

- Distinct connections per brand; overlapping `kala_code` does not leak across brands.
- Query correctness and pagination once source ordering keys are verified.
- Accounting item query `EXISTS` only returns returned-item set; test this intentionally to prevent mislabeling as all items.
- Same item sold, returned, then sold again under a different representative; confirm observable SQL semantics and flag anything beyond provided schema.
- Price update does not change historical invoice tables; verify rows affected and IRR/USD values.
- Concurrent updates and stale-write risk documented/tested once ordering design is confirmed.

Tests must not require Docker. When test SQL is unavailable, skip integration tests transparently with documented connection configuration; do not falsely report them as passing. Run `dotnet restore`, `dotnet build -c Release`, and `dotnet test -c Release` and record output.

## 13. Step-by-step implementation phases

### Phase 1 — New solution foundation

- Create .NET 10 solution, minimal API project, xUnit test project, dependencies, `.gitignore`, README and example JSON brand/database configuration.
- Add **JSON-backed** API key/brand/SQL Server settings (`Mahak:Brands`), validated startup options, brand-specific SQL connection factory, ProblemDetails, request cancellation, health and optional OpenAPI. Commit safe JSON templates and exclude real secret-bearing JSON from Git.
- Verify no EF package or stored-procedure usage.

### Phase 2 — Secure brand-specific SQL access

- Authentication/authorization and JSON-backed API-key-to-brand resolver (with tests for duplicate keys and missing database config).
- Brand-specific database selection via `SqlConnectionStringBuilder.InitialCatalog` from the authenticated brand's JSON settings; never accept a caller-supplied database.
- Safe SQL access conventions, bounded pagination design and structured logging.
- Security tests for cross-brand access and invalid keys.

### Phase 3 — Three read endpoints

- Implement items, representatives, and invoices using confirmed legacy SQL shape where safe.
- Preserve source semantics; distinguish `ReturnedItemsOnly` from `AllItems` requirements.
- For unknown DTO columns/order keys, do not fabricate; use documented limited projection / explicit incomplete-schema blocker, with tests.
- Do not make the invoice API assert a definitive lifecycle when return semantics are unverified.

### Phase 4 — Price update endpoint

- Implement validation, authorization, parameterized UPDATE of `GoodCostDiscount`, affected-row verification, and responses.
- Explicitly document lack of guaranteed stale-write prevention until persistent ordering/idempotency protocol is agreed.
- Run read/write integration tests on sandbox database only.

### Phase 5 — Final hardening / handoff

- Review API contracts, diagnostics, request-size and timeouts, TLS/security, logs, race conditions and SQL measurements.
- Deliver sample HTTP requests, an environment configuration guide, a deployment checklist for Windows/IIS or Windows Service hosting if requested, and a useful `IMPLEMENTATION_REPORT.md`.

## 14. Implementation acceptance criteria

- [ ] New, independent .NET 10 API solution compiles successfully.
- [ ] Minimal API and one-project simplified N-layer structure used.
- [ ] **No EF/EF Core and no stored procedures anywhere.**
- [ ] Exactly four authenticated business endpoints implemented (as far as confirmed schema allows).
- [ ] Distinct keys loaded from JSON route correctly to independent, JSON-configured SQL Server brand catalogs.
- [ ] Real API keys, SQL credentials, and brand databases are configured with protected production JSON; no secrets committed to Git.
- [ ] Missing/duplicate key or invalid database settings trigger safe startup failures.
- [ ] No user-supplied database/table/SQL identifiers are trusted.
- [ ] No full unbounded reads; pagination is deterministic where source schema permits.
- [ ] Known representative filtering respects Fax and GrpCode rules.
- [ ] Invoice product threshold is `> 10000`; nonreturned anti-join behavior is documented.
- [ ] Product identifiers are treated consistently; `GoodCode = kala_code`; serial suffix `"19"` understood.
- [ ] Price update writes only `CostLevel_Sell_1` (IRR) and `CostLevel_Sell_3` (USD) for matching `GoodCode`.
- [ ] MAHAK API performs **no USD exchange-rate calculation**.
- [ ] No historical invoice or sold-product price is recalculated or overwritten.
- [ ] Security, SQL cancellation, error handling, and logs work.
- [ ] Build/tests executed and outcomes documented honestly, including skipped integration tests.
- [ ] Unknown schema and stale-write semantics are explicitly reported, not silently assumed.

## 15. Facts requiring external confirmation (do not guess)

1. Real MAHAK `GoodInf` columns, which columns hold item cost/current price, and whether the legacy `BackFact_Detail` filter was ever intended for *all* item synchronization.
2. Exact brand-to-database mapping for all brands, plus API keys and SQL credentials to insert into **protected deployment JSON**; never invent or commit these values.
3. Real primary/unique keys and data types for `GoodInf`, `PerInf`, `Fact_Fo`, `Fact_Fo_Detail`, `BackFact_Detail`, and `GoodCostDiscount`; possible leading-zero codes.
4. Additional representative DTO fields needed by KahradEnterprise; verification of Fax formatting and `GrpCode` rules across brands.
5. Safe cursor ordering keys or usable change indicators for incremental sync.
6. Whether all nonreturned invoice rows from the legacy query always represent only the latest active assignment after return and resale. Confirm with accountant and a sample real product when available.
7. Whether `BackFact_Detail.sanad_code` always points to the original sale, and whether return header/date and exact return identifiers are accessible.
8. Whether price-update writes can be made strictly ordered/idempotent using an approved operation ledger/sequence or a dedicated integration store; whether updates can affect multiple `GoodCostDiscount` rows per `GoodCode`.
9. Whether `ConsumerPriceDollar` permits fractional values and what precision is required. Conversion policy is owned elsewhere.
10. Whether MAHAK DB schema changes/index creation are forbidden or just unapproved; assume **not permitted** until stated.

## 16. Expected final Codex output

At completion, summarize:

1. Exact generated files/project structure.
2. Which of the four endpoints are **operational**, **partial**, or **blocked by unknown schema**.
3. Security isolation implementation and test results.
4. SQL queries used and any **explicit deviation** from legacy queries.
5. Commands run (`restore`, `build`, `test`) and results.
6. Required JSON configuration files, `appsettings.Production.example.json`, deployment guidance, and secure configuration examples (never real credentials).
7. List of remaining questions that must be answered before connecting the actual MAHAK production database.

**Do not ask to replace the KahradEnterprise architecture; this is a standalone accounting-adapter service.**
