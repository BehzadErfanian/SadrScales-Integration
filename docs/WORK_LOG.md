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
- Kept the editable Word guide and future PDF/checksum binaries out of `main`; approved guide binaries will be GitHub Release assets after M1 validation.

### Public push and guard

- Public M0 foundation commit: `857a1d5ff4da1c79ffff1885b62f088cec00225d`.
- GitHub Actions guard run #1 reached the validator but failed because the original `TrimStart('\\','/')` path normalization was Windows-specific under PowerShell Core/Linux. No forbidden public file was reported.
- Portability fix commit: `2e91334d6f7c6e63c6f51c037a8d33b8ed8efa0c`.
- Replaced path trimming with cross-platform regex normalization.
- GitHub Actions `Public repository guard` run #2 completed successfully on Ubuntu / PowerShell Core.

### Handoff

- M0 is closed.
- Current phase is M1: Contract v1 freeze and public documentation.
- Parallel repository-administration item remains: review/enable GitHub security features available to the public repository.
- The exact next work is maintained in `PROJECT_STATUS.md` and `ROADMAP.md`.
