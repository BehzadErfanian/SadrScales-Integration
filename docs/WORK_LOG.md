# Work Log

## 2026-08-16 — M0 / M1
- Public foundation/security/governance created.
- Contract v1 frozen against effective Sadr Scales 5.2.1 schema.
- Bilingual docs, SQL samples and regression checklist prepared.
- An initial 34-page Persian guide artifact was prepared during M1; this was later superseded by the final 5.2.1 release guide recorded below.

## 2026-08-16 — M2 SDK / SQL / retry hardening
- Basic `netstandard2.0` SDK built with `Microsoft.Data.SqlClient 7.0.2`.
- Real SQL Server 2022 suite established.
- Bounded/cancellable retry added only to safe connection/read boundaries.
- PR #3 merged as `5fe058148a41385950e0800aff8f10e581668eeb`.
- PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.
- PR #5 merged as `77d3c6330e0741a2c2f92eaec62fb8f50c781702`.

## 2026-08-16 — M2 .NET Framework 4.8 compatibility
- Generated NuGet package restored, built and executed by a real `net48` Console consumer on Windows Server 2022.
- Final net48 build: 0 warnings / 0 errors.
- Runtime loaded SDK and `Microsoft.Data.SqlClient` from the restored package dependency graph.
- PR #6 merged as `7af437c4394661b3c53321987c81477805049658`.
- Post-merge SDK CI: build/test/pack PASS; SQL Server integration PASS; net48 package consumer PASS.
- Post-merge Public Repository Guard: PASS.

## 2026-08-17 — M2 atomic batch + C# Quick Start
- Added bounded `UpsertBatchAsync`: maximum 200 PLUs, unique `PluNo` per call, complete validation before SQL access, one transaction, full rollback on any failure.
- Added aggregate batch result counts.
- Added unit and real-SQL coverage for invalid/duplicate/oversized batches, aggregate results, semantic no-op and rollback-after-partial-progress behavior.
- Added read-only-by-default C# Quick Start using `SADR_SCALES_CONNECTION_STRING`; no embedded credentials and no payload dump.
- PR #7 squash-merged as `39b0beac63c4e30974283a0306cedb330c433f6e`.
- Exact post-merge SDK CI run `31997736325`: PASS for build/test/pack, Quick Start, SQL Server integration and net48 package consumer.
- Exact post-merge Public Repository Guard run `31997736339`: PASS.
- Continuity commit brought `main` to `50a8c2694b044f7112f4824f3fd40e261a3dbb98` before v1 release hardening began.

## 2026-08-18 — M2 v1.0.0 release hardening
- Created `m2/v1-release-hardening` from exact `main` SHA `50a8c2694b044f7112f4824f3fd40e261a3dbb98`.
- Opened Draft PR #8: `M2: Prepare SadrScales Integration v1.0.0 release`.
- Prepared SDK package metadata for version `1.0.0`.
- Added CI-oriented deterministic/repository metadata and enabled .NET package validation.
- Added explicit NuGet package validation for DLL/XML/readme/repository/commit metadata.
- Updated net48 consumer to restore the `1.0.0` package.
- Added bilingual developer troubleshooting guides.
- Rewrote bilingual Getting Started docs for the real SDK 1.x/Quick Start/package path.
- Rewrote root English/Persian README files as developer onboarding pages.
- Added `API_COMPATIBILITY.md` with post-1.0 SemVer/API/Contract compatibility policy.
- Recorded D-022: `1.0.0` remains unsigned; strong-name signing is deferred unless a concrete supported-consumer requirement appears.
- Removed obsolete duplicate `samples/CSharp` placeholder; executable sample remains under `samples/csharp`.
- Reconciled the official Integration Guide to the final Sadr Scales 5.2.1 asset:
  - file `SadrScales_Integration_Database_Guide_5.2.1_FA.pdf`;
  - 38 pages;
  - SHA-256 `182be9aa73348a35a299ab0fad22e5e9deeba800ef9222c0145ba582b02e281b`;
  - source release commit `1048749f52faba35e69464b64983e772c1c857e3`.
