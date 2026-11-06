# Kahrad Pricing — P0: Complete the Pricing Engine

> **Codex implementation specification** — 2026-11-06
> **Priority:** P0 / Phase 1 only
> **Target:** Existing Kahrad Pricing .NET 10 solution, SQL Server, EF Core, Clean Architecture, xUnit
> **Language:** English for precise coding instructions; key business requirements originate from the product owner's Persian discussions.

## دستور کوتاه برای Codex

این فایل فقط برای **اولویت اول: تکمیل موتور قیمت‌گذاری** است. ابتدا کد موجود را بررسی کن، سپس فرمول‌ها، قوانین قیمت ثابت و افزایش دستی، تنظیمات مستقل هر برند، درصدهای نمایندگان، سقف تخفیف، گردکردن و تست‌های دقیق را تکمیل کن. ساختار موجود را حفظ کن. **Worker انتشار قیمت، Price Inquiry API، اتصال واقعی حسابداری، صفحات Angular و فرایند ثبت فروش را در این مرحله کامل نکن**؛ فقط قراردادهای ضروری و سازگار با آن‌ها را تعریف کن.

---

## 0. Mission and non-negotiable constraints

Implement a **correct, deterministic, testable, production-quality Pricing Engine** that can calculate, from a consistent read-only input snapshot, the following **three distinct monetary values**:

1. `ProductionCost` — total product item cost, **not rounded** by the selling-price rounding rule.
2. `RepresentativePrice` — factory-to-representative selling price, **rounded upward to a multiple of 5,000**.
3. `CustomerSellingPrice` — representative-to-end-customer price, **rounded upward to a multiple of 5,000**.

All monetary arithmetic must use `.NET decimal`, not `double` or `float`. Use the solution's established currency conventions; **do not silently assume IRR versus toman**. No hidden network requests, database reads, Redis calls, global mutable settings, or time-dependent behavior inside calculators.

**Scope guard:** This task is the engine + requisite domain/application policy storage/validation + tests, NOT the end-to-end pricing platform. In particular:

- Do **not** implement Pricing Worker scheduling, calculation-run orchestration, atomic publication, Redis, HybridCache, Price Inquiry API, live accounting HTTP, sales endpoints, warranty screens, or Angular/Vue UI in this task.
- Do **not** rewrite functioning Products, Brands, Representatives, Accounting, Identity, Sales, or Guarantees features.
- Do **not** add CQRS, MediatR, Vertical Slice, microservices, Docker, Kubernetes, or unrelated package upgrades.
- Do **not** invent accounting API endpoints or representative types/coefficients not specified by business rules.
- Do **not** make changes to already finalized sold-unit price snapshots.
- Follow existing repository ID types (`long`, `Guid`, etc.) and naming conventions after inspection; illustrative names below are **not permission to create duplicates**.
- Never reset or overwrite unrelated uncommitted changes. Generate new EF Core migrations only if schema changes are truly required; do not alter applied migration history.

## 1. Mandatory repository inspection before coding

The supplied implementation report (dated 2026-11-06) describes a substantial backend foundation. **The separately supplied dependency report is old; do not use it as a current inventory.** The current repository is the authority.

1. Open `Kahrad.Pricing.slnx` and inspect **all current projects**, including modules included inside other project folders. Build a current inventory rather than trusting either report.
2. Open the existing engine, reported at `Src/Modules/Pricing/Kahrad.Pricing.Pricing.Engine/PriceMath.cs`; inspect methods and their existing tests. Reuse/refactor rather than duplicating `PriceMath`, rounding functions, or representative calculations.
3. Inspect existing Pricing Domain/Application/Infrastructure services, `ProductFactoryPriceOverride` (or equivalent), EF Core configuration and migrations.
4. Inspect Products: `Product`, local `Item`, `ProductItem` composition, quantities, units, brand relation, and any local-to-accounting mappings. Identify the canonical source of the current item price, including brand scope.
5. Inspect Brands: brand-specific configuration/policy fields and their source of truth.
6. Inspect Representatives: actual brand ownership, pricing policies, general discounts, product-specific discounts, and any representative type model that already exists. **Do not fabricate a new representative-type formula.**
7. Inspect Sales immutable snapshots. Ensure the engine will never update completed sales.
8. Inspect how tests are organized, existing xUnit configuration, database test availability, API conventions, and auditing/concurrency patterns.
9. Check for any approved calculation rules in existing tests or product documentation. Record conflicts with this specification before changing behavior; **do not silently replace a business-approved rule**.

