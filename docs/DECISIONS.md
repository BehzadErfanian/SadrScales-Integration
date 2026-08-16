# Decision Log

Accepted decisions are recorded here so future work does not need to recover them from chat history.

## D-001 — Dedicated public integration repository
**Date:** 2026-08-16  
**Status:** Accepted
Integration tooling and public developer documentation live in `SadrScales-Integration`, separate from private runtime repositories.

## D-002 — GitHub is the developer source of truth
**Date:** 2026-08-16  
**Status:** Accepted
Software vendors receive one public GitHub link. Source, docs, samples, compatibility and SDK releases are maintained here.

## D-003 — SQL Contract v1 is the 5.2.1 public contract
**Date:** 2026-08-16  
**Status:** Accepted
Basic public surface is `SADR_ItemClass`, `SADR_Item` and read-only `SADR_Logs`. REST/Webhook is not a 5.2.1 capability.

## D-004 — Device protocols remain private
**Date:** 2026-08-16  
**Status:** Accepted
Direct PLUS/LSG/Aclas/device protocols, captures and reverse-engineering/vendor material are never published here.

## D-005 — One destination application per Sadr Scales installation
**Date:** 2026-08-16  
**Status:** Accepted
Structured-sales processing status is not designed as a shared multi-consumer message bus.

## D-006 — Documentation is part of every change
**Date:** 2026-08-16  
**Status:** Accepted
Accepted decisions, completed work, remaining work and next steps are recorded in repository docs.

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
No `LICENSE` is published until the company explicitly approves the public software license.

## D-010 — Future no-code/REST work is separate from 5.2.1
**Date:** 2026-08-16  
**Status:** Accepted
No-code connector and REST/Webhook Gateway are separately versioned future work.

## D-011 — Binary reference guides are release assets
**Date:** 2026-08-16  
**Status:** Accepted
Editable Word and official PDF binaries remain out of `main`; approved PDF/checksum are Release assets.

## D-012 — Basic SQL Contract v1 is frozen against effective 5.2.1 schema
**Date:** 2026-08-16  
**Status:** Accepted
The baseline is the schema after Sadr Scales 5.2.1 completes its migration/check. Breaking public changes require a new contract version.

## D-013 — M1 engineering does not block M2 on host/release administration
**Date:** 2026-08-16  
**Status:** Accepted
License, Release-asset upload and GitHub host settings remain explicit pre-v1.0 gates but do not block SDK source work.

## D-014 — C# SDK v1 targets .NET Standard 2.0
**Date:** 2026-08-16  
**Status:** Accepted for pre-1.0 validation
Real .NET Framework 4.8 consumer compatibility validation is required before v1.0.

## D-015 — Microsoft.Data.SqlClient is the SDK SQL provider
**Date:** 2026-08-16  
**Status:** Accepted for pre-1.0 validation
The foundation uses `Microsoft.Data.SqlClient 7.0.2`; caller owns connection/security configuration.

## D-016 — Basic SDK does not own destination sales state
**Date:** 2026-08-16  
**Status:** Accepted
Sales reads are read-only and the destination owns its durable cursor/state.

## D-017 — Real SQL integration tests use a disposable synthetic SQL Server
**Date:** 2026-08-16  
**Status:** Accepted
CI uses disposable SQL Server 2022 with synthetic Contract v1 schema/data only.

## D-018 — Do not automatically retry transactional writes before ambiguity is tested
**Date:** 2026-08-16  
**Status:** Accepted
Connection/read retry and transactional write retry are separate. A write is not automatically replayed after command/transaction execution begins.

## D-019 — Bounded retry is limited to safe connection/read boundaries
**Date:** 2026-08-16  
**Status:** Accepted for pre-1.0 validation
The SDK implements a small explicit transient retry policy compatible with its `netstandard2.0` target. It retries connection establishment before any operation begins and replays complete read-only `ValidateAsync` / `Sales.ReadAfterAsync` attempts on fresh connections. It does not wrap transaction-scoped item/group writes in operation-level retry.

Defaults are 2 retries after the first attempt and 250 ms base delay with bounded exponential backoff capped at 5 seconds. Retry honors cancellation and rethrows the final native exception. Authentication/configuration failures are not blindly classified as transient.
