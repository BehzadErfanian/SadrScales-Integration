# GitHub Security Administration Checklist

This repository has source-controlled security/governance gates and a public release pipeline. The items in the first section are GitHub **host settings** and must be verified on the repository itself by an owner/admin.

## Host settings required before public v1.0.0

- [ ] Secret scanning is enabled for the public repository.
- [ ] Push protection for detected secrets is enabled.
- [ ] Dependabot vulnerability alerts are enabled.
- [ ] Dependabot security updates are enabled.
- [ ] Private vulnerability reporting is enabled.
- [ ] Code scanning / CodeQL default setup is configured for C#.
- [ ] `main` branch protection is configured after PR #8 is merged and the final required checks are known on `main`.
- [ ] `main` requires the validated CI/security checks before normal contributor merges.
- [ ] Force pushes and branch deletion are disabled for protected `main`.
- [ ] Release tags are not moved/reused after a stable release is published.

## Single-maintainer branch-protection policy

For the current single-maintainer repository, branch protection should **not require an external approving reviewer**, because that can make routine maintenance impossible when no second maintainer exists.

The intended v1 policy is:

- require current CI/security status checks;
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

## Release hardening after v1.0.0 publication

After the first stable Release has been verified and published, evaluate GitHub **Immutable Releases**. Do not enable immutability before the Draft Release workflow and human asset verification are complete, because the current release process intentionally needs to create/update Draft assets before publication.

The host-setting results should be recorded in this file and `PROJECT_STATUS.md` once verified. Source-controlled protections must never be weakened to compensate for a missing host toggle.
