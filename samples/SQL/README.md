# SQL Contract v1 samples

These samples implement the **basic public Sadr Scales SQL Contract v1** with synthetic values only. They do not contain direct scale protocols, customer credentials or production data.

## Order

1. [`00-validate-contract.sql`](00-validate-contract.sql) — read-only validation of the basic 5.2.1 migrated schema.
2. [`01-upsert-item.sql`](01-upsert-item.sql) — synthetic item-group + PLU upsert. It rolls back by default.
3. [`02-read-sales-incremental.sql`](02-read-sales-incremental.sql) — read-only incremental `SADR_Logs` consumer query.

## Expected results

### `00-validate-contract.sql`

On a compatible Sadr Scales 5.2.1 test database after the application schema check/migration has run:

```text
PASS - Sadr Scales SQL Contract v1 basic schema is available.
```

A missing/mismatched public object throws a Contract v1 error instead of silently continuing.

### `01-upsert-item.sql`

Default behavior is a dry run:

- creates/updates synthetic group `DEMO` inside a transaction;
- creates/updates synthetic `PluNo = 900001`;
- selects the resulting row for inspection;
- rolls the transaction back when `@ApplyChanges = 0`.

Expected final message in default mode:

```text
ROLLBACK - dry-run complete; no data retained.
```

### `02-read-sales-incremental.sql`

Returns up to `@BatchSize` rows with `ID > @LastProcessedId`, ordered by `ID ASC`. It performs no writes.

The destination application must persist the rows first, then persist its own new cursor. Use `(DeviceNo, FID, SubID)` as the preferred stable duplicate key.

## Application-code rule

The local T-SQL variables in these files exist only so a developer can safely inspect/run a sample in SSMS. Real application code must use parameterized commands rather than building SQL through string concatenation.

## Scope

These samples intentionally do not write:

- Scale Registry;
- Mapping/hotkey tables;
- structured-sales status fields;
- runtime cursor/state tables;
- direct device protocols.

See [`docs/CONTRACT_V1_FREEZE.md`](../../docs/CONTRACT_V1_FREEZE.md) for the frozen surface.