**Before implementation output** a short audit in the working notes (or `docs/pricing-engine-p0-audit.md`, if the repository already uses docs): existing types, reusable code, missing functionality, rule conflicts, schema impact, and planned changes. Then implement the safe, confirmed portions without waiting for a new prompt.

## 2. Confirmed business rules — authoritative for this task

### 2.1 Item-based production cost

For product `p` in brand `b`:

```text
ProductionCost(p,b) = Σ [QuantityUsed(p,item) × EffectiveBrandItemUnitPrice(b,item)]
```

- A product contains a list of local items (materials/parts/components) and a decimal usage quantity per item.
- Item prices originate from the accounting-price synchronization process, but **there is no requirement to call accounting while calculating a price**.
- If local items are shared across brands, their effective price must remain **brand-specific**.
- Units and unit conversion must be correct: the unit for `QuantityUsed` must match the unit to which `UnitPrice` applies. Prefer the existing mapping's conversion logic. Do not multiply mismatched units without conversion.
- Missing mapping, unknown price, missing unit conversion, invalid quantity, and inconsistent brand must produce a **structured calculation failure**, not silently `0` or a guessed amount.
- Do not round `ProductionCost` using the 5,000 selling-price rule.
- Product definition, translations, features and specifications remain owned by Products. Do not change their semantics.

### 2.2 Brand-specific factory calculation

For each brand, factory policy must resolve independently:

- `BeautyPercentage`
- `FactorySalesPercentage` (avoid naming collision with representative `SalesPercentage`)
- `CommercePercentage`
- `FactoryRoundingStep`, default **5,000** only for confirmed/default policy
- An optional `RepresentativeFactor` is **unresolved business functionality**. Existing approved factor behavior may be reused only when evidenced in current code/tests; otherwise the engine treats factor as `1` and explicitly records that no additional adjustment was applied.
- Warranty pricing and placement remain partially unresolved; use an explicit provided `WarrantyAmount` (default `0` for scenarios without warranty), not an invented lookup or provider.

**Current documented formula assumption** (simple additive factory percentages on the same base):

```text
FactoryPercentTotal = BeautyPercentage + FactorySalesPercentage + CommercePercentage
CalculatedFactoryBase = ProductionCost × (1 + FactoryPercentTotal / 100)
FactoryAfterFactor = CalculatedFactoryBase × RepresentativeFactor
```

This additive interpretation is a **working assumption**, not an independently re-confirmed factory policy. First inspect existing approved tests/configuration; if an existing authoritative rule differs, isolate the strategy behind an explicit policy and document the conflict instead of silently changing historical results. Never automatically switch to compounded percentages.

### 2.3 Fixed price and manual increase per product

The product owner explicitly requires both:

- **Fixed factory selling price**: a fixed selling price for a product instead of the factory formula output.
- **Manual additive amount**: e.g., add `5,000` to that product's factory selling price, **separate from production cost**.

Proposed calculation precedence, consistent with the earlier approved design document:

```text
FactoryBase = (PriceMode == Fixed)
    ? FixedFactorySellingPrice
    : FactoryAfterFactor

UnroundedRepresentativePrice = FactoryBase
    + ManualFactoryPriceAdjustment
    + WarrantyAmount

RepresentativePrice = RoundUp(UnroundedRepresentativePrice, FactoryRoundingStep)
```

Requirements:

- `PriceMode` should be explicit (`Calculated` / `Fixed`) rather than ambiguous nullable fields when possible.
- `FixedFactorySellingPrice` is required and positive when fixed mode is selected; absent/unused in calculated mode according to existing conventions.
- `ManualFactoryPriceAdjustment` defaults to `0`; its confirmed use is **nonnegative addition**. Negative manual adjustment must not be treated as a backdoor discount without a new business decision.
- `ProductionCost` always remains based on items even when the fixed selling price is selected. It must not include either the manual adjustment or the warranty amount.
- Changing the effective item price may update calculated current prices, but must not replace configured fixed selling values or manual amounts.
- **Factory percentage/factor should not be stacked on top of the fixed price** unless that is explicitly the existing approved behavior. Under the current precedence, a fixed price replaces the formula-derived factory amount, then manual adjustment and warranty are added once.
- Keep warranty/factor steps isolated in the code; exact applicability and scope will be confirmed later. Do not build an extensive warranty pricing subsystem here.

