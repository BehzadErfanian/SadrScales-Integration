# Roadmap

This roadmap is the canonical plan. Update it when scope or sequencing changes.

## M0 — Public repository foundation

**Goal:** establish a secure, self-documenting public repository.

- [x] Repository structure.
- [x] README in Persian and English.
- [x] Security boundary.
- [x] Project status / continuity / decisions / backlog.
- [x] SQL Contract v1 documentation baseline.
- [x] Full Persian reference guide policy defined.
- [x] Sample-language structure.
- [x] Public-repository validation guard.
- [x] Create GitHub repository.
- [ ] Push reviewed M0 foundation and verify the public repository guard.
- [ ] Review GitHub rendering and links.
- [ ] Enable available GitHub security features.

## M1 — Contract v1 freeze and public documentation

**Goal:** ensure the public contract is accurate, stable and usable without access to private Sadr Scales source.

- [ ] Re-validate Contract v1 fields and rules against Sadr Scales 5.2.1.
- [ ] Freeze basic public surface: `SADR_ItemClass`, `SADR_Item`, `SADR_Logs`.
- [ ] Keep Registry/Mapping/structured sales clearly marked advanced/controlled unless explicitly promoted.
- [ ] Produce official PDF from the latest Persian Integration & Database Guide.
- [ ] Create concise Quick Start documents.
- [ ] Complete English contract documentation.
- [ ] Add synthetic example data and expected results.
- [ ] Add contract regression checklist.

## M2 — C# Integration SDK v1

**Goal:** make the common integration path small and safe.

Planned functional areas:

- schema/contract validation;
- connection handling without embedded credentials;
- item-group upsert;
- item/PLU upsert and batch import;
- incremental sales read with caller-owned cursor;
- structured invoice lookup/ack helpers where explicitly enabled;
- bounded retry for transient SQL failures;
- idempotency helpers/examples;
- clear advanced APIs separated from the basic contract.

Open design work:

- [ ] Select target framework(s).
- [ ] Freeze public API names and sync/async model.
- [ ] Decide packaging strategy (DLL + NuGet).
- [ ] Define exception/result model.
- [ ] Define logging abstraction without leaking secrets.
- [ ] Add unit and integration tests.

## M3 — Reference samples

**Goal:** let a software vendor run a working sample quickly.

- [ ] C# Console Quick Start.
- [ ] C# end-to-end item + sales example.
- [ ] Raw SQL examples.
- [ ] Safe sample configuration template.
- [ ] Local/synthetic test database instructions.

## M4 — Multi-language examples

These examples implement SQL Contract v1 directly; they do not reimplement device protocols.

- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] Language-neutral mapping table for SQL types and null handling.

## M5 — CI, packaging and GitHub Releases

- [ ] Build/test GitHub Actions workflow.
- [ ] Public-repository security guard on every PR.
- [ ] Package SDK.
- [ ] Generate XML docs/package metadata.
- [ ] SHA-256 manifest.
- [ ] Signed/tagged release procedure where appropriate.
- [ ] `v1.0.0` GitHub Release with SDK, samples, guide and checksums.

## M6 — Sadr website developer experience

- [ ] Replace the old simple developer guide on `sadrgroup.ir/app/developers/` with a concise landing page.
- [ ] Link to this GitHub repository as the developer source of truth.
- [ ] Link/download the official full Integration Guide PDF.
- [ ] Link the latest GitHub Release / SDK.
- [ ] Keep the main Sadr Scales application download on the official Sadr website.
- [ ] Keep the user guide at the user-facing documentation path.

## Future — Integration Gateway / no-code connector

Not part of Sadr Scales 5.2.1 or SQL Contract v1.

Potential future work:

- REST API / Webhook gateway;
- configurable connector utility for integrations requiring little or no custom code;
- remote/realtime integration surfaces;
- Contract v2 only when a real new public interface exists.
