# GitHub Security Administration Checklist

This repository already has a public-boundary CI guard that runs on pushes and pull requests. The items below are repository-host settings that must be reviewed by an owner/admin in GitHub Settings because they are outside the source tree.

## Recommended repository settings

- [ ] Secret scanning is enabled/available for the public repository.
- [ ] Push protection for detected secrets is enabled when available.
- [ ] Dependabot alerts are enabled when available.
- [ ] Dependabot security updates are enabled when appropriate.
- [ ] Private vulnerability reporting is enabled when appropriate for a public SDK project.
- [ ] A `main` branch ruleset/branch protection policy is reviewed before external contributors become active.
- [ ] Required status check includes `Public repository guard` before merge when branch rules are enabled.
- [ ] Force-push/deletion of protected release branches/tags is restricted when release governance is introduced.

## Source-controlled controls already present

- [x] Public/private security boundary documented.
- [x] Forbidden sensitive file extensions rejected by repository validator.
- [x] Public-boundary validator runs in GitHub Actions on push and pull request.
- [x] Device protocols, packet captures, private keys, customer data and private runtime source are explicitly out of scope.
- [x] No real credentials are permitted in examples or logs.

This administrative checklist is deliberately separate from SDK implementation. Pending host-setting toggles do not justify weakening or bypassing the source-controlled public repository guard.
