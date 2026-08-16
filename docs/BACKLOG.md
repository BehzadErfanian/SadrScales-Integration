# Backlog

**P0** blocks safe public release; **P1** needed for v1.0; **P2** after v1.0.

## P0 — Release administration/security
- [ ] Company-approved public software license.
- [ ] Complete `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md`.
- [ ] Upload official guide PDF/checksum as GitHub Release assets.

## P1 — Contract/docs
- [x] Frozen bilingual Contract v1 + regression checklist + SQL samples.
- [x] Official guide prepared/QA'd and SHA recorded.
- [ ] Troubleshooting matrix.

## P1 — SDK foundation/hardening
- [x] Basic SDK + unit/package CI.
- [x] Real SQL Server 2022 tests.
- [x] Bounded safe connection/read retry.
- [x] Retry merged; exact main CI/Public Guard green.

## P1 — .NET Framework compatibility
- [x] Real `net48` NuGet-package consumer smoke application.
- [x] Windows 2022 package-consumer build/runtime job.
- [x] Local-package restore succeeds.
- [x] net48 build succeeds with 0 warnings / 0 errors and warnings-as-errors.
- [x] net48 runtime executes and loads SDK + SqlClient dependency graph.
- [x] Branch SDK CI `31970792734` + Public Guard `31970792738` green.
- [ ] PR/merge/post-merge verification.

## P1 — Developer experience/package hardening
- [ ] Atomic bounded item batch API.
- [ ] Batch tests: mixed counts, duplicate prevalidation, rollback, semantic rowversion no-op.
- [ ] Executable read-only-default C# console sample.
- [ ] Source Link/package validation.
- [ ] Strong-name policy decision.

## P1 — Release
- [ ] Final license approval.
- [ ] Package/checksums/release notes.
- [ ] `v1.0.0` GitHub Release.
- [ ] Website developer landing page.

## P2 — Advanced/language samples
- [ ] Explicit advanced structured-invoice helpers evaluation.
- [ ] Python / Node.js / Java / PHP.

## Future
- [ ] No-code connector evaluation.
- [ ] Separately versioned REST/Webhook Gateway.
