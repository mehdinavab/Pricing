# MAHAK Integration API

Standalone .NET 10 accounting adapter for Kahrad/Rodor integrations. It uses Minimal API, Dapper, and `Microsoft.Data.SqlClient`; it does not use EF Core, stored procedures, a caller-selected database, or exchange-rate conversion.

## Endpoint status

All business routes require HTTPS and `X-API-Key` and are isolated to the database configured for that key.

| Route | Status | Notes |
|---|---|---|
| `POST /api/v1/items/sync` | Partial | Only explicit `ReturnedItemsOnly` is operational. `AllItems` returns `409` until `GoodInf` fields, price columns, filters, and a stable key are confirmed. |
| `POST /api/v1/representatives/sync` | Partial | Returns only verified `code`, `Fax`, and `GrpCode` fields. |
| `POST /api/v1/invoices/sync` | Partial | Implements the supplied non-returned legacy projection and does not claim definitive ownership/lifecycle semantics. |
| `POST /api/v1/sales/prices` | Operational with documented ordering limitation | Updates current IRR/USD price fields. It does not update invoice history or prevent a stale caller request from overwriting a newer value. |

The three reads are bounded to one page. A non-null cursor is rejected, and a result larger than the requested page returns `409`, until real unique MAHAK ordering keys are confirmed. This prevents an invented cursor from silently skipping or duplicating accounting rows.

Supporting routes:

- `GET /health/live` checks only process liveness.
- `GET /health/ready` checks every enabled brand database without returning connection details.
- `GET /openapi/v1.json` is available only in Development.

## Secure JSON configuration

Copy `appsettings.Production.example.json` to `appsettings.Production.json` on the MAHAK server, set `ASPNETCORE_ENVIRONMENT=Production`, and replace every placeholder with operator-confirmed values. The real production file is ignored by Git.

Each `Mahak:Brands` entry contains its own enabled API keys and SQL Server settings. The authenticated key selects the brand and catalog. Request bodies containing unknown selectors such as `brandId`, `database`, or `connectionString` are rejected.

Generate a strong key in PowerShell:

```powershell
[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

For Windows integrated authentication, set `AuthenticationMode` to `IntegratedSecurity`, omit `UserId` and `Password`, and grant only the application pool/service identity the required SQL permissions. For SQL authentication, use `SqlPassword` and configure a narrowly scoped SQL login. Consider separate application deployments/credentials for reads and the price write when operations require stronger separation.

Restrict the production JSON file to Administrators and the service identity. Example (adjust identities and inheritance policy with the server administrator):

```powershell
icacls .\appsettings.Production.json /inheritance:r
icacls .\appsettings.Production.json /grant:r "Administrators:F" "DOMAIN\MahakApiService:R"
```

Do not put real keys or SQL credentials in the example file, source control, command history, support tickets, or application logs.

Key rotation requires a restart in v1:

1. Add the new enabled key beside the old enabled key in protected JSON.
2. Restart and verify the new key.
3. Disable/remove the old key, restart, and verify it returns `401`.

Startup validation rejects enabled brands with missing SQL settings, short/duplicate active keys, unsupported authentication modes, unsafe timeout/pool settings, and Production placeholders. Production also requires at least one enabled brand.

## Run

```powershell
dotnet restore MAHAK.Integration.slnx
dotnet run --project Src/MAHAK.Integration.Api/MAHAK.Integration.Api.csproj
```

Example requests:

```http
POST /api/v1/items/sync
X-API-Key: <brand-key>
Content-Type: application/json

{"scope":"ReturnedItemsOnly","cursor":null,"pageSize":500}
```

```http
POST /api/v1/representatives/sync
X-API-Key: <brand-key>
Content-Type: application/json

{"cursor":null,"pageSize":500}
```

```http
POST /api/v1/invoices/sync
X-API-Key: <brand-key>
Content-Type: application/json

{"cursor":null,"pageSize":500}
```

```http
POST /api/v1/sales/prices
X-API-Key: <brand-key>
Content-Type: application/json

{
  "goodCode":"12050",
  "consumerPrice":150000000,
  "consumerPriceDollar":100,
  "operationId":"37a09c7d-c8ed-4f69-b470-e11c9d167582"
}
```

`operationId` is required for caller traceability but is not persisted in MAHAK. Identical updates are state-idempotent; durable deduplication and stale-write ordering are not guaranteed.

## SQL permissions

Read access is needed only for verified projections from:

- `dbo.GoodInf`
- `dbo.BackFact_Detail`
- `dbo.PerInf`
- `dbo.Fact_Fo`
- `dbo.Fact_Fo_Detail`

Write access is needed only for `CostLevel_Sell_1` and `CostLevel_Sell_3` on `dbo.GoodCostDiscount`. The update is transactional; if more than one row matches `GoodCode`, it rolls back and returns `409`.

Do not grant schema modification rights. No index, Change Tracking, table, or stored procedure is created by this service.

## Verification and test database

```powershell
dotnet build MAHAK.Integration.slnx -c Release
dotnet test MAHAK.Integration.slnx -c Release
```

Set `MAHAK_TEST_SQL_SERVER` only to a dedicated, schema-compatible test database to enable SQL integration tests. Never point it at production. Without it, SQL tests are visibly skipped.

## Deployment checklist

- Install the .NET 10 hosting/runtime bundle.
- Install protected `appsettings.Production.json`; confirm brand/catalog mapping with the operator.
- Set `ASPNETCORE_ENVIRONMENT=Production` and run behind HTTPS.
- Restrict firewall/IP access to authorized Kahrad systems.
- Grant least-privilege SQL rights and protect the service account.
- Verify wrong and cross-brand keys return `401` without SQL access.
- Verify `/health/live` and `/health/ready` from the internal monitor.
- Keep Development OpenAPI disabled in Production.
- Configure log retention and confirm logs contain no API keys, Fax values, payloads, or credentials.
- Back up protected configuration through the approved restricted process.

## Performance checklist

Before changing SQL or indexes, capture representative payload size, duration, SQL execution plan, logical reads, CPU, waits, network latency, timeout frequency, and connection-pool usage per brand. Any index recommendation requires measured evidence and DBA approval.

## Required confirmations

1. Actual database mapping, API keys, authentication mode, and least-privilege SQL credentials for each brand.
2. `GoodInf` all-item projection and real price columns.
3. Actual SQL types and unique keys for every referenced MAHAK table, including whether identifiers preserve leading zeros.
4. Additional representative fields and cross-brand Fax/GrpCode verification.
5. Stable pagination or change-tracking fields.
6. Return/resale semantics and whether the legacy invoice projection always represents current assignment.
7. `BackFact_Detail` return header/date/link semantics.
8. Whether `GoodCostDiscount.GoodCode` is unique and the approved durable stale-write/idempotency design.
9. USD decimal precision and legitimate zero-price policy. Currency conversion remains outside this API.
