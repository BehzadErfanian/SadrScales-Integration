# Work Log

This is a chronological engineering log. It complements `CHANGELOG.md`: the changelog is public/release-oriented, while this file records project progress and handoff context.

## 2026-08-16 — M0 foundation

### Agreed

- Build a dedicated repository named `SadrScales-Integration` for software vendors.
- The repository is public and serves as the single developer link shared with POS/ERP/accounting vendors.
- The repository contains source, documentation, language samples, compatibility information and future GitHub Releases.
- SQL Contract v1 is the current public integration contract for Sadr Scales 5.2.1.
- The full Persian Integration & Database Guide is the detailed technical reference.
- Direct device communication protocols and captures remain private.
- Project state must be documented continuously so future chats/sessions can resume from repository files.

### Completed

- Created M0 repository structure.
- Added Persian/English README.
- Added security policy and explicit public/private boundary.
- Added project status, roadmap, backlog, decision log, compatibility and release policy.
- Added concise Persian/English Contract v1 docs.
- Added planned sample folders for C#, SQL, Python, Node.js, Java and PHP.
- Added pull-request/issue templates.
- Added public-repository validation script and CI workflow.
- Created `BehzadErfanian/SadrScales-Integration` on GitHub as a public repository.
- Reviewed the M0 files before first public push; no forbidden capture/key/database/credential material was found.
- Removed the bootstrap MIT `LICENSE` because the public license still requires explicit company approval.
- Kept binary guide/release assets out of the main branch; the official guide will be a GitHub Release asset after M1 validation.
- Pushed the reviewed M0 foundation to `main` (`857a1d5ff4da1c79ffff1885b62f088cec00225d`).

### CI follow-up

- First `Public repository guard` run reached the validator but failed because `TrimStart('\\','/')` is not portable to PowerShell Core on Linux (`'\\'` is a two-character string there).
- This was a validator portability defect, not a security-boundary violation.
- Replaced the path trim with a cross-platform regex normalization and queued a new guard run.

### Next exact action

Verify the corrected GitHub Actions guard passes, then mark M0 complete and begin M1 contract/documentation freeze.
