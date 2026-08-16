# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16  
**Phase:** M2 — C# Integration SDK v1 foundation implementation  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Purpose

Create one public, polished GitHub repository that software vendors can receive as the single developer link for integrating POS, ERP and accounting systems with Sadr Scales.

The repository contains public source code, contract documentation, samples, compatibility information, release assets and enough project history to continue development without depending on chat history.

## Completed foundation

### M0

- Public repository and bilingual README.
- Explicit public/private security boundary.
- Source-controlled public repository guard running on push and pull request.
- Roadmap, backlog, decisions, work log, compatibility and release policy.

### M1

- SQL Contract v1 re-validated against effective Sadr Scales 5.2.1 migrated schema and frozen.
- Basic public surface: `SADR_ItemClass`, `SADR_Item`, read-only `SADR_Logs`.
- Persian/English Contract v1 and Quick Starts.
- Executable synthetic SQL validation/upsert/sales samples.
- Contract freeze record and regression checklist.
- Official 34-page Persian Integration & Database Guide prepared and visually QA'd.
- Official PDF SHA-256 recorded as `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

Pre-v1.0 administrative gates remain explicit: company-approved license, GitHub host-security checklist, and official PDF/checksum Release upload.

## M2 foundation being implemented

Branch: `m2/sdk-foundation`

Design choices:

- library target: `netstandard2.0`;
- SQL provider: `Microsoft.Data.SqlClient 7.0.2`;
- caller owns the connection string and deployment security policy;
- SDK never logs or rewrites the raw connection string;
- async-first public operations with cancellation support;
- short-lived pooled SQL connections;
- explicit transactions for writes;
- destination-owned sales cursor; SDK never acknowledges basic `SADR_Logs` by writing to Sadr data.

Initial API surface under CI validation:

- `SadrScalesClient.ValidateAsync()`;
- `ItemGroups.UpsertAsync(...)`;
- `Items.UpsertAsync(...)`;
- `Sales.ReadAfterAsync(...)`;
- `SadrWriteResult` with Inserted/Updated/Unchanged semantics;
- `SadrSalesBatch` returning a cursor candidate only after a successful read.

Advanced Registry/Mapping/structured-sales/device protocol APIs remain outside this basic SDK foundation.

## Exact next step

1. Commit the M2 SDK foundation to `m2/sdk-foundation`.
2. Require both Public Repository Guard and SDK CI to build/test/pack successfully.
3. Fix every CI issue on the branch before opening/merging the M2 PR.
4. After the foundation is green, add bounded transient-SQL retry, safe integration-test infrastructure and .NET Framework 4.8 consumer compatibility validation.
5. Add executable C# Quick Start only after the foundation API compiles cleanly and is reviewed.

## Handoff rule

A future chat/session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md` and `SECURITY_BOUNDARY.md`. Chat history is not the project source of truth.
