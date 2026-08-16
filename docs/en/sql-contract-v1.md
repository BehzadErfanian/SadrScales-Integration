# Sadr Scales SQL Integration Contract v1 — 5.2.1

**Status:** Basic public surface frozen — 2026-08-16

This is the concise public SQL contract for POS, ERP and accounting integrations with Sadr Scales 5.2.1. It was re-validated against the **effective database schema after Sadr Scales 5.2.1 runs its own schema creation/migration checks**. A legacy installer script or a customer database that has not yet been migrated is not the Contract v1 baseline.

The full Persian reference guide documents Registry, Mapping, Structured Sales and other controlled/advanced areas. The source-verification record is available in [CONTRACT_V1_FREEZE.md](../CONTRACT_V1_FREEZE.md).

## Basic public surface

### `dbo.SADR_ItemClass` — item groups

Public operations: **SELECT / INSERT / UPDATE**.

Contract columns:

- `ItemClassCode varchar(50)` — required primary key;
- `ItemClassName nvarchar(100)` — nullable;
- `Descriptions nvarchar(150)` — nullable.

The default group code `0` is maintained by Sadr Scales and should not be removed without a controlled migration.

### `dbo.SADR_Item` — PLU/item master

Public operations: **SELECT / INSERT / UPDATE**. For normal integration, `DeleteFlag` is preferred to physical deletion.

`PluNo` is the public Contract v1 item identity:

- `PluNo <> 0`;
- `PluNo` is unique;
- referenced `ItemClassCode` must already exist;
- never manually write `TimeStamp` / `rowversion`.

Basic item fields:

```text
ItemClassCode
PluNo
PluUnit
UnitPrice
PrintFormat
PluCost
BarFormat
BarFlags
ItemCode
IndexBarcode
Tare
ShelfDate
ShelfDatePrint
SaleDatePrint
SaleTimePrint
OnlyTare
TaxRate
PluName
Text1..Text7
DeleteFlag
```

Important details:

- `ID` and `IDitem` are legacy schema fields and are **not** Contract v1 identities.
- `TaxNo` and `SendFlag` exist in the current schema but the basic Contract v1 path does not require callers to write them; keep database defaults unless a separately documented controlled scenario says otherwise.
- `SADR_ItemSyncState` is internal and must not be written by external integrations.
- Avoid rewriting every PLU on every cycle when no effective value changed.

### `dbo.SADR_Logs` — accepted sales feed

Public operation: **SELECT only**.

Integration-relevant schema rules:

- `ID` is an `int IDENTITY` and the recommended consumer cursor;
- `DeviceNo` is `int` in the current migrated schema and identifies the scale;
- `(DeviceNo, FID, SubID)` is unique and is the recommended destination duplicate key;
- `FID`, `SubID` and `PLU` are non-zero.

Readable fields:

```text
ID
DeviceNo
Identify
DateTime
FID
SID
Salesman
SubID
TotalPrice
PLU
Class
Dept
Amount
Unit
LogType
Tax
Text1..Text4
UnitPrice
CoFID
PLUName
```

## Incremental sales read

```sql
SELECT TOP (@BatchSize)
    ID, DeviceNo, Identify, [DateTime], FID, SID,
    Salesman, SubID, TotalPrice, PLU, Class, Dept,
    Amount, Unit, LogType, Tax,
    Text1, Text2, Text3, Text4,
    UnitPrice, CoFID, PLUName
FROM dbo.SADR_Logs
WHERE ID > @LastProcessedId
ORDER BY ID ASC;
```

Consumer rules:

1. Persist `LastProcessedId` in destination-owned durable state.
2. Persist/commit the batch in the destination first, then advance the destination cursor.
3. Make import idempotent; `(DeviceNo, FID, SubID)` is the preferred stable duplicate key.
4. Never UPDATE or DELETE `SADR_Logs` for acknowledgement/cursor management.
5. Do not assume IDs are contiguous; Contract v1 only relies on `ID > cursor` ordering.

## Advanced / controlled areas

Scale Registry, Store, Mapping, hotkeys, `SADR_Total` and `SADR_Detail` are documented for controlled scenarios but are outside the basic public Contract v1 path. `LableStatus` and `ItemStatus` represent processing by the destination software for that installation; they are not the scale-receive cursor.

## General rules

- Use parameterized queries.
- Use transactions for related multi-table writes.
- Keep transient SQL retry bounded; never retry forever.
- Never commit real credentials or customer data to source, samples, logs or this repository.
- REST/Webhook is not a Sadr Scales 5.2.1 Contract v1 capability.

## Executable SQL samples

Synthetic SQL samples live in [`samples/SQL`](../../samples/SQL/README.md). Run `00-validate-contract.sql` first against a test database after Sadr Scales has completed its own schema migration/check.
