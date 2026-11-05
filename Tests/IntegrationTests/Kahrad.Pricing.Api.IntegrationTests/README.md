# API integration tests

This suite follows the reference at `C:\Users\Mehdi\Desktop\on-tech\OmidNikClub3\New folder\omidnikclub\Test\OmidNicClub.Api.IntegrationTests`:

- A shared WebApplicationFactory collection fixture.
- Setup/ApiTestBase.cs with per-test initialization/disposal, authenticated HttpClient, JSON helpers and fresh scoped database reads.
- Authentication and catalog/SMS setup extension methods.
- Respawn reset between tests and independent ApiTests classes organized by endpoint area.
- Real HTTP, database assertions, authentication, application services, repositories, validation and auditing.

The reference uses .NET 8, PostgreSQL/Testcontainers and vertical-slice handler discovery. Kahrad uses its existing .NET 10, native SQL Server, service/repository architecture and routes. Those unrelated reference choices were not copied. No Docker, PostgreSQL, EF InMemory, MediatR, alternative response envelope or new production authentication path is introduced.

## Run

From the repository root on a machine with the authorized local SQL Server instance:

```powershell
.\Tests\Run-ApiIntegrationTests.ps1
```

To use another named native SQL Server instance:

```powershell
.\Tests\Run-ApiIntegrationTests.ps1 -SqlServer '.\SQLEXPRESS'
```

After a successful build, add `-NoBuild` to avoid restoring/rebuilding. For a CI/test SQL account, set the connection string securely through the environment and run:

```powershell
$env:KAHRAD_TEST_SQL_SERVER = '<authorized SQL Server connection>'
dotnet test Tests/IntegrationTests/Kahrad.Pricing.Api.IntegrationTests -v minimal
```

The PowerShell runner uses Windows integrated authentication to localhost/master when the variable is absent and restores the caller's environment afterward. Direct dotnet test without the variable explicitly skips the SQL facts/theories; it does not silently use the application connection string. The SQL account needs permission to create and remove the generated test database. Connection strings and real passwords are not committed.

## Isolation and lifecycle

KahradPricingApiWebAppFactory creates `KahradApiTests_{guid}`, overrides all nine implemented module DbContexts to that database, and applies the actual Identity, Brands, Products, Guarantees, SMS, AccountingIntegration, Representatives, Pricing, and Sales migrations. It never migrates the user's application database. An existing database file cannot be attached. Reset and deletion require an exact match to the factory-generated database name and a valid GUID suffix.

ApiTestBase resets data, creates the baseline catalog/SMS configuration, seeds roles and a synthetic strong-password SuperAdmin through Identity, and logs in using the real password endpoint. It disposes its client and resets again after each test. Respawn includes the nine module schemas and preserves migration history; baseline language/provider/module settings are recreated. The collection disables parallelization to prevent reset races. The factory closes the web host and removes only its generated database at collection teardown.

The test host uses environment Testing, ephemeral data-protection keys, synthetic JWT settings and a test-only account. The existing MehdiNavab account, its password, and application databases are not used. Test overrides do not mutate process-global application connection settings. HTTP tests use real JWT validation and the actual token issuer, refresh-token repository, audit service and module services. ISmsProviderResolver and IAccountingProviderClient are replaced with recording external-provider boundaries, so no live SMS or accounting request is sent. Hangfire workers are not started by the API fixture; tests invoke the existing SMS outbox endpoint explicitly.

## Coverage

91 SQL/API cases across Authentication, Users, Products, Brands, Guarantees, SMS, Accounting, Representatives, Sales, Localization and Infrastructure:

- Seeded SuperAdmin login, wrong credentials, inactive users, validation, hashed refresh storage, rotation, replay/family revocation and logout.
- Real user creation/roles/audits, normal password policy, unauthorized writes, Admin/SuperAdmin target protections, ordinary-user reset.
- Product Unicode content/default translation, duplicate codes, missing references, updates, soft deletion and auditing.
- Localized public detail/search, quality weights and neutral fallback, independent shared labels and textual values, typed numeric values, stable counts/pagination, manual translation rowversion conflicts, inactive languages, default-language protection and all four missing-translation targets.
- Brand and vendor CRUD/validation/duplicates/soft deletion; guarantee-plan/certificate duration and missing-plan behavior.
- SMS queue versus dispatch, success/attempt logs, delayed retry, uncertain outcomes, repeated processing and unauthorized raw OTP sends.
- DI/validator registration, generated-database isolation, reset behavior and actual Unicode migration seed values.
- Brand-scoped accounting mirrors and price history, safe incomplete scans, item mappings, representative import/match, invoice read-only sync, secret-reference responses, local sale snapshots and factory price overrides.
- English, Persian and Arabic response resources, Accept-Language quality selection and X-Region fallback.

Existing unit, host/security, SMS persistence and populated-legacy catalog migration suites remain as separate focused regressions.

## Defects discovered and fixed

The first run found the missing-translations projection was ordered after constructing a response record, which EF Core could not translate. CatalogRepository.GetMissingAsync now orders/pages the entity query before projecting, and all four target cases are covered.

Earlier Persian/Arabic seed display labels and some older test fixtures had become question-mark literals. The fixtures now contain real Unicode. New incremental migration `20261105143057_RestoreCatalogLanguageDisplayNames` repairs only the exact known placeholder labels for fa/ar, using NCHAR literals independent of shell/editor encoding. It preserves manually supplied names and does not rewrite product translations or pricing. Its Down intentionally does not restore corrupted labels. No previously applied migration was edited, and this migration has not been applied to the user's application database by the integration-test task.

Validated against native SQL Server: all 91 API cases passed. The complete solution regression passed 158 tests across eight projects, with zero failures or skips. The solution build completed with zero warnings or errors.
