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

## Documentation

Full documentation, SQL Contract v1, security boundaries and samples are maintained in the public repository:

`https://github.com/BehzadErfanian/SadrScales-Integration`

## Important

- The SDK does not implement or expose proprietary scale wire protocols.
- The caller owns the SQL Server connection string and deployment security configuration.
- Do not embed real credentials in source code.
- Pre-1.0 APIs may still change until the first stable release.
