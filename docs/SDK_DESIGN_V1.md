# C# SDK v1 Design

**Status:** M2 hardening / pre-1.0  
**Contract:** SQL Contract v1  
**Sadr Scales baseline:** 5.2.1

## Design goals

The C# SDK is a thin, explicit client over the frozen public SQL contract. It does **not** reimplement direct device protocols and does not own business state that belongs to the destination POS/ERP/accounting application.

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

The SDK is intended to be consumable from both existing .NET Framework applications and modern .NET applications. A real .NET Framework 4.8 consumer compatibility build/test remains required before v1.0.

## SQL provider

Initial provider: **`Microsoft.Data.SqlClient 7.0.2`**.

### Connection-string security behavior

The SDK treats the connection string as caller-owned configuration and does **not** silently rewrite encryption, certificate trust, user IDs or passwords.

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
- does not delete the default group;
- the write command itself is never automatically replayed after it starts.

### Items

`SadrItemClient.UpsertAsync(...)`

- `PluNo` is the identity;
- does not write legacy `ID` / `IDitem`;
- does not write SQL-owned `TimeStamp`/rowversion;
- does not require `TaxNo` or `SendFlag` in the basic path;
- semantic no-op does not touch rowversion;
- physical delete is not part of the basic API;
- the write command itself is never automatically replayed after it starts.

### Sales

`SadrSalesClient.ReadAfterAsync(cursor, batchSize)`

- reads `SADR_Logs` only;
- always orders by ascending `ID`;
- never updates/deletes feed rows;
- returns `LastReadId` only as a cursor candidate;
- caller persists the cursor only after destination commit;
- destination remains responsible for idempotency, preferably using `(DeviceNo, FID, SubID)`;
- recognized transient connection/read failures may replay the complete read on a fresh connection within the configured bounded retry policy.

## Connection lifetime

Each operation uses a short-lived `SqlConnection`; SqlClient pooling provides physical connection reuse. No shared mutable connection is kept across unrelated operations.

## Transactions

Write operations use explicit SQL transactions. If an operation fails, rollback is attempted while the original exception remains authoritative.

Opening the connection can be retried because no command/transaction has started yet. Once a write transaction begins, the SDK does **not** automatically replay the write operation.

## Error model

- argument/contract-input problems use standard .NET argument exceptions;
- SQL connectivity/server errors remain `SqlException` so callers retain native diagnostic information;
- Contract v1 schema errors are translated to `SadrContractMismatchException` with the original SQL error as inner exception;
- when bounded retry is exhausted, the final underlying exception is rethrown rather than wrapped in a custom aggregate exception.

## Bounded transient retry policy

The SDK uses its own small retry policy instead of depending on SqlClient's configurable retry provider surface. This keeps behavior explicit and compatible with the SDK's `netstandard2.0` target.

Public options:

- `TransientRetryCount` — retries **after** the initial attempt; default `2`; valid `0..5`.
- `TransientRetryBaseDelayMilliseconds` — first delay; default `250`; valid `1..5000`.

Backoff:

- exponential (`base`, `2×base`, `4×base`, ...);
- each delay is capped at `5000 ms`;
- every delay honors the caller cancellation token.

Retry is allowed for:

1. **connection establishment** before any SQL operation starts; and
2. **complete read-only operations** (`ValidateAsync`, `Sales.ReadAfterAsync`) on a fresh connection.

Retry is **not** applied to the transaction-scoped execution of:

- `ItemGroups.UpsertAsync`;
- `Items.UpsertAsync`.

This separation avoids replaying a write whose server-side commit state may be ambiguous after a connection failure.

The current conservative transient-number set includes common timeout/transport/deadlock/service-busy/resource-unavailable conditions. Authentication failure (`18456`) and generic database-open/configuration error (`4060`) are deliberately not blindly retried.

The error-number list is an implementation policy, not a public compatibility contract; it may be adjusted when real failure evidence warrants it.

## Real SQL validation

CI runs the SDK against a disposable SQL Server 2022 instance using synthetic Contract v1 schema/data. Current SQL-backed coverage includes:

- Contract validation;
- semantic group/item writes;
- rowversion preservation on no-op;
- real rowversion change on update;
- sales identity gaps and read-only behavior;
- schema mismatch exception mapping.

## Logging

The SDK does not impose a logging framework dependency. Diagnostics must never emit raw credentials/connection strings or customer sale payloads by default.

## Advanced surface

Registry, Scale Mapping, hotkeys and structured invoice/Ack APIs are **not** part of the basic SDK foundation. If introduced, they must be separated as advanced/controlled APIs.

## Packaging gates before v1.0

- company-approved public license;
- .NET Framework 4.8 consumer compatibility build/test;
- package metadata and Source Link;
- strong-name decision for .NET Framework/.NET Standard consumers;
- API compatibility/package validation policy;
- release checksums and guide assets.
