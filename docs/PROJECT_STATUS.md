# Project Status — SadrScales-Integration

**Last updated:** 2026-08-18  
**Phase:** Phase 1 — Full Integration Surface Audit completed on branch; maintainer review pending  
**Current stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Current published integration contract:** `SQL Contract v1`

## Canonical next-generation plan

The accepted future direction is recorded in:

- `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md`

The Phase 1 audit is recorded in:

- `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`

The master plan remains the canonical source for the product goal, completeness-first priorities, repository simplification, transport-independent domain design, coding/comment/region rules, testing strategy, Developer Sample App, seeded/random demo-data generation, internal simulator completion, protected public scale emulator, POS simulator and full single-PC Integration Lab.

The audit maps the externally useful Sadr Scales 5.2.1 surface into Safe Data Contract, Managed Runtime Command and Internal / Do Not Expose categories. It also records the design gaps that must be resolved before a new public Contract/API is implemented.

The existing `v1.0.0` release remains immutable and valid for its published Basic SQL Contract. The next public version number is intentionally undecided until the new Contract/API compatibility design is complete.

**Next exact step:** maintainer/owner review of the Phase 1 audit. After acceptance, begin Phase 2 Contract & Domain Design. Do not add large new Public SDK clients before the Phase 2 decisions are accepted.

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

### Exact stable release source

`a6bccc7c13a8afba29b6860869d2a942b1231803`

### Main CI for stable release

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

## Completed engineering and developer readiness for v1.0.0

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

NuGet.org publication remains a separate decision because it requires package-account/ownership/publication-policy administration.

## Scope boundary of published v1.0.0

`v1.0.0` does not expose direct device protocols, packet captures, private firmware/vendor data, customer data, private keys or internal Sadr Scales runtime source.

Its Basic SQL Contract is intentionally narrower than the accepted long-term Integration Platform goal. The Phase 1 audit documents that gap explicitly; future work must address it through Contract/Domain design rather than silently changing the published v1 contract.

## Phase 1 audit result

The audit confirms that the useful integration surface is substantially broader than the v1 Basic SQL Contract. Important surfaces include stores, scale configuration, live runtime state, scale/group/item assignments, hot keys, structured invoices, aggregate-barcode lookup, sales query/reporting and managed device commands.

It also confirms that some operations must not be exposed as raw SQL merely because tables exist. Scale lifecycle and live device operations require Runtime orchestration; internal sync/migration/recovery/protocol state remains private.

Open design gates are listed in `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md` and must be resolved in Phase 2 before implementation.

## Handoff rule

A future session begins with `AGENTS.md`, then `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md`, then `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`, then this status file and only the current-state/contract references relevant to the task.
