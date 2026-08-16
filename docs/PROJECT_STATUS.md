# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — bounded connection/read retry hardening  
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
- build/test/pack job: PASS;
- SQL Server 2022 integration job: PASS — 5/5 SQL-backed tests;
- Public Repository Guard run `31970073055`: PASS.

The real-SQL suite verifies Contract validation, Inserted/Unchanged/Updated behavior, rowversion no-op/update behavior, sales ID gaps/read-only semantics and schema-mismatch exception mapping.

## Active branch

`m2/read-retry-policy`

Goal: add bounded transient retry without making transactional writes ambiguous.

### Retry boundary

- Connection opening may retry before any SQL command/transaction begins.
- `ValidateAsync` and `Sales.ReadAfterAsync` may retry the complete read-only operation on a fresh connection.
- `ItemGroups.UpsertAsync` and `Items.UpsertAsync` do **not** automatically replay the write command after transaction execution begins.
- Retry is bounded, cancellation-aware and rethrows the final native exception when exhausted.

Initial public defaults under validation:

- `TransientRetryCount = 2`;
- `TransientRetryBaseDelayMilliseconds = 250`;
- exponential delay capped at 5000 ms.

## Pre-v1.0 administrative gates

Still open:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

1. Build/test the retry branch with the existing unit and SQL Server integration suites.
2. Add/fix retry-specific tests until CI is clean.
3. Open PR only after branch Build/Test/Pack, SQL integration and Public Guard are green.
4. After merge, begin .NET Framework 4.8 consumer compatibility validation.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
