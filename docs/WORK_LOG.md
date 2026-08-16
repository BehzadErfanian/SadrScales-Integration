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
- Branch CI clean: 8/8, 0 warnings/errors, clean `.nupkg` + `.snupkg`.
- PR #3 passed Public Repository Guard + SDK CI and squash-merged to `main` as `5fe058148a41385950e0800aff8f10e581668eeb`.
- Post-merge SDK CI run `31969619533` PASS.
- Post-merge Public Repository Guard run `31969619624` PASS.

## 2026-08-16 — M2 SQL-backed hardening

### Research/decision
- Reviewed Microsoft.Data.SqlClient retry documentation before adding retry.
- Built-in retry providers do not automatically retry commands running inside an open transaction.
- Write retry remains deferred until commit ambiguity is explicitly tested; no blanket retry is wrapped around transactional upserts.
- Selected official Microsoft SQL Server 2022 Linux container for disposable CI integration tests.

### Implemented on `m2/sql-integration-tests`
- New `SadrScales.Integration.SqlTests` project.
- Disposable per-run SQL database with synthetic schema matching the basic Contract v1 objects used by the SDK.
- Startup waits for SQL Server and uses no production/customer credentials.
- Tests cover Contract validation, semantic group/item writes, rowversion behavior, sales ID gaps/read-only behavior and schema-mismatch exception mapping.
- Extended SDK CI with a separate SQL Server 2022 service-container job.

### SQL integration CI cycle 1 — commit `8cfc5ac9d5b7915418671f249e927f760a46758d`
- Existing build/test/pack job: PASS.
- SQL integration project build: PASS — 0 warnings / 0 errors.
- SQL tests: 4/5 PASS.
- Passing real-SQL behaviors:
  - Contract validator success;
  - item-group Inserted / Unchanged / Updated;
  - PLU Inserted / Unchanged / Updated with unchanged rowversion on semantic no-op;
  - sales reader handles an intentional identity gap and does not mutate source rows.
- The only failure was in the **test harness**, not the SDK: the mismatch test attempted `DROP INDEX UX_SADR_Item_PluNo`, but the real schema creates that named object as a UNIQUE table constraint. SQL Server correctly rejected direct `DROP INDEX` because it enforces the UNIQUE constraint.
- Fixed the mismatch test to use `ALTER TABLE ... DROP CONSTRAINT` and restore the same UNIQUE constraint in `finally`.

### Next
- Re-run branch CI after the harness correction.
- Open PR only after all 5 SQL integration tests and existing SDK/package gates are green.
