# Sadr Scales Integration Sample App

This is the executable WinForms reference application for the Vendor-Ready integration surface.

The application intentionally grows one tested capability at a time instead of creating many unrelated demo programs.

## Shared connection

Enter the Sadr Scales SQL connection string at the top of the form, or set `SADR_SCALES_CONNECTION_STRING` before launch.

The current application contains separate **Invoices** and **Scales** tabs that use the same connection.

## Invoices tab

1. Enter a 14-digit structured `TotalBarcode`.
2. Click **Lookup invoice**.
3. Inspect the header and detail grids.
4. `FoundUnread` means the invoice has not yet been explicitly ACKed.
5. `AlreadyRead` still displays the complete invoice so recovery/re-import remains possible.

### ACK safety

The sample never auto-ACKs a lookup.

To test ACK manually you must:

1. check **Enable ACK write**;
2. click **ACK invoice**;
3. confirm the warning dialog.

In a real POS/ERP integration, ACK must be called only after the destination database transaction has committed successfully.

ACK updates `SADR_Total.LableStatus = 1` and is idempotent.

## Scales tab

1. Click **Refresh scales** to read the registered Sadr Scales rows.
2. Inspect Scale ID, IP, port, model, store, `Online / Offline / Unknown` status and AutoSend configuration in the grid.
3. Selecting a row copies its Scale ID into the resend control.

### Resend safety

Resend requests are explicit SQL writes, so the buttons remain disabled until **Enable resend writes** is checked. Each request also requires a confirmation dialog.

- **Request item resend** records an item AutoSend resend request.
- **Request HotKey resend** records a HotKey AutoSend resend request only for models whose 5.2.1 automatic HotKey path supports it.
- PLUS reports `UnsupportedModel` for automatic HotKey resend rather than false success.

`Requested` means the SQL AutoSend watermark was reset. It does **not** mean the physical scale has already received the items/HotKeys. Actual transfer occurs during a later eligible AutoSend cycle.

## Run

Open/build on Windows with .NET Framework 4.8 support:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp/SadrScales.Integration.SampleApp.csproj
```

The public sample does not contain customer credentials, direct scale protocols, packet formats or private Runtime implementation.

## Planned growth

Future Vendor-Ready slices will add Stores, Groups, Items, Assignments, HotKeys, Sales, Reports and Demo Data to this same application.
