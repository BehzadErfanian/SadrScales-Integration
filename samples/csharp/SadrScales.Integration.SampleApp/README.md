# Sadr Scales Integration Sample App

This is the executable WinForms reference application for the Vendor-Ready integration surface.

The application intentionally grows one tested capability at a time instead of creating many unrelated demo programs.

## Current implemented flow — Structured Invoices

1. Enter the Sadr Scales SQL connection string, or set `SADR_SCALES_CONNECTION_STRING` before launch.
2. Enter a 14-digit structured `TotalBarcode`.
3. Click **Lookup invoice**.
4. Inspect the header and detail grids.
5. `FoundUnread` means the invoice has not yet been explicitly ACKed.
6. `AlreadyRead` still displays the complete invoice so recovery/re-import remains possible.

## ACK safety

The sample never auto-ACKs a lookup.

To test ACK manually you must:

1. check **Enable ACK write**;
2. click **ACK invoice**;
3. confirm the warning dialog.

In a real POS/ERP integration, ACK must be called only after the destination database transaction has committed successfully.

ACK updates `SADR_Total.LableStatus = 1` and is idempotent.

## Run

Open/build on Windows with .NET Framework 4.8 support:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp/SadrScales.Integration.SampleApp.csproj
```

The public sample does not contain customer credentials, direct scale protocols, packet formats or private Runtime implementation.

## Planned growth

Future Vendor-Ready slices will add the agreed tabs/areas for Stores, Groups, Items, Scales, Assignments, HotKeys, Sales, Reports and Demo Data to this same application.