### 2.4 Representative-to-customer selling price

Each representative belongs to the appropriate brand (or has an explicit brand association already defined in the repository). Their pricing uses:

- `RepresentativeSalesPercentage`
- `RepresentativeGeneralDiscountPercentage`
- `RepresentativeProductDiscountPercentage` (unique per representative/product, default `0` when no specific discount is registered)

**Confirmed algebraic method (NOT sequential/compounded):**

```text
NetPercentage = RepresentativeSalesPercentage
              - RepresentativeGeneralDiscountPercentage
              - RepresentativeProductDiscountPercentage

SalesAmount          = RepresentativePrice × RepresentativeSalesPercentage / 100
GeneralDiscount      = RepresentativePrice × RepresentativeGeneralDiscountPercentage / 100
ProductDiscount      = RepresentativePrice × RepresentativeProductDiscountPercentage / 100

RawCustomerPrice = RepresentativePrice
                 + SalesAmount
                 - GeneralDiscount
                 - ProductDiscount
```

**Strict minimum price:**

```text
REQUIRE RawCustomerPrice >= RepresentativePrice
CustomerSellingPrice = RoundUp(RawCustomerPrice, CustomerRoundingStep)
```

- Check the **raw price before rounding**; rounding must never mask an invalid below-purchase price.
- Do **not** automatically clamp below-purchase prices upward to the purchase price. Return a meaningful, machine-readable validation/domain error so the representative or administrator can correct the policy.
- Equivalent percentage invariant for nonnegative `RepresentativePrice`: `GeneralDiscountPercentage + ProductDiscountPercentage <= RepresentativeSalesPercentage`. Enforce when creating/updating policies where practical and **again in the calculator**.
- If the general discount or sales percentage is changed, revalidate existing active product discounts and report conflicts. Do not silently delete or shrink existing discounts.
- General and product discounts are additive percentage points against the **same** `RepresentativePrice`; never calculate product discount after applying general discount.
- Factory price remains unaffected by representative discounts.
- Customer rounding step is confirmed as **5,000**, upward; the step must remain configurable through the existing policy structure only if that behavior already exists or is required for testability; default production configuration should be 5,000 for both selling levels.

### 2.5 Immutable historical sales

A sale of one physical unit must freeze **that unit's finalized sale monetary snapshot**:

- `ProductionCost` at sale time (where available), `RepresentativePrice`, `CustomerSellingPrice`, currency, applied policy/price version references, and relevant pricing adjustments.
- Never recompute/rewrite finalized sold-unit money after an item sync, brand policy change, manual adjustment, or representative discount change.
- **Do not freeze the entire catalog product just because some units have sold.** Remaining/next units can have new current prices.
- Integrate only at type/contract boundary in this P0 task; do not implement the Sales workflow, accounting posting, or warranty registration here.

## 3. Explicitly unresolved: do not guess

Record each of these in the final report with current behavior and an extension point:

1. Factory percentage calculation **additive vs compounded** where source code/approved examples disagree; current design assumes additive.
2. Exact `RepresentativeFactor` semantics and whether they vary by representative type, brand, or product; a neutral factor of `1` is acceptable if no confirmed policy is present, but no invented coefficient schedules.
3. Warranty fee source, applicability, and whether it participates in markup; this P0 formula takes an explicit `WarrantyAmount` after selecting calculated/fixed factory base.
4. Currency/price unit (rial vs toman); use the established source-of-truth convention and expose currency explicitly in outputs when known.
5. How discounts behave when a policy is edited while existing per-product discounts conflict; prefer reject-with-conflicts pending explicit administrative workflow.
6. How representative types affect factory purchase price. The implementation report mentions that representative-type price workflows are missing; no reliable formula was supplied. Provide a future strategy/extension point, **not fictional business calculations**.
7. Whether every product can be priced absent a valid mapping; missing inputs must fail safely and not publish an invented price.

