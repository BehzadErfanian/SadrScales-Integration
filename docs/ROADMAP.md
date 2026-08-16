# Roadmap

This roadmap is the canonical plan. Update it when scope or sequencing changes.

## M0 — Public repository foundation

**Goal:** establish a secure, self-documenting public repository.

- [x] Repository structure and bilingual README.
- [x] Security boundary and public-repository CI guard.
- [x] Status/roadmap/backlog/decisions/work log.
- [x] Public GitHub repository and CI verification.

## M1 — Contract v1 freeze and public documentation

**Goal:** make the public contract accurate and usable without private runtime source.

- [x] Re-validate effective 5.2.1 schema.
- [x] Freeze `SADR_ItemClass`, `SADR_Item`, read-only `SADR_Logs`.
- [x] Keep Registry/Mapping/structured sales advanced/controlled.
- [x] Complete bilingual Quick Starts/contract docs.
- [x] Add executable synthetic SQL samples and regression checklist.
- [x] Produce/page-QA official Persian guide PDF and record SHA-256.
- [ ] Upload approved PDF/checksum as GitHub Release assets before public v1.0 distribution.
- [ ] Complete owner/admin GitHub host-security checklist.

**M1 engineering exit gate:** complete.

## M2 — C# Integration SDK v1

**Goal:** make the common integration path small, safe and source-available.

### Foundation — in progress

- [x] Choose initial target: `netstandard2.0`.
- [x] Choose SQL provider: `Microsoft.Data.SqlClient 7.0.2`.
- [x] Define async-first basic API boundary.
- [x] Define caller-owned connection/cursor behavior.
- [x] Scaffold SDK-style library.
- [x] Implement Contract v1 schema validator.
- [x] Implement semantic item-group upsert.
- [x] Implement semantic PLU/item upsert.
- [x] Implement read-only incremental sales batch.
- [x] Add first unit tests and SDK CI workflow.
- [ ] Pass SDK CI build/test/pack on branch and PR.

### M2 follow-up after foundation is green

- [ ] Add bounded transient SQL retry with cancellation and explicit limits.
- [ ] Add safe SQL integration-test environment/schema tests.
- [ ] Add .NET Framework 4.8 consumer compatibility build/test.
- [ ] Add bounded item batch API.
- [ ] Add executable C# Console Quick Start.
- [ ] Add package metadata/Source Link and package validation.
- [ ] Decide strong naming before v1.0 packaging.

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

- [ ] SDK build/test GitHub Actions fully green on `main`.
- [x] Public-repository security guard.
- [ ] NuGet/DLL packaging policy finalized.
- [ ] XML docs + Source Link/package metadata.
- [ ] SHA-256 manifest and release procedure.
- [ ] `v1.0.0` GitHub Release.

## M6 — Sadr website developer experience

- [ ] Replace old simple developer guide with concise landing page.
- [ ] Link GitHub repo as developer source of truth.
- [ ] Link official guide and latest SDK Release.
- [ ] Keep main application download on Sadr site and user guide on user-facing path.

## Future — Integration Gateway / no-code connector

Not part of Sadr Scales 5.2.1 / SQL Contract v1. Any REST/Webhook/no-code work is separately versioned future scope.
