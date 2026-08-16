# SQL Contract v1 Regression Checklist

Use this checklist before publishing an Integration SDK release against Sadr Scales 5.2.1.

## Schema baseline

- [ ] Sadr Scales has run its current database schema creation/migration path successfully.
- [ ] `dbo.SADR_ItemClass` exists with `ItemClassCode varchar(50)` as its key.
- [ ] `dbo.SADR_Item` exists.
- [ ] `dbo.SADR_Item.PluNo` is `int`, non-zero by constraint and unique by index/constraint.
- [ ] `dbo.SADR_Item.ItemClassCode` references `dbo.SADR_ItemClass.ItemClassCode`.
- [ ] `dbo.SADR_Item.TimeStamp` exists and remains SQL-managed `timestamp/rowversion`.
- [ ] `dbo.SADR_Logs` exists.
- [ ] `dbo.SADR_Logs.ID` is an identity integer primary cursor column.
- [ ] `dbo.SADR_Logs.DeviceNo` is `int` after migration.
- [ ] `(DeviceNo, FID, SubID)` is unique.
- [ ] `FID`, `SubID` and `PLU` are constrained to non-zero values.

## Basic write behavior

- [ ] Group upsert uses parameters and does not delete the default group.
- [ ] Item upsert rejects `PluNo = 0`.
- [ ] Item upsert requires the referenced group to exist.
- [ ] Item upsert never writes `TimeStamp`, `ID` or `IDitem`.
- [ ] Basic item upsert does not depend on explicitly writing `TaxNo` or `SendFlag`.
- [ ] Logical removal uses `DeleteFlag` in normal integration.
- [ ] Batch operations are bounded and transactional where multiple writes belong together.

## Sales read behavior

- [ ] Sales reader queries `ID > cursor` and orders by `ID ASC`.
- [ ] Cursor is persisted in destination-owned state, not in `SADR_Logs`.
- [ ] Destination import is duplicate-safe using `(DeviceNo, FID, SubID)` or an equivalent stable unique key.
- [ ] Cursor advances only after successful destination commit.
- [ ] Reader tolerates gaps in `ID` values.
- [ ] Integration never updates/deletes `SADR_Logs` to acknowledge the basic feed.

## Boundary

- [ ] Basic SDK does not silently write Registry/Mapping/runtime state.
- [ ] Structured sales helpers, if present, are explicitly marked advanced/controlled.
- [ ] No direct device protocol implementation or proprietary capture/vendor data is present in the public repository.
- [ ] No real credentials or customer data appear in tests, samples, docs or logs.

## Documentation consistency

- [ ] Persian and English Contract v1 documents describe the same public surface.
- [ ] Quick Starts point to Contract v1 and use the same rules.
- [ ] SQL samples pass `00-validate-contract.sql` against a 5.2.1 test database.
- [ ] Compatibility documentation still maps Contract v1 to Sadr Scales 5.2.1+ only where explicitly verified.
