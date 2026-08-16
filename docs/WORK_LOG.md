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
- Initial SQL run was 4/5 only because mismatch test harness used `DROP INDEX` on a UNIQUE table constraint; corrected to `ALTER TABLE DROP/ADD CONSTRAINT`.
- Second branch SQL run: 5/5 PASS.
- PR #4 merged as `676a78fa0d2c0826d823571fad8882bb5585a90f`.
- Post-merge SDK CI run `31970073088`: PASS for both build/test/pack and SQL integration jobs.
- Post-merge Public Repository Guard run `31970073055`: PASS.

## 2026-08-16 — M2 bounded retry hardening

### Research
- Reviewed current Microsoft.Data.SqlClient configurable retry documentation before implementation.
- SqlClient supports configurable retry on connections/commands, but its API surface/support differs by target; the public SDK remains `netstandard2.0`.
- Microsoft guidance stresses choosing retriable exceptions deliberately rather than blindly retrying every SQL failure.
- Transactional write replay remains unsafe without explicit commit-ambiguity semantics.

### Implemented on `m2/read-retry-policy`
- Added `TransientRetryCount` (default 2, range 0..5).
- Added `TransientRetryBaseDelayMilliseconds` (default 250 ms, range 1..5000).
- Added internal bounded exponential retry policy capped at 5000 ms per delay.
- Added explicit conservative transient SQL error-number classifier.
- Added cancellation-aware retry delays.
- Connection opening retries only before any command/transaction begins.
- `ValidateAsync` and `Sales.ReadAfterAsync` retry the complete read-only operation using a fresh connection.
- Transaction-scoped `ItemGroups.UpsertAsync` / `Items.UpsertAsync` command execution remains non-retried; only safe pre-operation connection establishment can retry.
- Added retry unit tests for success-after-retry, exhaustion, non-transient behavior, cancellation, error classification and bounded backoff.
- Existing SQL integration suite remains the regression gate for real SQL behavior.

### Next
- Run branch SDK CI + SQL integration + Public Guard.
- Fix any compile/runtime issue before PR.
