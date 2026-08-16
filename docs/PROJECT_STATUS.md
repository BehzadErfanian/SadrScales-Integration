# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — .NET Framework 4.8 compatibility verified; batch/sample work next  
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

### Latest compatibility branch validation

Branch: `m2/net48-compatibility`  
Validated commit: `e2b8a7169fcd9226034dba070ed35f7fcbef7216`.

SDK CI run `31970792734`: PASS for all three jobs:

- build/test/pack: PASS;
- SQL Server 2022 integration: PASS — 5/5;
- `.NET Framework 4.8` package consumer: PASS.

The net48 consumer gate:

- restored the generated local NuGet package;
- built with `TreatWarningsAsErrors=true`;
- build result: **0 warnings / 0 errors**;
- executed successfully under .NET Framework 4.8;
- loaded `SadrScales.Integration, Version=0.1.0.0`;
- loaded `Microsoft.Data.SqlClient, Version=7.0.0.0` from the package dependency graph.

Public Repository Guard run `31970792738`: PASS.

### Compatibility investigation history

- Cycle 1 failed before package evaluation because PowerShell `--source` parsing treated the nuget.org URL as a Windows path.
- Cycle 2 fixed restore using `NuGet.CI.config`; package restore/build/runtime all passed but the smoke harness had one nullable-flow warning.
- Cycle 3 replaced the custom null assertion with compiler-recognized flow and enabled warnings-as-errors; all jobs passed warning-free.

No SDK runtime incompatibility was found.

## Pre-v1.0 administrative gates

Still open:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

1. Merge the net48 compatibility PR after PR-level CI.
2. Verify exact `main` merge SHA.
3. Start `m2/batch-and-csharp-sample`.
4. Add an **atomic bounded item batch API** with deterministic all-or-nothing semantics per call.
5. Add an executable C# Quick Start that is read-only by default and never embeds credentials.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
