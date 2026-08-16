# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — .NET Framework 4.8 package-consumer compatibility  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Completed engineering gates

- M0 public repository/security/governance foundation.
- M1 SQL Contract v1 source audit/freeze and official reference preparation.
- M2 basic C# SDK foundation.
- M2 real SQL Server 2022 integration-test hardening.
- M2 bounded connection/read retry hardening.

### Latest `main` validation

Retry PR #5 merged to `main` as:

`77d3c6330e0741a2c2f92eaec62fb8f50c781702`

Post-merge:

- SDK CI `31970450480`: PASS — build/test/pack + SQL Server integration;
- Public Repository Guard `31970450492`: PASS.

Current unit baseline: 17/17.  
Current SQL integration baseline: 5/5.

## Active compatibility branch

`m2/net48-compatibility`

Goal: prove the **generated NuGet package** is consumable by a real .NET Framework 4.8 application, not merely assert that `netstandard2.0` is theoretically compatible.

The compatibility consumer:

1. runs on GitHub `windows-2022`, which includes .NET Framework 4.8;
2. creates the SDK `.nupkg`;
3. restores a `net48` console application from that local package + public dependency feed;
4. builds the net48 consumer;
5. runs it under .NET Framework 4.8;
6. exercises public SDK options/models/client construction;
7. loads the `Microsoft.Data.SqlClient` dependency resolved for the net48 package graph.

No SQL connection is opened by this smoke test; SQL runtime behavior remains covered separately by the disposable SQL Server 2022 integration suite.

## Pre-v1.0 administrative gates

Still open:

- company-approved public software license;
- GitHub owner/admin security-settings checklist;
- official Integration Guide PDF + checksum upload as GitHub Release assets.

## Exact next step

1. Get all three SDK CI jobs green on `m2/net48-compatibility`.
2. Fix any package/binding/runtime compatibility issue found on Windows.
3. Require Public Repository Guard.
4. Merge only after PR CI is green and verify the exact `main` merge SHA.
5. Next: bounded item batch API + executable C# Quick Start.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`.
