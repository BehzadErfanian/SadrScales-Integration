# Roadmap

## M0 — Public repository foundation — complete
- [x] Public repository, bilingual README, security boundary, continuity docs and Public Guard.
- [x] Public support/contribution/code-of-conduct files.
- [x] CODEOWNERS and hardened Issue/PR intake.
- [x] Weekly Dependabot policy for NuGet and GitHub Actions.

## M1 — Contract v1 / public documentation — engineering complete
- [x] Freeze Contract v1 against effective Sadr Scales 5.2.1 schema.
- [x] Bilingual contract docs, SQL samples and regression checklist.
- [x] Final 5.2.1 Integration Guide identity pinned to the 38-page release asset and SHA-256.
- [x] Bilingual Getting Started and troubleshooting paths.
- [x] Vendor production-readiness/go-live checklist.
- [x] Complete/review repository-owner GitHub host-security controls.
- [ ] Actual Guide upload to stable GitHub Release after tag.

## M2 — C# Integration SDK v1 — release candidate complete

### Foundation — complete
- [x] `netstandard2.0`, `Microsoft.Data.SqlClient 7.0.2`.
- [x] Contract validator, semantic item/group upserts and read-only incremental sales.
- [x] Unit/package CI and documentation.

### SQL-backed hardening — complete
- [x] Disposable SQL Server 2022 integration suite.
- [x] Contract, write, batch and rollback paths exercised against real SQL Server.

### Retry/reliability — complete
- [x] Safe bounded/cancellable connection/read retry.
- [x] Transaction-scoped writes deliberately non-replayed after execution begins.

### PLU batch — complete
- [x] Atomic `UpsertBatchAsync`, maximum 200 unique PLUs per call.
- [x] Complete prevalidation, aggregate results and full transaction rollback on failure.
- [x] Semantic no-op/rowversion behavior covered.

### .NET Framework 4.8 compatibility — complete
- [x] Real NuGet-package consumer application.
- [x] Local generated-package restore.
- [x] Windows Server 2022 `net48` build/runtime smoke.
- [x] Warnings treated as errors.
- [x] SDK + SqlClient dependency graph loads at runtime.

### Developer experience — complete for v1
- [x] Executable read-only-by-default C# Quick Start.
- [x] Environment-variable connection-string configuration.
- [x] Root English/Persian developer landing pages.
- [x] Bilingual troubleshooting guide.
- [x] Raw SQL reference samples.
- [x] Public support policy and production-readiness checklist.
- [x] Obsolete duplicate C# sample placeholder removed.

### Package/release hardening — complete for release candidate
- [x] Package version/metadata prepared for `1.0.0`.
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

### GitHub host security — complete before v1
- [x] Secret Scanning enabled.
- [x] Push Protection enabled.
- [x] Dependabot vulnerability alerts enabled.
- [x] Dependabot security updates enabled.
- [x] Private Vulnerability Reporting enabled.
- [x] C# CodeQL default setup configured.

## M3 — Stable `v1.0.0` publication — in progress
- [x] Approve MIT public software license and add `LICENSE`/package license metadata.
- [x] Joint provider/copyright identity documented and package-validated.
- [x] Complete GitHub owner/admin host-security gate.
- [ ] Final PR CI/review/merge.
- [ ] Validate exact merged `main`.
- [ ] Configure `main` branch protection using validated post-merge checks.
- [ ] Tag exact validated source as `v1.0.0`.
- [ ] Confirm protected release run passes.
- [ ] Inspect Draft Release package/DLL/symbols/developer kit/guide/manifest/checksums.
- [ ] Publish GitHub Release.

## M4 — Multi-language reference examples — after v1
- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] Language-neutral SQL type/null mapping table.

Until these wrappers exist, non-C# developers use the documented SQL Contract v1 and executable SQL samples as the language-neutral integration source of truth.

## M5 — Developer distribution improvements — after v1
- [ ] Website developer landing page linking to GitHub/release/guide.
- [ ] Evaluate NuGet.org publication and package ownership policy.

## Future

No-code connectors, REST/Webhook Gateway and advanced structured-invoice helpers remain separately versioned future scope. Direct scale wire protocols remain private.
