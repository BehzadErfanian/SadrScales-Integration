# Project Status — SadrScales-Integration

**Last updated:** 2026-08-18  
**Phase:** `v1.0.0` stable public release — published  
**Current stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Stable release identity

- Git tag: `v1.0.0`
- Stable source commit: `a6bccc7c13a8afba29b6860869d2a942b1231803`
- Release ID: `372167195`
- Protected Release run: `32112295891` — PASS
- Protected Release artifact: `SadrScales-Integration-v1.0.0-1`
- Artifact ID: `9315377547`
- License: MIT
- Providers/copyright identity: **Tozin Sadr and Behzad Erfanian**

The stable tag points to the exact validated merge commit and must never be moved or reused.

## Stable release assets

The published release contains exactly these eight primary assets:

1. `SadrScales.Integration.1.0.0.nupkg`
2. `SadrScales.Integration.1.0.0.snupkg`
3. `SadrScales.Integration-1.0.0-Binaries.zip`
4. `SadrScales.Integration-1.0.0-DeveloperKit.zip`
5. `SadrScales_Integration_Database_Guide_5.2.1_FA.pdf`
6. `release-manifest.json`
7. `SHA256SUMS.txt`
8. `RELEASE_NOTES.md`

All Draft Release assets were downloaded directly and verified before publication. File sizes matched GitHub asset metadata, `SHA256SUMS.txt` validated the release files, NuGet metadata was checked for package identity/version/MIT/both providers, the Binaries ZIP contained the compiled SDK and XML documentation, and the Developer Kit contained the expected public handoff/governance material.

## Final validation evidence

### Exact merged `main`

`a6bccc7c13a8afba29b6860869d2a942b1231803`

### Main CI

- SDK CI run `32111583869`: **PASS**
- Public Repository Guard run `32112295906`: **PASS**

Required status checks verified on the stable source:

- `build-test-pack`: PASS
- `sql-integration-test`: PASS
- `net48-package-consumer`: PASS
- `validate-public-boundary`: PASS

### Protected release

Protected Release run `32112295891`: **PASS**

Jobs:

- `build-test-pack`: PASS
- `sql-integration-test`: PASS
- `net48-package-consumer`: PASS
- `draft-release`: PASS

The `draft-release` job successfully:

- checked out the tagged source;
- rebuilt and repacked the SDK;
- validated the NuGet package;
- downloaded and SHA-verified the official Integration Guide;
- built the final release bundle/checksums;
- uploaded release engineering evidence;
- created/updated the Draft GitHub Release.

## Official Integration Guide

- File: `SadrScales_Integration_Database_Guide_5.2.1_FA.pdf`
- Pages: 38
- SHA-256: `182be9aa73348a35a299ab0fad22e5e9deeba800ef9222c0145ba582b02e281b`
- Source Sadr Scales release commit: `1048749f52faba35e69464b64983e772c1c857e3`

## GitHub security and branch protection

Repository-owner verification confirmed before publication:

- Secret Scanning: enabled;
- Push Protection: enabled;
- Dependabot vulnerability alerts: enabled;
- Dependabot security updates: enabled and not paused;
- Private Vulnerability Reporting: enabled;
- CodeQL default setup for C#: configured with default query suite and `remote_and_local` threat model.

`main` branch protection is active with:

- required checks: `build-test-pack`, `sql-integration-test`, `net48-package-consumer`, `validate-public-boundary`;
- strict/up-to-date status checks;
- admin enforcement;
- conversation resolution required;
- force pushes disabled;
- branch deletion disabled;
- zero required approving reviews while the repository has one maintainer.

## Completed engineering and developer readiness

- SQL Contract v1 audit/freeze and bilingual contract documentation.
- C# SDK targeting `netstandard2.0` with `Microsoft.Data.SqlClient 7.0.2`.
- Real SQL Server 2022 integration tests.
- Bounded safe connection/read retry.
- Transaction-scoped writes intentionally non-replayed after execution begins.
- Real .NET Framework 4.8 generated-package consumer validation.
- Atomic bounded PLU batch API, maximum 200 unique PLUs.
- Executable read-only-by-default C# Quick Start.
- Bilingual Getting Started and troubleshooting documentation.
- Raw SQL path for non-C# consumers.
- Production-readiness/go-live checklist.
- Support, security, contribution and code-of-conduct policies.
- CODEOWNERS and Dependabot policy.
- Source Link/repository package metadata.
- NuGet package-shape/.NET package validation.
- SDK API/SemVer compatibility policy.
- Strong-name decision: stable v1 remains unsigned unless a real supported-consumer requirement appears.
- Automated release bundles, manifest, SHA-256 and protected tag workflow.

## Distribution model

GitHub is the developer source of truth. Stable SDK packages, compiled binaries, Developer Kit, official guide and checksums are distributed from the `v1.0.0` GitHub Release rather than committed as binary clutter to `main`.

NuGet.org publication remains a separate post-v1 decision because it requires package-account/ownership/publication-policy administration.

## Scope boundary

`v1.0.0` does not expose direct device protocols, packet captures, private firmware/vendor data, customer data, private keys or internal Sadr Scales runtime source.

Python/Node.js/Java/PHP wrappers, REST/Webhook Gateway, no-code connectors and advanced structured-invoice helpers remain post-v1/future work. Non-C# developers can use SQL Contract v1 and the executable SQL samples now.

## Next phase

The first stable public SDK release is closed. Normal work moves to post-v1 maintenance and developer reach. Any breaking SDK API change requires Semantic Versioning discipline, and any breaking SQL surface change requires a new SQL Contract version.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md`, `API_COMPATIBILITY.md`, `PRODUCTION_READINESS_CHECKLIST.md` and `SECURITY_BOUNDARY.md`.
