# Backlog

`v1.0.0` is published and the stable-release gates are closed. Remaining items are post-v1 maintenance and developer reach.

## P0 — Stable v1.0.0 release — complete
- [x] MIT public software license approved; `LICENSE` and NuGet package metadata identify Tozin Sadr and Behzad Erfanian.
- [x] Repository-owner host security verified: Secret Scanning, Push Protection, Dependabot alerts/security updates, Private Vulnerability Reporting and C# CodeQL default setup enabled/configured.
- [x] Release-hardening PR #8 merged to `main`.
- [x] Exact merged `main` validated.
- [x] `main` branch protection configured using the validated required checks.
- [x] Stable tag `v1.0.0` created on exact validated commit `a6bccc7c13a8afba29b6860869d2a942b1231803`.
- [x] Protected Release run `32112295891` passed.
- [x] Draft Release assets downloaded and SHA-256 verified before publication.
- [x] Final 38-page Integration Guide published and hash-verified.
- [x] GitHub Release ID `372167195` published as stable, non-prerelease `v1.0.0`.

## P1 — Contract/docs — complete for v1
- [x] Frozen bilingual Contract v1 + regression checklist + SQL samples.
- [x] Final Sadr Scales 5.2.1 Integration Guide identity reconciled to the final 38-page release asset.
- [x] Bilingual Getting Started updated for SDK 1.x.
- [x] Bilingual troubleshooting matrix.
- [x] Vendor production-readiness/go-live checklist.
- [x] SDK API compatibility/versioning policy.
- [x] Root English/Persian developer landing README.
- [x] Public support policy.

## P1 — SDK foundation/hardening — complete for v1
- [x] Basic SDK + unit/package CI.
- [x] Real SQL Server 2022 integration tests.
- [x] Bounded safe connection/read retry.
- [x] Transaction-scoped writes remain non-replayed.
- [x] Atomic bounded PLU batch API, maximum 200 unique PLUs.
- [x] Batch tests: mixed counts, duplicate prevalidation, rollback and semantic rowversion no-op behavior.
- [x] Executable read-only-default C# Quick Start.
- [x] Source Link/repository package metadata.
- [x] NuGet package shape validation.
- [x] .NET package validation enabled.
- [x] Package validator checks MIT license plus both provider identities.
- [x] Strong-name policy decided: v1.0.0 remains unsigned unless a concrete supported-consumer requirement appears.

## P1 — .NET Framework compatibility — complete for v1
- [x] Real `net48` NuGet-package consumer smoke application.
- [x] Windows Server 2022 package-consumer build/runtime job.
- [x] Local generated-package restore succeeds.
- [x] net48 build/runtime succeeds with warnings-as-errors.
- [x] SqlClient dependency graph loads from restored package.

## P1 — Public repository operations — complete for v1
- [x] CODEOWNERS identifies the public repository owner/reviewer.
- [x] Weekly Dependabot policy for NuGet and GitHub Actions.
- [x] Hardened bug/feature Issue forms and disabled blank public Issues.
- [x] Contribution policy documents test/security/license expectations.
- [x] Security policy routes sensitive reports away from public Issues.
- [x] Public Repository Guard requires stable-release governance/security files.
- [x] Secret Scanning and Push Protection enabled.
- [x] Dependabot vulnerability alerts and security updates enabled.
- [x] Private Vulnerability Reporting enabled.
- [x] CodeQL default setup configured for C#.
- [x] Protected `main` requires validated CI/security checks.
- [x] Force pushes and branch deletion disabled for protected `main`.

## P1 — Release engineering — complete for v1
- [x] `1.0.0` package metadata prepared and published.
- [x] Automated Binaries ZIP + Developer Kit ZIP.
- [x] Developer Kit includes support/contribution/changelog/production-readiness material.
- [x] Automated release manifest + `SHA256SUMS.txt`.
- [x] Release manifest records providers + MIT license.
- [x] Final guide download + pinned SHA-256 verification in protected release workflow.
- [x] Tag/version match gate.
- [x] Tag release reruns SDK, SQL Server and net48 gates.
- [x] Tag workflow creates/updates a Draft GitHub Release only after all gates pass.
- [x] Normal PR CI smoke-builds the release bundle.
- [x] Final release assets independently downloaded and verified before publication.

## P2 — Developer reach
- [ ] Website developer landing page linking GitHub as source of truth.
- [ ] Evaluate NuGet.org publication after package/account ownership policy is decided.
- [ ] Python reference sample.
- [ ] Node.js reference sample.
- [ ] Java reference sample.
- [ ] PHP reference sample.
- [ ] Language-neutral SQL type/null mapping table.

## P2 — Post-v1 maintenance
- [ ] Evaluate GitHub Immutable Releases after the current Draft→verify→publish workflow is reviewed for compatibility with immutability.
- [ ] Review whether a second trusted maintainer should be added; if so, consider one required approval and CODEOWNERS review.
- [ ] Periodically review dependency/security alerts and update package/runtime compatibility evidence.
- [ ] Keep SQL Contract compatibility explicit for every future Sadr Scales release.

## Future
- [ ] Explicit advanced structured-invoice helpers evaluation.
- [ ] No-code connector evaluation.
- [ ] Separately versioned REST/Webhook Gateway.
