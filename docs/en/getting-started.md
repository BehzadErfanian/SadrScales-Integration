# Getting Started — Sadr Scales Integration

This is the shortest supported path for software vendors using the **SadrScales.Integration `v1.1.0`** toolkit.

> The frozen public SQL/SDK contract is based on Sadr Scales `5.2.1`. Sadr Scales `5.3` is the current application release and is in final post-release compatibility/vendor rehearsal with this unchanged contract.

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

Your software does not implement PLUS/LSG/Aclas wire protocols. Sadr Scales keeps ownership of direct device communication, session/reconnect behavior and model differences.

## 2. Prerequisites

- `SadrScales.Integration 1.1.0` package.
- Sadr Scales `5.2.1` contract-compatible installation; `5.3` is the current application compatibility target.
- SQL Server access to the database used by Sadr Scales.
- For C#: .NET Framework 4.8 or modern .NET capable of consuming `netstandard2.0`.

Run Sadr Scales once first so its own schema setup/check completes.

Use a test/lab database while learning or running Demo/acceptance flows. Never put production credentials in source code.

## 3. Validate before doing anything else

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

Non-C# stacks can use the read-only [`00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql).

A mismatch is a stop condition. Do not bypass validation or invent writes against undocumented internal tables/columns.

## 4. See the approved capabilities

Read [Vendor-Ready Capabilities](capabilities.md).

The frozen 1.1 surface includes:
- Stores, Item Groups and Items/PLUs;
- Price History read;
- registered Scales and coarse `Online / Offline / Unknown` status;
- Assignments, per-scale Mapping and group HotKeys;
- Item/HotKey resend requests;
- incremental Sales Feed;
- filtered Sales Query, Summary and typed Reports;
- structured Invoice lookup by TotalBarcode or ScaleID + FID;
- explicit idempotent ACK and full `AlreadyRead` recovery.

## 5. Use the released package

For a `v1.1.0` package downloaded to a local folder:

```bash
dotnet add package SadrScales.Integration --version 1.1.0 --source <download-folder>
```

Typical entry point:

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();

await client.Stores.UpsertAsync(store);
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

The repository's package-only Vendor Acceptance project intentionally has **no SDK ProjectReference** and is used to prove the released-package consumer path.

## 6. Run the executable Developer Sample

Main reference application:
[`samples/csharp/SadrScales.Integration.SampleApp`](../../samples/csharp/SadrScales.Integration.SampleApp/README.md)

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

The smaller read-only Quick Start remains under [`samples/csharp/SadrScales.Integration.QuickStart`](../../samples/csharp/SadrScales.Integration.QuickStart/README.md).

## 7. Invoice rule that must not be broken

```text
Read invoice
→ Save in destination
→ Commit destination transaction
→ ACK source invoice
```

Lookup never ACKs automatically. ACK is idempotent. An ACKed invoice remains fully readable with `AlreadyRead` so recovery/re-import remains possible.

## 8. Sales Feed rule that must not be broken

`Sales.ReadAfterAsync` is a destination-owned synchronization feed.

1. read rows after the stored cursor;
2. persist destination rows;
3. commit the destination transaction;
4. only then persist the new cursor.

Use `(DeviceNo, FID, SubID)` as the preferred duplicate-protection key. `Sales.QueryAsync` is a separate search/report API and does not replace the Feed cursor.

## 9. Non-C# / Raw SQL

Use the documented recipes under [`samples/SQL`](../../samples/SQL/README.md). They cover the approved SQL contract without requiring C#.

Do not invent new writes against internal Sadr tables/columns that are not part of the documented contract/recipes.

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

Future managed commands, Service/REST and Webhook/realtime capabilities belong to **Sadr Scales 5.4+** architecture and are not part of the frozen 1.1 SQL/SDK contract.
