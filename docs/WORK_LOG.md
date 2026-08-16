# Work Log

## 2026-08-16 — M0 / M1
- Public foundation/security/governance created.
- Contract v1 frozen against effective Sadr Scales 5.2.1 schema.
- Bilingual docs, SQL samples, regression checklist and official 34-page Persian guide prepared/QA'd.
- Official guide SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

## 2026-08-16 — M2 SDK / SQL / retry hardening
- Basic `netstandard2.0` SDK built with `Microsoft.Data.SqlClient 7.0.2`.
- Contract validator, semantic group/item upserts and read-only sales batches implemented.
- Real SQL Server 2022 suite: 5/5 current SQL tests.
- Bounded/cancellable retry added only to safe connection/read boundaries; transaction-scoped writes remain non-replayed.
- PR #3 merged as `5fe058148a41385950e0800aff8f10e581668eeb`.
- PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.
- PR #5 merged as `77d3c6330e0741a2c2f92eaec62fb8f50c781702`.
- Retry post-merge SDK CI `31970450480`: PASS; Public Guard `31970450492`: PASS.

## 2026-08-16 — M2 .NET Framework 4.8 compatibility

### Research/design
- Microsoft .NET Standard compatibility documentation lists .NET Framework 4.8 as supporting .NET Standard 2.0 and recommends 4.7.2+ over older 4.6.1-era consumption.
- Microsoft.Data.SqlClient 7.0.2 publishes both .NET Standard 2.0 and direct .NET Framework 4.6.2+ assets.
- GitHub Windows Server 2022 runner image includes .NET Framework 4.8.
- Decided to validate the generated NuGet package, not just a project reference.

### Implemented on `m2/net48-compatibility`
- Added SDK-style `net48` Console consumer under `tests/`.
- Consumer references only `SadrScales.Integration` package version `0.1.0-alpha.1`.
- Windows CI packs SDK, restores net48 app from local nupkg + public dependency feed, builds it and runs the EXE under .NET Framework 4.8.
- Runtime smoke checks public SDK options/client/models and loads the `Microsoft.Data.SqlClient` dependency resolved through the package graph.

### Compatibility CI cycle 1 — `aceb1d4cbae13df7511e88f264d4443dd5127e09`
- Public Guard / existing build-test-pack / SQL Server integration: PASS.
- net48 job packed SDK but restore never reached real dependency resolution because multiline PowerShell `--source` parsing turned the nuget.org URL into a local path (`NU1301`).
- Replaced CLI source arguments with dedicated `NuGet.CI.config`.

### Compatibility CI cycle 2 — `c60c23a84a6a8eaf92e82b90ef292d684aa02f8a`
- SDK CI `31970684539`: PASS for all three jobs.
- Package restore from local nupkg: PASS.
- net48 build: PASS.
- net48 runtime smoke: PASS.
- Runtime loaded `SadrScales.Integration, Version=0.1.0.0` and `Microsoft.Data.SqlClient, Version=7.0.0.0`.
- Existing SQL Server integration: PASS.
- Public Repository Guard `31970684536`: PASS.
- One nullable-flow compiler warning (`CS8602`) remained in the smoke harness because a custom Assert does not inform C# nullable flow analysis; runtime behavior was successful.
- Replaced the custom null assertion with explicit `if (x == null) throw` flow and set the net48 consumer to `TreatWarningsAsErrors=true` so the final compatibility gate cannot be green with compiler warnings.

### Next
- Run compatibility CI cycle 3; require net48 restore/build/runtime plus existing SDK/SQL jobs and Public Guard to pass with zero consumer warnings before opening the PR.