## 4. Module boundaries and implementation placement

Use **existing names and projects** wherever possible. The report mentions:

```text
Src/Modules/Pricing/
  Kahrad.Pricing.Pricing.Domain/
  Kahrad.Pricing.Pricing.Application/
  Kahrad.Pricing.Pricing.Infrastructure/
  Kahrad.Pricing.Pricing.Engine/  <-- existing PriceMath.cs

Src/Modules/Products/       <-- composition, local items
Src/Modules/Brands/         <-- brand identity/configuration
Src/Modules/Representatives/ <-- representative policies/discounts
Src/Modules/Sales/          <-- immutable historical sale models, perhaps grouped elsewhere
```

- **Engine**: pure calculators, mathematical helpers and immutable computation input/output contracts; no EF Core, SQL, HTTP, Redis, logging framework or ASP.NET dependencies. If current design places calculations in Domain, follow the established direction without creating duplicate calculator classes.
- **Pricing Domain**: policy invariants, price modes, domain errors, policy state changes and values. Keep independent of EF Core/Sieve/provider types.
- **Pricing Application**: read-only contracts to obtain the consistent product composition, effective brand item prices, brand policy, overrides, representative policy and per-product discounts; orchestrate pure engine execution and map failures to application results.
- **Pricing Infrastructure**: only repositories/EF configurations/migrations required to support missing price policy/override data; resolve existing data rather than creating duplicate tables.
- **Products**: owns composition and item identity, not factory/customer price formulas. Provide read-only data access/contract if absent.
- **Accounting**: stores/provides synchronized effective item prices and source identifiers; actual HTTP synchronization is out of scope.
- **Representatives**: owns representative identity and brand association; clarify whether pricing-specific settings belong to Representatives or Pricing according to **existing ownership** and avoid cross-module entity ownership duplication.
- **Sales**: consumes a frozen price result on sale finalization in a later phase; must not be changed by this calculation operation.

**Architecture rule:** Avoid cyclic references. If Engine currently has no project references, keep it pure. Use immutable input objects passed to the Engine, not repository interfaces inside Engine. Preserve dependency-injection and module boundary conventions in the solution.

## 5. Data contracts and invariants (adapt actual types)

Illustrative names (do not blindly paste these if equivalents already exist):

```csharp
public enum FactoryPriceMode { Calculated, Fixed }

public sealed record PriceComponentInput(
    string ItemCode,
    decimal UnitPrice,
    decimal UsedQuantity,
    decimal UnitConversionFactor = 1m);

public sealed record BrandFactoryPolicyInput(
    decimal BeautyPercentage,
    decimal FactorySalesPercentage,
    decimal CommercePercentage,
    decimal RepresentativeFactor,
    decimal FactoryRoundingStep);

public sealed record FactoryOverrideInput(
    FactoryPriceMode Mode,
    decimal? FixedFactorySellingPrice,
    decimal ManualFactoryPriceAdjustment);

public sealed record RepresentativePolicyInput(
    decimal SalesPercentage,
    decimal GeneralDiscountPercentage,
    decimal ProductDiscountPercentage,
    decimal CustomerRoundingStep);

public sealed record PricingCalculationInput(
    /* actual IDs and currency per repository */
    IReadOnlyList<PriceComponentInput> Components,
    BrandFactoryPolicyInput FactoryPolicy,
    FactoryOverrideInput FactoryOverride,
    RepresentativePolicyInput RepresentativePolicy,
    decimal WarrantyAmount);
```

Keep no implicit `Now` lookup; a caller-supplied `AsOfUtc` / input-price version may be included in calculation metadata. The same input must always yield the same output.

Output should expose enough detail for audits/troubleshooting:

```text
ProductionCost
CalculatedFactoryBase
FactoryAfterFactor
FactoryPriceMode
FixedFactorySellingPrice (if active)
ManualFactoryPriceAdjustment
WarrantyAmount
FactoryRawSellingPrice
RepresentativePrice
RepresentativeSalesAmount
GeneralDiscountAmount
ProductDiscountAmount
NetRepresentativePercentage
RawCustomerPrice
CustomerSellingPrice
FactoryRoundingStep
CustomerRoundingStep
BrandId / ProductId / RepresentativeId (from validated request context)
SourcePriceVersion or input fingerprint (if available)
Currency (when established)
```

