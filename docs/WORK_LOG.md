# Work Log

## 2026-08-16 — M0 / M1
- Public foundation/security/governance created.
- Contract v1 frozen against effective Sadr Scales 5.2.1 schema.
- Bilingual docs, SQL samples, regression checklist and official 34-page Persian guide prepared/QA'd.
- Official guide SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

## 2026-08-16 — M2 SDK / SQL / retry hardening
- Basic `netstandard2.0` SDK built with `Microsoft.Data.SqlClient 7.0.2`.
- Real SQL Server 2022 suite established.
- Bounded/cancellable retry added only to safe connection/read boundaries.
- PR #3 merged as `5fe058148a41385950e0800aff8f10e581668eeb`.
- PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.
- PR #5 merged as `77d3c6330e0741a2c2f92eaec62fb8f50c781702`.

## 2026-08-16 — M2 .NET Framework 4.8 compatibility
- Generated NuGet package restored, built and executed by a real `net48` Console consumer on Windows Server 2022.
- Final net48 build: 0 warnings / 0 errors.
- Runtime loaded `SadrScales.Integration, Version=0.1.0.0` and `Microsoft.Data.SqlClient, Version=7.0.0.0`.
- PR #6 merged as `7af437c4394661b3c53321987c81477805049658`.
- Post-merge SDK CI: build/test/pack PASS; SQL Server integration PASS; net48 package consumer PASS.
- Post-merge Public Repository Guard: PASS.

## 2026-08-17 — M2 atomic batch + C# Quick Start
- Started `m2/batch-and-csharp-sample` from merge SHA `7af437c4394661b3c53321987c81477805049658`.
- Added bounded `UpsertBatchAsync` design: maximum 200 PLUs, unique `PluNo` per call, complete validation before SQL access, one transaction, full rollback on any failure.
- Added aggregate batch result counts.
- Added unit tests for duplicate, oversized and empty batches.
- Added real SQL Server tests for aggregate results and rollback-after-partial-progress.
- Added read-only-by-default C# Quick Start using `SADR_SCALES_CONNECTION_STRING`; no embedded credentials and no payload dump.
- Added CI build of the Quick Start.
- Branch remains pending until exact-head SDK CI and Public Repository Guard both pass.
