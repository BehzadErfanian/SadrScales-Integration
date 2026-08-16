# Getting Started — Sadr Scales Integration

This is the basic integration path for **Sadr Scales 5.2.1 / SQL Contract v1**.

## 1. Architecture

```text
POS / ERP / Accounting
        ↓
Sadr Scales SQL Contract v1
        ↓
Sadr Scales Runtime
        ↓
Supported scales
```

Your application works with the SQL Server database used by Sadr Scales. Sadr Scales remains responsible for device sessions, retries, Registry, model differences and direct scale communication.

## 2. Before integrating

1. Run Sadr Scales 5.2.1 so its own schema migration/check completes.
2. In a test environment, run [`samples/SQL/00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql).
3. Keep real credentials out of source and sample configuration.

## 3. Basic public objects

- `dbo.SADR_ItemClass` — item groups; SELECT/INSERT/UPDATE.
- `dbo.SADR_Item` — PLU/item master; SELECT/INSERT/UPDATE.
- `dbo.SADR_Logs` — accepted sales feed; SELECT only.

Registry, Mapping and Structured Sales are outside the basic path.

## 4. Items

- Create the referenced group before the item.
- `PluNo` must be unique and non-zero.
- `PluNo` is the Contract v1 item identity; do not use legacy `ID`/`IDitem` as integration identities.
- Never write `TimeStamp/rowversion`.
- Prefer `DeleteFlag` to physical deletion for normal integration.
- See the safe dry-run sample: [`01-upsert-item.sql`](../../samples/SQL/01-upsert-item.sql).

## 5. Sales

Basic cursor shape:

```sql
SELECT TOP (@BatchSize) *
FROM dbo.SADR_Logs
WHERE ID > @LastProcessedId
ORDER BY ID ASC;
```

Production code should select explicit columns; see [`02-read-sales-incremental.sql`](../../samples/SQL/02-read-sales-incremental.sql).

Consumer rules:

- keep the cursor in destination-owned durable state;
- commit destination data first, then advance the cursor;
- use `(DeviceNo, FID, SubID)` for destination duplicate protection;
- never update/delete `SADR_Logs` for cursor management;
- tolerate gaps in `ID` values.

## 6. Next documentation

- [SQL Contract v1](sql-contract-v1.md)
- [Contract Freeze Record](../CONTRACT_V1_FREEZE.md)
- [Regression Checklist](../CONTRACT_V1_REGRESSION_CHECKLIST.md)
- [Full Persian technical guide](../reference/README.md)
- [Security boundary](../SECURITY_BOUNDARY.md)

The C# SDK is planned for M2. Until it is released, this repository's Contract v1 and SQL samples are the public integration source of truth.