**Security:** detailed cost breakdown is for authorized internal roles only. Public website DTOs must never include production cost or unpublished policies. Creating a public read API is **out of scope**.

### Validation rules

- Quantities, unit prices, conversion factors and money values must be finite/nonnegative, with `UsedQuantity > 0` and `ConversionFactor > 0` for each participating component; validate any special zero-quantity semantics from actual repo before changing them.
- Reject missing effective brand-specific item prices, unknown brand association, duplicate/conflicting item entries where they would lead to double counting, and invalid arithmetic overflow.
- `FixedFactorySellingPrice > 0` if fixed mode; manual additive amount `>= 0`; warranty `>= 0`.
- Discount percentages must be in `0..100`; representative sales percentage and factory markup percentages must be nonnegative, with any maximum taken from approved business validation (do not invent arbitrary caps).
- `RepresentativeFactor > 0` when enabled; neutral default is `1` only where a factor is not configured under a confirmed neutral policy.
- `RoundingStep > 0`; production policy rounds to **5,000** unless the repo has an already accepted effective policy migration. Reject inconsistent rounding configuration instead of silently returning unintended prices.
- Compute and check raw minimum customer price before rounding. Reject a negative net margin even if `RepresentativePrice` is `0` or rounding might conceal it.
- Brand must match product, item-price scope and representative association. Do not trust a caller-supplied `BrandId` without a verified relation.
- If legitimate items may cost `0`, distinguish an explicitly known zero-priced item from a **missing item price**. Never conflate the two.
- Use checked/explicitly guarded decimal arithmetic as appropriate. Keep error codes consistent with existing exception/problem-response design.

## 6. Single rounding rule (confirmed)

```csharp
public static decimal RoundUpToStep(decimal amount, decimal step = 5000m)
{
    if (amount < 0m) throw new ArgumentOutOfRangeException(nameof(amount));
    if (step <= 0m) throw new ArgumentOutOfRangeException(nameof(step));
    return Math.Ceiling(amount / step) * step;
}
```

Reuse the existing rounding implementation if equivalent. Call rounding **once at the end of the factory selling-price calculation**, after fixed/calculated selection and manual + warranty amounts; call rounding **once at the end of the representative customer-price calculation**. Never round the individual item subtotal or each markup/discount stage to 5,000.

Examples (`RoundUpToStep(x, 5000)`):

| Raw | Result |
| ---: | ---: |
| 0 | 0 |
| 120000 | 120000 |
| 120001 | 125000 |
| 123456 | 125000 |
| 125000 | 125000 |
| 125001 | 130000 |
| 129999 | 130000 |
| 1234567 | 1235000 |

All monetary examples are unit-neutral until currency has been confirmed.

## 7. Numerical acceptance fixtures — automated tests required

The numbers below define **repeatable expected results**. For factory-side fixtures, use simple additive brand percentages as documented in §2.2. If the current solution has a conflicting approved factory formula, show it in audit and align fixtures only after documenting why.

### Case A — baseline three prices

```text
Item 1: UnitPrice = 100000, Quantity = 2  -> 200000
Item 2: UnitPrice = 150000, Quantity = 1  -> 150000
ProductionCost = 350000

Beauty = 10%, FactorySales = 15%, Commerce = 5%
CalculatedFactoryBase = 350000 × 1.30 = 455000
RepresentativeFactor = 1
Fixed mode = Calculated
Manual adjustment = 0
Warranty = 50000
Raw factory selling price = 505000
RepresentativePrice = 505000

RepresentativeSales = 20%, GeneralDiscount = 5%, ProductDiscount = 3%
NetPercentage = 12%
RawCustomerPrice = 505000 × 1.12 = 565600
CustomerSellingPrice = 570000
```

### Case B — larger illustrative end-to-end example

```text
ProductionCost = 10000000
Beauty = 10%; FactorySales = 15%; Commerce = 5%
CalculatedFactoryBase = 13000000
Factor = 1; Warranty = 500000; Manual = 0
RepresentativePrice = 13500000
RepresentativeSales = 20%; GeneralDiscount = 5%; ProductDiscount = 3%
RawCustomerPrice = 15120000
CustomerSellingPrice = 15120000
```

