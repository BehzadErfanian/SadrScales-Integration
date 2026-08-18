# Changelog

## [Unreleased]

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
- SDK API/SemVer compatibility policy.
- Source Link/repository metadata and NuGet package-shape validation.
- .NET package validation during pack hardening.
- Automated Binaries ZIP, Developer Kit ZIP, release manifest and SHA-256 checksum generation.
- Protected tag release workflow that reruns SDK/SQL/net48 gates, verifies the official Integration Guide SHA-256 and creates a Draft GitHub Release.
- Release-bundle smoke validation in normal pull-request CI.
- Machine-readable final Sadr Scales 5.2.1 Integration Guide identity.

### Changed
- Package metadata is prepared for the first stable `1.0.0` line.
- Root English/Persian README files now provide a direct developer onboarding path instead of pre-1.0 planning status.
- Compatibility claims now require executable package-consumer/runtime evidence rather than target-framework tables alone.
- Read-only validation/sales operations may replay safely after recognized transient failures; transaction-scoped writes remain non-replayed after execution begins.
- Final Integration Guide identity supersedes the earlier M1 34-page pre-release artifact with the final 38-page Sadr Scales 5.2.1 release asset.

### Removed
- Obsolete duplicate `samples/CSharp` placeholder after the real executable sample was established under `samples/csharp`.

### Security / compatibility decisions
- SDK `1.0.0` remains unsigned (no strong-name) unless a concrete supported-consumer identity requirement is demonstrated in the future.
- Direct device protocols, captures, private keys, customer data and internal Sadr Scales runtime/release infrastructure remain outside the public repository.

### Release blockers still open
- Explicit company approval of the public software license and addition of `LICENSE`/NuGet license metadata.
- Owner/admin review of GitHub repository security settings.
- Stable `v1.0.0` tag and final Draft Release verification/publication.
