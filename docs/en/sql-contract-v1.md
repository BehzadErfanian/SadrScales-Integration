# Sadr Scales SQL Integration Contract v1 — 5.2.1

This is the concise public SQL contract. The full Persian reference guide documents additional controlled/advanced schema areas.

## Basic public objects

### Item input

- `dbo.SADR_ItemClass` — read/insert/update.
- `dbo.SADR_Item` — read/insert/update; logical disable is preferred to physical deletion for normal integration.

### Sales output

- `dbo.SADR_Logs` — accepted sales feed; **read-only** for external integrations.

## Required rules

1. `PluNo <> 0`.
2. `PluNo` is unique.
3. Referenced `ItemClassCode` must exist first.
4. Never manually insert/update SQL `TimeStamp` / `rowversion` fields.
5. Use parameterized queries and transactions for multi-table writes.
6. Do not UPDATE or DELETE `SADR_Logs`.
7. Persist the consumer sales cursor in the destination application's own durable state; ascending `ID` is the basic cursor.
8. Do not update every PLU on every cycle without a real reason.
9. Scale Registry, internal mappings and runtime synchronization state are outside the basic public Contract v1.

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

Advance `LastProcessedId` only after the destination system successfully persists the batch.

## Compatibility

Contract v1 is documented for Sadr Scales 5.2.1. A future REST/Webhook interface will use a separately versioned public contract and is not a current 5.2.1 capability.
