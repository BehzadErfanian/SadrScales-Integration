# GitHub Security Administration Checklist

This repository has source-controlled security/governance gates and a public release pipeline. Host-security controls and branch protection were verified by the repository owner on 2026-08-18 before/at the first stable public release.

## Host settings verified for public v1.0.0

- [x] Secret scanning is enabled for the public repository.
- [x] Push protection for detected secrets is enabled.
- [x] Dependabot vulnerability alerts are enabled.
- [x] Dependabot security updates are enabled and not paused.
- [x] Private vulnerability reporting is enabled.
- [x] Code scanning / CodeQL default setup is configured for C# with the default query suite and `remote_and_local` threat model.
- [x] `main` branch protection is configured using exact validated post-merge checks.
- [x] `main` requires the validated CI/security checks before normal contributor merges.
- [x] Force pushes and branch deletion are disabled for protected `main`.
- [x] Release tags are governed by policy: stable release tags are never moved/reused after publication.

### Host-security verification evidence — 2026-08-18

Repository-owner verification returned:

- Dependabot vulnerability alerts: HTTP 204 / enabled;
- Dependabot security updates: `enabled = True`, `paused = False`;
- Secret scanning: `status = enabled`;
- Secret scanning push protection: `status = enabled`;
- Private vulnerability reporting: `enabled = True`;
- CodeQL default setup: `state = configured`, `query_suite = default`, `threat_model = remote_and_local`, standard runner.

### `main` branch-protection verification

Required checks:

- `build-test-pack`
- `sql-integration-test`
- `net48-package-consumer`
- `validate-public-boundary`

Verified policy:

- strict/up-to-date required status checks: enabled;
- admin enforcement: enabled;
- conversation resolution: required;
- force pushes: disabled;
- branch deletion: disabled;
- required approving review count: 0 while the repository has one maintainer.

The release source commit protected by these controls is `a6bccc7c13a8afba29b6860869d2a942b1231803`.

## Single-maintainer branch-protection policy

For the current single-maintainer repository, branch protection does **not require an external approving reviewer**, because that can make routine maintenance impossible when no second maintainer exists.

The v1 policy is:

- require current validated CI/security status checks;
- require branch to be up to date before merge;
- require conversation resolution;
- disallow force pushes;
- disallow branch deletion;
- keep required pull-request approvals at zero until a second trusted maintainer/reviewer exists.

When additional maintainers are formally added, review this policy and consider requiring one approving review plus CODEOWNERS review for security/release-sensitive paths.

## Source-controlled controls — complete

- [x] Public/private security boundary documented.
- [x] Forbidden sensitive file extensions/names rejected by repository validator.
- [x] Public-boundary validator runs in GitHub Actions on push and pull request.
- [x] Direct device protocols, packet captures, private keys, customer data and private runtime source are explicitly out of scope.
- [x] No real credentials are permitted in examples or logs.
- [x] MIT `LICENSE` identifies Tozin Sadr and Behzad Erfanian.
- [x] NuGet validation requires MIT plus both provider identities.
- [x] `CODEOWNERS` assigns repository/security/release ownership to `@BehzadErfanian`.
- [x] Dependabot version-update configuration exists for NuGet and GitHub Actions.
- [x] Blank public Issues are disabled; bug/feature forms require sanitized information.
- [x] Security-sensitive reports are routed away from normal public Issues.
- [x] `SUPPORT.md`, `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md` and production-readiness guidance are present.
- [x] Release pipeline creates checksummed packages/bundles and keeps GitHub Release as Draft pending human inspection.
- [x] Public Repository Guard requires the stable-release governance/security files.

## Stable release verification

`v1.0.0` was published only after:

- exact `main` validation;
- protected CI checks;
- protected-tag release run `32112295891`;
- direct Draft asset download/size verification;
- `SHA256SUMS.txt` validation;
- release-manifest identity/version/commit/license/provider validation;
- NuGet package metadata validation;
- Binaries/Developer Kit structure validation;
- official Integration Guide SHA-256 validation.

## Post-v1 hardening

Evaluate GitHub **Immutable Releases** only after confirming that it remains compatible with the current Draft → human verification → publish process. Do not change release immutability or tag-governance behavior casually after a stable public release exists.

Source-controlled protections must never be weakened to compensate for a missing host toggle.
