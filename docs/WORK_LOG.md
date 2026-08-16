# Work Log

This chronological engineering log preserves handoff context.

## 2026-08-16 — M0 foundation
- Created public repository/governance/security boundary and verified Public Repository Guard.
- Withheld software license pending explicit company approval.

## 2026-08-16 — M1 Contract v1 + reference
- Re-validated/froze basic Contract v1 against effective Sadr Scales 5.2.1 post-migration schema.
- Added bilingual specs/Quick Starts, regression checklist and synthetic SQL samples.
- PR #1 merged as `5fdac401392a9709fcd68ba2846be7941f60a4a0` after guard PASS.
- Generated/page-QA'd official 34-page Persian guide; SHA-256 `5a9e36cfe633d41ff8f9a6f0453299ad37edfd28562c76d2d0dc097e499f0258`.
- PR #2 merged as `b953db28a0c26af9655ddbf21bb52da9735bc92b` after guard PASS.

## 2026-08-16 — M2 SDK foundation
- Chose `netstandard2.0` and `Microsoft.Data.SqlClient 7.0.2`.
- Implemented `SadrScalesClient`, Contract validator, semantic group/item upserts and read-only incremental sales batches.
- Added 8 unit tests, SDK design doc and restore/build/test/pack CI.
- PR #3 passed Public Repository Guard + SDK CI and squash-merged to `main` as `5fe058148a41385950e0800aff8f10e581668eeb`.
- Post-merge SDK CI `31969619533` PASS; Public Guard `31969619624` PASS.

## 2026-08-16 — M2 SQL-backed hardening

### Research/decision
- Reviewed Microsoft.Data.SqlClient retry documentation before adding retry.
- Built-in retry providers do not automatically retry commands running inside an open transaction.
- Write retry remains deferred until commit ambiguity is explicitly tested.
- Selected official Microsoft SQL Server 2022 Linux container for disposable CI integration tests.

### Implemented
- `SadrScales.Integration.SqlTests` project with disposable synthetic Contract v1 database.
- CI SQL Server 2022 service job.
- Tests for Contract validation, semantic group/item writes, rowversion no-op behavior, sales ID gaps/read-only semantics and schema mismatch mapping.

### SQL integration CI cycle 1 — `8cfc5ac9d5b7915418671f249e927f760a46758d`
- Existing build/test/pack: PASS.
- SQL integration build: PASS, 0 warnings / 0 errors.
- SQL tests: 4/5 PASS.
- Four real-SQL SDK behaviors passed.
- Only failure was test-harness DDL: attempted `DROP INDEX` on a named UNIQUE table constraint.
- Corrected mismatch test to drop/restore the object via `ALTER TABLE ... DROP/ADD CONSTRAINT`.

### SQL integration CI cycle 2 — `19208d9ba4ac32a7174ed8d45c832a0086b7ee5e`
- SDK CI run `31969914826`: PASS.
- Existing unit/build/package job: PASS.
- SQL Server 2022 integration job: PASS.
- SQL integration tests: **5/5 PASS**.
- Public Repository Guard run `31969914926`: PASS.
- No SDK runtime defect was identified by the first real-SQL suite.

### Next
- Open SQL-backed hardening PR and require both workflows on PR before merge.