### Case C — fixed factory price and additive amount (not production cost)

```text
ProductionCost = 800000   # remains 800000, regardless of selling mode
CalculatedFactoryBase may still be computed for diagnostics
PriceMode = Fixed
FixedFactorySellingPrice = 1234000
ManualFactoryPriceAdjustment = 5000
WarrantyAmount = 0
RawFactorySellingPrice = 1239000
RepresentativePrice = 1240000
RepresentativeSales = 0; GeneralDiscount = 0; ProductDiscount = 0
CustomerSellingPrice = 1240000
```

### Case D — invalid customer sale below representative purchase

```text
RepresentativePrice = 1000000
RepresentativeSales = 10%; GeneralDiscount = 5%; ProductDiscount = 6%
NetPercentage = -1%
RawCustomerPrice = 990000
=> DOMAIN VALIDATION ERROR (never clamp and never round to conceal it)
```

### Case E — margin exactly zero is allowed

```text
RepresentativePrice = 1000000
RepresentativeSales = 20%; GeneralDiscount = 5%; ProductDiscount = 15%
NetPercentage = 0%
RawCustomerPrice = 1000000
CustomerSellingPrice = 1000000
```

### Case F — rounding after customer calculation

```text
RepresentativePrice = 125000
RepresentativeSales = 12.5%; GeneralDiscount = 5%; ProductDiscount = 2.5%
NetPercentage = 5%
RawCustomerPrice = 131250
CustomerSellingPrice = 135000
```

### Case G — fixed price does not affect item costing

```text
Same product, same items and quantities
Change FixedFactorySellingPrice from 1000000 to 1500000
ProductionCost MUST remain unchanged
RepresentativePrice MAY change
Previously finalized sold-unit snapshots MUST remain unchanged
```

### Case H — per-brand item-price isolation

```text
One local ItemId shared/reused where the data model permits it:
Brand A effective unit price = 100000
Brand B effective unit price = 130000
Used quantity = 2
Brand A ProductionCost = 200000
Brand B ProductionCost = 260000
Never use Brand A's item price for Brand B, including missing-data fallbacks.
```

### Case I — per-representative product discounts

```text
Same published factory RepresentativePrice = 1000000
Both reps have Sales = 20%, GeneralDiscount = 5%
Rep A ProductDiscount = 3% -> Raw = 1120000 -> Customer = 1120000
Rep B ProductDiscount = 10% -> Raw = 1050000 -> Customer = 1050000
Factory RepresentativePrice does not change between reps.
```

### Case J — fractional quantities and no premature rounding

```text
Item 1: UnitPrice = 12345.67, UsedQuantity = 0.5
Item contribution = 6172.835
ProductionCost should preserve exact decimal calculation within chosen precision policy
Apply the 5000 rounding only on final SELLING prices, not per item.
```

## 8. Unit test matrix (xUnit; reuse existing test projects)

At minimum write passing tests for:

1. Sum of item `UnitPrice × UsedQuantity` with 1, many and fractional quantities.
2. Brand-specific effective item price; brand mismatch and missing input must fail safely.
3. Quantity/unit conversion application; conversion factor zero/negative rejected.
4. Factory percentages additive under documented policy; factory and representative percentages must not be mixed.
5. Fixed mode replaces formula-derived price for selling, while item-based cost remains intact.
6. Manual `+5000` applies once; it never changes `ProductionCost`.
7. Warranty amount applies exactly once at the documented stage; no warranty results in zero addition.
8. Both selling prices round upward to step 5,000; already aligned amounts are unchanged.
9. Negative/zero rounding step and negative money rejected as appropriate.
10. Representative net percentage is algebraic, **not compounded**.
11. Discount sum equals representative sales percentage => at-cost customer sale allowed.
12. Discount sum exceeds sales percentage => failure **before** rounding; no clamping.
13. Independent representative/product discount combinations and absent discount default zero.
14. Updating general representative discount with conflicting per-product discounts produces a clear validation result with affected product IDs where available.
15. Same immutable calculation input gives identical result twice; calculator does not access clock or network.
16. Malformed/missing item-price data cannot produce a misleading zero selling price.
17. Overflow/decimal precision boundary tested according to project's SQL precision.
18. New calculation does **not modify** immutable finalized sold price snapshots.
19. Tests with Case A through Case J, with exact values and assertions.

