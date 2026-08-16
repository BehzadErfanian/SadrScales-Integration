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
- [x] PR #5 merged as `77d3c6330e0741a2c2f92eaec62fb8f50c781702`.
- [x] Post-merge SDK CI/Public Guard green.

### .NET Framework 4.8 package compatibility — branch green
- [x] Real `net48` package consumer application.
- [x] Restore consumer from locally generated SDK NuGet package.
- [x] Build/run on Windows Server 2022 with .NET Framework 4.8.
- [x] Load SqlClient dependency from restored package graph.
- [x] Final branch gate `31970792734`: all three SDK CI jobs PASS.
- [x] net48 build: 0 warnings / 0 errors with warnings-as-errors.
- [x] Public Guard `31970792738`: PASS.
- [ ] PR CI/review/merge + post-merge verification.

### Next — batch + developer sample
- [ ] Atomic bounded PLU batch API.
- [ ] Batch unit + real-SQL rollback/count/rowversion tests.
- [ ] Executable C# Console Quick Start, read-only by default.
- [ ] Safe environment-variable configuration example.
- [ ] CI build/sample smoke validation.

### Later v1.0 hardening
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
