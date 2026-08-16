# Work Log

This chronological engineering log preserves handoff context.

## 2026-08-16 — M0 / M1
- Created public repository/governance/security boundary and verified Public Repository Guard.
- Froze Contract v1 against effective Sadr Scales 5.2.1 schema.
- Added bilingual docs/SQL samples/regression checklist.
- Prepared/page-QA'd official 34-page Persian guide; SHA-256 `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.
- PR #1 merged as `5fdac401392a9709fcd68ba2846be7941f60a4a0`; PR #2 as `b953db28a0c26af9655ddbf21bb52da9735bc92b`.

## 2026-08-16 — M2 SDK foundation
- Selected `netstandard2.0` and `Microsoft.Data.SqlClient 7.0.2`.
- Implemented Contract validator, semantic item/group upserts and read-only sales batches.
- Added unit/package CI and SDK docs.
- PR #3 merged as `5fe058148a41385950e0800aff8f10e581668eeb`; post-merge CI/Public Guard PASS.

## 2026-08-16 — M2 SQL-backed hardening
- Added disposable SQL Server 2022 CI with synthetic Contract v1 schema/data.
- Initial SQL run was 4/5 only because mismatch test harness used `DROP INDEX` on a UNIQUE table constraint; corrected to table-constraint DDL.
- Second branch SQL run: 5/5 PASS.
- PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.
- Post-merge SDK CI `31970073088`: PASS for build/test/pack and SQL integration.
- Post-merge Public Repository Guard `31970073055`: PASS.

## 2026-08-16 — M2 bounded retry hardening

### Research/design
- Reviewed current Microsoft.Data.SqlClient retry documentation before implementation.
- Kept transactional write replay separate from connection/read retry to avoid commit ambiguity.
- Chose an explicit SDK-owned bounded policy compatible with the `netstandard2.0` library surface.

### Implemented — commit `4042efd6c50857622767224ea31e2c175241cd82`
- `TransientRetryCount`: default 2, range 0..5.
- `TransientRetryBaseDelayMilliseconds`: default 250 ms, range 1..5000.
- Bounded exponential delay capped at 5000 ms and cancellation-aware.
- Conservative explicit transient SQL error classifier.
- Connection opening retries only before any command/transaction begins.
- `ValidateAsync` and `Sales.ReadAfterAsync` replay the complete read-only operation on a fresh connection when the failure is recognized transient.
- Transaction-scoped `ItemGroups.UpsertAsync` / `Items.UpsertAsync` command execution remains non-retried.
- Added retry tests for success-after-retry, exhaustion, non-transient failure, cancellation, error classification and delay cap.

### Branch validation
- SDK CI `31970279834`: PASS.
- Build: 0 warnings / 0 errors.
- Unit tests: **17/17 PASS**.
- NuGet + symbol package: PASS.
- SQL Server 2022 integration suite: **5/5 PASS**.
- Public Repository Guard `31970279841`: PASS.

### Next
- Open bounded-retry PR, require both PR workflows, then merge/post-merge verify before starting .NET Framework 4.8 compatibility.
