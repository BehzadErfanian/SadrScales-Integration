# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — SQL-backed SDK hardening ready for PR  
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

PR #3 merged to `main` as `5fe058148a41385950e0800aff8f10e581668eeb`.

Post-merge validation:

- SDK CI `31969619533`: PASS — restore/build/test/pack.
- Public Repository Guard `31969619624`: PASS.
- Unit baseline: 8/8.
- Build: 0 warnings / 0 errors.

## SQL-backed hardening branch

Branch: `m2/sql-integration-tests`

Latest validated code commit: `19208d9ba4ac32a7174ed8d45c832a0086b7ee5e`.

### Real SQL Server CI result

SDK CI run `31969914826`: PASS.

- existing restore/build/unit-test/pack job: PASS;
- SQL Server 2022 service initialization: PASS;
- SQL integration project restore/build: PASS;
- SQL integration tests: **5/5 PASS**;
- SQL integration build: 0 warnings / 0 errors.

Public Repository Guard run `31969914926`: PASS.

Verified against disposable real SQL Server:

- Contract validator accepts the frozen synthetic Contract v1 schema;
- item-group Upsert reports Inserted / Unchanged / Updated correctly;
- PLU Upsert preserves SQL rowversion on semantic no-op and changes it on real update;
- sales reader handles intentional identity gaps in ascending order and does not mutate `SADR_Logs`;
- a real schema mismatch maps to `SadrContractMismatchException`.

The first SQL integration run (`31969825350`) was 4/5 because the mismatch **test harness** attempted to drop a UNIQUE table constraint using `DROP INDEX`. The test was corrected to `ALTER TABLE ... DROP CONSTRAINT`; no SDK runtime defect was found in that run.

## Retry boundary

Automatic transactional write retry remains intentionally deferred. Connection/read retry and write retry will be designed separately after this SQL-backed hardening PR. Retry must be bounded/cancellable and must not create ambiguous duplicate writes.

## Pre-v1.0 administrative gates

Still open:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

1. Open the SQL-backed hardening PR.
2. Require SDK CI (including SQL Server job) and Public Repository Guard on the PR.
3. Merge only when all jobs are green and verify `main` post-merge.
4. Next engineering branch: bounded retry for connection/read-safe operations, then .NET Framework 4.8 consumer compatibility.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
