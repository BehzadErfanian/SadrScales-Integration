# Work Log

## 2026-08-16 — M0 / M1
- Public foundation/security/governance created.
- Contract v1 frozen against effective Sadr Scales 5.2.1 schema.
- Bilingual docs, SQL samples, regression checklist and official 34-page Persian guide prepared/QA'd.
- Official guide SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

## 2026-08-16 — M2 SDK / SQL / retry hardening
- Basic `netstandard2.0` SDK built with `Microsoft.Data.SqlClient 7.0.2`.
- Real SQL Server 2022 suite: 5/5 current SQL tests.
- Bounded/cancellable retry added only to safe connection/read boundaries.
- PR #3 merged as `5fe058148a41385950e0800aff8f10e581668eeb`.
- PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.
- PR #5 merged as `77d3c6330e0741a2c2f92eaec62fb8f50c781702`.
- Retry post-merge SDK CI `31970450480`: PASS; Public Guard `31970450492`: PASS.

## 2026-08-16 — M2 .NET Framework 4.8 compatibility

### Design
- Validate the generated NuGet package, not merely a project reference or theoretical framework table.
- Windows CI builds/runs a real `net48` Console consumer and loads the SqlClient dependency selected by the restored package graph.

### Cycle 1 — `aceb1d4cbae13df7511e88f264d4443dd5127e09`
- Existing SDK/SQL/Public Guard jobs: PASS.
- net48 package created, but restore failed before compatibility evaluation because multiline PowerShell `--source` parsing turned nuget.org into a local path (`NU1301`).
- Replaced CLI source flags with `NuGet.CI.config`.

### Cycle 2 — `c60c23a84a6a8eaf92e82b90ef292d684aa02f8a`
- All three SDK CI jobs: PASS.
- Package restore/build/runtime: PASS.
- Runtime loaded SDK `0.1.0.0` and `Microsoft.Data.SqlClient 7.0.0.0`.
- One `CS8602` warning remained in the smoke harness because custom Assert did not inform nullable flow analysis.

### Cycle 3 — `e2b8a7169fcd9226034dba070ed35f7fcbef7216`
- Replaced custom null assertion with explicit null flow and set `TreatWarningsAsErrors=true`.
- SDK CI run `31970792734`: **PASS for all three jobs**.
- build/test/pack: PASS.
- SQL Server 2022 integration: PASS — 5/5.
- net48 local-package restore: PASS.
- net48 build: **0 Warning(s), 0 Error(s)**.
- net48 runtime: PASS.
- Runtime output confirms:
  - `SadrScales.Integration, Version=0.1.0.0` loaded;
  - `Microsoft.Data.SqlClient, Version=7.0.0.0` loaded from the package dependency graph.
- Public Repository Guard run `31970792738`: PASS.
- No SDK/package/runtime net48 incompatibility remains in this gate.

### Next
- Open compatibility PR, require PR-level SDK CI + Public Guard, merge only when green, then verify `main` exact SHA.
