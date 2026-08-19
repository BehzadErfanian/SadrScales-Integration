# Project Status — SadrScales-Integration

**Last updated:** 2026-08-19  
**Phase:** Vendor-Ready 5.2.1 implementation  
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

The next software-vendor outreach happens only after a **Vendor-Ready Stable Baseline** is complete:

```text
Complete 5.2.1 SQL Integration
        ↓
SDK + Raw SQL + Docs + executable Sample
        ↓
End-to-End Vendor Acceptance Test
        ↓
Stable RC / Freeze
        ↓
One serious vendor outreach / letter
```

Sadr Scales 5.3 work must not delay this baseline.

## Frozen Vendor-Ready 5.2.1 scope

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

## Implementation progress

### Slice 1 — Structured Invoice + explicit ACK

**Status:** implemented on PR #15; CI green at the core + SQL + Windows/net48 gates before merge review.

Implemented SDK surface:

```csharp
client.Invoices.GetByBarcodeAsync(totalBarcode)
client.Invoices.GetAsync(scaleId, fid)
client.Invoices.AcknowledgeAsync(totalBarcode)
client.Invoices.AcknowledgeAsync(scaleId, fid)
SadrInvoiceClient.BuildTotalBarcode(scaleId, fid)
```

Contract behavior:

```text
FoundUnread -> full invoice, no source mutation
AlreadyRead -> full invoice + informational warning
NotFound    -> no invoice

Destination Save/Commit
        ↓
Explicit ACK
        ↓
SADR_Total.LableStatus = 1
```

ACK is idempotent: `Acknowledged / AlreadyAcknowledged / NotFound`.

Evidence included in the same slice:

- real disposable SQL Server integration tests;
- proof that lookup does not auto-ACK;
- proof that ACK changes `LableStatus` only after explicit call;
- proof that repeated ACK is harmless;
- proof that `AlreadyRead` still returns complete detail rows;
- ScaleID + FID aggregate-barcode lookup;
- Raw SQL recipe for non-C# consumers;
- Persian and English structured-invoice reference docs;
- `.NET Framework 4.8` package-consumer coverage for the new public Invoice API;
- executable WinForms Developer Sample with header/detail grids;
- Sample ACK write guard + confirmation;
- Windows CI build of the WinForms Sample.

### Executable Developer Sample strategy

The WinForms app at:

`samples/csharp/SadrScales.Integration.SampleApp`

is the single growing executable reference. Future slices add their areas/tabs to this app instead of creating unrelated demo executables.

Current Sample capability: Structured Invoice lookup + explicit guarded ACK.

Planned Vendor-Ready areas remain:

```text
Connection
Scales
Stores
Groups
Items
Assignments
Hot Keys
Sales
Invoices
Reports
Demo Data
```

Demo Data must be seedable/reproducible and protected against accidental production-DB population.

## Remaining release gate

Before the software-vendor letter/outreach:

1. finish remaining SQL/SDK capabilities in small reviewed slices;
2. keep Raw SQL examples for non-C# consumers;
3. complete Scale status, Groups, Mapping, HotKey and Resend tests;
4. implement Sales Query/Summary and Reports;
5. complete the WinForms Sample areas;
6. add seeded/reproducible Demo Data with Production-DB guard;
7. simplify the public developer path and complete reference docs;
8. keep `.NET Framework 4.8`, SQL Server CI and public-boundary checks green;
9. run an external-developer-style end-to-end acceptance test;
10. freeze naming/contract at RC except bug/security/compatibility fixes;
11. publish the Vendor-Ready stable release;
12. only then send the software-vendor outreach letter.

## Release-version gate before PR #15 merge

Historical `v1.0.0` is immutable. The current project file still identifies build output as `1.0.0`, while PR #15 adds new public API surface.

Before merging Vendor-Ready implementation into `main`, the next additive package/release version must be frozen so a changed package is never produced under the historical `1.0.0` identity.

## Sadr Scales 5.3 follow-up

`CMD-001` — per-scale typed Integration Command Mailbox is planned for Sadr Scales 5.3, not the current Vendor-Ready baseline.

High-level semantics:

```text
one stable mailbox/row per Scale
one active command per Scale
RequestId
Typed CommandCode / flags
Idle / Pending / Running / Succeeded / Failed / Rejected
ResultCode / ResultMessage
```

Runtime performs validation, licensing, connection/busy checks, model capability and actual private protocol work. Service/REST may later become another transport over the same Command Domain.

## Stable release identity

- Git tag: `v1.0.0`
- Stable source commit: `a6bccc7c13a8afba29b6860869d2a942b1231803`
- Release ID: `372167195`
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
