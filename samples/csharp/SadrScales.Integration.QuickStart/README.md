# C# Quick Start

This sample is intentionally **read-only by default**. It validates SQL Contract v1 and reads one bounded sales batch. It never embeds credentials and never writes to `SADR_ItemClass`, `SADR_Item` or `SADR_Logs`.

## Run

Set the connection string outside source control:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "<your SQL Server connection string>"
dotnet run --project samples/csharp/SadrScales.Integration.QuickStart
```

The console reports contract validation, row count and the returned cursor candidate. It does not print customer sale payloads.

## Item writes

Item writes are explicit SDK calls and are intentionally absent from the executable quick start. For bounded atomic PLU synchronization, application code can call:

```csharp
SadrItemBatchWriteResult result = await client.Items.UpsertBatchAsync(items);
```

One call accepts at most `SadrItemClient.MaxBatchSize` (200) PLUs. The SDK validates the complete batch before SQL access, rejects duplicate `PluNo` values, and commits all rows in one transaction or rolls the whole call back.

For more than 200 PLUs, page explicitly in the destination application so each committed transaction boundary is visible and recoverable.
