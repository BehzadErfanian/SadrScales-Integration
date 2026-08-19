# SQL integration samples

These samples use synthetic/example values only. They do not contain direct scale protocols, customer credentials or production data.

## Published SQL Contract v1 samples

1. [`00-validate-contract.sql`](00-validate-contract.sql) — read-only validation of the basic 5.2.1 migrated schema.
2. [`01-upsert-item.sql`](01-upsert-item.sql) — synthetic item-group + PLU upsert. It rolls back by default.
3. [`02-read-sales-incremental.sql`](02-read-sales-incremental.sql) — read-only incremental `SADR_Logs` consumer query.

These three samples belong to the already-published basic SQL Contract v1 surface.

## Vendor-Ready 5.2.1 samples under development

4. [`03-structured-invoice-lookup-ack.sql`](03-structured-invoice-lookup-ack.sql) — structured invoice lookup plus optional explicit invoice ACK.
5. [`04-scale-status-resend.sql`](04-scale-status-resend.sql) — registered-scale/status read plus explicitly enabled Item/HotKey AutoSend resend requests.
6. [`05-catalog.sql`](05-catalog.sql) — stores, groups, active/all item semantics, logical delete and read-only price history.
7. [`06-assignments-mapping-hotkeys.sql`](06-assignments-mapping-hotkeys.sql) — scale-group assignments, per-scale item mapping and user-managed group HotKey templates.

The Vendor-Ready samples are additive and are being prepared for `1.1.0`. They do not change the historical `v1.0.0` tag or its frozen Contract v1 behavior.

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

### `04-scale-status-resend.sql`

Default behavior is read-only because `@ApplyResend = 0`:

- lists registered scales and maps persisted status to `Online / Offline / Unknown`;
- shows the selected scale;
- changes no resend state.

For an explicit Item resend request:

```text
@ApplyResend = 1
@RequestItems = 1
```

This resets `LastSendItem` to zero for the selected scale. For an explicit HotKey resend request, also select `@RequestHotKeys = 1`; the sample only accepts the model families whose Sadr Scales 5.2.1 automatic HotKey path supports this operation.

`Requested` means the SQL AutoSend watermark was reset. It does **not** mean the physical scale has already received the data. The actual transfer happens during a later eligible AutoSend cycle.

### `05-catalog.sql`

Default behavior is read-only because `@ApplyChanges = 0`:

- lists stores and item groups;
- lists active items with `DeleteFlag = 0`;
- reads one selected PLU even when logically deleted;
- reads recent price history newest first;
- performs no catalog write.

When writes are explicitly enabled, the sample demonstrates a Store upsert and an optional **logical** PLU delete. Logical delete sets `DeleteFlag = 1`; it never physically removes the PLU row.

The existing [`01-upsert-item.sql`](01-upsert-item.sql) remains the full Item Group + PLU upsert example. Price History remains read-only in the 1.1.0 Vendor-Ready contract; the Integration SDK does not invent a rule that every external price update must create a PriceLog row.

### `06-assignments-mapping-hotkeys.sql`

Default behavior is read-only because `@ApplyChanges = 0`:

- reads canonical item-group assignments for one scale;
- reads that scale's item mapping;
- reads positive-PLU user HotKeys for one group;
- changes no configuration.

With writes explicitly enabled, the sample demonstrates full transactional replacements. Individual write families have separate flags so a developer cannot accidentally replace all three areas at once.

Important semantics:

- Assignment replacement resets the selected scale's Item AutoSend state.
- Per-scale mapping replacement validates the scale HotKey layout and resets both Item and HotKey AutoSend state.
- Group HotKey replacement removes/replaces only positive-PLU user rows. Zero/negative internal/system rows are preserved.
- A group HotKey change resets HotKey AutoSend state only for scales canonically assigned to that group.

## Application-code rule

The local T-SQL variables in these files exist only so a developer can safely inspect/run a sample in SSMS. Real application code must use parameterized commands rather than building SQL through string concatenation.

## Security boundary

The public samples do not expose or write direct device protocol packets/opcodes, private keys, customer data or arbitrary Runtime commands.

The sanctioned Vendor-Ready writes are narrowly defined business operations such as invoice ACK, AutoSend resend requests, catalog upserts, logical item delete and validated configuration replacement; internal protocol/runtime state remains private.
