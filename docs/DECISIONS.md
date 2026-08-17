# Decision Log

Accepted decisions are recorded here so future work does not need to recover them from chat history.

## D-001 — Dedicated public integration repository
**Date:** 2026-08-16  
**Status:** Accepted
Integration tooling and developer docs live in `SadrScales-Integration`, separate from private runtime repositories.

## D-002 — GitHub is the developer source of truth
**Date:** 2026-08-16  
**Status:** Accepted
Software vendors receive one public GitHub link for source, docs, samples, compatibility and SDK releases.

## D-003 — SQL Contract v1 is the 5.2.1 public contract
**Date:** 2026-08-16  
**Status:** Accepted
Basic public surface is `SADR_ItemClass`, `SADR_Item` and read-only `SADR_Logs`.

## D-004 — Device protocols remain private
**Date:** 2026-08-16  
**Status:** Accepted
Direct device protocols, captures and vendor/reverse-engineering material are never published here.

## D-005 — One destination application per Sadr Scales installation
**Date:** 2026-08-16  
**Status:** Accepted
Structured-sales processing state is not designed as a shared multi-consumer bus.

## D-006 — Documentation is part of every change
**Date:** 2026-08-16  
**Status:** Accepted
Decisions, completed work, remaining work and next steps are recorded in repository docs.

## D-007 — Full guide + short developer path
**Date:** 2026-08-16  
**Status:** Accepted
Full guide is the deep reference; README/Quick Start/SDK provide the short path.

## D-008 — GitHub Releases for distributable artifacts
**Date:** 2026-08-16  
**Status:** Accepted
Compiled SDK/package, samples, guide and checksums are Release assets, not binary clutter in `main`.

## D-009 — Public license requires explicit approval
**Date:** 2026-08-16  
**Status:** Proposed / confirm before v1.0
No public software `LICENSE` is added without company approval.

## D-010 — Future no-code/REST is separate
**Date:** 2026-08-16  
**Status:** Accepted
No-code connector and REST/Webhook Gateway are separately versioned future work.

## D-011 — Binary reference guides are release assets
**Date:** 2026-08-16  
**Status:** Accepted
Word/PDF guide binaries remain out of `main`; approved PDF/checksum are Release assets.

## D-012 — Contract v1 baseline is effective 5.2.1 migrated schema
**Date:** 2026-08-16  
**Status:** Accepted
Breaking public changes require a new contract version.

## D-013 — M1 administration does not block M2 source engineering
**Date:** 2026-08-16  
**Status:** Accepted
License, Release upload and host-security toggles remain explicit pre-v1.0 gates.

## D-014 — C# SDK v1 targets .NET Standard 2.0
**Date:** 2026-08-16  
**Status:** Accepted
The shared SDK assembly targets `netstandard2.0` to serve modern .NET and .NET Framework consumers.

## D-015 — Microsoft.Data.SqlClient is the SQL provider
**Date:** 2026-08-16  
**Status:** Accepted
Pre-1.0 uses `Microsoft.Data.SqlClient 7.0.2`; caller owns connection/security configuration.

## D-016 — SDK does not own destination sales state
**Date:** 2026-08-16  
**Status:** Accepted
Sales read is read-only and the destination owns durable cursor/state.

## D-017 — SQL integration tests use disposable synthetic SQL Server
**Date:** 2026-08-16  
**Status:** Accepted
CI uses disposable SQL Server 2022 with synthetic Contract v1 schema/data only.

## D-018 — Transactional writes are not automatically replayed
**Date:** 2026-08-16  
**Status:** Accepted
Connection/read retry is separated from transactional write execution to avoid commit ambiguity.

## D-019 — Bounded retry is limited to safe connection/read boundaries
**Date:** 2026-08-16  
**Status:** Accepted
Connection open and complete read-only operations may use bounded/cancellable retry; transaction-scoped writes do not replay after execution begins.

## D-020 — .NET Framework compatibility must be proved by consuming the package
**Date:** 2026-08-16  
**Status:** Accepted
Before v1.0, compatibility with .NET Framework 4.8 is validated by building and running a real `net48` application on Windows that restores the generated `SadrScales.Integration` NuGet package. A project-reference-only build or theoretical TFM table is insufficient. The smoke consumer must also load the SqlClient dependency selected by the restored package graph.

## D-021 — PLU batch writes use explicit bounded atomic transactions
**Date:** 2026-08-17  
**Status:** Accepted
`UpsertBatchAsync` accepts at most 200 unique PLUs per call. The complete batch is validated before SQL access and committed in one transaction. Any write failure rolls back the entire call. Larger imports are explicitly paged by the destination application rather than hidden inside one SDK call.
