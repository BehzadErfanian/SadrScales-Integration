# SadrScales.Integration — SadrScales 5.5 Certification Plan

Last updated: 2026-09-18

## Decision

The current public Integration contract remains frozen for the SadrScales 5.5 closure cycle.

- Public contract: SQL Contract v1
- Current SDK line: `1.1.x`
- Current SDK target: `netstandard2.0`
- C# SDK + language-independent SQL recipes remain the supported integration surface.
- Python / Java / NodeJS / PHP samples remain planned and are not a SadrScales 5.5 release blocker.
- No REST, Webhook, Command Mailbox or new transport is added during the 5.5 certification cycle.

SadrScales 5.5 must not be listed as supported merely because it is newer than 5.2.1. Compatibility is established only by executable evidence against the actual 5.5 Release Candidate.

## Version decision

After the compatibility audit:

- If SQL Contract v1 and the public SDK API work unchanged: release `1.1.1` and document it as certified for SadrScales 5.5.
- If an additive public API change is genuinely required: use `1.2.0`.
- A breaking public API or SQL Contract change is outside the 5.5 closure scope and must not be introduced merely to finish the release.

## Preliminary source audit — 2026-09-18

Source-level compatibility review completed before executable certification:

- SadrScales `DLLs/Scripts/DB.sql` is byte-identical across `release/5.2`, `release/5.2.2`, `release/5.3`, `release/5.4` and `release/5.5` (same Git blob SHA `5e494b5b3e14e7be7e32e29b96cdca14f99be028`).
- SadrScales 5.5 adds nullable `SADR_Scale.PlusDeviceId` plus internal indexes for stable PLUS identity and changes the legacy endpoint index to non-unique.
- The public SDK does not read `PlusDeviceId` and does not define `ScaleIP + Port` uniqueness as a public Contract v1 guarantee.
- No public SDK API change or SQL Contract v1 expansion is indicated by this source audit.

Result: **provisional version path remains 1.1.1**. Executable Gates B-D are still required before any 5.5 compatibility claim.

Local-only certification runner: `tools/Invoke-LocalCertification.ps1`. GitHub Actions are not required for this certification cycle.

## Certification sequence

### Gate A — 5.5 RC baseline

Dependency: SadrScales 5.5 Release Candidate is frozen enough for compatibility testing.

Record:
- exact SadrScales commit/tag candidate;
- database/schema baseline;
- SDK commit/package candidate;
- SQL Server/test environment.

Pass condition: no known release-blocking schema churn remains.

### Gate B — Contract audit

Audit every SDK-used table, column and behavior against the 5.5 database.

At minimum verify:
- Stores
- Item Groups
- Items / PLUs
- Price History
- registered scales and status
- Scale Assignments
- Item Mapping
- Group HotKeys
- Item/HotKey resend requests
- Sales Feed
- Sales Query / Summary
- Daily / Scale / Item reports
- Structured Invoice lookup
- explicit ACK
- `AlreadyRead` recovery

Pass condition: all Contract v1 assumptions are either unchanged or corrected without silently widening the public contract.

### Gate C — Automated/package verification

Run the release-level evidence once for the final candidate:

- SDK unit tests
- SQL integration tests
- NuGet/package validation
- .NET Framework 4.8 package consumer
- modern .NET consumer / QuickStart
- package-only Vendor Acceptance
- public repository/security guard

GitHub Actions should not run on every small documentation or maintenance commit. Use the release/certification gate when needed.

Pass condition: all supported consumer/runtime gates pass against the final package candidate.

## Local pre-certification evidence — 2026-09-18

Behzad executed `tools/Invoke-LocalCertification.ps1` on Windows against local SQL Server using SDK candidate `1.1.1-rc.1`.

Recorded PASS evidence:

- public repository boundary: PASS
- SDK unit tests: 23/23 PASS
- modern .NET QuickStart build: PASS
- NuGet package validation: PASS
- SQL integration tests: 30/30 PASS
- .NET Framework 4.8 package consumer load/runtime: PASS
- package-only Vendor Acceptance: PASS
- WinForms Developer Sample build: PASS
- candidate package/hash generation: PASS

Candidate package: `SadrScales.Integration.1.1.1-rc.1.nupkg`.

This closes the local automated/package pre-certification gate. SadrScales 5.5 compatibility is **not yet published/certified** until Gate D One-PC acceptance passes for both PLUS and LSG.

### Gate D — Real 5.5 end-to-end acceptance

Use SadrScales 5.5 + SQL + Developer Simulator and validate both PLUS and LSG.

For each scale family:

`PLU -> Sale -> SadrScales -> SQL -> SDK lookup -> destination Save/Commit -> ACK -> AlreadyRead`

Verify exact invoice header, details, amounts, prices, FID/identity and aggregate barcode.

Pass condition: both scale families complete the end-to-end flow with no physical scale required.

### Gate E — Compatibility publication

Only after Gates A-D pass:

- add SadrScales `5.5` to `docs/COMPATIBILITY.md`;
- update README / Getting Started wording;
- publish the certified package version;
- update release notes and DeveloperKit assets;
- record exact SadrScales and SDK baselines used for certification.

## 5.5 deliverables

The Integration part of the final developer bundle consists of:

- certified `SadrScales.Integration` NuGet package;
- binaries/symbols as approved by release policy;
- C# QuickStart;
- WinForms Developer Sample;
- SQL recipes for non-C# stacks;
- Persian and English integration guides;
- 5.5 Compatibility Matrix;
- One-PC Lab guide coordinated with SadrScales and Developer Simulator.

## Explicitly out of scope

- new device wire protocols;
- direct protocol access from the SDK;
- REST or WebSocket API;
- Webhooks;
- Command Mailbox;
- Android/Web integration clients;
- new language SDK implementations purely to close 5.5.

Those capabilities belong to Sadr Platform unless separately justified later.

## Freeze after certification

Once the certified 5.5 package is released, this Legacy integration line receives only bug, security, compatibility and documentation fixes needed to support the frozen SadrScales 5.5 product. New platform capabilities move to Sadr Platform.
