# Project Status — SadrScales-Integration

**Last updated:** 2026-08-19  
**Phase:** Vendor-Ready 5.2.1 implementation  
**Current stable release:** `v1.0.0`  
**Next Vendor-Ready package version:** `1.1.0`  
**Supported published Sadr Scales baseline:** `5.2.1`  
**Current published integration contract:** `SQL Contract v1`

## Canonical references

Read in this order:

1. `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md`
2. `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`
3. `.github/maintainers/INTEGRATION_CONTRACT_DOMAIN_DESIGN_FA.md`
4. `docs/DECISIONS.md`
5. this file

Chat history is not the project source of truth.

## Immediate business priority

The next software-vendor outreach must happen only after a **Vendor-Ready Stable Baseline** is complete.

```text
Complete 5.2.1 SQL Integration
        ↓
SDK + Raw SQL + Docs + Sample
        ↓
End-to-End Vendor Acceptance Test
        ↓
Stable RC / Freeze
        ↓
One serious vendor outreach / letter
```

Sadr Scales 5.3 features must not delay this baseline.

## Frozen integration decisions

- SQL remains the current third-party integration transport for Sadr Scales 5.2.1.
- `SADR_Scale.Status` is the supported SQL source for coarse Online/Offline state.
- Structured Invoice lookup is supported by TotalBarcode and by ScaleID + FID.
- Invoice lookup never auto-ACKs.
- Explicit ACK happens only after destination Save/Commit succeeds.
- Invoice ACK sets `SADR_Total.LableStatus = 1` and is idempotent.
- An ACKed invoice still returns complete data with `AlreadyRead`; this is a warning, not a block.
- Item AutoSend resend request uses `LastSendItem = 0`.
- HotKey AutoSend resend request uses `LastSendKey = 0` where supported.
- Resend request success means the DB resend state was recorded; it does not mean the physical scale already received the data.
- Canonical multi-group scale assignment is `SADR_ScaleItemClass`; the public SDK hides that table behind `ScaleAssignments`.
- Per-scale item mapping and group HotKey templates are separate concepts and separate public clients.
- Public group HotKey APIs manage only positive-PLU user rows; zero/negative internal/system rows remain private and are preserved.
- Replace-style configuration operations are atomic. An `Unchanged` operation does not create a new resend request.
- `Sales.ReadAfterAsync` remains the ID-ascending destination-owned synchronization feed.
- `Sales.QueryAsync` is a separate newest-first search/report surface and never advances or replaces the destination feed cursor.
- Sales Query and Reports share one filter contract: inclusive Start, exclusive End, exact Identify, PLU, Scale ID/DeviceNo and FID.
- Sales summary counts invoices by distinct `(DeviceNo, FID)`; raw row count and invoice count are intentionally different concepts.
- Unit codes 0/1/3 contribute to sales weight and Unit 2 contributes to sales quantity, matching Sadr Scales 5.2.1.
- Common period helpers preserve Today, Saturday-based CurrentWeek and Persian-calendar CurrentMonth semantics.
- Public reports are typed Daily / Scale / Item results; Item report is capped at 5000 rows as in 5.2.1.
- A per-scale typed Command Mailbox is planned for **Sadr Scales 5.3**, not for the immediate 5.2.1 Vendor-Ready release.
- Service/REST can later become another transport over the same Command Domain.

## Vendor-Ready 5.2.1 SQL scope

```text
Connection/schema validation
Store read/upsert
Item Group read/upsert
Item/PLU read/upsert/bounded batch/soft-delete
Price history read
Static Scale read
Scale Online/Offline status read
Scale Group Assignment
Scale Item Mapping
HotKey Template
Request Item Resend
Request HotKey Resend where supported
Sales Feed
Sales Query / Summary
Structured Invoice lookup
Invoice ACK
Daily / Scale / Item Reports
```

## Completed implementation slices

### Slice 1 — Structured Invoice + ACK

Completed and merged:

- structured lookup by TotalBarcode and ScaleID + FID;
- explicit idempotent ACK;
- `FoundUnread / AlreadyRead / NotFound`;
- full invoice remains available after ACK;
- Raw SQL recipe, Persian/English docs and Invoices Sample flow;
- disposable SQL Server and .NET Framework 4.8 package-consumer coverage.

### Slice 2 — Scales + Status + Resend

Completed and merged:

- `client.Scales.GetAllAsync()` / `GetAsync()` / `GetStatusAsync()`;
- public `Online / Offline / Unknown` status mapping;
- static scale metadata;
- Item and supported HotKey AutoSend resend requests without exposing internal watermarks;
- `Requested / NotFound / UnsupportedModel` results;
- Raw SQL recipe, Persian/English docs and Scales Sample flow;
- disposable SQL Server and .NET Framework 4.8 coverage.

`Requested` always means the resend state was recorded for a later eligible AutoSend cycle; it is not a physical-device completion result.

### Slice 3 — Stores + Catalog completion

Completed and merged on PR #17:

- Store read/upsert with `StoreCode` as stable identity;
- Item Group read/list plus existing semantic upsert;
- Item read/list with active-only default and optional include-deleted mode;
- idempotent `SoftDeleteAsync` using `DeleteFlag = 1`, never physical item deletion;
- logically deleted rows remain individually readable for recovery/inspection;
- Price History read by PLU and recent-list read, newest first;
- Price History remains read-only; no new automatic PriceLog rule is invented;
- Raw SQL catalog recipe, Persian/English docs and Catalog Sample pages;
- disposable SQL Server tests and .NET Framework 4.8 public-surface coverage;
- Item update Sample preserves non-edited PLU fields before upsert.

### Slice 4 — Scale Assignments + Mapping + HotKeys

