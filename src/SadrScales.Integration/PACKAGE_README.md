# SadrScales.Integration

C# client library for the public **Sadr Scales SQL Contract v1**.

Target baseline: **Sadr Scales 5.2.1+** where Contract v1 compatibility has been verified.

## Basic usage

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);

SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

The basic sales API is read-only. Persist destination data first, then persist `batch.LastReadId` in your own POS/ERP/accounting system.

## Bounded transient retry

The SDK has a small bounded retry policy for safe connection/read boundaries. Defaults are 2 retries after the first attempt with a 250 ms base delay.

```csharp
var options = new SadrScalesClientOptions(connectionString)
{
    TransientRetryCount = 2,
    TransientRetryBaseDelayMilliseconds = 250
};

var client = new SadrScalesClient(options);
```

Connection establishment can retry before any command starts. Complete read-only Contract validation and sales reads can retry on a fresh connection. Transaction-scoped item/group write commands are deliberately **not** automatically replayed after execution begins.

## Documentation

Full documentation, SQL Contract v1, security boundaries and samples are maintained in the public repository:

`https://github.com/BehzadErfanian/SadrScales-Integration`

## Important

- The SDK does not implement or expose proprietary scale wire protocols.
- The caller owns the SQL Server connection string and deployment security configuration.
- Do not embed real credentials in source code.
- Pre-1.0 APIs may still change until the first stable release.
