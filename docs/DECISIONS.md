# Decision Log

Accepted decisions are recorded here so future work does not need to recover them from chat history.

## D-001 — Dedicated public integration repository
**Date:** 2026-08-16  
**Status:** Accepted

Integration tooling and public developer documentation live in `SadrScales-Integration`, separate from private runtime repositories.

## D-002 — GitHub is the developer source of truth
**Date:** 2026-08-16  
**Status:** Accepted

Software vendors receive one public GitHub link. Source, docs, samples, compatibility and SDK releases are maintained there; the Sadr website developer page points to them.

## D-003 — SQL Contract v1 is the 5.2.1 public contract
**Date:** 2026-08-16  
**Status:** Accepted

Basic public surface is centered on `SADR_ItemClass`, `SADR_Item` and read-only `SADR_Logs`. REST/Webhook is not a 5.2.1 capability.

## D-004 — Device protocols remain private
**Date:** 2026-08-16  
**Status:** Accepted

Direct PLUS/LSG/Aclas/device protocols, packet captures and reverse-engineering/vendor material are never published here.

## D-005 — One destination application per Sadr Scales installation
**Date:** 2026-08-16  
**Status:** Accepted

Structured-sales processing status is designed around the installation's destination software, not a shared multi-consumer message bus.

## D-006 — Documentation is part of every change
**Date:** 2026-08-16  
**Status:** Accepted

Accepted scope/architecture decisions, completed work, remaining work and next steps are recorded in repository documentation. Chat memory is not project state.

## D-007 — Full guide + short developer path
**Date:** 2026-08-16  
**Status:** Accepted

The detailed guide is the full reference; README/Quick Start/SDK provide the short path.

## D-008 — GitHub Releases for distributable artifacts
**Date:** 2026-08-16  
**Status:** Accepted

Compiled SDK/package, sample bundles, official guide and checksums are GitHub Release assets rather than binary clutter in `main`.

## D-009 — Public license requires explicit approval
**Date:** 2026-08-16  
**Status:** Proposed / confirm before v1.0

A permissive license is intended, but no `LICENSE` is published until explicitly approved by the company.

## D-010 — Future no-code/REST work is separate from 5.2.1
**Date:** 2026-08-16  
**Status:** Accepted

No-code connector and REST/Webhook Gateway are future, separately versioned work and do not expand SQL Contract v1.

## D-011 — Binary reference guides are release assets
**Date:** 2026-08-16  
**Status:** Accepted

Editable Word and official PDF binaries are not committed to `main`; the approved PDF/checksum are distributed through GitHub Releases.

## D-012 — Basic SQL Contract v1 is frozen against effective 5.2.1 schema
**Date:** 2026-08-16  
**Status:** Accepted

The baseline is the schema after Sadr Scales 5.2.1 completes its own schema creation/migration checks. Frozen basic surface:

- `SADR_ItemClass`: SELECT / INSERT / UPDATE;
- `SADR_Item`: SELECT / INSERT / UPDATE, `PluNo` public identity;
- `SADR_Logs`: SELECT only, destination-owned ascending `ID` cursor and `(DeviceNo, FID, SubID)` preferred duplicate key.

Breaking public changes require a new contract version.

## D-013 — M1 engineering does not block M2 on host/release administration
**Date:** 2026-08-16  
**Status:** Accepted

Once Contract v1 and the official reference PDF are validated and immutably identified by SHA-256, SDK source development may proceed. GitHub host-security toggles, binary Release upload and final license approval remain explicit pre-v1.0 administrative gates and must not be silently forgotten or treated as completed.
