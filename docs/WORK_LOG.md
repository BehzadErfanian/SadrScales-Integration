# Work Log

This is a chronological engineering log. It complements `CHANGELOG.md`: the changelog is public/release-oriented, while this file records project progress and handoff context.

## 2026-08-16 — M0 foundation

### Agreed

- Build a dedicated repository named `SadrScales-Integration` for software vendors.
- The repository is intended to become public and serve as the single developer link shared with POS/ERP/accounting vendors.
- The repository must contain source, documentation, language samples, compatibility and GitHub Releases.
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

### GitHub repository created

- Created `BehzadErfanian/SadrScales-Integration` on GitHub as a public repository.
- Re-ran the local public-boundary review before first push.
- Removed the bootstrap MIT `LICENSE` because the license decision is still pending explicit company approval; publishing a license must not precede that decision.
- Prepared the reviewed M0 foundation for first public push.
- Aligned the repository with release policy by keeping binary guide/release assets out of the main branch. The official guide will be distributed as a GitHub Release asset after M1 validation.
