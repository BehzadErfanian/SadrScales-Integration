# Changelog

All notable public changes to this project will be documented here.

## [Unreleased]

### Added
- Public repository foundation and security guard.
- Frozen SQL Contract v1 documentation for Sadr Scales 5.2.1.
- Bilingual Quick Starts, regression checklist and synthetic SQL samples.
- Official Persian Integration Guide release identity/SHA.
- `SadrScales.Integration` C# SDK foundation targeting `netstandard2.0`.
- Contract validator, semantic item-group/item upserts and read-only incremental sales client.
- Unit tests and SDK restore/build/test/pack workflow.
- Real SQL Server 2022 integration-test suite using disposable synthetic Contract v1 schema/data.
- Bounded transient connection/read retry options and internal retry policy.
- Retry-specific tests for exhaustion, non-transient failures, cancellation and bounded backoff.

### Changed
- SQL-backed hardening is now part of normal SDK CI; Contract/item/rowversion/sales behavior is tested against real disposable SQL Server.
- Connection establishment may retry recognized transient failures before an operation begins.
- Read-only Contract validation and sales reads may replay completely on a fresh connection after recognized transient failures.
- Transactional item/group write command execution remains deliberately non-retried to avoid commit ambiguity.

### Fixed
- Public repository validator is portable across Windows PowerShell and PowerShell Core/Linux.
- Corrected SQL mismatch test fixture to drop/recreate a UNIQUE table constraint using table-constraint DDL.
