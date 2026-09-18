# Project Status — SadrScales-Integration

**Last updated:** 2026-09-18  
**Current SDK line:** `1.1.x`  
**Current package candidate in certification branch:** `1.1.1-rc.1`  
**Current certified Sadr Scales baseline:** `5.2.1`  
**Next objective:** complete local executable certification of SQL Contract v1 / SDK against SadrScales `5.5`

## Canonical references

1. `docs/SADR_SCALES_5_5_CERTIFICATION_PLAN.md` — current execution plan for the 5.5 closure cycle.
2. `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md` — long-term integration architecture.
3. `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md` — original 5.2.1 surface audit.
4. `.github/maintainers/INTEGRATION_CONTRACT_DOMAIN_DESIGN_FA.md` — contract/domain design.
5. `docs/DECISIONS.md` — historical decisions.

Chat history is not the project source of truth.

## Decision for the SadrScales 5.5 closure cycle

The public contract is frozen. The immediate task is **certification, not feature expansion**.

- SQL Contract v1 remains the public database contract.
- C# SDK and language-independent SQL recipes remain the official developer integration surface.
- Python / Java / NodeJS / PHP samples remain planned and are not blockers for SadrScales 5.5.
- REST, Webhook, Command Mailbox and new service transports are deferred to Sadr Platform.
- SadrScales 5.5 is not considered supported until the explicit certification gates pass.

Source audit found no public API/Contract v1 change, so the active candidate is `1.1.1-rc.1` and the intended certified release remains `1.1.1`. Only executable evidence of a genuinely necessary additive API change would move it to `1.2.0`.

## Frozen public integration scope

The 5.5 certification must preserve and verify this existing surface:

```text
Connection/schema validation
Stores
Item Groups
Items / PLUs
Price History read
Static Scale read
Online / Offline status
Scale Group Assignment
Scale Item Mapping
Group HotKey Template
Item AutoSend resend request
supported HotKey AutoSend resend request
Sales Feed
Sales Query / Summary
Structured Invoice lookup
Invoice ACK
Daily / Scale / Item Reports
```

Structured invoice lookup remains non-destructive. Destination Save/Commit happens before explicit ACK, ACK is idempotent, and `AlreadyRead` must continue to return complete invoice data for recovery.

## Existing developer assets

The repository already contains:

- `SadrScales.Integration` C# SDK;
- NuGet packaging metadata;
- .NET Framework 4.8 package consumer verification;
- modern .NET QuickStart;
- SQL integration tests;
- package-only Vendor Acceptance application;
- WinForms Developer Sample;
- Raw SQL recipes;
- Persian and English documentation;
- guarded DemoLab tooling.

These assets are reused for 5.5 certification. They are not rewritten unless the compatibility audit finds a real defect.

**Local package pre-certification:** PASS — 2026-09-18 on SDK `1.1.1-rc.1` (23/23 unit tests, 30/30 SQL tests, NuGet, modern .NET, net48 package consumer, package-only Vendor Acceptance and WinForms Sample build all PASS).

## 5.5 certification gates

### Gate A — SadrScales 5.5 RC baseline
Record the exact runtime/database candidate that will be certified.

### Gate B — SQL Contract audit
Verify every SDK-used table/column/behavior against the 5.5 database.

### Gate C — package/runtime evidence
Run unit tests, SQL integration tests, package validation, net48 consumer, modern .NET consumer and package-only Vendor Acceptance against the final candidate.

### Gate D — real One-PC integration acceptance
For both PLUS and LSG, using the Developer Simulator rather than physical scales:

`PLU -> Sale -> SadrScales 5.5 -> SQL -> SDK -> Save/Commit -> ACK -> AlreadyRead`

### Gate E — final physical release acceptance
After A-D and release-package readiness, the owning SadrScales release must pass Final Physical Acceptance with real PLUS and LSG scales. Until that final gate passes, `1.1.1` remains the intended certification version and must not be published as part of a Final/Stable 5.5 baseline.

### Gate F — publication
Only after A-E pass, update the Compatibility Matrix to include 5.5 and publish the certified SDK package and docs.

Details and pass conditions are in `docs/SADR_SCALES_5_5_CERTIFICATION_PLAN.md`.

## Final developer bundle relationship

The SDK is one part of the final Legacy delivery bundle:

```text
SadrScales 5.5
        +
SadrScales.Integration certified package
        +
SadrScaleSimulator.Developer stable
        +
C# Sample / QuickStart
        +
One-PC Lab guide
```

The intended outcome is that a developer can execute the complete supported integration workflow without owning a physical PLUS or LSG scale.

The intended certified set is `SadrScales 5.5 <-> SadrScales.Integration 1.1.1 <-> SadrScaleSimulator.Developer 1.0.0`. It is a target baseline, not a current certification claim; it becomes the Certified Baseline only after the SadrScales Final Physical Acceptance passes.

## Security boundary

The public repository and vendor-facing tooling must not expose proprietary device protocols, raw packets/captures, reverse-engineering notes, private keys, customer production data, private firmware/vendor material or arbitrary raw protocol execution.

The Developer Simulator may reproduce approved observable behavior, but protocol/debug implementation remains outside the public Integration contract.

## Freeze after SadrScales 5.5

After the certified 5.5 bundle is released:

- this Legacy integration line accepts bug, security, compatibility and documentation fixes only;
- new platform capabilities move to Sadr Platform;
- SadrScales/Integration remains a supported Legacy reference and compatibility surface, not the architecture base for the next platform.

## Exact next step

Complete the One-PC SadrScales 5.5 + Developer Simulator acceptance, then wait for the owning SadrScales Final Physical Acceptance before publishing `1.1.1` as 5.5-certified. Do not expand the public contract unless executable evidence proves a change is necessary.
