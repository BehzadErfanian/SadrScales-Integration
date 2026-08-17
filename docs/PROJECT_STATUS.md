# Project Status — SadrScales-Integration

**Last updated:** 2026-08-17  
**Phase:** M2 — atomic batch API + C# Quick Start validated; PR next  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Completed engineering gates

- M0 public repository/security/governance foundation.
- M1 SQL Contract v1 source audit/freeze and official reference preparation.
- M2 basic C# SDK foundation.
- M2 real SQL Server 2022 integration-test hardening.
- M2 bounded connection/read retry hardening.
- M2 real .NET Framework 4.8 NuGet-package consumer compatibility.

### Latest merged baseline

PR #6 merged as `7af437c4394661b3c53321987c81477805049658`.
Post-merge SDK CI completed with build/test/pack, SQL Server 2022 integration and net48 package-consumer jobs all PASS. Public Repository Guard also PASS.

## Current branch

Branch: `m2/batch-and-csharp-sample`.
Validated implementation commit: `0e32d53852209f052bedc1daf789d57d7ea624cf`.

Current work adds:

- `SadrItemClient.UpsertBatchAsync(...)`;
- hard limit of 200 PLUs per atomic call;
- full pre-validation before SQL access;
- duplicate `PluNo` rejection;
- one SQL transaction per batch and all-or-nothing rollback;
- aggregate Inserted / Updated / Unchanged result;
- SQL-backed rollback and aggregate-result tests;
- executable C# Quick Start that validates the contract and reads sales only;
- CI build gate for the Quick Start.

### Exact branch validation

SDK CI run `31997207988`: PASS.

- build/test/pack: PASS;
- C# Quick Start restore/build: PASS;
- SQL Server 2022 integration: PASS;
- `.NET Framework 4.8` package consumer: PASS.

Public Repository Guard run `31997207943`: PASS.

The branch is ready for PR review/merge. Any documentation-only follow-up commit must also pass the Public Repository Guard before merge.

## Pre-v1.0 administrative gates

Still open:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

1. Open PR for `m2/batch-and-csharp-sample`.
2. Require PR-level SDK CI + Public Repository Guard.
3. Squash merge only when green and verify exact `main` SHA.
4. Continue M2 package/release hardening: Source Link, package validation/API compatibility and strong-name decision.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
