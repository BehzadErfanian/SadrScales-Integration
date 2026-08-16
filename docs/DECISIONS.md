# Decision Log

Accepted decisions are recorded here so future work does not need to recover them from chat history.

## D-001 — Dedicated public integration repository

**Date:** 2026-08-16  
**Status:** Accepted

Integration tooling and public developer documentation live in a separate repository named `SadrScales-Integration`, not inside the private Sadr Scales runtime repository.

## D-002 — GitHub is the developer source of truth

**Date:** 2026-08-16  
**Status:** Accepted

Software vendors receive one public GitHub link. Source, docs, samples, compatibility and Integration SDK releases are maintained there. The Sadr website developer page is a landing page that points to the repository and official guide/release.

## D-003 — SQL Contract v1 is the 5.2.1 public contract

**Date:** 2026-08-16  
**Status:** Accepted

The basic public SQL Contract v1 is centered on `SADR_ItemClass`, `SADR_Item` and read-only `SADR_Logs`. REST/Webhook is not advertised as a 5.2.1 capability.

## D-004 — Device protocols remain private

**Date:** 2026-08-16  
**Status:** Accepted

Direct PLUS/LSG/Aclas/device protocol source, packet captures and reverse-engineering/vendor material are never published in this repository. Integration goes through Sadr Scales.

## D-005 — One destination application per Sadr Scales installation

**Date:** 2026-08-16  
**Status:** Accepted

The documented structured-sales status fields are designed around the installation's destination sales/accounting software. The repository does not design a shared `LableStatus`/`ItemStatus` field as a multi-consumer message bus. A materially separate consumer architecture should use its own installation/integration state.

## D-006 — Documentation is part of every change

**Date:** 2026-08-16  
**Status:** Accepted

Every accepted scope/architecture decision, completed task, remaining task and next step must be recorded in repository documentation in the same change. Chat memory is not project state.

## D-007 — Full guide + short developer path

**Date:** 2026-08-16  
**Status:** Accepted

The detailed Integration & Database Guide remains the full technical reference. README/Quick Start/SDK provide the short path; developers are not forced to read the full guide before first use.

## D-008 — GitHub Releases for distributable Integration artifacts

**Date:** 2026-08-16  
**Status:** Accepted

Compiled SDK/package, sample bundle, official guide and checksums are published through GitHub Releases rather than committed as versioned binary clutter on the main branch.

## D-009 — Public license requires explicit approval

**Date:** 2026-08-16  
**Status:** Proposed / confirm before v1.0

The project intends to use a permissive license so software vendors can use and adapt the public integration source. No `LICENSE` is published during M0/M1. The exact license (MIT or another approved permissive license) requires explicit company approval before the first public SDK release.

## D-010 — Future no-code/REST work is separate from 5.2.1

**Date:** 2026-08-16  
**Status:** Accepted

A configurable no-code connector and REST/Webhook Gateway are future work and must not expand or destabilize the 5.2.1 SQL Contract v1 scope.

## D-011 — Binary reference guides are release assets

**Date:** 2026-08-16  
**Status:** Accepted

The editable Word guide is not committed to the public main branch. After M1 validation, the official PDF and SHA-256 file are published through GitHub Releases. Markdown contract/quick-start documentation remains in Git for reviewability and history.

## D-012 — Basic SQL Contract v1 is frozen against the effective 5.2.1 schema

**Date:** 2026-08-16  
**Status:** Accepted

The Contract v1 baseline is the database schema after Sadr Scales 5.2.1 completes its own schema creation/migration checks. Legacy installer SQL or an unmigrated customer database does not override the effective runtime schema.

The frozen basic surface is:

- `SADR_ItemClass`: SELECT / INSERT / UPDATE;
- `SADR_Item`: SELECT / INSERT / UPDATE, with `PluNo` as the public identity and logical removal preferred;
- `SADR_Logs`: SELECT only, with destination-owned ascending `ID` cursor and `(DeviceNo, FID, SubID)` as the preferred destination duplicate key.

Registry, Mapping, structured-sales and runtime-state objects remain advanced/controlled. Backward-compatible clarifications/examples may remain Contract v1; a breaking public change requires a new contract version.
