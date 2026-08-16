# Work Log

This is a chronological engineering log. It complements `CHANGELOG.md`: the changelog is public/release-oriented, while this file records project progress and handoff context.

## 2026-08-16 — M0 foundation

- Created dedicated public integration repository and governance structure.
- Established public/private security boundary and CI guard.
- Added bilingual README/contract baseline, roadmap, backlog, decision log and sample-language structure.
- Removed proposed MIT license before publication because company approval is still required.
- Public repository guard was made cross-platform and verified green on GitHub Actions.

## 2026-08-16 — M1 Contract v1 source audit and freeze

- Re-validated the basic public contract against the effective Sadr Scales 5.2.1 schema after application migration/check.
- Frozen basic surface: `SADR_ItemClass`, `SADR_Item`, read-only `SADR_Logs`.
- Confirmed `PluNo` public identity, legacy `ID/IDitem`, SQL-owned rowversion, and destination-owned sales cursor/idempotency rules.
- Kept Registry/Mapping/structured-sales/runtime-state advanced/controlled.
- Added Persian/English frozen specs, Quick Starts, regression checklist and executable synthetic SQL samples.
- M1 Contract Freeze PR #1 passed the public repository guard and was squash-merged to `main` as `5fdac401392a9709fcd68ba2846be7941f60a4a0`.
- Post-merge public repository guard also completed successfully.

## 2026-08-16 — M1 official reference preparation

- Selected the latest user-reviewed Integration & Database Guide as the publication source (SQL Contract v1, Sadr Scales 5.2.1, document revision 2.1).
- Generated the official 34-page PDF.
- Inspected all 34 rendered pages for clipping, overlap, broken glyphs, table/layout problems and page-flow defects.
- PDF structural preflight passed; file is openable, unencrypted, text-based and uses embedded fonts.
- Official PDF filename: `SadrScales_Integration_Database_Guide_5.2.1_FA.pdf`.
- Official PDF SHA-256: `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.
- Binary remains out of `main` by policy and is ready to become a GitHub Release asset.
- Repository-host security endpoints available through the connected GitHub integration did not expose the required admin settings, so an explicit owner/admin UI checklist was added rather than guessing their state.
- M1 engineering work is complete; release-asset upload, host-security toggles and license approval remain pre-v1.0 administrative gates.

## Handoff

Current phase is M2: C# SDK foundation. Read `PROJECT_STATUS.md` and `ROADMAP.md` for the exact next work.
