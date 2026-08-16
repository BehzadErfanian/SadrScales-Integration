# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — SQL-backed SDK hardening  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Completed

### M0
Public repository foundation, governance, bilingual entry documentation and Public Repository Guard.

### M1
SQL Contract v1 source audit/freeze, bilingual contract/Quick Starts, executable SQL samples, regression checklist and official 34-page Persian guide preparation/QA.

Official guide SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

### M2 SDK foundation

PR #3 `M2: Add first C# Integration SDK foundation` merged to `main` as:

`5fe058148a41385950e0800aff8f10e581668eeb`

Post-merge validation on that exact SHA:

- SDK CI run `31969619533`: PASS — restore/build/test/pack.
- Public Repository Guard run `31969619624`: PASS.
- Unit test baseline: 8/8.
- Build baseline: 0 warnings / 0 errors.
- Package smoke build: `.nupkg` and `.snupkg` created cleanly.

## Active hardening branch

`m2/sql-integration-tests`

Goal: verify the public SDK against a real disposable SQL Server using only synthetic Contract v1 schema/data.

Planned SQL-backed tests:

- Contract validator passes against the frozen schema.
- Item-group Upsert returns Inserted / Unchanged / Updated correctly.
- PLU Upsert leaves SQL rowversion unchanged when the semantic data is unchanged.
- Real PLU update changes rowversion and reports Updated.
- Sales reader tolerates identity gaps, preserves ascending cursor order and never mutates `SADR_Logs`.
- Schema mismatch maps to `SadrContractMismatchException`.

CI uses an ephemeral Microsoft SQL Server 2022 Linux container. Its SA password is a public synthetic CI-only value and has no relationship to company/customer credentials.

## Retry decision

Automatic write retry is intentionally **not** added yet. Microsoft.Data.SqlClient's built-in retry provider does not automatically retry a command executing inside an open transaction. More importantly, the SDK must not create duplicate/ambiguous writes after a connection failure. Retry will be added only after SQL integration coverage exists and read/open/write semantics are explicitly separated.

## Pre-v1.0 administrative gates

Still open and must not be forgotten:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

1. Get `m2/sql-integration-tests` green on both existing unit/package CI and the new SQL Server integration-test job.
2. Fix every real SQL mismatch on the branch.
3. Merge only after PR CI is green.
4. Then add bounded transient retry for connection/read-safe operations, followed by .NET Framework 4.8 consumer compatibility and bounded PLU batch API.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
