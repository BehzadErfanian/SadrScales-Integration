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
- [x] Basic SDK + 8 unit tests + package CI.
- [x] Disposable SQL Server 2022 integration tests.
- [x] PR #4 merged; post-merge SDK/SQL/Public Guard green.

## P1 — Retry hardening
- [x] Add bounded retry options.
- [x] Add explicit transient classifier/backoff policy.
- [x] Retry safe connection-open boundary.
- [x] Retry complete read-only Contract/Sales operations.
- [x] Keep transactional write command execution non-retried.
- [x] Add retry unit tests including cancellation and non-transient behavior.
- [ ] Branch/PR CI green and merge.
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
