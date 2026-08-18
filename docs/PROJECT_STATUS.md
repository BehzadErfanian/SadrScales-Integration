# Project Status — SadrScales-Integration

**Last updated:** 2026-08-18  
**Phase:** `v1.0.0` release candidate — engineering, developer-readiness and GitHub host-security gates complete; merge/publication pending  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Current release-candidate branch

- Branch: `m2/v1-release-hardening`
- PR: `#8 — M2: Prepare SadrScales Integration v1.0.0 release`
- Base: `main`

`main` remains the last merged public baseline until PR #8 is merged. The stable tag is not created before exact post-merge validation.

## Completed engineering and public-developer readiness

- M0 public repository/security/governance foundation.
- SQL Contract v1 audit/freeze and bilingual contract documentation.
- C# SDK targeting `netstandard2.0` with `Microsoft.Data.SqlClient 7.0.2`.
- Real SQL Server 2022 integration-test hardening.
- Bounded connection/read retry with transaction-scoped writes intentionally non-replayed.
- Real .NET Framework 4.8 NuGet-package consumer compatibility.
- Atomic bounded PLU batch API (maximum 200 unique PLUs per call).
- Executable read-only-default C# Quick Start.
- Bilingual Getting Started and troubleshooting paths.
- Vendor production-readiness/go-live checklist.
- Public support policy and sanitized Issue intake.
- CODEOWNERS and weekly Dependabot configuration for NuGet/GitHub Actions.
- Hardened contribution/security policies and public repository guard.
- Source Link/repository package metadata.
- NuGet package-shape validation and .NET package validation.
- SDK API/SemVer compatibility policy.
- Strong-name decision: v1.0.0 remains unsigned unless a concrete supported-consumer need is demonstrated.
- Automated Binaries ZIP / Developer Kit ZIP / release manifest / SHA-256 checksums.
- Developer Kit includes license, support, contribution, changelog and production-readiness material.
- Release manifest records MIT license and both providers.
- Protected tag release workflow with tag/version matching, final Guide SHA verification and Draft GitHub Release creation.
- Final Sadr Scales 5.2.1 Integration Guide identity reconciled to the final 38-page release asset.
- MIT License approved for the public repository/SDK.
- Public provider/copyright identity set to **Tozin Sadr and Behzad Erfanian** in `LICENSE`, NuGet metadata, package README, repository README and NOTICE.
- Package validation fails if either provider identity or the MIT license expression is missing.

## Final audited PR validation before security-gate closure docs

Validated source head:

`044f168ca6c9cfd157befa0c820e0cbd82e309e6`

SDK CI run `32110435350` / run #92: **PASS**

- SDK restore/build/tests: PASS;
- C# Quick Start restore/build: PASS;
- NuGet pack: PASS;
- package shape + MIT/provider + Source Link/repository metadata validation: PASS;
- release-bundle smoke build: PASS;
- release-bundle evidence upload: PASS;
- disposable SQL Server 2022 integration suite: PASS;
- real .NET Framework 4.8 package consumer restore/build/runtime: PASS.

Public Repository Guard run `32110435351` / run #157: **PASS**

- public-boundary validation: PASS;
- required governance/release/security files: PASS;
- sensitive-file boundary: PASS;
- MIT/provider/CODEOWNERS assertions: PASS.

The commits after this validated head only close/document the verified host-security gate; the final PR head must still pass the same CI/public-guard checks before merge.

## GitHub host-security gate — complete

Repository-owner verification on 2026-08-18 confirmed:

- Secret Scanning: enabled;
- Push Protection: enabled;
- Dependabot vulnerability alerts: enabled;
- Dependabot security updates: enabled and not paused;
- Private Vulnerability Reporting: enabled;
- CodeQL default setup: configured for C#, default query suite, `remote_and_local` threat model.

`main` branch protection is intentionally configured **after** PR #8 merges so the exact post-merge status-check identities can be required without creating a self-lockout in the current single-maintainer repository.

## Stable-release gates still open

1. **Final PR #8 CI/public-guard pass on the exact final head.**
2. **PR #8 merge** into `main`.
3. **Post-merge `main` validation** on the exact merged source.
4. **Configure `main` branch protection** using the validated checks; no external reviewer requirement while the repository has one maintainer.
5. **Tag `v1.0.0`** on the exact validated `main` commit.
6. **Protected Release verification** — SDK/SQL/net48/tag/Guide/release-bundle jobs must pass.
7. **Draft GitHub Release inspection** — verify package, symbols, Binaries ZIP, Developer Kit ZIP, Guide, manifest and SHA-256 checksums before publication.
8. **Publish GitHub Release** only after the Draft assets are verified.

## Distribution model for v1.0.0

GitHub is the developer source of truth. Stable binaries/packages are attached to GitHub Releases rather than committed to `main`.

The first release supports local installation of the downloaded `.nupkg`. NuGet.org publication remains a separate post-v1 decision because it introduces package-account/ownership/publication-policy administration rather than SDK functionality.

## Scope boundary

`v1.0.0` does not expose direct device protocols, packet captures, private firmware/vendor data, customer data, private keys or internal Sadr Scales runtime source.

Python/Node.js/Java/PHP wrappers, REST/Webhook Gateway, no-code connectors and advanced structured-invoice helpers remain post-v1/future work. Non-C# developers can use the language-neutral SQL Contract v1 and executable SQL samples now.

## Exact next step

Wait for the final PR #8 CI/public-guard run after this documentation closure, mark the PR ready, merge it, validate exact `main`, configure branch protection, tag `v1.0.0`, verify the protected Draft Release and publish it.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md`, `API_COMPATIBILITY.md`, `PRODUCTION_READINESS_CHECKLIST.md` and `SECURITY_BOUNDARY.md`.