If SQL Server integration-test infrastructure is available, add focused persistence tests for policies and overrides (unique keys, decimal precision, concurrency, migrations); do **not** count skipped SQL tests as executed verification. Do not add a new database/container dependency just to pass unit tests.

## 9. Persistence and application work: minimal additions only

If existing project models already support these concepts, **complete/extend them**, rather than creating parallel entities.

| Data concept | Recommended ownership | Important persistence rule |
| --- | --- | --- |
| `BrandPricingPolicy` | Pricing or existing Brands policy service | one effective policy per brand, history/version as supported |
| Effective brand item price | Existing Accounting/Pricing read model | unique brand/external/local item identity and source/version metadata |
| `ProductFactoryPriceOverride` | Pricing | unique ProductId (or BrandId + ProductId if required by current data model); fixed mode + additive amount |
| Representative pricing policy | Pricing or Representatives per existing ownership | unique representative/(brand association), version/concurrency |
| Per-product representative discount | Pricing or Representatives per existing ownership | unique representative + product in brand scope |
| Computation breakdown | Engine/Application | immutable DTO/value object; not automatically a published DB record |

- Use EF Core entity configurations and SQL Server `decimal(p,s)` with adequate precision; preserve the currently established application's precision conventions if sufficient. Do not truncate or silently round intermediate calculations due to schema precision.
- Use rowversion/optimistic concurrency for mutable pricing policies where the repository supports it.
- Ensure no direct reuse of one brand's policy or representative discount across unrelated brands.
- Any migration must be additive/backward compatible where feasible; script/review generated schema changes and indexes. Do not apply migrations against production from Codex.
- Existing Products' multilingual names, shared feature/specification structure, translations, and catalog CRUD must remain intact.
- Do not persist a calculated factory price as if it were the immutable price of a sold unit.
- Do not create a new `PublishedProductPrice` table or calculation batch version under this P0 scope unless an existing schema requires a small compatibility extension; publication is Phase 2.

### Application services

Provide a clean reusable operation to **calculate a product price preview** using existing read contracts; it may be an internal service (or an existing secured admin endpoint, only if already present) with validated `ProductId` and `RepresentativeId`/brand association. Avoid introducing a public Price Inquiry endpoint now.

The service should:

1. Resolve product + authoritative brand and relevant composition.
2. Resolve current effective brand-scoped item prices and units from already synchronized local data.
3. Resolve factory pricing policy, product override, representative policy and product discount.
4. Build a consistent immutable calculation input and invoke the pure engine.
5. Return three prices + auditable calculation breakdown + explicit errors for invalid/missing inputs.
6. Perform no writes to finalized sales or published price pointers.

For consistent reads, use a version-stamped input snapshot when existing infrastructure supports it; otherwise document the concurrency risk. Full transactional publication/version allocation belongs to a later phase.

## 10. Domain error taxonomy

Prefer the existing error/result infrastructure. If absent, propose concise, stable codes such as:

```text
PRICING_PRODUCT_NOT_FOUND
PRICING_BRAND_MISMATCH
PRICING_ITEM_PRICE_MISSING
PRICING_INVALID_UNIT_CONVERSION
PRICING_INVALID_BRAND_POLICY
PRICING_FIXED_PRICE_REQUIRED
PRICING_INVALID_PRICE_ADJUSTMENT
PRICING_REPRESENTATIVE_POLICY_MISSING
PRICING_DISCOUNT_BELOW_PURCHASE_PRICE
PRICING_INVALID_ROUNDING_STEP
PRICING_DECIMAL_OVERFLOW
```

Errors should include relevant safe IDs and reason details, never accounting API keys or sensitive customer data. For policy conflict, include the list/count of affected products if practical.

## 11. Implementation plan / commit-sized sequence

**Phase A — audit and tests first**

1. Inspect actual solution, `PriceMath`, existing override/policy entities and tests.
2. Document code/requirement mismatches and all open business decisions.
3. Add numerical acceptance tests (Cases A–J) to existing test project(s); see expected failures before implementation where practical.

