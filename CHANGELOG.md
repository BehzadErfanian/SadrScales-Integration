# Changelog

All notable public changes to this project will be documented here.

The format follows the spirit of Keep a Changelog and Semantic Versioning.

## [Unreleased]

### Added

- Initial public repository foundation.
- Frozen SQL Contract v1 documentation for Sadr Scales 5.2.1.
- Project governance, security boundary and public-repository CI guard.
- Contract regression checklist and executable synthetic SQL samples.
- GitHub host-security administration checklist.
- Official Persian Integration & Database Guide release identity and SHA-256.
- Initial `SadrScales.Integration` C# SDK foundation targeting `netstandard2.0`.
- Contract schema validator and `SadrContractMismatchException`.
- Basic item-group and item/PLU upsert clients with semantic no-op behavior.
- Read-only incremental sales client and destination-owned cursor candidate.
- SDK unit tests and GitHub Actions restore/build/test/pack workflow.
- `SDK_DESIGN_V1.md`.

### Changed

- Moved active engineering from Contract/documentation M1 to SDK M2.
- Basic SDK uses `Microsoft.Data.SqlClient 7.0.2` and caller-owned connection configuration.
- Basic sales consumption remains destination-owned and does not mutate Sadr sales rows.

### Fixed

- Made the public-repository validator path normalization portable across Windows PowerShell and PowerShell Core on Linux/GitHub Actions.
