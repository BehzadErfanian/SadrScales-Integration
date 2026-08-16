# Work Log

## 2026-08-16 — M0 / M1
- Public foundation/security/governance created.
- Contract v1 frozen against effective Sadr Scales 5.2.1 schema.
- Bilingual docs, SQL samples, regression checklist and official 34-page Persian guide prepared/QA'd.
- Official guide SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.

## 2026-08-16 — M2 SDK foundation / SQL hardening
- Basic `netstandard2.0` SDK built with `Microsoft.Data.SqlClient 7.0.2`.
- Contract validator, semantic group/item upserts and read-only sales batches implemented.
- Real SQL Server 2022 suite added; 5/5 current SQL tests.
- PR #3 merged as `5fe058148a41385950e0800aff8f10e581668eeb`.
- PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.

## 2026-08-16 — M2 bounded retry
- Explicit bounded/cancellable retry added only to safe connection/read boundaries.
- Transaction-scoped write commands remain non-replayed.
- Branch: 17/17 unit tests + 5/5 SQL tests + clean package + Public Guard PASS.
- PR #5 merged as `77d3c6330e0741a2c2f92eaec62fb8f50c781702`.
- Post-merge SDK CI `31970450480`: PASS.
- Post-merge Public Repository Guard `31970450492`: PASS.

## 2026-08-16 — M2 .NET Framework 4.8 compatibility

### Research/design
- Microsoft .NET Standard compatibility documentation lists .NET Framework 4.8 as supporting .NET Standard 2.0 and recommends 4.7.2+ over older 4.6.1-era consumption.
- Microsoft.Data.SqlClient 7.0.2 publishes both .NET Standard 2.0 and direct .NET Framework 4.6.2+ assets.
- GitHub Windows Server 2022 runner image includes .NET Framework 4.8.
- Decided to validate the **generated package**, not just the source project.

### Implemented on `m2/net48-compatibility`
- Added SDK-style `net48` Console consumer under `tests/`.
- Consumer references only `SadrScales.Integration` package version `0.1.0-alpha.1`.
- New Windows CI job packs the SDK, restores the net48 app from the local nupkg plus public dependency feed, builds it, and runs the EXE under .NET Framework 4.8.
- Runtime smoke checks SDK public options/client/models and loads the `Microsoft.Data.SqlClient` assembly resolved through the package dependency graph.
- No SQL connection is performed in this gate; SQL behavior remains covered by the separate real SQL Server suite.

### Next
- Run all three SDK CI jobs plus Public Guard; fix any real package/binding/runtime incompatibility before PR.
