# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — C# Integration SDK v1 foundation ready for PR  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Completed foundation

### M0

Public repository, bilingual README, explicit security boundary, continuity documentation and source-controlled public repository guard.

### M1

SQL Contract v1 source audit/freeze, bilingual contract/Quick Starts, executable synthetic SQL samples, regression checklist and official 34-page Persian guide preparation/QA.

Official guide SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

Pre-v1.0 administrative gates remain explicit: company-approved license, GitHub host-security checklist and official PDF/checksum Release upload.

## M2 foundation implemented on `m2/sdk-foundation`

Design:

- `netstandard2.0` reusable library target;
- `Microsoft.Data.SqlClient 7.0.2`;
- caller-owned SQL connection/security configuration;
- async-first operations with cancellation support;
- short-lived pooled SQL connections;
- explicit write transactions;
- destination-owned sales cursor; no hidden Sadr-side consumer state.

Basic API:

- `SadrScalesClient.ValidateAsync()`;
- `ItemGroups.UpsertAsync(...)`;
- `Items.UpsertAsync(...)`;
- `Sales.ReadAfterAsync(...)`;
- Inserted/Updated/Unchanged write result;
- read-only sales batch with `LastReadId` cursor candidate.

## Branch validation

Latest branch commit before PR: `666ba48d381db73e7397f8be92ada02b7a3c153b`.

GitHub Actions results:

- Public Repository Guard: PASS.
- SDK restore: PASS.
- SDK build: PASS — 0 warnings / 0 errors.
- Unit tests: PASS — 8/8.
- NuGet package smoke test: PASS — `.nupkg` and `.snupkg` generated.
- Initial NuGet missing-readme quality message was fixed with a dedicated package README; second pack is clean.

## Exact next step

1. Open M2 foundation PR.
2. Require Public Repository Guard and SDK CI to pass on the PR.
3. Review PR diff and merge only when both gates are green.
4. Verify post-merge `main` CI.
5. Continue M2 hardening: bounded transient retry, safe SQL integration tests, .NET Framework 4.8 consumer compatibility, bounded item batch API and executable C# Quick Start.

Advanced Registry/Mapping/structured-sales/device protocol APIs remain outside the basic SDK foundation.

## Handoff rule

A future chat/session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
