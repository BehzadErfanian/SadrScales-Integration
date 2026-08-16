# SQL Contract v1 Freeze Record

**Date:** 2026-08-16  
**Sadr Scales baseline:** 5.2.1  
**Contract:** SQL Contract v1  
**Status:** Basic public surface frozen

## Purpose

This record defines the stable public SQL surface that Integration SDKs and samples may build against for Sadr Scales 5.2.1.

The contract was re-validated against the effective 5.2.1 database schema after Sadr Scales runs its own schema creation/migration checks. Raw legacy installer scripts or pre-migration customer databases are not the contract baseline.

## Frozen basic public surface

### `dbo.SADR_ItemClass`

Supported external operations: **SELECT / INSERT / UPDATE**.

Contract columns:

| Column | SQL type | Contract rule |
|---|---|---|
| `ItemClassCode` | `varchar(50)` | Primary key; required |
| `ItemClassName` | `nvarchar(100)` | Nullable |
| `Descriptions` | `nvarchar(150)` | Nullable |

The default group code `0` is maintained by Sadr Scales and should not be removed without a controlled migration.

### `dbo.SADR_Item`

Supported external operations: **SELECT / INSERT / UPDATE**. For normal integration, logical removal through `DeleteFlag` is preferred to physical deletion.

Public identity: `PluNo`.

Contract columns used by the basic v1 item path:

`ItemClassCode`, `PluNo`, `PluUnit`, `UnitPrice`, `PrintFormat`, `PluCost`, `BarFormat`, `BarFlags`, `ItemCode`, `IndexBarcode`, `Tare`, `ShelfDate`, `ShelfDatePrint`, `SaleDatePrint`, `SaleTimePrint`, `OnlyTare`, `TaxRate`, `PluName`, `Text1`..`Text7`, `DeleteFlag`.

Required rules:

- `PluNo` is non-zero and unique.
- `ItemClassCode` must reference an existing `SADR_ItemClass` row.
- `TimeStamp`/`rowversion` is owned by SQL Server and must never be written by an integration.
- `ID` and `IDitem` are legacy schema fields and are **not** Contract v1 identity fields.
- `TaxNo` and `SendFlag` exist in the current schema but are not required inputs for the basic Contract v1 path; leave their database defaults unless a separately documented controlled scenario requires otherwise.
- Avoid writing every PLU on every polling cycle when no effective data changed.
- `SADR_ItemSyncState` is internal runtime state and is never written by an external integration.

### `dbo.SADR_Logs`

Supported external operation: **SELECT only**.

Current Contract v1 schema uses:

- `ID int IDENTITY` as the recommended consumer cursor;
- `DeviceNo int` as the scale identifier;
- unique sale-row identity `(DeviceNo, FID, SubID)`;
- non-zero `FID`, `SubID` and `PLU`.

External consumers must:

- keep `LastProcessedId` in their own durable destination state;
- query with `ID > @LastProcessedId ORDER BY ID ASC`;
- advance the destination cursor only after destination persistence succeeds;
- make destination import idempotent, preferably with `(DeviceNo, FID, SubID)` as a duplicate key;
- never UPDATE or DELETE `SADR_Logs` for cursor management;
- never assume `ID` values are contiguous.

A physical scale deletion is an administrative operation and can cascade to dependent sales/mapping rows. This is another reason the destination system must own its durable integration state.

## Advanced / controlled surface

The following are documented for controlled scenarios but are **not** promoted into the basic Contract v1 SDK surface by this freeze:

- `SADR_Store`
- `SADR_Scale`
- `SADR_ScaleItemClass`
- `SADR_KeyAssignment`
- `SADR_ScaleItemMap`
- `SADR_Total`
- `SADR_Detail`
- runtime cursor/status fields
- internal synchronization tables/views

Structured sales status fields (`LableStatus`, `ItemStatus`) describe processing by the destination software for that Sadr Scales installation. They are not the scale-receive cursor.

## Compatibility rule

Compatible documentation clarifications and additional examples may be added within Contract v1. A change that breaks an existing Contract v1 integration requires a newly versioned public contract.

REST/Webhook and a no-code connector are not Sadr Scales 5.2.1 Contract v1 capabilities.
