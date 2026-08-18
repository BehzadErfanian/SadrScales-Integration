# Getting Started — Sadr Scales Integration

This is the shortest supported path for **Sadr Scales 5.2.1 / SQL Contract v1 / SadrScales.Integration 1.x**.

## 1. Architecture

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration / SQL Contract v1
        ↓
Sadr Scales Runtime
        ↓
Supported scales
```

Your application integrates with Sadr Scales. Sadr Scales remains responsible for device sessions, retries, Registry, model differences and direct scale communication.

## 2. Prerequisites

- Sadr Scales 5.2.1 or a later release explicitly compatible with SQL Contract v1.
- Access to the SQL Server database used by that Sadr Scales installation.
- For the C# SDK: .NET Framework 4.8 or a modern .NET runtime capable of consuming `netstandard2.0`.

Run Sadr Scales once first so its own schema migration/check completes.

## 3. Validate the contract first

C#:

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

Or run the read-only SQL validator:

[`samples/SQL/00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql)

Do not continue against an unknown/mismatched schema.

## 4. Try the executable C# Quick Start

The repository contains a build-validated sample:

[`samples/csharp/SadrScales.Integration.QuickStart`](../../samples/csharp/SadrScales.Integration.QuickStart/README.md)

It reads the connection string only from `SADR_SCALES_CONNECTION_STRING`, validates Contract v1 and performs a read-only sales query by default.

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.QuickStart
```

Never commit a real connection string.

## 5. Use the release NuGet package

For a GitHub Release package downloaded to a local folder:

```bash
dotnet add package SadrScales.Integration --version 1.0.0 --source <download-folder>
```

Then:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);

SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

## 6. Items and PLUs

- Create the referenced group before the item.
- `PluNo` must be unique and non-zero.
- `PluNo` is the Contract v1 public item identity; do not use legacy `ID`/`IDitem` as integration identities.
- Never write `TimeStamp/rowversion`.
- `UpsertAsync` avoids unnecessary updates when semantic values are unchanged.
- `UpsertBatchAsync` accepts at most **200 unique PLUs** in one atomic transaction. Larger imports must be paged by the caller.
- Transaction-scoped writes are not automatically replayed after execution begins.

Raw SQL dry-run: [`samples/SQL/01-upsert-item.sql`](../../samples/SQL/01-upsert-item.sql).

## 7. Sales

The SDK reads accepted sales incrementally and never updates/deletes `SADR_Logs` for acknowledgement.

Consumer rules:

- keep the cursor in destination-owned durable state;
- persist destination data first, then advance the cursor;
- use `(DeviceNo, FID, SubID)` for destination duplicate protection;
- tolerate gaps in `ID` values.

Raw SQL sample: [`samples/SQL/02-read-sales-incremental.sql`](../../samples/SQL/02-read-sales-incremental.sql).

## 8. If something fails

Start with [Troubleshooting](troubleshooting.md).

Then read:

- [SQL Contract v1](sql-contract-v1.md)
- [SDK Design v1](../SDK_DESIGN_V1.md)
- [Compatibility](../COMPATIBILITY.md)
- [Security boundary](../SECURITY_BOUNDARY.md)
- [Full Persian technical guide](../reference/README.md)

Direct PLUS/LSG/Aclas wire protocols are intentionally outside this public integration surface.
