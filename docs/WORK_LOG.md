# Work Log

This is a chronological engineering log. It complements `CHANGELOG.md` and preserves handoff context.

## 2026-08-16 — M0 foundation

- Created dedicated public integration repository and governance structure.
- Established public/private security boundary and CI guard.
- Added bilingual README/contract baseline, roadmap, backlog, decision log and sample structure.
- Withheld software license pending explicit company approval.
- Verified cross-platform public repository guard on GitHub Actions.

## 2026-08-16 — M1 Contract v1 freeze

- Re-validated basic public contract against effective Sadr Scales 5.2.1 post-migration schema.
- Froze `SADR_ItemClass`, `SADR_Item` and read-only `SADR_Logs` basic surface.
- Added bilingual specs/Quick Starts, regression checklist and executable synthetic SQL samples.
- PR #1 passed public repository guard and squash-merged as `5fdac401392a9709fcd68ba2846be7941f60a4a0`.
- Post-merge guard passed.

## 2026-08-16 — M1 reference preparation

- Generated and page-by-page QA'd official 34-page Persian Integration & Database Guide PDF.
- Recorded SHA-256 `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.
- Added GitHub host-security owner/admin checklist because connected API cannot verify/toggle those admin settings.
- PR #2 passed public guard and squash-merged as `b953db28a0c26af9655ddbf21bb52da9735bc92b`.
- M1 engineering closed; release-asset/security UI/license administration remains pre-v1.0.

## 2026-08-16 — M2 SDK foundation implementation

### Decisions

- Initial library target: `netstandard2.0`, with .NET Framework 4.8 consumer compatibility required before v1.0.
- SQL provider: `Microsoft.Data.SqlClient 7.0.2`.
- Caller owns SQL connection-string/security configuration; SDK does not silently weaken encryption/trust settings and never logs the raw connection string.
- Basic sales API is read-only and does not own the destination cursor.

### Implemented on `m2/sdk-foundation`

- SDK-style `SadrScales.Integration` project.
- `SadrScalesClient` entry point and validated client options.
- Contract v1 schema validator with dedicated mismatch exception.
- Parameterized, transactional and semantic item-group upsert.
- Parameterized, transactional and semantic item/PLU upsert without legacy/internal fields.
- Read-only incremental sales batch ordered by `SADR_Logs.ID`.
- Input guards that fail before SQL access.
- First unit tests.
- SDK GitHub Actions workflow for restore/build/test/pack.
- `SDK_DESIGN_V1.md` describing API/security/error/cursor/packaging boundaries.

### Verification

- Rechecked `SADR_ItemClass` and public `SADR_Item` SQL types/lengths against Sadr Scales source.
- First SDK CI run on commit `77587763bbda8c83516ed73253675b801dfb44a4` completed restore/build/test/pack successfully.
- Build result: 0 warnings, 0 errors.
- Test result: 8/8 passed.
- NuGet `.nupkg` and `.snupkg` were created successfully.
- Public Repository Guard also passed.
- NuGet emitted one package-quality message because the package had no internal README; a dedicated package README was added immediately rather than leaving that quality issue for later.

### Next

- Re-run both CI gates after the package README cleanup.
- If clean, open M2 foundation PR and merge only after PR CI passes.
- Then continue with bounded retry, safe SQL integration tests and .NET Framework 4.8 consumer compatibility.
