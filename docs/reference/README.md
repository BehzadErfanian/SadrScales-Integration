# Reference documents

The final Persian **Sadr Scales Integration & Database Guide** for Sadr Scales 5.2.1 / SQL Contract v1 is a release/reference asset rather than source clutter in this repository.

## Final release identity

The authoritative machine-readable identity is:

[`integration-guide-5.2.1.json`](integration-guide-5.2.1.json)

Current final asset:

- File: `SadrScales_Integration_Database_Guide_5.2.1_FA.pdf`
- Pages: `38`
- SHA-256: `182be9aa73348a35a299ab0fad22e5e9deeba800ef9222c0145ba582b02e281b`
- Software baseline: `Sadr Scales 5.2.1`
- SQL Contract: `v1`
- Source release commit: `1048749f52faba35e69464b64983e772c1c857e3`

This identity supersedes the earlier 34-page pre-release guide artifact recorded during M1. The final 5.2.1 documentation was regenerated and validated as part of the final Sadr Scales release pipeline.

## Binary publication policy

The editable Word file and official PDF are intentionally **not committed to `main`**.

For the Integration SDK release, release engineering downloads the public final PDF, verifies it against the pinned SHA-256 above, and only then includes it as a GitHub Release asset. A hash mismatch is a release blocker.

This keeps repository history lightweight while preserving an exact, auditable identity for the official guide.
