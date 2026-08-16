# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16 22:54 +03:30  
**Phase:** M1 — Contract v1 freeze and public documentation  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Purpose

Create one public, polished GitHub repository that software vendors can receive as the single developer link for integrating POS, ERP and accounting systems with Sadr Scales.

The repository must contain source code, contract documentation, samples, compatibility information, release assets and enough project history to continue development without depending on chat history.

## M0 foundation — complete

- Public GitHub repository: `BehzadErfanian/SadrScales-Integration`.
- Repository visibility: Public.
- Public/private security boundary documented.
- English and Persian README published.
- Project continuity policy (`AGENTS.md`) published.
- Roadmap, backlog, decision log, work log, compatibility and release policy published.
- SQL Contract v1 concise documentation published in Persian and English.
- Multi-language sample structure prepared.
- GitHub issue/PR templates prepared.
- Public repository security validator and GitHub Actions guard enabled.
- First guard run exposed a PowerShell Core portability defect in the validator; the defect was fixed.
- Corrected `Public repository guard` run passed on GitHub Actions.
- No device protocol captures, private keys, customer databases/credentials or private Sadr Scales runtime source were published.
- No public software `LICENSE` is published yet; the license requires explicit company approval.
- Binary Word/PDF guide files are kept out of `main` and will be distributed as GitHub Release assets after M1 validation.

## What is intentionally not implemented yet

- C# SDK/library code.
- Final SDK API surface.
- Framework target decision.
- NuGet package.
- Executable C#/SQL/Python/Node.js/Java/PHP samples.
- GitHub Release `v1.0.0`.
- Public website developer-page update.

## Exact next step — M1

1. Re-validate the public SQL Contract v1 fields/rules against the current Sadr Scales 5.2.1 source/schema.
2. Freeze the basic public surface around `SADR_ItemClass`, `SADR_Item` and read-only `SADR_Logs`.
3. Keep Registry/Mapping/structured-sales areas explicitly advanced/controlled unless a new decision promotes them.
4. Produce the official PDF from the approved Persian Integration & Database Guide and prepare it for a GitHub Release asset.
5. Add synthetic executable SQL examples and expected results.
6. Review GitHub security settings available to the public repository (secret scanning/dependency alerts where available).
7. Only after M1 is clean, begin M2 C# SDK/API design.

## Handoff rule

A future chat/session must begin by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md` and `SECURITY_BOUNDARY.md`. Chat history is not the project source of truth.
