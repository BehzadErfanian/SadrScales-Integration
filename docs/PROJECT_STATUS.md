# Project Status — SadrScales-Integration

**Last updated:** 2026-08-19  
**Phase:** Phase 2 — Contract scope frozen for Vendor-Ready 5.2.1 implementation planning  
**Current stable release:** `v1.0.0`  
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

The owner does not want external software companies to be asked repeatedly to review and update their integrations. Therefore the current objective is:

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

## Owner-confirmed Phase 2 decisions

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
- The earlier Service-only runtime-command direction is superseded.
- A per-scale typed Command Mailbox is planned for **Sadr Scales 5.3**, not for the immediate 5.2.1 Vendor-Ready release.
- Service/REST can later become another transport over the same Command Domain.

These decisions are recorded in `docs/DECISIONS.md` through D-030.

## Vendor-Ready 5.2.1 SQL scope

The implementation target now covers:

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

1. complete the final SQL Contract for the 5.2.1 capabilities above;
2. implement all corresponding SDK APIs;
3. keep Raw SQL examples for non-C# consumers;
4. add real SQL integration tests for Structured Invoice + ACK;
5. test Scale status, Group/Mapping/HotKey and Resend semantics;
6. implement Sales Query/Summary and Reports coverage;
7. provide a runnable WinForms Developer Sample;
8. provide seeded/reproducible Demo Data with Production-DB guard;
9. keep a short Getting Started path plus a complete reference guide;
10. keep `.NET Framework 4.8` consumer validation and SQL Server CI green;
11. run an external-developer-style end-to-end acceptance test;
12. freeze naming/contract after RC except for bug/security/compatibility fixes.

## Sadr Scales 5.3 follow-up

A per-scale Command Mailbox is now a planned 5.3 capability.

High-level semantics:

```text
one stable mailbox/row per Scale
one active command per Scale
RequestId
Typed CommandCode / flags
Idle / Pending / Running / Succeeded / Failed / Rejected
ResultCode / ResultMessage
```

The Runtime performs validation, licensing, connection/busy checks, model capability and actual protocol work. Device protocol details remain private.

Examples planned for 5.3 include immediate SendItems with options such as ClearExisting, RetrieveItems, HotKey commands, RetrieveSales and supported setting operations.

Exact schema and command code values are not part of the current 5.2.1 Integration release and will be frozen during 5.3 design.

## Exact next step

1. finish Phase 2 PR #14 and merge it after required CI passes;
2. convert the Vendor-Ready capability matrix into small implementation slices;
3. implement current 5.2.1 SQL/SDK capabilities first;
4. complete Sample + Demo Data;
5. run Vendor Acceptance Test;
6. freeze and publish the Vendor-Ready release;
7. only then prepare/send the software-vendor outreach letter.

## Stable release identity

- Git tag: `v1.0.0`
- Stable source commit: `a6bccc7c13a8afba29b6860869d2a942b1231803`
- Release ID: `372167195`
- Protected Release run: `32112295891` — PASS
- License: MIT
- Providers/copyright identity: **Tozin Sadr and Behzad Erfanian**

The stable tag is immutable and must never be moved or reused.

## Stable v1.0.0 validation evidence

Required checks on the stable source:

- `build-test-pack`: PASS
- `sql-integration-test`: PASS
- `net48-package-consumer`: PASS
- `validate-public-boundary`: PASS

The published `v1.0.0` Basic SQL Contract remains valid and unchanged while the next Vendor-Ready contract is developed additively.

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
