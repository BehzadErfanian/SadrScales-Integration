# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — bounded connection/read retry ready for PR  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Completed milestones

### M0 — public foundation
Public repository, governance/continuity docs, bilingual entry documentation, security boundary and Public Repository Guard.

### M1 — Contract v1 + reference
SQL Contract v1 frozen against the effective 5.2.1 migrated schema; bilingual Contract/Quick Starts; executable SQL samples; regression checklist; official 34-page Persian guide prepared and visually QA'd.

Official guide SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

### M2 SDK foundation
PR #3 merged as `5fe058148a41385950e0800aff8f10e581668eeb`; post-merge restore/build/unit-test/pack and Public Guard passed.

### M2 SQL-backed hardening
PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.

Post-merge exact-SHA validation:

- SDK CI run `31970073088`: PASS;
- SQL Server 2022 integration: PASS — 5/5;
- Public Repository Guard run `31970073055`: PASS.

## Bounded retry branch

Branch: `m2/read-retry-policy`  
Validated code commit: `4042efd6c50857622767224ea31e2c175241cd82`.

Implemented boundary:

- connection opening may retry before a command/transaction starts;
- `ValidateAsync` and `Sales.ReadAfterAsync` may replay the complete read-only operation on a fresh connection;
- `ItemGroups.UpsertAsync` and `Items.UpsertAsync` do **not** replay the transaction-scoped write command after execution begins;
- retry is bounded, cancellation-aware and preserves the final native exception.

Public defaults:

- `TransientRetryCount = 2`;
- `TransientRetryBaseDelayMilliseconds = 250`;
- exponential delay capped at 5000 ms.

### Branch validation

SDK CI run `31970279834`: PASS.

- restore/build/test/pack: PASS;
- build: 0 warnings / 0 errors;
- unit tests: **17/17 PASS**;
- NuGet + symbol package creation: PASS;
- SQL Server 2022 integration: **5/5 PASS**.

Public Repository Guard run `31970279841`: PASS.

## Pre-v1.0 administrative gates

Still open:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

1. Open bounded-retry PR.
2. Require SDK CI (including real SQL) and Public Repository Guard on the PR.
3. Merge only when all PR jobs are green and verify `main` post-merge.
4. Next engineering gate: .NET Framework 4.8 consumer compatibility validation.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
