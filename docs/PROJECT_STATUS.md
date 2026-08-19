# Project Status — SadrScales-Integration

**Last updated:** 2026-08-19  
**Phase:** Vendor-Ready 5.2.1 — final RC gate  
**Current stable release:** `v1.0.0`  
**Next Vendor-Ready package version:** `1.1.0`  
**Supported Sadr Scales baseline:** `5.2.1`

## Canonical references

1. `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md`
2. `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`
3. `.github/maintainers/INTEGRATION_CONTRACT_DOMAIN_DESIGN_FA.md`
4. `docs/DECISIONS.md`
5. this file

Chat history is not the project source of truth.

## Business objective

The next software-vendor outreach happens only after the `1.1.0` Vendor-Ready baseline is frozen and green.

```text
Complete 5.2.1 Integration
→ SDK + Raw SQL + Docs + Sample
→ Package-only Vendor Acceptance
→ 1.1.0 RC Freeze
→ one serious vendor outreach
```

Sadr Scales 5.3 work must not delay this baseline.

## Frozen 5.2.1 integration scope

```text
Connection/schema validation
Stores
Item Groups
Items / PLUs
Price History read
Static Scale read
Online / Offline status
Scale Group Assignment
Scale Item Mapping
Group HotKey Template
Item AutoSend resend request
supported HotKey AutoSend resend request
Sales Feed
Sales Query / Summary
Structured Invoice lookup
Invoice ACK
Daily / Scale / Item Reports
```

## Completed slices

### Slice 1 — Structured Invoice + ACK

Merged and green. Includes lookup by TotalBarcode and ScaleID+FID, full details, explicit idempotent ACK, `FoundUnread / AlreadyRead / NotFound`, Raw SQL, docs, Sample and SQL/net48 tests.

### Slice 2 — Scales + Status + Resend

Merged and green. Includes scale metadata/status and Item/HotKey AutoSend resend requests without exposing internal watermarks.

### Slice 3 — Stores + Catalog

Merged on PR #17 and green. Includes Store/Group/Item reads and writes, bounded batch, logical delete, Price History read, Raw SQL, docs and Sample.

### Slice 4 — Assignments + Mapping + HotKeys

Merged on PR #18 and green. Includes canonical multi-group assignment, validated per-scale mapping/Copy, user group HotKey templates, system-row preservation and correct resend-state behavior.

### Slice 5 — Sales Query + Reports

Merged on PR #19. Main merge commit: `230f73938181a6eb8ebe06a44832e08a8c502e7e`.

Includes:

- existing incremental `Sales.ReadAfterAsync` Feed unchanged;
- newest-first filtered/paged `Sales.QueryAsync`;
- complete-filter Summary;
- distinct invoice count by `(DeviceNo, FID)`;
- Today / Saturday-week / Persian-month helpers;
- typed Daily / Scale / Item reports;
- Raw SQL, docs, Sample, SQL tests and net48 coverage.

### Slice 6 — Demo Data + Vendor Acceptance + RC

Implemented on PR #20 and now at final exact-head CI / RC review.

Completed:

- non-packable `SadrScales.Integration.DemoLab` helper;
- deterministic scenario generation with explicit Seed;
- synthetic Stores / Groups / Items / disabled Scales / Assignments / Mappings / HotKeys / Sales / Invoices;
- reserved Demo scale IDs and TEST-NET addresses;
- strict Demo database guard;
- marker initialization only on clearly non-production, schema-compatible, empty databases after exact-name confirmation;
- guarded generation and row-only reset while preserving schema/marker/default rows;
- executable WinForms Demo Data UI;
- unit and disposable SQL Server tests for determinism, guard refusal, generation and reset safety;
- package-only `SadrScales.Integration.VendorAcceptance` app with **no ProjectReference** to SDK source;
- external-developer flow: Validate → Catalog → Scale config → Status/Resend → Sales Feed/Query/Reports → Invoice Save/Commit → ACK → AlreadyRead recovery → logical delete;
- dedicated `vendor-acceptance` CI job against disposable SQL Server 2022;
- simplified English/Persian README and Getting Started paths;
- English/Persian Vendor-Ready Capabilities index.

## RC freeze rule

After PR #20 passes every required check on its exact final Head and is merged, the `1.1.0` contract/naming is frozen.

Before vendor outreach, only these changes are permitted:

- bug fixes;
- security fixes;
- compatibility fixes;
- documentation corrections that do not change the frozen contract.

No new feature should be added to `1.1.0` after RC freeze.

## Required final gates

- `build-test-pack`
- `sql-integration-test`
- WinForms Developer Sample build
- `net48-package-consumer`
- `vendor-acceptance`
- `validate-public-boundary`
- release bundle smoke validation

The vendor letter/outreach is blocked until all final checks are green and the RC is frozen.

## Important integration decisions

- SQL is the current third-party transport for Sadr Scales 5.2.1.
- `SADR_Scale.Status` is the supported coarse SQL status source.
- invoice lookup never auto-ACKs;
- destination Save/Commit happens before ACK;
- ACK sets `SADR_Total.LableStatus = 1` and is idempotent;
- `AlreadyRead` never blocks full invoice recovery;
- Sales Feed cursor belongs to the destination application;
- Sales Query is separate from Feed synchronization;
- public HotKey APIs preserve internal zero/negative system rows;
- DemoLab is not a production SDK surface;
- production Scale lifecycle and arbitrary Runtime commands are not exposed through 5.2.1 raw SQL.

## Sadr Scales 5.3 follow-up

A typed per-scale Command Mailbox remains planned for 5.3. Runtime owns validation, licensing, connection/busy state, model capability and protocol execution. Service/REST can later become another transport over the same Command Domain.

## Out of immediate scope

```text
5.3 Command Mailbox implementation
Service / REST transport
Realtime Runtime progress
Public Scale Emulator
Full Integration Lab
Firmware/File public operations
```

## Stable release identity

Historical stable release:

- tag `v1.0.0`
- source `a6bccc7c13a8afba29b6860869d2a942b1231803`
- release ID `372167195`

The historical tag is immutable.

## Security boundary

The public repository must not expose proprietary device protocols, raw packets/captures, private keys, customer production data, private firmware/vendor material or arbitrary Runtime/protocol execution.