Completed and merged on PR #18. Main baseline after merge: `fba134b8c1a19c9f51365fb246db2cc1f9492f1c`.

- `client.ScaleAssignments.GetGroupsAsync(scaleId)`;
- atomic `ReplaceGroupsAsync` with `NotFound / Unchanged / Replaced`;
- assignment changes reset Item AutoSend state only when the assignment actually changes;
- `client.ScaleMappings.GetAsync(scaleId)`;
- atomic `ReplaceAsync` and validated `CopyAsync` for per-scale mapping;
- mapping validation covers duplicate PLU, duplicate per-scale ItemCode, duplicate HotKey position, Page/Key pairing and persisted scale HotKey layout;
- incompatible Copy leaves the destination unchanged;
- real mapping changes reset Item + HotKey AutoSend state;
- `client.HotKeys.GetGroupAsync()` / `ReplaceGroupAsync()` for user-managed group templates;
- public HotKey API hides and preserves zero/negative internal/system rows;
- real group HotKey changes reset HotKey AutoSend state only for scales assigned to that group;
- unchanged configuration does not create a new resend request;
- guarded Raw SQL reference, Persian/English configuration guides and WinForms configuration flows;
- disposable SQL Server, WinForms Build and .NET Framework 4.8 coverage all passed before merge.

### Slice 5 — Sales Query + Reports

Implemented on PR #19 and pending final exact-head CI/merge:

- existing `client.Sales.ReadAfterAsync(...)` remains unchanged as the destination-owned incremental feed;
- `client.Sales.QueryAsync(filter)` provides newest-first filtered/paged search without touching that cursor;
- optional filters: inclusive Start, exclusive End, exact Identify, PLU, Scale ID/DeviceNo and FID;
- page size follows 5.2.1 normalization of 50..2000;
- complete-filter `RecordCount / InvoiceCount / TotalPrice / TotalWeight / TotalQuantity` summary;
- distinct invoice identity is `(DeviceNo, FID)`;
- weight uses Unit 0/1/3 and quantity uses Unit 2;
- `SadrSalesPeriod` exposes Today, Saturday-based CurrentWeek and Persian CurrentMonth boundaries;
- typed `client.Reports.GetDailyAsync`, `GetByScaleAsync` and `GetByItemAsync`;
- Item report remains capped at 5000 rows;
- Query and all Reports use the same filter builder and are read-only;
- SQL integration tests verify ordering, filters, summary arithmetic, invoice counting, unit semantics and all three report shapes;
- Raw SQL `07-sales-query-reports.sql` is read-only and documents Feed-vs-Query separation;
- Persian and English Sales/Reports guides added;
- .NET Framework 4.8 public-surface coverage added;
- WinForms Developer Sample includes a read-only Sales/Reports area with filters, Today/Week/Persian-Month presets, Query and three report views.

The next additive package version remains frozen as `1.1.0`. Historical `v1.0.0` remains immutable.

## Remaining implementation slice

### Slice 6 — Demo Data + Vendor Acceptance + RC

Target:

- deterministic seeded Demo Data, including reproducible `Seed` support;
- safe generation of Stores / Groups / Items / Scales / Assignments / HotKeys and related sample relationships;
- explicit production/customer-database guard before Demo generation/reset;
- Demo reset flow;
- results visible in the executable Developer Sample;
- external-developer-style end-to-end acceptance test starting from the public package/repository surface;
- Getting Started / Capabilities / Recipes / Reference cleanup so maintainers' material does not dominate the vendor path;
- final compatibility review;
- `1.1.0` RC freeze;
- after RC, only bug/security/compatibility fixes before vendor outreach.

## Out of immediate scope

These items must not hold up Vendor Outreach:

```text
Sadr Scales 5.3 Command Mailbox implementation
Service / REST transport
Realtime Runtime progress API
Public Scale Emulator
Full Integration Lab
Advanced runtime device commands beyond the 5.2.1 SQL surface
Firmware/File/Label public operations
```

## Vendor-Ready release gate

Before the next software-vendor letter/outreach:

1. complete the final SQL Contract for the approved 5.2.1 capabilities;
2. implement all corresponding SDK APIs;
3. keep Raw SQL examples for non-C# consumers;
4. keep real SQL integration tests for all sanctioned writes and important read semantics;
5. complete the runnable WinForms Developer Sample;
6. provide seeded/reproducible Demo Data with Production-DB guard;
7. keep a short Getting Started path plus a complete reference guide;
8. keep `.NET Framework 4.8` consumer validation and SQL Server CI green;
9. run an external-developer-style end-to-end acceptance test;
10. freeze naming/contract after RC except for bug/security/compatibility fixes.

## Sadr Scales 5.3 follow-up

A per-scale Command Mailbox is a planned 5.3 capability. Runtime remains responsible for validation, licensing, connection/busy state, model capability and actual protocol execution. Raw device protocol details remain private.

## Stable release identity

- Git tag: `v1.0.0`
- Stable source commit: `a6bccc7c13a8afba29b6860869d2a942b1231803`
- Release ID: `372167195`
- Protected Release run: `32112295891` — PASS
- License: MIT
- Providers/copyright identity: **Tozin Sadr and Behzad Erfanian**

The stable tag is immutable and must never be moved or reused.

## Security boundary

The public repository and future Integration surface must not expose:

- proprietary device protocols or raw packet formats;
- captures/PCAPs or reverse-engineering material;
- private firmware/vendor data;
- private keys or secrets;
- customer production data;
- arbitrary SQL execution;
- arbitrary Runtime/protocol command execution.

## Handoff rule

A future session starts from the canonical references listed at the top of this file. Chat history is not the project source of truth.
