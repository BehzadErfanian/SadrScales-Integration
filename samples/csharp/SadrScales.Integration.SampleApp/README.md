# Sadr Scales Integration Sample App

This is the executable WinForms reference application for the Vendor-Ready integration surface.

The application intentionally grows one tested capability at a time instead of creating many unrelated demo programs.

## Shared connection

Enter the Sadr Scales SQL connection string at the top of the form, or set `SADR_SCALES_CONNECTION_STRING` before launch.

The current application contains **Invoices**, **Scales**, **Catalog** and **Assignments / Mapping / HotKeys** areas that use the same connection.

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

## Assignments / Mapping / HotKeys

This area demonstrates three different configuration concepts and deliberately keeps them separate.

### Scale Assignments

- Enter a Scale ID and load its canonical item groups.
- The desired replacement list is entered as comma-separated group codes.
- **Replace groups** is a complete atomic replacement, not an incremental add/remove operation.
- A real change requests Item AutoSend re-evaluation for the scale.
- Repeating the same assignment returns `Unchanged` and does not create a fresh resend request.

### Scale Mapping

- Load one scale's complete PLU/ItemCode mapping.
- Edit the staging grid, then use **Replace mapping** to atomically replace the complete map.
- `PageNo` and `KeyNo` are optional, but when used they must both be populated and fit the target scale's configured HotKey layout.
- Duplicate PLUs, ItemCodes and HotKey positions are rejected.
- **Copy mapping** validates the destination layout before replacing it; incompatible copies leave the destination untouched.
- A real mapping change requests both Item and HotKey AutoSend re-evaluation.

### Group HotKeys

- Enter an item-group code and load the user-managed HotKey template.
- The grid intentionally shows only positive-PLU user keys.
- Zero/negative internal/system rows are hidden and preserved during replacement.
- **Replace HotKeys** atomically replaces the user-managed template for the selected group.
- A real change requests HotKey AutoSend re-evaluation only for scales canonically assigned to that group.

### Configuration write safety

All Replace/Copy operations remain disabled until **Enable configuration writes** is selected, and each write requires an explicit confirmation.

`Replaced` means the SQL configuration committed successfully. It does **not** mean a physical scale already received the new configuration. Actual transfer is performed later by an eligible Sadr Scales AutoSend cycle.

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

The next slice adds **Sales Query + Reports**. The final Vendor-Ready slice adds seeded Demo Data, a production-database guard, the external-developer acceptance flow and `1.1.0` RC cleanup to this same application.
