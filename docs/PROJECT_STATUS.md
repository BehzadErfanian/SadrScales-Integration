# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M1 — Contract v1 frozen; reference-release preparation remaining  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Purpose

Create one public, polished GitHub repository that software vendors can receive as the single developer link for integrating POS, ERP and accounting systems with Sadr Scales.

The repository must contain source code, contract documentation, samples, compatibility information, release assets and enough project history to continue development without depending on chat history.

## M0 foundation — complete

- Public GitHub repository: `BehzadErfanian/SadrScales-Integration`.
- Public/private security boundary documented and enforced by CI guard.
- English and Persian README published.
- Project continuity/governance documents published.
- No device protocol captures, private keys, customer databases/credentials or private runtime source published.
- No public software `LICENSE` is published yet; license selection requires explicit company approval.
- Binary Word/PDF guide files remain out of `main`; approved binaries are GitHub Release assets.

## M1 Contract v1 freeze — complete

The basic Contract v1 surface was re-validated against the effective Sadr Scales 5.2.1 schema after the application's own schema migration/check path.

Frozen basic surface:

- `dbo.SADR_ItemClass` — SELECT / INSERT / UPDATE.
- `dbo.SADR_Item` — SELECT / INSERT / UPDATE; `PluNo` is the public item identity and logical removal is preferred.
- `dbo.SADR_Logs` — SELECT only; destination-owned cursor by ascending `ID`.

Additional freeze rules:

- `SADR_Item.ID` / `IDitem` are legacy and not public identities.
- SQL `TimeStamp` / `rowversion` is never caller-written.
- `TaxNo` / `SendFlag` are not required caller inputs in the basic v1 path.
- sales import is destination-idempotent; `(DeviceNo, FID, SubID)` is the preferred duplicate key.
- sales IDs may contain gaps.
- Registry/Mapping/structured-sales/runtime-state areas remain advanced/controlled.
- breaking public changes require a new contract version.

Public artifacts added for the freeze:

- Persian and English Contract v1 specifications.
- Persian and English Quick Starts.
- `docs/CONTRACT_V1_FREEZE.md`.
- `docs/CONTRACT_V1_REGRESSION_CHECKLIST.md`.
- executable synthetic SQL validation/upsert/sales-read samples.

## What is intentionally not implemented yet

- C# SDK/library code and final API surface.
- Framework target decision.
- NuGet package.
- C#/Python/Node.js/Java/PHP executable samples.
- GitHub Release `v1.0.0`.
- Public website developer-page update.
- REST/Webhook/no-code connector.

## Exact next step — finish M1

1. Produce the official PDF from the approved Persian Integration & Database Guide.
2. Generate SHA-256 for the official guide and prepare both as GitHub Release/reference assets.
3. Review/enable repository security features available on GitHub (secret scanning/dependency/security settings where available).
4. Add the remaining Contract v1 troubleshooting matrix if useful during final documentation QA.
5. Once those M1 release/documentation gates are clean, begin **M2 — C# Integration SDK v1** and freeze its target framework/API design before implementation.

## Handoff rule

A future chat/session must begin by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md` and `SECURITY_BOUNDARY.md`. Chat history is not the project source of truth.
