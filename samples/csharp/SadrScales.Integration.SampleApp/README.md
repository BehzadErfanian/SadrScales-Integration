# Sadr Scales Integration Sample App

This is the executable WinForms reference application for the Vendor-Ready integration surface.

The application intentionally grows one tested capability at a time instead of creating many unrelated demo programs.

## Shared connection

Enter the Sadr Scales SQL connection string at the top of the form, or set `SADR_SCALES_CONNECTION_STRING` before launch.

The current application contains **Invoices**, **Scales** and **Catalog** areas that use the same connection.

## Invoices tab

1. Enter a 14-digit structured `TotalBarcode`.
2. Click **Lookup invoice**.
3. Inspect the header and detail grids.
4. `FoundUnread` means the invoice has not yet been explicitly ACKed.
5. `AlreadyRead` still displays the complete invoice so recovery/re-import remains possible.

### ACK safety

The sample never auto-ACKs a lookup.

To test ACK manually you must enable the ACK write and confirm the warning dialog. In a real POS/ERP integration, ACK must be called only after the destination database transaction has committed successfully.

## Scales tab

Click **Refresh scales** to read registered Scale ID, IP, port, model, store, `Online / Offline / Unknown` status and AutoSend configuration.

Resend buttons remain disabled until **Enable resend writes** is selected, and each request requires confirmation.

`Requested` means the SQL AutoSend watermark was reset. It does **not** mean the physical scale has already received the items/HotKeys.

## Catalog tab

Catalog has nested **Stores**, **Groups** and **Items** pages plus one shared **Enable catalog writes** guard. Reads remain available while writes are disabled.

### Stores

- Refresh the store list.
- Select a row to load its fields.
- Upsert by stable `StoreCode` only after enabling catalog writes.

### Groups

- Refresh item groups.
- Select/edit `ItemClassCode`, name and description.
- Upsert only after enabling catalog writes.

### Items

- Refresh active items, or explicitly select **Include deleted**.
- Select a PLU to inspect/edit Group, Name and Price.
- Upsert preserves all non-edited fields of an existing PLU by reading the complete item first; it does not overwrite print/barcode/tare/text settings with defaults.
- **Soft delete** sets `DeleteFlag = 1` after confirmation and never physically removes the row.
- A deleted PLU can still be inspected/recovered through the SDK.
- **Price history** is read-only and loads recent history for the selected PLU.

## Write-safety rule

The Sample App is a reference tool, not a production administration console. Every current write family has an explicit guard, and higher-impact operations also require confirmation.

Demo Data in a later Vendor-Ready slice will add a stronger production-database guard before synthetic data generation is allowed.

## Run

Open/build on Windows with .NET Framework 4.8 support:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp/SadrScales.Integration.SampleApp.csproj
```

The public sample does not contain customer credentials, direct scale protocols, packet formats or private Runtime implementation.

## Planned growth

The next slices add Scale Assignments/Mapping/HotKeys, then Sales/Reports, then seeded Demo Data and the final external-developer acceptance flow to this same application.
