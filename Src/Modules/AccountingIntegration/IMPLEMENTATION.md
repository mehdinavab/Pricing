# Accounting integration: local implementation and contract gaps

This work follows [KAHRAD_ACCOUNTING_INTEGRATION_CODEX.md](../../../KAHRAD_ACCOUNTING_INTEGRATION_CODEX.md). The source did not contain a local Item/BOM model, a representative master, a sales snapshot/outbox, or accounting mirrors. Brands, Products, Identity, Guarantees, the API host, and the Worker already existed. The Angular admin source currently has an initial app shell but no authenticated administration feature framework or accounting API client.

## Implemented locally

- Products owns local `Item` and `ProductItem` composition. Product item quantities use the local item's unit. Accounting sync never writes the local item catalog. Product Item APIs allow creation, search, composition reads, and bounded replacement.
- AccountingIntegration owns one brand connection, external item/price history, representative and invoice mirrors, item mappings, and sync runs. External identifiers are unique within a brand. Connection APIs accept only `env:VARIABLE_NAME` secret references and return only a `HasSecretReference` flag. Saved connections remain disabled until the real provider contract is implemented.
- A normalized `IAccountingProviderClient` boundary and a `NotConfiguredAccountingProviderClient` exist. The test host supplies a recording provider. No production HTTP route, authentication header, paging method, or sale-price posting payload has been invented.
- Complete item scans upsert by `(BrandId,ExternalItemId)` and add a price history row only if amount or currency changes. A failed or partial scan cannot mark absent items as missing. Completed representative syncs can mark external representatives absent without deleting local records. Invoice imports update only the external mirror.
- The Representatives module owns brand-scoped local representatives and explicit external mappings. Import is idempotent; a code collision requires an explicit match. Existing local representative names and Identity roles are not rewritten by matching.
- Pricing.Engine contains only the confirmed production cost, algebraic customer percentage, pre-rounding no-loss check, and upward 5,000 step rules. Pricing owns a versioned, brand/product-scoped factory override record. The override is stored separately and does not change Product.BasePrice or publish a selling price.
- Sales owns an immutable `SoldProductUnit` monetary snapshot and one `AccountingSalePostOutbox` intent per finalized sale. The local transaction stores both together, and a repeated operation ID resolves to the original sale. Unknown provider outcome moves the outbox to manual review. There is no public finalization endpoint because a trusted published-price reader does not yet exist.

## Administrative API

All new write endpoints require a SuperAdmin JWT until brand-scoped permissions are implemented. They use the existing API host, POST convention, audit service, and translated problem responses.

| Route prefix | Actions |
| --- | --- |
| `/api/products/items` | `create`, `search`, `composition/get`, `composition/replace` |
| `/api/accounting` | `connections/list`, `connections/save`, `items/search`, `items/map`, `items/unmap`, `invoices/search`, `sync/items`, `sync/representatives`, `sync/invoices`, `sync/history` |
| `/api/representatives/accounting` | `search`, `import`, `map` |
| `/api/pricing/product-override` | `get`, `save` |

Sync actions currently return `503 NotConfigured` in production because the real adapter is unavailable. The test fake makes the same orchestration executable in integration tests. No recurring accounting jobs are registered; a schedule and provider limits are unknown.

## Migrations and rollout

New EF migrations were generated for Products, AccountingIntegration, Representatives, Pricing and Sales. They have not been applied to the application database. Review the SQL scripts and existing data before applying them. The API integration suite applies every module migration only to a generated `KahradApiTests_{guid}` database and deletes it after the suite. It never imports or changes live accounting data.

## Remaining contract decisions

The real provider API base URL, authentication header, DTOs, stable IDs, pagination, units, currency, invoice meanings, posting target/idempotency contract, and rate limits are still absent. Factory percentage stacking, representative factor, warranty amount, currency conversion, and product repricing triggers remain open in the architecture. Until they are confirmed, mapped accounting prices are recorded as history but are not promoted into published Product prices. Sales finalization cannot be exposed to clients before an approved published price and representative policy exist. Outbox delivery and worker retries cannot safely send a request without the provider's posting and duplicate-detection contract.

The existing GuaranteeCertificate already stores buyer name, phone, national code, purchase/start/expiry dates and a product serial. Linking it to a finalized sold unit requires the approved physical-unit/serial lifecycle. The Angular admin source has no reusable authenticated admin feature structure yet; wiring screens now would create a disconnected interface. Backend routes and response contracts are ready for that UI work once the provider and price publication flows are confirmed.
