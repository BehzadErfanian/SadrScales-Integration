# Backlog

Priority meanings: **P0** blocks safe public release; **P1** needed for v1.0; **P2** useful after v1.0.

## P0 — Public release administration / security

- [ ] Confirm public software license with company before first SDK release.
- [ ] Complete `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md` in GitHub Settings.
- [ ] Upload approved Integration Guide PDF/checksum as GitHub Release assets before public v1.0 distribution.

## P1 — Contract / docs

- [x] Freeze SQL Contract v1 after 5.2.1 source/schema verification.
- [x] Publish bilingual Contract/Quick Starts/freeze record/regression checklist.
- [x] Add executable synthetic SQL samples.
- [x] Produce/page-QA official guide PDF and record SHA-256.
- [ ] Add troubleshooting matrix for common SQL integration failures.

## P1 — SDK foundation

- [x] Select `netstandard2.0` and `Microsoft.Data.SqlClient 7.0.2`.
- [x] Define basic API/security/cursor boundaries.
- [x] Implement schema validator, item-group upsert, PLU upsert and incremental sales reader.
- [x] Add unit tests and SDK restore/build/test/pack CI.
- [x] Branch CI green: 8/8 tests, 0 build warnings/errors, clean package creation.
- [ ] PR CI/review and merge to `main`.
- [ ] Verify post-merge CI.

## P1 — SDK hardening

- [ ] Bounded transient-SQL retry policy.
- [ ] Safe SQL integration tests.
- [ ] .NET Framework 4.8 consumer compatibility test.
- [ ] Bounded item batch API.
- [ ] C# console sample.
- [ ] Source Link/package metadata.
- [ ] Strong-name policy decision.

## P1 — Release

- [ ] Final license approval.
- [ ] Package/checksums/release notes.
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