**Phase B — engine**

4. Implement/refactor single shared rounding helper and factory computation.
5. Implement/refactor representative algebraic pricing and minimum purchase-price validation.
6. Add explicit fixed/calculated price mode + manual addition handling and complete breakdown.
7. Validate item prices, quantities, units, percentages, representative-brand relations, missing data and overflow.

**Phase C — application/persistence compatibility**

8. Reuse/extend current brand policy, product override, representative policy and product discount read/write models where actually absent.
9. Add only necessary EF Core configuration, indexes, migrations and dependency-injection changes.
10. Provide a reusable Application service with read-only input resolution and pure engine invocation.

**Phase D — verification and report**

11. Run targeted pricing unit tests, then relevant tests, then full solution build. Address failures introduced by this work.
12. Attempt available integration tests; report skipped, failed-to-start and environment-blocked projects separately.
13. Inspect diff for unintended modifications to Sales, Products multilingual, Accounting HTTP, Workers, Media and UI.
14. Produce a final summary with exact paths, test commands/results, assumptions, migrations and follow-up tasks.

## 12. Definition of Done — P0 is complete only when

- [ ] Existing implementation is audited; nothing duplicated needlessly.
- [ ] A pure deterministic engine returns `ProductionCost`, `RepresentativePrice`, and `CustomerSellingPrice` with an auditable breakdown.
- [ ] Brand-level factory policy is applied correctly and independently.
- [ ] Item-price/quantity/unit issues yield structured errors, not false zero prices.
- [ ] Fixed factory selling price and additive manual increase work independently of production cost.
- [ ] Representative sales %, general discount % and product discount % use the **algebraic method**.
- [ ] Representative cannot sell below purchase price; invalid case rejected before rounding.
- [ ] Both selling prices round **up to multiples of 5,000** exactly once each; production cost is not rounded to 5,000.
- [ ] Representative/product discount and brand association are enforced.
- [ ] No finalized sold-unit historical monetary snapshot is modified by calculations or item-price updates.
- [ ] Cases A–J and validation/unit tests pass with exact expected values.
- [ ] Solution builds successfully; skipped/blocked test results are disclosed rather than called passing.
- [ ] Existing APIs, multilingual products and other modules remain compatible.
- [ ] All unconfirmed details (factor, warranty, currency, factory formula conflict, representative types) are documented, not silently guessed.

**Not part of this Definition of Done:** CalculationRun, atomic version publication, Hangfire pricing jobs, Redis, Price Inquiry API, external accounting provider, actual invoice/sale posting, frontend screens, or production deployment.

## 13. Required final Codex response

When finished, report in this order:

1. **What existed before / what changed** — with exact code paths.
2. **The final implemented formulas** and one example computed by tests.
3. **Reused vs newly created entities, services, policies and migration names**.
4. **Test commands and exact pass/fail/skip counts**; note integration-test environment issues.
5. **Business assumptions/open decisions** (with clear flags, not buried in prose).
6. **Any out-of-scope work explicitly left for P1/P2**, especially price Worker, atomic publication and Price Inquiry.
7. **Potential backward compatibility and data migration risks**.

## 14. Copy/paste Codex kickoff prompt

```text
Read KAHRAD_PRICING_ENGINE_P0_CODEX.md in full and implement ONLY P0
(Complete the Pricing Engine) in the current Kahrad.Pricing.slnx.

The dependency-report.txt inventory is outdated. Audit the actual repository.
Reuse existing PriceMath, ProductFactoryPriceOverride, policies, entities,
DI, EF migrations and tests whenever possible.

Implement deterministic decimal computation for ProductionCost,
RepresentativePrice and CustomerSellingPrice; brand-specific factory policies;
fixed price/manual +5000; algebraic representative discounts;
strict no-below-purchase rule before rounding; and 5000-step upward rounding
of both selling prices. Preserve finalized sold-unit price snapshots.

Execute numerical acceptance tests A-J, relevant unit tests, and build.
Do not implement Worker, price publishing, Price Inquiry API, Redis,
real accounting HTTP, Sales workflows, frontend or unrelated refactors.
Document any unresolved coefficient/warranty/factory-formula assumptions.
Provide exact file paths, commands and actual test results.
```
