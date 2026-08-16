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
- Public Repository Guard: PASS.
- Existing build/test/pack: PASS.
- Existing SQL Server 2022 integration: PASS.
- net48 job created the SDK package successfully, but failed **before package compatibility was evaluated**: the multiline PowerShell `dotnet restore --source ... --source https://...` command caused NuGet to interpret the nuget.org URL as a relative/local Windows path and return NU1301.
- No net48 compile/runtime/package-graph incompatibility was observed in this run because restore never reached dependency resolution.
- Replaced CLI source arguments with a dedicated `NuGet.CI.config` containing the local package source and nuget.org; restore now uses `--configfile` to avoid Windows quoting/path interpretation ambiguity.

### Next
- Re-run all SDK CI jobs after the source-configuration fix.
- If restore reaches dependency resolution, address any real net48 package/binding/runtime issue revealed by that run.
