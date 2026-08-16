# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — C# Integration SDK v1 foundation  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Purpose

Create one public, polished GitHub repository that software vendors can receive as the single developer link for integrating POS, ERP and accounting systems with Sadr Scales.

The repository contains public source code, contract documentation, samples, compatibility information, release assets and enough project history to continue development without depending on chat history.

## M0 foundation — complete

- Public GitHub repository created.
- Public/private security boundary documented and enforced by CI guard.
- English and Persian README published.
- Project continuity/governance documents established.
- No direct device protocols, captures, private keys, customer data or private Sadr Scales runtime source published.
- Public software license is intentionally still pending explicit company approval.

## M1 Contract v1 + documentation — engineering complete

The basic SQL Contract v1 was re-validated against the effective Sadr Scales 5.2.1 schema after the application's own migration/check path and is now frozen.

Basic surface:

- `dbo.SADR_ItemClass` — SELECT / INSERT / UPDATE.
- `dbo.SADR_Item` — SELECT / INSERT / UPDATE; `PluNo` is the public item identity.
- `dbo.SADR_Logs` — SELECT only; destination-owned incremental cursor by ascending `ID`.

Completed artifacts:

- Persian and English frozen Contract v1 documents.
- Persian and English Quick Starts.
- Contract freeze record and regression checklist.
- Executable synthetic SQL validation/upsert/sales samples.
- Official Persian Integration & Database Guide PDF prepared and visually QA'd.
- Official PDF identity: SHA-256 `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

Parallel administrative items that do **not** block M2 source development:

- upload the approved PDF/checksum as a GitHub Release asset at the appropriate release point;
- complete the owner/admin GitHub host-security checklist in `GITHUB_SECURITY_ADMIN_CHECKLIST.md`.

## M2 exact next step

Build the first C# SDK foundation on top of the frozen basic contract:

1. freeze target framework and SQL provider choice;
2. document public API boundaries and exception/result behavior;
3. scaffold `SadrScales.Integration` as an SDK-style library;
4. implement contract validation first;
5. then item-group/item APIs and incremental sales reader;
6. add CI build/tests before merging implementation to `main`.

Advanced Registry/Mapping/structured-sales APIs remain outside the basic SDK surface unless explicitly introduced as separate advanced APIs later.

## Handoff rule

A future chat/session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md` and `SECURITY_BOUNDARY.md`. Chat history is not the project source of truth.
