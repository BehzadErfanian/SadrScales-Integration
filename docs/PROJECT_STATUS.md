# Project Status — SadrScales-Integration

**Last updated:** 2026-08-17  
**Phase:** M2 — batch/Quick Start merged; package hardening next  
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
- M2 bounded atomic PLU batch API.
- M2 executable read-only C# Quick Start.

## Latest merged baseline

PR #7 was squash-merged to `main` as:

`39b0beac63c4e30974283a0306cedb330c433f6e`

Post-merge validation on that exact SHA:

- SDK CI run `31997736325`: PASS;
  - build/test/pack: PASS;
  - C# Quick Start restore/build: PASS;
  - SQL Server 2022 integration: PASS;
  - `.NET Framework 4.8` package consumer: PASS.
- Public Repository Guard run `31997736339`: PASS.

The merged batch API keeps the accepted Contract v1 semantics:

- maximum 200 PLUs per atomic call;
- complete validation before SQL access;
- duplicate `PluNo` rejection;
- one transaction per call and full rollback on failure;
- aggregate Inserted / Updated / Unchanged result;
- no automatic replay of transaction-scoped writes;
- larger imports are explicitly paged by the destination application.

The C# Quick Start remains read-only by default, reads the connection string from `SADR_SCALES_CONNECTION_STRING`, embeds no credentials and does not dump sale payloads.

## Pre-v1.0 administrative gates

Still open:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

Continue M2 package/release hardening during the next public-engineering session:

1. Source Link / repository metadata;
2. package validation and API-compatibility policy;
3. strong-name decision;
4. release packaging/checksum automation.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