- Added machine-readable guide identity so release automation can download and hash-verify the public PDF.
- Added `New-ReleaseBundle.ps1` producing package/symbol, Binaries ZIP, Developer Kit ZIP, release notes, manifest and `SHA256SUMS.txt`.
- Normal SDK CI smoke-builds the full release bundle and uploads release evidence.
- Added protected `v*.*.*` tag release workflow that reruns SDK/Quick Start, SQL Server 2022 and net48 package-consumer gates, verifies tag/version match, downloads/verifies the official Guide and creates/updates a Draft GitHub Release.
- Added MIT License with joint provider/copyright identity: Tozin Sadr and Behzad Erfanian.
- Added CODEOWNERS, Dependabot configuration, hardened Issue forms, support policy, contribution guidance and production-readiness checklist.
- Verified GitHub host security: Secret Scanning, Push Protection, Dependabot alerts/security updates, Private Vulnerability Reporting and C# CodeQL default setup.
- Final exact PR #8 SDK CI run `32111436949` / #98: PASS.
- Final exact PR #8 Public Repository Guard run `32111437048` / #169: PASS.
- PR #8 merged to `main` as `a6bccc7c13a8afba29b6860869d2a942b1231803`.

## 2026-08-18 — Stable v1.0.0 publication
- Exact merged `main` commit verified: `a6bccc7c13a8afba29b6860869d2a942b1231803`.
- SDK CI on exact `main`: run `32111583869` — PASS.
- Public Repository Guard on exact `main`: run `32112295906` — PASS.
- Required checks verified: `build-test-pack`, `sql-integration-test`, `net48-package-consumer`, `validate-public-boundary`.
- Configured protected `main` with strict required checks, admin enforcement and conversation resolution; force pushes/deletion disabled; zero external approvals while there is one maintainer.
- Stable tag `v1.0.0` verified on exact stable commit; tag is governed as immutable-by-policy and must never be moved/reused.
- Protected Release run `32112295891`: PASS.
- Protected Release job `draft-release`: PASS, including official Guide download/hash verification, final bundle/checksum creation, evidence upload and Draft Release creation.
- Release engineering artifact: `SadrScales-Integration-v1.0.0-1`, artifact ID `9315377547`.
- Draft GitHub Release found as Release ID `372167195`, `Draft=True`, `Prerelease=False` before publication.
- Verified exact eight primary release assets: NuGet package, symbols, Binaries ZIP, Developer Kit ZIP, final Guide, manifest, SHA256SUMS and release notes.
- Directly downloaded Draft assets; GitHub asset sizes matched.
- `SHA256SUMS.txt` verification passed.
- Release manifest verification passed for product/version/Contract/baseline/commit/MIT/both providers.
- NuGet metadata verification passed for package ID/version/MIT/Tozin Sadr/Behzad Erfanian.
- Binaries ZIP contained compiled SDK DLL and XML API documentation.
- Developer Kit contained license, support, contribution, changelog and production-readiness material.
- Official 38-page Integration Guide SHA-256 verified as `182be9aa73348a35a299ab0fad22e5e9deeba800ef9222c0145ba582b02e281b`.
- Draft Release published successfully as stable, non-prerelease `v1.0.0`.
- `v1.0.0` is now the official stable public SadrScales Integration release.

## 2026-08-18 — Post-release documentation/CI closure
- Created `docs/post-v1.0.0-release-closure` from the exact stable source commit without modifying the published tag.
- Updated English/Persian developer landing pages from release-candidate wording to stable `v1.0.0` status.
- Closed release status/backlog/roadmap/security-administration documentation with final evidence.
- Finalized the historical `1.0.0` changelog and opened a clean `Unreleased` section for post-v1 changes.
- Found a branch-protection governance edge case: required `SDK CI` job contexts could be absent on documentation-only PRs because the workflow used path filters.
- Removed the `SDK CI` path filters so required SDK/SQL/net48 checks run on every pull request and every push to `main`/`m2/**`, keeping Branch Protection deterministic.
- The post-release closure is intentionally outside tag `v1.0.0`; the stable release source remains `a6bccc7c13a8afba29b6860869d2a942b1231803`.
