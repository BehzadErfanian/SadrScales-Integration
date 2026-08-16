# Backlog

Priority meanings: **P0** blocks safe public release; **P1** needed for v1.0; **P2** useful after v1.0.

## P0 — Public release administration / security

- [ ] Confirm public software license with company before first SDK release.
- [ ] Complete `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md` in GitHub Settings.
- [ ] Upload approved Integration Guide PDF/checksum as GitHub Release assets before public v1.0 distribution.

## P1 — Contract / docs

- [x] Freeze SQL Contract v1 after 5.2.1 source/schema verification.
- [x] Publish bilingual Contract v1 / Quick Starts / freeze record / regression checklist.
- [x] Add executable synthetic SQL samples.
- [x] Produce/page-QA official guide PDF and record SHA-256.
- [ ] Add troubleshooting matrix for common SQL integration failures.

## P1 — SDK foundation

- [x] Select `netstandard2.0` initial target.
- [x] Select `Microsoft.Data.SqlClient 7.0.2`.
- [x] Define `SadrScalesClient` basic API and security/cursor boundaries.
- [x] Implement schema validator.
- [x] Implement item-group upsert.
- [x] Implement item/PLU upsert.
- [x] Implement incremental read-only `SADR_Logs` reader.
- [x] Add unit tests and SDK CI build/test/pack workflow.
- [ ] Get branch/PR SDK CI green.

## P1 — SDK hardening

- [ ] Add bounded transient-SQL retry policy.
- [ ] Add safe SQL integration tests.
- [ ] Add .NET Framework 4.8 consumer compatibility test.
- [ ] Add bounded item batch API.
- [ ] Add C# console sample.
- [ ] Add Source Link/package metadata.
- [ ] Decide strong-name policy.

## P1 — Release

- [ ] Final license approval.
- [ ] Package and checksums.
- [ ] Release notes template.
- [ ] `v1.0.0` GitHub Release.
- [ ] Update website developer landing page.

## P2 — Advanced / language examples

- [ ] Evaluate structured invoice helpers only as explicit advanced APIs.
- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.

## P2 / Future generation

- [ ] Evaluate configurable no-code connector.
- [ ] Design REST/Webhook Gateway as a separately versioned contract.
