# Changelog

## [Unreleased]

### Added
- Public repository foundation/security guard and frozen SQL Contract v1 docs.
- Bilingual Quick Starts, regression checklist and synthetic SQL examples.
- C# `SadrScales.Integration` SDK foundation targeting `netstandard2.0`.
- Contract validator, semantic item/group upserts and read-only sales reader.
- Unit/package CI and disposable SQL Server 2022 integration suite.
- Bounded transient connection/read retry with conservative classification and cancellation-aware backoff.
- .NET Framework 4.8 NuGet-package consumer compatibility smoke application and Windows CI job.

### Changed
- Read-only validation/sales operations may replay safely after recognized transient failures; transaction-scoped writes remain non-replayed.
- Compatibility claims now require an actual package consumer build/runtime gate in addition to TFM compatibility documentation.

### Fixed
- Public repository validator portability across Windows/Linux PowerShell.
- SQL mismatch fixture DDL for named UNIQUE table constraints.
