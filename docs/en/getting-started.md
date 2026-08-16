# Getting Started

> Status: Foundation draft. Executable SDK samples will be added during M2/M3.

## Architecture

Your POS/ERP application integrates with the SQL Server database used by Sadr Scales. Sadr Scales remains responsible for scale sessions, retries, durable communication and device-model differences.

```text
POS / ERP
    ↓
SQL Contract v1
    ↓
Sadr Scales
    ↓
Supported scales
```

## Basic public objects

- `dbo.SADR_ItemClass` — item groups, read/write.
- `dbo.SADR_Item` — PLU/item master, read/write.
- `dbo.SADR_Logs` — accepted sales feed, read-only.

## Minimum rules

- Use parameterized queries.
- `PluNo` must be unique and non-zero.
- Create the referenced item group before the item.
- Never write SQL `rowversion/TimeStamp` manually.
- Never update/delete `SADR_Logs` for cursor management.
- Persist the sales cursor in your own application state.
- Never commit real credentials or customer data.

See [SQL Contract v1](sql-contract-v1.md) and the [full Persian technical guide](../reference/README.md).
