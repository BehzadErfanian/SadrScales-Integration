# Roadmap

This roadmap is the canonical plan.

## M0 — Public repository foundation
- [x] Public repository, bilingual README, security boundary, continuity docs and Public Repository Guard.

## M1 — Contract v1 freeze and public documentation
- [x] Freeze basic Contract v1 against effective 5.2.1 schema.
- [x] Bilingual docs, SQL samples, regression checklist and official guide QA/SHA.
- [ ] Upload approved guide PDF/checksum as GitHub Release assets before v1.0.
- [ ] Complete owner/admin GitHub host-security checklist.

**M1 engineering:** complete.

## M2 — C# Integration SDK v1

### Foundation — complete
- [x] `netstandard2.0` + `Microsoft.Data.SqlClient 7.0.2`.
- [x] Schema validator, semantic item/group upserts and read-only incremental sales.
- [x] Unit tests and restore/build/test/pack CI.
- [x] PR #3 merged and post-merge CI green.

### SQL-backed hardening — complete
- [x] Disposable SQL Server 2022 CI tests using synthetic Contract v1 data.
- [x] 5/5 real-SQL tests.
- [x] PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.
- [x] Post-merge SDK CI `31970073088` PASS.
- [x] Post-merge Public Guard `31970073055` PASS.

### Bounded transient retry — branch green
- [x] Separate safe connection/read retry from transactional write execution.
- [x] Add bounded/cancellable retry options and explicit transient classifier.
- [x] Retry connection open before commands/transactions begin.
- [x] Retry complete read-only Contract validation and sales reads on a fresh connection.
- [x] Keep item/group transaction-scoped command execution non-retried.
- [x] Add retry engine/classifier/options unit tests.
- [x] Branch SDK CI `31970279834` PASS — 17/17 unit tests + 5/5 SQL tests + pack.
- [x] Branch Public Guard `31970279841` PASS.
- [ ] PR CI/review/merge and post-merge verification.

### Next after retry merge
- [ ] .NET Framework 4.8 consumer compatibility build/test.
- [ ] Bounded item batch API.
- [ ] Executable C# Console Quick Start.
- [ ] Source Link/package validation.
- [ ] Strong-name decision before v1.0.

## M3 — Reference samples
- [ ] C# Console Quick Start/end-to-end example.
- [x] Raw SQL Contract v1 examples.
- [ ] Safe sample configuration/local synthetic DB instructions.

## M4 — Multi-language examples
- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] Language-neutral SQL type/null mapping table.

## M5 — Packaging and GitHub Releases
- [x] SDK restore/build/test/pack CI on `main`.
- [x] Public Repository Guard.
- [ ] NuGet/DLL packaging policy, Source Link, checksums and release procedure.
- [ ] `v1.0.0` GitHub Release.

## M6 — Sadr website developer experience
- [ ] Replace old developer guide with concise landing page.
- [ ] Link GitHub as developer source of truth and link guide/latest SDK Release.

## Future — Integration Gateway / no-code connector
Separately versioned future scope; not part of Sadr Scales 5.2.1 / SQL Contract v1.
