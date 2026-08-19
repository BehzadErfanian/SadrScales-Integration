# Project Status — SadrScales-Integration

**Last updated:** 2026-08-19  
**Phase:** Phase 2 — Contract & Domain Design under owner review  
**Current stable release:** `v1.0.0`  
**Supported published Sadr Scales baseline:** `5.2.1`  
**Current published integration contract:** `SQL Contract v1`

## Canonical references

Read in this order:

1. `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md`
2. `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`
3. `.github/maintainers/INTEGRATION_CONTRACT_DOMAIN_DESIGN_FA.md`
4. this file

The Master Plan remains the source of truth for product direction. Phase 1 is complete and merged. Phase 2 now defines the transport-independent domain and exact SQL/runtime boundaries before implementation begins.

## Phase 2 decisions already confirmed by owner

- SQL is the current third-party integration transport for Sadr Scales 5.2.1.
- `SADR_Scale.Status` is the supported SQL source for coarse Online/Offline status because Sadr Scales persists these transitions.
- Structured Invoice lookup is supported by TotalBarcode and by ScaleID + FID.
- Invoice lookup never auto-ACKs.
- Explicit ACK occurs only after destination persistence/commit succeeds.
- Invoice-level ACK sets `SADR_Total.LableStatus = 1`.
- Re-reading an ACKed invoice still returns the full invoice and reports `AlreadyRead`; this is a warning, not a block.
- Repeated ACK is idempotent.

## Phase 2 boundaries now proposed for final owner review

- Store identity uses `StoreCode`.
- `SADR_ScaleItemClass` is the canonical multi-group source.
- Scale group replacement is a controlled SQL transaction and resets item-send state.
- Scale item mapping is a controlled SQL transaction with duplicate/layout validation and send-state reset.
- Group HotKey template and per-scale item mapping are separate concepts.
- HotKey writes remain SQL data operations, but replace/delete semantics must reset affected send state when row deletion would otherwise be invisible.
- Raw Add/Update/Delete of `SADR_Scale` is not a supported public lifecycle API even though 5.2.1 discovers new DB rows periodically; internal lifecycle also performs validation, license checks, registry synchronization and connection/cleanup work.
- Direction proposed for vNext: a Managed SQL Command Channel lets third parties remain SQL-only while Sadr Scales itself executes scale lifecycle and device commands.
- Public device commands remain typed; raw protocol passthrough is forbidden.
- firmware/file/label operations remain outside the default public surface pending separate security/API review.

## Current exact next step

Owner review of the remaining unchecked Phase 2 gates in `.github/maintainers/INTEGRATION_CONTRACT_DOMAIN_DESIGN_FA.md`.

After acceptance:

1. record accepted decisions in `docs/DECISIONS.md`;
2. mark Phase 2 complete;
3. merge the Phase 2 PR after all required CI gates pass;
4. begin implementation planning in small, testable slices;
5. do not add large SDK clients or change Sadr Scales runtime before the implementation plan is accepted.

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

The public repository and future public Integration surface must not expose:

- proprietary device protocols or raw packet formats;
- captures/PCAPs or reverse-engineering material;
- private firmware/vendor data;
- private keys or secrets;
- customer production data;
- arbitrary SQL execution;
- arbitrary Runtime/protocol command execution.

## Handoff rule

A future session starts from the canonical references listed at the top of this file. Chat history is not the project source of truth.
