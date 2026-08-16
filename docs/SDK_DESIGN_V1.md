# C# SDK v1 Design

**Status:** M2 foundation / pre-1.0  
**Contract:** SQL Contract v1  
**Sadr Scales baseline:** 5.2.1

## Design goals

The C# SDK is a thin, explicit client over the frozen public SQL contract. It does **not** reimplement direct device protocols and does not own business state that belongs to the destination POS/ERP/accounting application.

The first API should make the common path small:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);

SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

The destination persists its own data first and advances its own durable sales cursor only after that commit succeeds.

## Target framework

Initial SDK target: **`netstandard2.0`**.

Reason: this is specifically a reusable library that must be consumable from both existing .NET Framework applications and modern .NET applications. Microsoft library guidance recommends .NET Standard 2.0 for this sharing scenario.

Consumer-compatibility testing will include .NET Framework 4.8 before v1.0, even though unit-test execution initially runs on a modern .NET test host.

## SQL provider

Initial provider: **`Microsoft.Data.SqlClient 7.0.2`**.

Reasons:

- it is the current Microsoft SQL Server data provider line;
- the package includes .NET Standard 2.0, .NET Framework 4.6.2 and .NET 8+ compatible targets;
- new SQL Server provider features are developed in Microsoft.Data.SqlClient.

### Connection-string security behavior

The SDK treats the connection string as caller-owned configuration and does **not** silently rewrite encryption, certificate trust, user IDs or passwords.

Microsoft.Data.SqlClient has modern encryption defaults. Existing deployments using self-signed/local SQL certificates may therefore need an explicit connection-string decision by the deploying organization. The SDK must not silently weaken provider security defaults just to make a connection succeed.

The SDK must never log the raw connection string.

## Basic public API boundary

### Client

`SadrScalesClient`

- `ValidateAsync()`
- `ItemGroups`
- `Items`
- `Sales`

### Item groups

`SadrItemGroupClient.UpsertAsync(...)`

- parameterized SQL;
- transaction-scoped upsert;
- returns Inserted / Updated / Unchanged;
- does not delete the default group.

### Items

`SadrItemClient.UpsertAsync(...)`

- `PluNo` is the identity;
- does not write legacy `ID` / `IDitem`;
- does not write SQL-owned `TimeStamp`/rowversion;
- does not require `TaxNo` or `SendFlag` in the basic path;
- update is semantic: an unchanged item should not execute an effective UPDATE merely to touch rowversion;
- physical delete is not part of the basic API.

### Sales

`SadrSalesClient.ReadAfterAsync(cursor, batchSize)`

- reads `SADR_Logs` only;
- always orders by ascending `ID`;
- never updates/deletes feed rows;
- returns `LastReadId` only as a cursor candidate;
- caller persists the cursor only after destination commit;
- destination remains responsible for idempotency, preferably using `(DeviceNo, FID, SubID)`.

## Connection lifetime

Each operation opens a short-lived `SqlConnection` and disposes it when the operation completes. SQL Client connection pooling provides physical connection reuse. The public client does not keep one shared mutable SQL connection open across unrelated calls.

## Transactions

Write operations use explicit SQL transactions. If an operation fails, rollback is attempted while the original exception remains authoritative.

## Error model

- argument/contract-input problems use standard .NET argument exceptions;
- SQL connectivity/server errors remain `SqlException` so the caller retains native diagnostic information;
- schema validation errors emitted by the Contract v1 validator are translated to `SadrContractMismatchException` with the original SQL error preserved as the inner exception.

## Retry policy

No hidden infinite retry is allowed. Bounded transient-SQL retry is an M2 follow-up and will be implemented only with explicit limits and cancellation support.

## Logging

The foundation does not impose a logging framework dependency. Future diagnostics must never emit raw credentials/connection strings or customer sale payloads by default.

## Advanced surface

Registry, Scale Mapping, hotkeys and structured invoice/Ack APIs are **not** part of this basic SDK foundation. If introduced, they must be clearly separated as advanced/controlled APIs so the safe common path remains small.

## Packaging gates before v1.0

- company-approved public license;
- .NET Framework 4.8 consumer compatibility build/test;
- package metadata and Source Link;
- strong-name decision for .NET Framework/.NET Standard consumers;
- API compatibility/package validation policy;
- release checksums and guide assets.
