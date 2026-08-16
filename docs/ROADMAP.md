# Roadmap

## M0 — Public repository foundation
- [x] Public repository, bilingual README, security boundary, continuity docs and Public Guard.

## M1 — Contract v1 / public documentation
- [x] Freeze Contract v1 against effective 5.2.1 schema.
- [x] Bilingual docs/SQL samples/regression checklist/official guide QA+SHA.
- [ ] Upload approved PDF/checksum as GitHub Release assets before v1.0.
- [ ] Complete owner/admin GitHub security checklist.

## M2 — C# Integration SDK v1

### Foundation — complete
- [x] `netstandard2.0`, `Microsoft.Data.SqlClient 7.0.2`.
- [x] Contract validator, semantic item/group upserts, read-only incremental sales.
- [x] Unit/package CI and docs.

### SQL-backed hardening — complete
- [x] Disposable SQL Server 2022 suite; 5/5 real-SQL tests.
- [x] PR #4 + post-merge gates green.

### Bounded retry — complete
- [x] Safe bounded connection/read retry; transactional writes non-replayed.
- [x] Retry branch unit 17/17 + SQL 5/5 + package/Public Guard green.
- [x] PR #5 merged as `77d3c6330e0741a2c2f92eaec62fb8f50c781702`.
- [x] Post-merge SDK CI `31970450480` PASS.
- [x] Post-merge Public Guard `31970450492` PASS.

### .NET Framework 4.8 package compatibility — in progress
- [x] Add real `net48` package consumer application.
- [x] Restore consumer from locally generated SDK NuGet package.
- [x] Build/run on Windows Server 2022 with .NET Framework 4.8.
- [x] Smoke public options/client/models and load SqlClient dependency.
- [ ] Branch CI green.
- [ ] PR CI/review/merge + post-merge verification.

### Next
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

## M5 — Packaging / Releases
- [x] SDK CI + Public Guard on main.
- [ ] NuGet/DLL packaging policy, Source Link, checksums and release procedure.
- [ ] `v1.0.0` GitHub Release.

## M6 — Website developer experience
- [ ] Replace old developer guide with concise landing page.
- [ ] Link GitHub as developer source of truth, official guide and latest SDK Release.

## Future
No-code/REST/Webhook are separately versioned future scope.
