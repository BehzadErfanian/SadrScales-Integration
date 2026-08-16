# Roadmap

This roadmap is the canonical plan. Update it when scope or sequencing changes.

## M0 — Public repository foundation

**Goal:** establish a secure, self-documenting public repository.

- [x] Repository structure.
- [x] README in Persian and English.
- [x] Security boundary.
- [x] Project status / continuity / decisions / backlog.
- [x] SQL Contract v1 documentation baseline.
- [x] Sample-language structure.
- [x] Public-repository validation guard.
- [x] Public GitHub repository and CI verification.

## M1 — Contract v1 freeze and public documentation

**Goal:** ensure the public contract is accurate, stable and usable without private Sadr Scales source.

- [x] Re-validate Contract v1 against the effective Sadr Scales 5.2.1 migrated schema.
- [x] Freeze basic surface: `SADR_ItemClass`, `SADR_Item`, `SADR_Logs`.
- [x] Keep Registry/Mapping/structured sales advanced/controlled.
- [x] Complete Persian/English Quick Starts and contract docs.
- [x] Add executable synthetic SQL examples and expected results.
- [x] Add contract regression checklist.
- [x] Produce and visually QA the official Persian Integration & Database Guide PDF.
- [x] Record the approved PDF filename and SHA-256 in repository documentation.
- [ ] Upload the approved PDF/checksum as GitHub Release assets at the appropriate release point.
- [ ] Complete owner/admin GitHub host-security checklist.

**M1 engineering exit gate:** complete. Remaining host/release administration proceeds in parallel and must be closed before public `v1.0.0` distribution.

## M2 — C# Integration SDK v1

**Goal:** make the common integration path small, safe and source-available.

Planned functional areas:

- schema/contract validation;
- connection handling without embedded credentials;
- item-group upsert;
- item/PLU upsert and bounded batch import;
- incremental sales read with caller-owned cursor;
- bounded retry for transient SQL failures;
- idempotency guidance/helpers;
- advanced APIs separated from the basic contract.

Design gates:

- [ ] Freeze target framework(s) and SQL provider.
- [ ] Freeze public API names and async model.
- [ ] Define exception/result model.
- [ ] Define logging behavior without leaking secrets.
- [ ] Scaffold SDK-style library.
- [ ] Add unit and integration tests.
- [ ] Add CI build/test workflow.

## M3 — Reference samples

- [ ] C# Console Quick Start.
- [ ] C# end-to-end item + sales example.
- [x] Raw SQL examples for frozen basic Contract v1.
- [ ] Safe sample configuration template.
- [ ] Local/synthetic test database instructions.

## M4 — Multi-language examples

- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] Language-neutral SQL type/null mapping table.

## M5 — Packaging and GitHub Releases

- [ ] Build/test GitHub Actions workflow for SDK code.
- [x] Public-repository security guard on pushes/PRs.
- [ ] Package SDK.
- [ ] Generate XML docs/package metadata.
- [ ] SHA-256 manifest.
- [ ] Release procedure.
- [ ] `v1.0.0` GitHub Release with SDK, samples, guide and checksums.

## M6 — Sadr website developer experience

- [ ] Replace old simple developer guide with concise landing page.
- [ ] Link this GitHub repository as developer source of truth.
- [ ] Link/download official full Integration Guide PDF.
- [ ] Link latest GitHub Release / SDK.
- [ ] Keep main Sadr Scales application download on official Sadr website.
- [ ] Keep user guide at user-facing documentation path.

## Future — Integration Gateway / no-code connector

Not part of Sadr Scales 5.2.1 / SQL Contract v1.

Potential future work: REST/Webhook gateway, configurable no-code connector, remote/realtime integration surfaces, and a separately versioned future contract only when a real public interface exists.
