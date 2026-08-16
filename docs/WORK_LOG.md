# Work Log

This is a chronological engineering log. It complements `CHANGELOG.md`: the changelog is public/release-oriented, while this file records project progress and handoff context.

## 2026-08-16 — M0 foundation

### Agreed

- Build a dedicated repository named `SadrScales-Integration` for software vendors.
- The repository is public and serves as the single developer link shared with POS/ERP/accounting vendors.
- Source, documentation, language samples, compatibility information and GitHub Releases belong in this project.
- SQL Contract v1 is the current public integration contract for Sadr Scales 5.2.1.
- The full Persian Integration & Database Guide is the detailed technical reference.
- Direct device communication protocols and captures remain private.
- Project state must be documented continuously so future chats/sessions can resume from repository files.

### Foundation work completed

- Created repository governance structure and continuity rules.
- Added Persian/English README and concise Contract v1 documentation.
- Added security policy and explicit public/private boundary.
- Added project status, roadmap, backlog, decision log, compatibility and release policy.
- Added planned sample folders for C#, SQL, Python, Node.js, Java and PHP.
- Added GitHub issue/PR templates.
- Added public-repository validation script and CI workflow.
- Created `BehzadErfanian/SadrScales-Integration` as a public GitHub repository.
- Reviewed the public bootstrap before push; no capture/key/database/customer-credential/private-runtime files were included.
- Removed the initially proposed MIT `LICENSE` before public publication because the software license requires explicit company approval.
- Kept editable/reference guide binaries out of `main`; approved guide binaries will be GitHub Release assets after M1 validation.

### Public push and guard

- Public M0 foundation commit: `857a1d5ff4da1c79ffff1885b62f088cec00225d`.
- GitHub Actions guard run #1 reached the validator but failed because the original path normalization was Windows-specific under PowerShell Core/Linux. No forbidden public file was reported.
- Portability fix commit: `2e91334d6f7c6e63c6f51c037a8d33b8ed8efa0c`.
- Replaced path trimming with cross-platform normalization.
- GitHub Actions `Public repository guard` run #2 completed successfully on Ubuntu / PowerShell Core.

### Handoff

- M0 is closed.
- Current phase is M1: Contract v1 freeze and public documentation.

## 2026-08-16 — M1 Contract v1 source audit and freeze

### Verification performed

- Re-validated the basic public contract against the effective Sadr Scales 5.2.1 schema after the application's schema migration/check path.
- Confirmed that legacy/raw installer schema is not sufficient as the public baseline because current 5.2.1 migration hardens older databases.
- Verified `SADR_ItemClass` public columns and default-group behavior.
- Verified `SADR_Item.PluNo` unique/non-zero contract and `ItemClassCode` relationship.
- Confirmed `SADR_Item.ID` and `IDitem` are not the Contract v1 identity.
- Confirmed SQL-managed `TimeStamp`/rowversion must not be caller-written.
- Confirmed `TaxNo` and `SendFlag` exist but are not required caller inputs for the basic item path.
- Verified `SADR_Logs.DeviceNo` is `int` in the current migrated schema.
- Verified unique sales-row key `(DeviceNo, FID, SubID)` and non-zero sale identifiers.
- Reconfirmed that Registry/Mapping/structured-sales/runtime-state objects remain advanced/controlled rather than basic SDK surface.

### Public artifacts produced

- Expanded Persian and English Contract v1 documents into source-verified frozen specifications.
- Updated both Quick Starts to use the frozen rules.
- Added `CONTRACT_V1_FREEZE.md`.
- Added `CONTRACT_V1_REGRESSION_CHECKLIST.md`.
- Added read-only schema validation SQL.
- Added synthetic transactional item-group/PLU upsert sample that rolls back by default.
- Added incremental read-only sales sample with destination cursor/idempotency guidance.
- Updated Roadmap, Backlog, Decisions and Project Status in the same change.

### Important consumer rule

Basic sales consumption is destination-owned: persist destination data first, then advance the destination cursor. Do not mutate `SADR_Logs` to manage the basic feed. IDs may contain gaps; `(DeviceNo, FID, SubID)` is the preferred duplicate key.

### Next

- Finish M1 by producing the official reviewed guide PDF + SHA-256 release assets.
- Review available GitHub repository security settings.
- Then begin M2 C# SDK framework/API design.
