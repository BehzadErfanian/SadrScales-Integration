# SQL integration samples

These samples use synthetic values only. They do not contain direct scale protocols, customer credentials or production data.

## Published SQL Contract v1 samples

1. [`00-validate-contract.sql`](00-validate-contract.sql) — read-only validation of the basic 5.2.1 migrated schema.
2. [`01-upsert-item.sql`](01-upsert-item.sql) — synthetic item-group + PLU upsert. It rolls back by default.
3. [`02-read-sales-incremental.sql`](02-read-sales-incremental.sql) — read-only incremental `SADR_Logs` consumer query.

These three samples belong to the already-published basic SQL Contract v1 surface.

## Vendor-Ready 5.2.1 samples under development

4. [`03-structured-invoice-lookup-ack.sql`](03-structured-invoice-lookup-ack.sql) — structured invoice lookup plus optional explicit invoice ACK.

The Vendor-Ready sample is additive and is being prepared for the release after `v1.0.0`. It does not change the historical v1.0.0 tag or its frozen Contract v1 behavior.

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

### `03-structured-invoice-lookup-ack.sql`

Default behavior is lookup-only:

- returns the structured `SADR_Total` header and current read state;
- returns all matching `SADR_Detail` rows;
- does **not** change `LableStatus` while `@Acknowledge = 0`.

After the destination application's own transaction commits successfully, set `@Acknowledge = 1` to perform the explicit idempotent ACK:

```text
Acknowledged
AlreadyAcknowledged
NotFound
```

An already acknowledged invoice remains readable in full; `AlreadyRead` is a warning, not a recovery block.

## Application-code rule

The local T-SQL variables in these files exist only so a developer can safely inspect/run a sample in SSMS. Real application code must use parameterized commands rather than building SQL through string concatenation.

## Security boundary

The public samples do not expose or write direct device protocol packets/opcodes, private keys, customer data or arbitrary Runtime commands.

The published basic Contract v1 remains frozen. Vendor-Ready additions are implemented and tested additively before the next stable release.
