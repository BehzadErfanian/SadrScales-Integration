# C# samples

The C# SDK foundation is now under active M2 development in `src/SadrScales.Integration`.

Target usage shape:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);

SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

The destination application must commit imported sales first and persist `batch.LastReadId` only after that destination commit succeeds.

Executable end-to-end samples will be added after the pre-1.0 SDK API is validated by CI and compatibility testing.
