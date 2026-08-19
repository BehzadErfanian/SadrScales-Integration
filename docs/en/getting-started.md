# Getting Started — Sadr Scales Integration

This is the shortest supported path for software vendors integrating with **Sadr Scales 5.2.1**.

> Stable public release is currently `v1.0.0`. The additive Vendor-Ready `1.1.0` line is being frozen and tested before the next vendor outreach.

## 1. Understand the boundary

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration or documented SQL
        ↓
Sadr Scales database/runtime
        ↓
Supported scales
```

Your software does not implement PLUS/LSG/Aclas wire protocols. Sadr Scales keeps ownership of device communication.

## 2. Prerequisites

- Sadr Scales `5.2.1` or a later version explicitly documented as compatible.
- SQL Server access to the database used by Sadr Scales.
- For C#: .NET Framework 4.8 or modern .NET capable of consuming `netstandard2.0`.

Run Sadr Scales once first so its schema check/migration completes.

## 3. Validate before doing anything else

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

Non-C# stacks can use the read-only [`00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql).

A mismatch is a stop condition. Do not bypass validation.

## 4. See what is available

Read the [Vendor-Ready Capabilities](capabilities.md) page.

The current approved 5.2.1 surface includes:

- Stores, Item Groups and Items/PLUs;
- registered Scales and Online/Offline status;
- Scale Assignments, per-scale Mapping and group HotKeys;
- Item/HotKey AutoSend resend requests;
- incremental Sales Feed;
- filtered Sales Query and typed Reports;
- structured Invoice lookup and explicit ACK.

## 5. Run the executable Developer Sample

The main reference application is:

[`samples/csharp/SadrScales.Integration.SampleApp`](../../samples/csharp/SadrScales.Integration.SampleApp/README.md)

It contains visible flows for the approved capabilities plus guarded Demo Data.

Set the connection string without committing credentials:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

The smaller read-only Quick Start remains available under [`samples/csharp/SadrScales.Integration.QuickStart`](../../samples/csharp/SadrScales.Integration.QuickStart/README.md).

## 6. C# package path

For a release package downloaded to a local folder:

```bash
dotnet add package SadrScales.Integration --version <release-version> --source <download-folder>
```

Typical entry point:

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();

await client.Stores.UpsertAsync(store);
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

## 7. Invoice rule that must not be broken

```text
Read invoice
→ Save in destination
→ Commit destination transaction
→ ACK source invoice
```

Lookup never ACKs automatically. An ACKed invoice remains fully readable with `AlreadyRead` so recovery/re-import is possible.

## 8. Sales Feed rule that must not be broken

`Sales.ReadAfterAsync` is a destination-owned synchronization feed.

1. read rows after your stored cursor;
2. persist destination rows;
3. commit destination transaction;
4. only then persist the new cursor.

Use `(DeviceNo, FID, SubID)` as the preferred duplicate-protection key. `Sales.QueryAsync` is a separate search/report API and does not replace the Feed cursor.

## 9. Non-C# / Raw SQL path

Use the documented recipes under [`samples/SQL`](../../samples/SQL/README.md). They cover the same approved 5.2.1 SQL capabilities without requiring C#.

Do not invent writes against internal tables/columns that are not part of those recipes.

## 10. Demo safety

Demo Data is intentionally isolated from the production SDK contract. The Sample requires a clearly non-production database, required schema, empty business data, explicit database-name confirmation and a Demo marker before generation/reset.

Never initialize the Demo marker on a customer/production database.

## 11. Reference only when needed

- [Capabilities](capabilities.md)
- [Catalog](catalog.md)
- [Scales / Status / Resend](scales-status-resend.md)
- [Assignments / Mapping / HotKeys](assignments-mapping-hotkeys.md)
- [Structured Invoice + ACK](structured-invoices.md)
- [Sales Query + Reports](sales-query-reports.md)
- [Raw SQL recipes](../../samples/SQL/README.md)
- [Troubleshooting](troubleshooting.md)
- [Security](../../SECURITY.md)

Direct device protocols, raw packets, private keys and arbitrary Runtime commands are intentionally outside this repository.
