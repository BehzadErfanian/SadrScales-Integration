# Backlog

Priority meanings: **P0** blocks safe public release; **P1** needed for v1.0; **P2** useful after v1.0.

## P0 — Public release administration / security
- [ ] Confirm public software license with company before first SDK release.
- [ ] Complete `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md` in GitHub Settings.
- [ ] Upload approved Integration Guide PDF/checksum as GitHub Release assets before public v1.0 distribution.

## P1 — Contract / docs
- [x] Freeze SQL Contract v1 and publish bilingual docs/regression checklist.
- [x] Add executable synthetic SQL samples.
- [x] Produce/page-QA official guide PDF and record SHA-256.
- [ ] Add troubleshooting matrix for common SQL integration failures.

## P1 — SDK foundation
- [x] `netstandard2.0` + `Microsoft.Data.SqlClient 7.0.2`.
- [x] Schema validator, item-group/PLU upsert and incremental sales reader.
- [x] Unit tests, clean package smoke build and SDK CI.
- [x] PR #3 merged; post-merge SDK CI + Public Guard green.

## P1 — SQL-backed SDK hardening
- [x] Add disposable SQL Server 2022 integration-test job.
- [x] Add synthetic Contract v1 test database fixture.
- [x] Add real DB tests for Contract validator, semantic upserts, rowversion behavior, sales gaps/read-only semantics and mismatch exception.
- [ ] Get branch/PR SQL integration CI green and merge.
- [ ] Add bounded transient retry for connection/read-safe operations.
- [ ] Add .NET Framework 4.8 consumer compatibility test.
- [ ] Add bounded item batch API.
- [ ] Add C# console sample.
- [ ] Add Source Link/package validation.
- [ ] Decide strong-name policy.

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
