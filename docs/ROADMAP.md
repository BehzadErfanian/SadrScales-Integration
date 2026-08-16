# Roadmap

This roadmap is the canonical plan. Update it when scope or sequencing changes.

## M0 — Public repository foundation
- [x] Repository structure, bilingual README, security boundary, continuity docs and Public Repository Guard.

## M1 — Contract v1 freeze and public documentation
- [x] Re-validate effective 5.2.1 schema and freeze basic Contract v1.
- [x] Complete bilingual Contract/Quick Starts, executable SQL samples and regression checklist.
- [x] Produce/page-QA official Persian guide and record SHA-256.
- [ ] Upload approved PDF/checksum as GitHub Release assets before public v1.0 distribution.
- [ ] Complete owner/admin GitHub host-security checklist.

**M1 engineering exit gate:** complete.

## M2 — C# Integration SDK v1

### Foundation — complete
- [x] `netstandard2.0` + `Microsoft.Data.SqlClient 7.0.2`.
- [x] Basic async API/caller-owned connection and sales cursor boundaries.
- [x] Schema validator, semantic item-group/PLU upserts and read-only incremental sales batch.
- [x] Unit tests and restore/build/test/pack CI.
- [x] PR #3 merged to `main` as `5fe058148a41385950e0800aff8f10e581668eeb`.
- [x] Post-merge SDK CI and Public Repository Guard passed.

### SQL-backed hardening — branch green
- [x] Disposable SQL Server 2022 CI architecture with synthetic Contract v1 schema/data.
- [x] SQL integration-test project and service-container CI job.
- [x] Contract validation coverage.
- [x] Item-group Inserted / Unchanged / Updated coverage.
- [x] PLU semantic no-op/update + rowversion coverage.
- [x] Sales ID-gap, ascending cursor and read-only coverage.
- [x] Dedicated schema-mismatch exception coverage.
- [x] Branch SDK CI green: 5/5 SQL integration tests + existing 8/8 unit tests/package job.
- [x] Branch Public Repository Guard green.
- [ ] PR CI/review and merge to `main`.
- [ ] Post-merge `main` CI verification.

### M2 next hardening
- [ ] Bounded transient retry for connection/read-safe operations only.
- [ ] .NET Framework 4.8 consumer compatibility build/test.
- [ ] Bounded item batch API.
- [ ] Executable C# Console Quick Start.
- [ ] Source Link/package validation.
- [ ] Strong-name decision before v1.0.

Advanced Registry/Mapping/structured invoice helpers are not part of the basic SDK foundation.

## M3 — Reference samples
- [ ] C# Console Quick Start and end-to-end example.
- [x] Raw SQL examples for Contract v1.
- [ ] Safe sample configuration/local synthetic DB instructions.

## M4 — Multi-language examples
- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] Language-neutral SQL type/null mapping table.

## M5 — Packaging and GitHub Releases
- [x] SDK restore/build/test/pack workflow green on `main` foundation.
- [x] Public-repository security guard.
- [ ] NuGet/DLL packaging policy finalized.
- [ ] XML docs + Source Link/package metadata.
- [ ] SHA-256/release procedure.
- [ ] `v1.0.0` GitHub Release.

## M6 — Sadr website developer experience
- [ ] Replace old simple developer guide with concise landing page.
- [ ] Link GitHub repo as developer source of truth.
- [ ] Link official guide and latest SDK Release.

## Future — Integration Gateway / no-code connector
Not part of Sadr Scales 5.2.1 / SQL Contract v1. REST/Webhook/no-code work is separately versioned future scope.
