# Project Status — SadrScales-Integration

**Last updated:** 2026-08-16 22:39 +03:30  
**Phase:** M0 — Public repository foundation  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Purpose

Create one public, polished GitHub repository that software vendors can receive as the single developer link for integrating POS, ERP and accounting systems with Sadr Scales.

The repository must contain source code, contract documentation, samples, compatibility information, release assets and enough project history to continue development without depending on chat history.

## Current completed foundation

- Public GitHub repository created: `BehzadErfanian/SadrScales-Integration`.
- Public/private security boundary documented.
- Root English and Persian README created.
- Project continuity policy created (`AGENTS.md`).
- Roadmap, backlog and decision log created.
- SQL Contract v1 summarized in Persian and English.
- Full Persian 5.2.1 Integration & Database Guide is approved as the detailed source reference and will be published as a GitHub Release asset after M1 validation.
- Multi-language sample folders prepared.
- Public repository validation script and CI security guard prepared.

## What is intentionally not implemented yet

- C# SDK/library code.
- Final SDK API surface.
- Framework target decision.
- NuGet package.
- Executable C#/SQL/Python/Node.js/Java/PHP samples.
- GitHub Release `v1.0.0`.
- Public website developer-page update.

## Exact next step

1. Publish this reviewed M0 foundation to `BehzadErfanian/SadrScales-Integration` and verify the public-repository guard.
2. Review GitHub security settings available for the public repository (secret scanning / dependency alerts where available).
3. Start M1: freeze/validate the public Contract v1 documentation against Sadr Scales 5.2.1 and turn the current full guide into the official PDF/reference release artifact.
4. Only after M1 is clean, design the C# SDK API and target frameworks (M2).
5. Explicitly approve the public software license before distributing the first SDK release.

## Definition of done for M0

M0 is complete when the public repository exists, the initial files are pushed, the security validator passes, and the project links/docs render correctly on GitHub.
