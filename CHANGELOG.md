# Changelog

## [Unreleased]

No unreleased changes yet.

## [1.0.0] — 2026-08-18

First stable public release of SadrScales Integration.

### Added
- Frozen bilingual SQL Contract v1 documentation and regression checklist for the Sadr Scales 5.2.1 public integration surface.
- Raw SQL schema-validation, safe PLU upsert and read-only incremental-sales samples.
- `SadrScales.Integration` C# SDK targeting `netstandard2.0` with `Microsoft.Data.SqlClient 7.0.2`.
- Contract schema validator.
- Semantic item-group and PLU upsert APIs.
- Atomic bounded `UpsertBatchAsync` for at most 200 unique PLUs per call, including aggregate Inserted/Updated/Unchanged results.
- Read-only incremental sales API with destination-owned durable cursor behavior.
- Bounded/cancellable retry for safe connection/read-only boundaries.
- Unit tests plus disposable SQL Server 2022 integration tests.
- Real .NET Framework 4.8 NuGet-package consumer restore/build/runtime CI gate.
- Executable read-only-by-default C# Quick Start using `SADR_SCALES_CONNECTION_STRING`.
- Bilingual Getting Started and troubleshooting guides.
- Production-readiness/go-live checklist for software vendors.
- Public support policy and hardened issue intake for sanitized bug/feature reports.
- Repository CODEOWNERS and automated NuGet/GitHub Actions dependency-update configuration.
- SDK API/SemVer compatibility policy.
- Source Link/repository metadata and NuGet package-shape validation.
- .NET package validation during pack hardening.
- Automated Binaries ZIP, Developer Kit ZIP, release manifest and SHA-256 checksum generation.
- Protected tag release workflow that reruns SDK/SQL/net48 gates, verifies the official Integration Guide SHA-256 and creates a Draft GitHub Release.
- Release-bundle smoke validation in normal pull-request CI.
- Machine-readable final Sadr Scales 5.2.1 Integration Guide identity.
- MIT `LICENSE` with joint provider/copyright identity for Tozin Sadr and Behzad Erfanian.
- Verified GitHub host-security baseline: Secret Scanning, Push Protection, Dependabot vulnerability alerts/security updates, Private Vulnerability Reporting and C# CodeQL default setup.
- Protected `main` branch requiring the validated build/SQL/net48/public-boundary checks.

### Changed
- Package metadata established the first stable `1.0.0` line and identifies both Tozin Sadr and Behzad Erfanian as authors/providers.
- NuGet validation verifies MIT license metadata, both provider identities, package shape and Source Link/repository metadata.
- Root English/Persian README files provide a direct developer onboarding, support and production-handoff path.
- Release bundles include license, support guidance, changelog, contribution guidance and production-readiness documentation where appropriate.
- Compatibility claims require executable package-consumer/runtime evidence rather than target-framework tables alone.
- Read-only validation/sales operations may replay safely after recognized transient failures; transaction-scoped writes remain non-replayed after execution begins.
- Final Integration Guide identity superseded the earlier M1 34-page pre-release artifact with the final 38-page Sadr Scales 5.2.1 release asset.
- Public repository validation requires the release/security/governance files expected for a stable public SDK.

### Removed
- Obsolete duplicate `samples/CSharp` placeholder after the real executable sample was established under `samples/csharp`.

### Security / compatibility decisions
- SDK `1.0.0` remains unsigned (no strong-name) unless a concrete supported-consumer identity requirement is demonstrated in the future.
- Direct device protocols, captures, private keys, customer data and internal Sadr Scales runtime/release infrastructure remain outside the public repository.
- Security-sensitive reports are explicitly routed away from normal public Issues.
- Stable release tags are never moved or reused after publication.
- `main` is protected with required validated checks, admin enforcement, conversation resolution and force-push/deletion disabled.

### Release evidence
- Stable source commit: `a6bccc7c13a8afba29b6860869d2a942b1231803`.
- SDK CI on `main`: run `32111583869` — PASS.
- Public Repository Guard on `main`: run `32112295906` — PASS.
- Protected Release: run `32112295891` — PASS.
- GitHub Release ID: `372167195`.
- Official 38-page guide SHA-256: `182be9aa73348a35a299ab0fad22e5e9deeba800ef9222c0145ba582b02e281b`.
- Draft assets were downloaded and verified before publication; release manifest, NuGet metadata, package/bundle structure and SHA-256 checks passed.
