# Decision Log

Accepted decisions are recorded here so future work does not need to recover them from chat history.

## D-001 — Dedicated public integration repository
**Date:** 2026-08-16  
**Status:** Accepted
Integration tooling and public developer documentation live in `SadrScales-Integration`, separate from private runtime repositories.

## D-002 — GitHub is the developer source of truth
**Date:** 2026-08-16  
**Status:** Accepted
Software vendors receive one public GitHub link. Source, docs, samples, compatibility and SDK releases are maintained here; the Sadr website developer page points to them.

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
Accepted scope/architecture decisions, completed work, remaining work and next steps are recorded in repository documentation.

## D-007 — Full guide + short developer path
**Date:** 2026-08-16  
**Status:** Accepted
The detailed guide is the full reference; README/Quick Start/SDK provide the short path.

## D-008 — GitHub Releases for distributable artifacts
**Date:** 2026-08-16  
**Status:** Accepted
Compiled SDK/package, samples, official guide and checksums are GitHub Release assets rather than binary clutter in `main`.

## D-009 — Public license requires explicit approval
**Date:** 2026-08-16  
**Status:** Proposed / confirm before v1.0
A permissive license is intended, but no `LICENSE` is published until explicitly approved by the company.

## D-010 — Future no-code/REST work is separate from 5.2.1
**Date:** 2026-08-16  
**Status:** Accepted
No-code connector and REST/Webhook Gateway are future, separately versioned work.

## D-011 — Binary reference guides are release assets
**Date:** 2026-08-16  
**Status:** Accepted
Editable Word and official PDF binaries are not committed to `main`; approved PDF/checksum are distributed through GitHub Releases.

## D-012 — Basic SQL Contract v1 is frozen against effective 5.2.1 schema
**Date:** 2026-08-16  
**Status:** Accepted
The baseline is the schema after Sadr Scales 5.2.1 completes its schema migration/check. Basic surface: `SADR_ItemClass`, `SADR_Item`, read-only `SADR_Logs`. Breaking public changes require a new contract version.

## D-013 — M1 engineering does not block M2 on host/release administration
**Date:** 2026-08-16  
**Status:** Accepted
Contract/reference engineering may proceed to SDK development while license, Release-asset upload and GitHub host settings remain explicit pre-v1.0 gates.

## D-014 — C# SDK v1 targets .NET Standard 2.0
**Date:** 2026-08-16  
**Status:** Accepted for pre-1.0 validation
The reusable SDK initially targets `netstandard2.0`; real .NET Framework 4.8 consumer compatibility validation is required before v1.0.

## D-015 — Microsoft.Data.SqlClient is the SDK SQL provider
**Date:** 2026-08-16  
**Status:** Accepted for pre-1.0 validation
The foundation uses `Microsoft.Data.SqlClient 7.0.2`. Caller owns connection/security configuration; SDK does not weaken encryption/trust settings or log the raw connection string.

## D-016 — Basic SDK does not own destination sales state
**Date:** 2026-08-16  
**Status:** Accepted
`Sales.ReadAfterAsync` is read-only and returns rows plus a cursor candidate. Destination commits its own data first and only then persists the new cursor.

## D-017 — Real SQL integration tests use a disposable synthetic SQL Server
**Date:** 2026-08-16  
**Status:** Accepted
M2 CI uses a disposable SQL Server 2022 Linux container with a synthetic Contract v1 schema and synthetic rows. No production/customer database, capture, device protocol or real credential is copied into the public repository. The CI SA password is an intentionally public throwaway value scoped only to the ephemeral runner.

## D-018 — Do not automatically retry transactional writes before ambiguity is tested
**Date:** 2026-08-16  
**Status:** Accepted
Connection/read retry and transactional write retry are treated separately. No automatic retry is added around `ItemGroups.UpsertAsync` or `Items.UpsertAsync` until integration tests and explicit commit-ambiguity rules exist. Retry must be bounded, cancellable and must never turn a lost response into an accidental duplicate write.
