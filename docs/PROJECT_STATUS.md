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
- A per-scale typed Command Mailbox is planned for **Sadr Scales 5.3**, not for the immediate 5.2.1 Vendor-Ready release.
- Service/REST can later become another transport over the same Command Domain.

These decisions are recorded in `docs/DECISIONS.md` through D-030.

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
Reports
```

## Completed implementation slices

### Slice 1 — Structured Invoice + ACK

Completed:

- `client.Invoices.GetByBarcodeAsync(totalBarcode)`;
- `client.Invoices.GetAsync(scaleId, fid)`;
- explicit idempotent ACK;
- `FoundUnread / AlreadyRead / NotFound`;
- full invoice remains available after ACK;
- Raw SQL recipe;
- Persian and English documentation;
- WinForms Developer Sample Invoices tab;
- disposable SQL Server tests;
- .NET Framework 4.8 package-consumer coverage.

### Slice 2 — Scales + Status + Resend

Completed on the active Vendor-Ready branch and pending final green CI/merge:

- `client.Scales.GetAllAsync()`;
- `client.Scales.GetAsync(scaleId)`;
- `client.Scales.GetStatusAsync(scaleId)`;
- public `Online / Offline / Unknown` status mapping;
- static scale metadata for third-party software;
- `RequestItemResendAsync(scaleId)` without exposing `LastSendItem`;
- `RequestHotKeyResendAsync(scaleId)` without exposing `LastSendKey`;
- `Requested / NotFound / UnsupportedModel` results;
- PLUS HotKey resend rejects unsupported automatic behavior instead of reporting false success;
- Raw SQL recipe with writes disabled by default;
- Persian and English documentation;
- WinForms Developer Sample Scales tab with guarded resend writes;
- disposable SQL Server tests;
- .NET Framework 4.8 package-consumer coverage.

`Requested` always means the resend state was recorded for a later eligible AutoSend cycle; it is not a physical-device completion result.

The next additive package version remains frozen as `1.1.0`. Historical `v1.0.0` remains immutable.

## Next implementation slices

### Slice 3 — Stores + Catalog completion

Target:

- Store read/upsert;
- Item Group read/upsert completion;
- Item/PLU read APIs in addition to the existing write APIs;
- supported soft-delete semantics;
- Price History read;
- Raw SQL examples and disposable SQL tests;
- Stores/Groups/Items pages in the same WinForms Developer Sample.

### Slice 4 — Scale Assignments + Mapping + HotKeys

Target:

- canonical multi-group scale assignment;
- Scale Item Mapping;
- group HotKey templates;
- validation/replace semantics matching Sadr Scales 5.2.1;
- resend-state behavior kept internal to SDK implementation;
- Sample + SQL tests + docs.

### Slice 5 — Sales Query + Reports

Target:

- filtered/paged sales query;
- summary totals;
- daily, scale and item reports;
- Sample + SQL tests + docs.

### Slice 6 — Demo Data + Vendor Acceptance + RC

Target:

- seeded/reproducible Demo Data;
- production-database guard;
- complete executable Sample flow;
- external-developer-style end-to-end acceptance test;
- documentation cleanup;
- `1.1.0` RC freeze.

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
