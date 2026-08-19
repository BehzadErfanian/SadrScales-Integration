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

## D-009 — Public SDK uses MIT with joint providers
**Date:** 2026-08-18  
**Status:** Accepted
The public `SadrScales-Integration` SDK and repository materials covered by `LICENSE` are distributed under the MIT License. The public copyright/provider identity is `Tozin Sadr and Behzad Erfanian`, and package metadata must identify both. This public license does not publish or license private Sadr Scales runtime source, proprietary device protocols, firmware, private keys, customer data or other material outside the licensed public repository.

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
Release upload and host-security toggles remain explicit pre-v1.0 gates.

## D-014 — C# SDK v1 targets .NET Standard 2.0
**Date:** 2026-08-16  
**Status:** Accepted
The shared SDK assembly targets `netstandard2.0` to serve modern .NET and .NET Framework consumers.

## D-015 — Microsoft.Data.SqlClient is the SQL provider
**Date:** 2026-08-16  
**Status:** Accepted
SDK v1 uses `Microsoft.Data.SqlClient 7.0.2`; caller owns connection/security configuration.

## D-016 — SDK does not own destination sales feed cursor
**Date:** 2026-08-16  
**Status:** Accepted
Sales Feed read is read-only and the destination owns its durable cursor/state. This does not remove the separate Structured Invoice acknowledgement contract accepted later in D-025.

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

## D-022 — SDK 1.0.0 is not strong-name signed
**Date:** 2026-08-18  
**Status:** Accepted
The tested v1 consumer surface does not require a strong-name identity. A real .NET Framework 4.8 package consumer already restores/builds/runs the unsigned SDK successfully. Strong names are treated as assembly identity/compatibility infrastructure rather than a security boundary. Introducing signing would also create a long-lived key and assembly-identity commitment. Therefore `1.0.0` remains unsigned; any future supported consumer that genuinely requires a strong-named dependency must trigger an explicit compatibility/key-lifecycle decision rather than an incidental packaging change.

## D-023 — Public repository host security is a release gate
**Date:** 2026-08-18  
**Status:** Accepted
Before the first stable public release, the repository must have Secret Scanning, Push Protection, Dependabot vulnerability alerts/security updates, Private Vulnerability Reporting and C# CodeQL default setup enabled/configured. These controls were verified on 2026-08-18. `main` branch protection is configured only after the release-hardening PR is merged, using the exact validated post-merge check identities. While the repository has one maintainer, branch protection does not require an unavailable second approving reviewer; it does require validated checks and disallows force-push/deletion.

## D-024 — SQL Scale status is the supported coarse third-party status for 5.2.1
**Date:** 2026-08-19  
**Status:** Accepted
For SQL-based third-party integration against Sadr Scales 5.2.1, `dbo.SADR_Scale.Status` is the supported source for coarse `Online` / `Offline` state because Sadr Scales itself persists those transitions. Rich transient Runtime state such as progress/current activity/last error is not part of the 5.2.1 SQL contract and may be added later through a managed service/API.

## D-025 — Structured Invoice lookup and explicit ACK are separate operations
**Date:** 2026-08-19  
**Status:** Accepted
Invoice lookup by TotalBarcode or logical `ScaleID + FID` never auto-acknowledges. The destination first receives the complete invoice and current read state, persists/commits its own transaction, and only then explicitly ACKs the invoice. Invoice-level ACK sets `SADR_Total.LableStatus = 1` and is idempotent.

## D-026 — AlreadyRead never blocks invoice recovery
**Date:** 2026-08-19  
**Status:** Accepted
If an invoice has already been ACKed (`LableStatus = 1`), a later lookup still returns the complete invoice and reports `AlreadyRead`. `AlreadyRead` is an informational warning, not a data-access block, so the destination can recover or re-import an invoice that was previously removed or lost after an earlier successful scan.

## D-027 — SQL Integration supports explicit AutoSend resend requests
**Date:** 2026-08-19  
**Status:** Accepted
Sadr Scales 5.2.1 already uses `SADR_Scale.LastSendItem` and `SADR_Scale.LastSendKey` as AutoSend watermarks. The public vNext SQL contract therefore supports `RequestItemResend(scaleId)` by setting `LastSendItem = 0`, and `RequestHotKeyResend(scaleId)` by setting `LastSendKey = 0` where the current model/runtime supports automatic HotKey transfer. These operations are resend requests, not immediate device commands: the next eligible AutoSend cycle performs the transfer when the scale is enabled, connected and configured for automatic sending. SDK methods hide the watermark detail; the Raw SQL path remains documented for non-C# consumers.

## D-028 — Runtime-only capabilities move to a Service, not a SQL command queue
**Date:** 2026-08-19  
**Status:** Accepted
SQL is extended only for behavior that Sadr Scales 5.2.1 can safely and predictably expose through its database. Scale lifecycle orchestration, immediate Send/Get commands, direct device sales retrieval, settings and richer Runtime state are not modeled as a new SQL command queue. They will be exposed later through a typed Sadr Integration Service that reuses Sadr Scales validation, licensing, registry, connection and model-capability logic while keeping device protocols private. The exact local/REST transport of that Service is a later implementation decision and must not change the public Domain semantics.
