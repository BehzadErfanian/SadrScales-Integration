# Backlog

**P0** blocks public `v1.0.0`; **P1** is engineering required for v1; **P2** is post-v1 expansion.

## P0 — Release administration/security
- [x] MIT public software license approved; `LICENSE` and NuGet package metadata identify Tozin Sadr and Behzad Erfanian.
- [ ] Complete/review `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md` owner/admin host settings.
- [ ] Publish the final verified Integration Guide/checksum as GitHub Release assets. Release automation is implemented; actual asset publication waits for the stable tag.

## P1 — Contract/docs
- [x] Frozen bilingual Contract v1 + regression checklist + SQL samples.
- [x] Final Sadr Scales 5.2.1 Integration Guide identity reconciled to the final 38-page release asset.
- [x] Bilingual Getting Started updated for SDK 1.x.
- [x] Bilingual troubleshooting matrix.
- [x] Vendor production-readiness/go-live checklist.
- [x] SDK API compatibility/versioning policy.
- [x] Root English/Persian developer landing README.
- [x] Public support policy.

## P1 — SDK foundation/hardening
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

## P1 — .NET Framework compatibility
- [x] Real `net48` NuGet-package consumer smoke application.
- [x] Windows Server 2022 package-consumer build/runtime job.
- [x] Local generated-package restore succeeds.
- [x] net48 build/runtime succeeds with warnings-as-errors.
- [x] SqlClient dependency graph loads from restored package.

## P1 — Public repository operations
- [x] CODEOWNERS identifies the public repository owner/reviewer.
- [x] Weekly Dependabot policy for NuGet and GitHub Actions.
- [x] Hardened bug/feature Issue forms and disabled blank public Issues.
- [x] Contribution policy documents test/security/license expectations.
- [x] Security policy routes sensitive reports away from public Issues.
- [x] Public Repository Guard requires stable-release governance/security files.

## P1 — Release engineering
- [x] `1.0.0` package metadata prepared.
- [x] Automated Binaries ZIP + Developer Kit ZIP.
- [x] Developer Kit includes support/contribution/changelog/production-readiness material.
- [x] Automated release manifest + `SHA256SUMS.txt`.
- [x] Release manifest records providers + MIT license.
- [x] Final guide download + pinned SHA-256 verification in protected release workflow.
- [x] Tag/version match gate.
- [x] Tag release reruns SDK, SQL Server and net48 gates.
- [x] Tag workflow creates/updates a **Draft** GitHub Release only after all gates pass.
- [x] Normal PR CI smoke-builds the release bundle.
- [ ] Merge release-hardening PR after final CI/review.
- [ ] Create stable `v1.0.0` tag after P0 gates are closed.
- [ ] Verify Draft Release assets/checksums and publish it.

## P2 — Developer reach
- [ ] Website developer landing page linking GitHub as source of truth.
- [ ] Evaluate NuGet.org publication after package/account ownership policy is decided.
- [ ] Python reference sample.
- [ ] Node.js reference sample.
- [ ] Java reference sample.
- [ ] PHP reference sample.
- [ ] Language-neutral SQL type/null mapping table.

## Future
- [ ] Explicit advanced structured-invoice helpers evaluation.
- [ ] No-code connector evaluation.
- [ ] Separately versioned REST/Webhook Gateway.
