# Roadmap

## M0 — Public repository foundation — complete
- [x] Public repository, bilingual README, security boundary, continuity docs and Public Guard.
- [x] Public support/contribution/code-of-conduct files.
- [x] CODEOWNERS and hardened Issue/PR intake.
- [x] Weekly Dependabot policy for NuGet and GitHub Actions.

## M1 — Contract v1 / public documentation — complete
- [x] Freeze Contract v1 against effective Sadr Scales 5.2.1 schema.
- [x] Bilingual contract docs, SQL samples and regression checklist.
- [x] Final 5.2.1 Integration Guide identity pinned to the 38-page release asset and SHA-256.
- [x] Bilingual Getting Started and troubleshooting paths.
- [x] Vendor production-readiness/go-live checklist.
- [x] Complete/review repository-owner GitHub host-security controls.
- [x] Publish and verify the final Guide in stable GitHub Release `v1.0.0`.

## M2 — C# Integration SDK v1 — complete

### Foundation
- [x] `netstandard2.0`, `Microsoft.Data.SqlClient 7.0.2`.
- [x] Contract validator, semantic item/group upserts and read-only incremental sales.
- [x] Unit/package CI and documentation.

### SQL-backed hardening
- [x] Disposable SQL Server 2022 integration suite.
- [x] Contract, write, batch and rollback paths exercised against real SQL Server.

### Retry/reliability
- [x] Safe bounded/cancellable connection/read retry.
- [x] Transaction-scoped writes deliberately non-replayed after execution begins.

### PLU batch
- [x] Atomic `UpsertBatchAsync`, maximum 200 unique PLUs per call.
- [x] Complete prevalidation, aggregate results and full transaction rollback on failure.
- [x] Semantic no-op/rowversion behavior covered.

### .NET Framework 4.8 compatibility
- [x] Real NuGet-package consumer application.
- [x] Local generated-package restore.
- [x] Windows Server 2022 `net48` build/runtime smoke.
- [x] Warnings treated as errors.
- [x] SDK + SqlClient dependency graph loads at runtime.

### Developer experience
- [x] Executable read-only-by-default C# Quick Start.
- [x] Environment-variable connection-string configuration.
- [x] Root English/Persian developer landing pages.
- [x] Bilingual troubleshooting guide.
- [x] Raw SQL reference samples.
- [x] Public support policy and production-readiness checklist.
- [x] Obsolete duplicate C# sample placeholder removed.

### Package/release hardening
- [x] Package version/metadata published as `1.0.0`.
- [x] MIT license approved and committed.
- [x] Package/provider identity: Tozin Sadr + Behzad Erfanian.
- [x] Source Link/repository metadata validated inside generated NuGet package.
- [x] .NET package validation enabled.
- [x] API/SemVer compatibility policy documented.
- [x] Strong-name decision recorded: unsigned v1.0.0.
- [x] Automated Binaries/Developer Kit bundles, release manifest and SHA-256 checksums.
- [x] Release bundle includes license/support/contribution/changelog/production-readiness material.
- [x] Release-bundle smoke gate in normal CI.
- [x] Protected tag workflow reruns SDK/SQL/net48 gates, verifies final Guide SHA and creates a Draft GitHub Release.

### GitHub host security
- [x] Secret Scanning enabled.
- [x] Push Protection enabled.
- [x] Dependabot vulnerability alerts enabled.
- [x] Dependabot security updates enabled.
- [x] Private Vulnerability Reporting enabled.
- [x] C# CodeQL default setup configured.
- [x] `main` branch protection configured with validated checks.
- [x] Admin enforcement and conversation resolution enabled.
- [x] Force pushes/deletion disabled on protected `main`.

## M3 — Stable `v1.0.0` publication — complete
- [x] Approve MIT public software license and add `LICENSE`/package license metadata.
- [x] Joint provider/copyright identity documented and package-validated.
- [x] Complete GitHub owner/admin host-security gate.
- [x] Merge release-hardening PR #8.
- [x] Validate exact merged `main` commit `a6bccc7c13a8afba29b6860869d2a942b1231803`.
- [x] Configure `main` branch protection using validated post-merge checks.
- [x] Tag exact validated source as `v1.0.0`.
- [x] Confirm Protected Release run `32112295891` passes.
- [x] Inspect and verify Draft Release package/DLL/symbols/developer kit/guide/manifest/checksums.
- [x] Publish GitHub Release ID `372167195` as stable/non-prerelease.

## M4 — Multi-language reference examples — post-v1
- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] Language-neutral SQL type/null mapping table.

Until these wrappers exist, non-C# developers use the documented SQL Contract v1 and executable SQL samples as the language-neutral integration source of truth.

## M5 — Developer distribution improvements — post-v1
- [ ] Website developer landing page linking GitHub/release/guide.
- [ ] Evaluate NuGet.org publication and package ownership policy.

## M6 — Post-v1 repository hardening
- [ ] Evaluate GitHub Immutable Releases after confirming compatibility with the Draft→verify→publish process.
- [ ] Add a second trusted maintainer when operationally appropriate; then revisit required approving reviews/CODEOWNERS enforcement.
- [ ] Keep dependency/security alerts and compatibility evidence current.

## Future

No-code connectors, REST/Webhook Gateway and advanced structured-invoice helpers remain separately versioned future scope. Direct scale wire protocols remain private.
