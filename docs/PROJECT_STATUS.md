# Project Status — SadrScales-Integration

**Last updated:** 2026-08-18  
**Phase:** M2 engineering complete — `v1.0.0` release candidate / administrative gates open  
**Target first stable release:** `v1.0.0`  
**Supported Sadr Scales baseline:** `5.2.1`  
**Public integration contract:** `SQL Contract v1`

## Current release-candidate branch

- Branch: `m2/v1-release-hardening`
- Draft PR: `#8 — M2: Prepare SadrScales Integration v1.0.0 release`
- Base: `main`

`main` remains the last merged public baseline until PR #8 passes final review and the pre-release administrative gates are closed.

## Completed engineering

- M0 public repository/security/governance foundation.
- M1 SQL Contract v1 audit/freeze and bilingual contract documentation.
- Basic C# SDK targeting `netstandard2.0`.
- Real SQL Server 2022 integration-test hardening.
- Bounded connection/read retry with transaction-scoped writes intentionally non-replayed.
- Real .NET Framework 4.8 NuGet-package consumer compatibility.
- Atomic bounded PLU batch API (maximum 200 unique PLUs per call).
- Executable read-only-default C# Quick Start.
- Bilingual Getting Started and troubleshooting path.
- Source Link/repository package metadata.
- NuGet package-shape validation and .NET package validation.
- SDK API/SemVer compatibility policy.
- Strong-name decision: v1.0.0 remains unsigned unless a concrete supported-consumer need is demonstrated.
- Automated Binaries ZIP / Developer Kit ZIP / release manifest / SHA-256 checksums.
- Protected tag release workflow with tag/version matching, final Guide SHA verification and Draft GitHub Release creation.
- Final Sadr Scales 5.2.1 Integration Guide identity reconciled to the final 38-page release asset.

## Latest release-engineering validation

SDK CI run `32106867152` exercised the new release-bundle path and passed all three jobs:

- SDK restore/build/tests: PASS;
- C# Quick Start restore/build: PASS;
- NuGet pack: PASS;
- NuGet package shape + Source Link/repository metadata validation: PASS;
- release-bundle smoke build + artifact upload: PASS;
- disposable SQL Server 2022 integration suite: PASS;
- real .NET Framework 4.8 package consumer restore/build/runtime: PASS.

Public Repository Guard run `32106867174`: PASS.

The release-bundle smoke gate proves that the candidate package can be transformed into the distributable package/symbol/binary/developer-kit/manifest/checksum set before any public tag is created.

## Stable-release gates still open

1. **Public software license** — company approval is required before adding `LICENSE` and package license metadata.
2. **GitHub owner/admin security settings** — review `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md`.
3. **Final PR #8 verification/merge** after the license decision and final CI.
4. **Tag `v1.0.0`** on the exact merged source.
5. **Protected Release verification** — all release jobs must pass, including download/hash verification of the final Integration Guide.
6. **Draft GitHub Release inspection** — verify package, symbols, Binaries ZIP, Developer Kit ZIP, Guide, manifest and SHA-256 checksums before publishing.

## Distribution model for v1.0.0

GitHub is the developer source of truth. Stable binaries/packages are attached to GitHub Releases rather than committed to `main`.

The first release supports local installation of the downloaded `.nupkg`. NuGet.org publication is intentionally a separate post-v1 decision because it introduces package-account/ownership/publication-policy administration rather than SDK functionality.

## Scope boundary

`v1.0.0` does not expose direct device protocols, packet captures, private firmware/vendor data, customer data, private keys or internal Sadr Scales runtime source.

Python/Node.js/Java/PHP wrappers, REST/Webhook Gateway, no-code connectors and advanced structured-invoice helpers remain post-v1/future work. Non-C# developers can use the language-neutral SQL Contract v1 and executable SQL samples now.

## Exact next step

Close the two administrative gates (public software license + GitHub host security review), then run final PR #8 CI/review, merge, tag `v1.0.0`, inspect the automatically created Draft Release and publish it.

## Handoff rule

A future session begins by reading `AGENTS.md`, this file, `DECISIONS.md`, `ROADMAP.md`, `BACKLOG.md`, `WORK_LOG.md`, `SDK_DESIGN_V1.md`, `API_COMPATIBILITY.md` and `SECURITY_BOUNDARY.md`.
