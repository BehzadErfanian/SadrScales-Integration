# Changelog

All notable public changes to this project will be documented here.

The format follows the spirit of Keep a Changelog and Semantic Versioning.

## [Unreleased]

### Added

- Initial public repository foundation.
- SQL Contract v1 documentation baseline for Sadr Scales 5.2.1.
- Project roadmap, decision log, backlog, security boundary and release policy.
- Reference policy for the approved Persian Integration & Database Guide (published as a release asset after M1 validation).
- Multi-language sample structure.
- Public-repository validation script and CI guardrail.
- Contract v1 freeze record and regression checklist.
- Executable synthetic SQL samples for schema validation, item upsert and incremental sales reading.

### Changed

- Froze the basic SQL Contract v1 surface after re-validation against the effective Sadr Scales 5.2.1 migrated schema.
- Expanded Persian and English contract/Quick Start documentation with exact item identity, legacy-field, rowversion, sales-cursor and idempotency rules.
- Clarified that Registry/Mapping/structured-sales/runtime state remain advanced/controlled rather than basic Contract v1 APIs.

### Fixed

- Made the public-repository validator path normalization portable across Windows PowerShell and PowerShell Core on Linux/GitHub Actions.
