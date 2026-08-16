# Roadmap

This roadmap is the canonical plan. Update it when scope or sequencing changes.

## M0 — Public repository foundation

- [x] Repository structure and bilingual README.
- [x] Security boundary and public-repository CI guard.
- [x] Status/roadmap/backlog/decisions/work log.
- [x] Public GitHub repository and CI verification.

## M1 — Contract v1 freeze and public documentation

- [x] Re-validate effective 5.2.1 schema.
- [x] Freeze `SADR_ItemClass`, `SADR_Item`, read-only `SADR_Logs`.
- [x] Complete bilingual Contract/Quick Starts.
- [x] Add executable synthetic SQL samples and regression checklist.
- [x] Produce/page-QA official Persian guide and record SHA-256.
- [ ] Upload approved PDF/checksum as GitHub Release assets before public v1.0 distribution.
- [ ] Complete owner/admin GitHub host-security checklist.

**M1 engineering exit gate:** complete.

## M2 — C# Integration SDK v1

### Foundation — branch validation complete

- [x] Initial target: `netstandard2.0`.
- [x] SQL provider: `Microsoft.Data.SqlClient 7.0.2`.
- [x] Async-first basic API/caller-owned connection and cursor boundaries.
- [x] SDK-style library scaffold.
- [x] Contract v1 schema validator.
- [x] Semantic item-group upsert.
- [x] Semantic PLU/item upsert.
- [x] Read-only incremental sales batch.
- [x] Unit tests and SDK CI workflow.
- [x] Branch SDK CI: restore/build/test/pack green; 8/8 tests; 0 build warnings/errors.
- [x] Dedicated NuGet package README; clean pack output.
- [ ] PR CI and review.
- [ ] Merge foundation to `main` and verify post-merge CI.

### M2 follow-up after foundation merge

- [ ] Bounded transient SQL retry with cancellation and explicit limits.
- [ ] Safe SQL integration-test environment/schema tests.
- [ ] .NET Framework 4.8 consumer compatibility build/test.
- [ ] Bounded item batch API.
- [ ] Executable C# Console Quick Start.
- [ ] Package metadata/Source Link and package validation.
- [ ] Strong-name decision before v1.0 packaging.

Advanced Registry/Mapping/structured invoice helpers are not part of the basic foundation.

## M3 — Reference samples

- [ ] C# Console Quick Start.
- [ ] C# end-to-end item + sales example.
- [x] Raw SQL examples for frozen Contract v1.
- [ ] Safe sample configuration template.
- [ ] Local/synthetic test DB instructions.

## M4 — Multi-language examples

- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] Language-neutral SQL type/null mapping table.

## M5 — Packaging and GitHub Releases

- [ ] SDK build/test GitHub Actions green on `main`.
- [x] Public-repository security guard.
- [ ] NuGet/DLL packaging policy finalized.
- [ ] XML docs + Source Link/package metadata.
- [ ] SHA-256 manifest/release procedure.
- [ ] `v1.0.0` GitHub Release.

## M6 — Sadr website developer experience

- [ ] Replace old simple developer guide with concise landing page.
- [ ] Link GitHub repo as developer source of truth.
- [ ] Link official guide and latest SDK Release.
- [ ] Keep application download on Sadr site and user guide on user-facing path.

## Future — Integration Gateway / no-code connector

Not part of Sadr Scales 5.2.1 / SQL Contract v1. REST/Webhook/no-code work is separately versioned future scope.
