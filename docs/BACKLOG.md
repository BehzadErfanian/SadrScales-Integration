# Backlog

Priority meanings: **P0** blocks safe public release; **P1** needed for v1.0; **P2** useful after v1.0.

## P0 — Public release administration / security
- [ ] Confirm public software license with company before first SDK release.
- [ ] Complete `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md`.
- [ ] Upload approved Integration Guide PDF/checksum as GitHub Release assets before v1.0.

## P1 — Contract / docs
- [x] Frozen bilingual SQL Contract v1 + regression checklist + SQL samples.
- [x] Official guide PDF prepared/QA'd and SHA recorded.
- [ ] Add troubleshooting matrix for common SQL integration failures.

## P1 — SDK foundation and real-SQL hardening
- [x] Basic SDK + unit/package CI.
- [x] Disposable SQL Server 2022 integration tests.
- [x] PR #4 merged; post-merge SDK/SQL/Public Guard green.

## P1 — Retry hardening
- [x] Bounded retry options + explicit transient classifier/backoff.
- [x] Safe connection-open retry.
- [x] Complete read-only Contract/Sales retry on fresh connection.
- [x] Transactional write command execution remains non-retried.
- [x] Retry unit tests including cancellation and non-transient behavior.
- [x] Branch CI: 17/17 unit tests, 5/5 SQL tests, package PASS, Public Guard PASS.
- [ ] PR CI/review and merge.
- [ ] Post-merge verification.

## P1 — Compatibility / developer experience
- [ ] .NET Framework 4.8 consumer compatibility test.
- [ ] Bounded item batch API.
- [ ] C# console sample.
- [ ] Source Link/package validation.
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

## P2 / Future
- [ ] Evaluate configurable no-code connector.
- [ ] Separately versioned REST/Webhook Gateway.
