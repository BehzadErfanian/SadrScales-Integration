# Sadr Scales Integration Sample App

This is the executable WinForms reference application for the Vendor-Ready integration surface.

The application intentionally grows one tested capability at a time instead of creating many unrelated demo programs.

## Shared connection

Enter the Sadr Scales SQL connection string at the top of the form, or set `SADR_SCALES_CONNECTION_STRING` before launch.

The current application contains **Invoices**, **Scales**, **Catalog**, **Assignments / Mapping / HotKeys** and **Sales / Reports** areas that use the same connection.

## Invoices

1. Enter a 14-digit structured `TotalBarcode`.
2. Click **Lookup invoice**.
3. Inspect the header and detail grids.
4. `FoundUnread` means the invoice has not yet been explicitly ACKed.
5. `AlreadyRead` still displays the complete invoice so recovery/re-import remains possible.

ACK is never automatic. The manual sample requires the ACK write guard and confirmation. A real destination must Save/Commit first and ACK only afterward.

## Scales

**Refresh scales** reads registered Scale ID, IP, port, model, store, `Online / Offline / Unknown` status and AutoSend configuration.

Resend buttons require the write guard and confirmation. `Requested` means the SQL AutoSend state was reset; it does not mean the physical scale already received the data.

## Catalog

Catalog contains **Stores**, **Groups** and **Items** pages plus one shared write guard.

- Store identity is `StoreCode`.
- Groups support read/upsert.
- Items support active/all views, read/upsert and logical delete.
- Item edit reads the complete existing PLU before changing selected fields, so print/barcode/tare/text settings are not overwritten with defaults.
- Soft delete sets `DeleteFlag = 1`; it never physically removes the row.
- Price History is read-only.

## Assignments / Mapping / HotKeys

This area demonstrates three deliberately separate configuration concepts.

### Scale Assignments

- Load canonical item groups for one Scale ID.
- Enter the complete desired replacement as comma-separated group codes.
- A real change requests Item AutoSend re-evaluation.
- Repeating the same assignment returns `Unchanged` and does not create a fresh resend request.

### Scale Mapping

- Load one scale's complete PLU/ItemCode mapping.
- Edit the staging grid and atomically replace the full map.
- Optional `PageNo` + `KeyNo` must be supplied together and fit the scale HotKey layout.
- Duplicate PLUs, ItemCodes and HotKey positions are rejected.
- Copy validates the destination before replacement; an incompatible destination remains unchanged.
- A real change requests Item + HotKey AutoSend re-evaluation.

### Group HotKeys

- Load the user-managed template for an item group.
- Only positive-PLU user keys are shown.
- Zero/negative internal/system rows are hidden and preserved.
- A real replacement requests HotKey AutoSend re-evaluation only for scales assigned to that group.

All Replace/Copy operations require **Enable configuration writes** plus confirmation.

## Sales / Reports

This page is entirely read-only and demonstrates the difference between search/reporting and the destination-owned incremental feed.

### Filters

Optional filters include:

- inclusive Start and exclusive End date/time;
- exact Identify;
- Scale ID;
- PLU;
- FID;
- Query page number and page size.

The **Today**, **This week** and **Persian month** buttons demonstrate Sadr Scales 5.2.1 period semantics. The week starts on Saturday and Current Month follows the Persian calendar.

### Query page

**Query page** calls `client.Sales.QueryAsync(filter)` and shows newest rows first.

The Summary line displays totals for the complete filter, not only the visible page:

```text
RecordCount | InvoiceCount | TotalPrice | TotalWeight | TotalQuantity
```

`InvoiceCount` uses distinct `(DeviceNo, FID)`. Unit codes 0/1/3 contribute to weight; Unit 2 contributes to quantity.

This Query page does **not** change or replace the cursor used by `Sales.ReadAfterAsync`.

### Reports

The same filter can run:

- **Daily report**;
- **By scale**;
- **By item**.

The item report is capped at 5000 aggregate rows, matching 5.2.1. Report results are typed SDK models, not public `DataTable` contracts.

## Write-safety rule

The Sample App is a reference tool, not a production administration console. Every current write family has an explicit guard, and higher-impact operations also require confirmation.

The final Demo Data slice will add a stronger production-database guard before synthetic data generation is allowed.

## Run

Open/build on Windows with .NET Framework 4.8 support:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp/SadrScales.Integration.SampleApp.csproj
```

The public sample does not contain customer credentials, direct scale protocols, packet formats or private Runtime implementation.

## Planned growth

The remaining Vendor-Ready slice adds **seeded Demo Data + production-database guard + external-developer acceptance + 1.1.0 RC cleanup** to this same application/repository.
