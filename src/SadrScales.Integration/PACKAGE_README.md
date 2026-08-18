# SadrScales.Integration

Official C# SDK for the public **Sadr Scales SQL Contract v1**.

**Provided and maintained by:** Tozin Sadr and Behzad Erfanian  
**License:** MIT  
**SDK line:** 1.x  
**Baseline:** Sadr Scales 5.2.1 or a later release explicitly compatible with SQL Contract v1  
**Target framework:** `netstandard2.0`

## Start safely

Always validate the database contract before normal integration work:

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

## Item groups and PLUs

```csharp
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

For bounded bulk PLU writes:

```csharp
SadrItemBatchWriteResult result = await client.Items.UpsertBatchAsync(items);
```

`UpsertBatchAsync` accepts at most **200 unique PLUs** per call. The complete batch is validated before SQL access and committed in one transaction. If one write fails, the complete call rolls back. Larger imports are explicitly paged by the caller.

## Read sales incrementally

```csharp
SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

The basic sales API is read-only. Your POS/ERP/accounting application owns durable import state:

1. persist the returned sales in your destination;
2. commit destination data;
3. only then persist `batch.LastReadId` as your next cursor.

Use `(DeviceNo, FID, SubID)` as the preferred destination duplicate-protection key and tolerate gaps in source `ID` values.

## Bounded transient retry

Safe connection/read boundaries have a small configurable retry policy. Defaults are 2 retries after the first attempt with a 250 ms base delay.

```csharp
var options = new SadrScalesClientOptions(connectionString)
{
    TransientRetryCount = 2,
    TransientRetryBaseDelayMilliseconds = 250
};

var client = new SadrScalesClient(options);
```

Connection establishment can retry before any command starts. Complete read-only contract validation and sales reads can replay on a fresh connection. Transaction-scoped item/group writes are deliberately **not automatically replayed** after execution begins because commit state can become ambiguous when a response is lost.

## Compatibility proof

The package is CI-tested against:

- a disposable SQL Server 2022 Contract v1 database;
- a real .NET Framework 4.8 application that restores and runs the generated NuGet package;
- a modern .NET C# Quick Start;
- package metadata/Source Link validation;
- release-bundle checksum generation.

## Documentation and sample

Repository:

`https://github.com/BehzadErfanian/SadrScales-Integration`

Start with:

- `docs/en/getting-started.md`
- `docs/en/troubleshooting.md`
- `docs/PRODUCTION_READINESS_CHECKLIST.md`
- `samples/csharp/SadrScales.Integration.QuickStart`
- `SUPPORT.md`

## Security boundary

- The SDK does not implement or expose proprietary PLUS/LSG/Aclas/device wire protocols.
- The caller owns the SQL Server connection string and deployment security configuration.
- Never embed production credentials in source code or public issue reports.
- Do not update/delete `SADR_Logs` to manage a consumer cursor.

## Support

Use sanitized GitHub Issues for public reproducible SDK/Contract problems. Customer-specific/commercial deployment support belongs in Tozin Sadr's official support channels. Security-sensitive reports must follow `SECURITY.md` and must not be posted publicly.

## License

The public SDK is released under the MIT License.

Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.
