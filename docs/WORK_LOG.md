# Work Log

This chronological engineering log preserves handoff context.

## 2026-08-16 — M0 foundation

- Created dedicated public integration repository, governance documents and security boundary.
- Added and verified cross-platform Public Repository Guard.
- Withheld software license pending explicit company approval.

## 2026-08-16 — M1 Contract v1 + reference

- Re-validated/froze the basic public Contract v1 against effective Sadr Scales 5.2.1 post-migration schema.
- Added bilingual specs/Quick Starts, regression checklist and executable synthetic SQL samples.
- PR #1 merged as `5fdac401392a9709fcd68ba2846be7941f60a4a0` after guard PASS.
- Generated/page-QA'd official 34-page Persian Integration Guide and recorded SHA-256 `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.
- Added GitHub host-security owner/admin checklist.
- PR #2 merged as `b953db28a0c26af9655ddbf21bb52da9735bc92b` after guard PASS.
- M1 engineering closed; release-asset/security UI/license administration remains pre-v1.0.

## 2026-08-16 — M2 SDK foundation

### Decisions and implementation

- Chose `netstandard2.0` initial library target and `Microsoft.Data.SqlClient 7.0.2`.
- Caller owns connection/security configuration; SDK does not silently weaken SQL security settings or log raw connection strings.
- Implemented `SadrScalesClient`, Contract validator, semantic group/item upserts and read-only incremental sales batches.
- Basic sales cursor remains destination-owned.
- Added first unit tests, SDK design doc and GitHub Actions restore/build/test/pack workflow.
- Rechecked public item/group SQL types and lengths against Sadr Scales source before CI.

### CI cycle 1 — commit `77587763bbda8c83516ed73253675b801dfb44a4`

- Public Repository Guard: PASS.
- Restore: PASS.
- Build: PASS — 0 warnings / 0 errors.
- Tests: PASS — 8/8.
- Pack: PASS — `.nupkg` and `.snupkg` created.
- NuGet package-quality message identified: missing internal package README.

### Package cleanup — commit `666ba48d381db73e7397f8be92ada02b7a3c153b`

- Added dedicated `PACKAGE_README.md` to the NuGet package.
- Public Repository Guard: PASS.
- Restore/build/test/pack: PASS.
- Tests: 8/8.
- Build: 0 warnings / 0 errors.
- NuGet package and symbol package created with clean pack output; previous missing-readme message is gone.

### Next

Open M2 foundation PR, require both CI workflows on PR, review diff, merge only when green, then verify `main` post-merge CI.
