# Project Status — SadrScales-Integration

**Last updated:** 2026-08-19  
**Phase:** Phase 2 — Contract & Domain Design under final owner review  
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

## Phase 2 owner-confirmed decisions

- SQL is the current third-party integration transport for Sadr Scales 5.2.1.
- `SADR_Scale.Status` is the supported SQL source for coarse Online/Offline status.
- Structured Invoice lookup is supported by TotalBarcode and by ScaleID + FID.
- Invoice lookup never auto-ACKs.
- Explicit ACK happens only after the destination persistence/commit succeeds.
- Invoice-level ACK sets `SADR_Total.LableStatus = 1` and repeated ACK is idempotent.
- Re-reading an ACKed invoice still returns the complete invoice and reports `AlreadyRead`; this warning does not block recovery/re-import.
- Item AutoSend resend is requested through `LastSendItem = 0`.
- HotKey AutoSend resend is requested through `LastSendKey = 0` where the current model/runtime supports automatic HotKey transfer.
- Resetting `LastSendItem` / `LastSendKey` is not an immediate device command; it marks the next eligible AutoSend cycle to resend.
- A successful Resend request means the DB trigger state was reset; it does not claim that the physical scale has already received the data.
- Runtime-only capabilities are planned for a typed Sadr Integration Service; the previously proposed SQL Command Queue direction is removed.

These confirmed decisions are recorded in `docs/DECISIONS.md` as D-024 through D-028.

## Current SQL vNext direction

The SQL surface being designed covers:

```text
Connection/schema validation
Store read/upsert
Item Group read/upsert
Item/PLU read/upsert/soft-delete
Price history read
Static Scale read
Scale Online/Offline status read
Scale group assignment
Scale item mapping
HotKey templates
Request Item Resend
Request HotKey Resend where supported
Sales feed/query/summary
Structured Invoice lookup
Invoice ACK
Reports
```

### AutoSend resend semantics

```text
RequestItemResend(scaleId)
→ SADR_Scale.LastSendItem = 0

RequestHotKeyResend(scaleId)
→ SADR_Scale.LastSendKey = 0
```

These are documented SQL operations and will also have simple SDK methods. The current Runtime processes them on a later eligible AutoSend cycle; the scale must be enabled, connected and configured for automatic sending. HotKey AutoSend remains model/capability dependent.

The SDK result for these methods reports whether the resend request was recorded successfully. Physical transfer completion is a separate concern that requires Runtime/Service visibility.

## Future Service boundary

The following capabilities are not forced into SQL:

```text
Safe Add/Update/Delete Scale lifecycle
Immediate Send/Get Items
Immediate Send/Get HotKeys
Get Sales directly from the device
Send/Get Specification
Set Date/Time
Salesman/Text/Print/Barcode/Paper operations where supported
Richer Runtime status/progress/activity/error information
```

They will be provided later through a typed Sadr Integration Service that keeps protocol details private and reuses Sadr Scales Runtime validation, licensing, registry, connection and model-capability logic.

The exact Service transport (local API / future REST, etc.) is intentionally deferred to implementation design.

## Remaining Phase 2 review gates

- Store/Scale relation;
- multi-group semantics;
- Mapping write semantics;
- HotKey write semantics;
- Scale lifecycle service boundary details;
- final device-command family list;
- firmware/file/label public exclusion pending separate review.

After final acceptance:

1. mark Phase 2 complete;
2. ensure every accepted decision is in `docs/DECISIONS.md`;
3. merge PR #14 only after all required CI gates pass;
4. begin implementation planning in small, testable slices;
5. do not modify Sadr Scales runtime or create large SDK clients before the implementation plan is accepted.

## Stable release identity

- Git tag: `v1.0.0`
- Stable source commit: `a6bccc7c13a8afba29b6860869d2a942b1231803`
- Release ID: `372167195`
- Protected Release run: `32112295891` — PASS
- Protected Release artifact: `SadrScales-Integration-v1.0.0-1`
- Artifact ID: `9315377547`
- License: MIT
- Providers/copyright identity: **Tozin Sadr and Behzad Erfanian**

The stable tag is immutable and must never be moved or reused.

## Stable v1.0.0 validation evidence

Required checks on the stable source:

- `build-test-pack`: PASS
- `sql-integration-test`: PASS
- `net48-package-consumer`: PASS
- `validate-public-boundary`: PASS

The published `v1.0.0` Basic SQL Contract remains valid and unchanged while the next-generation Integration Platform is designed additively.

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
