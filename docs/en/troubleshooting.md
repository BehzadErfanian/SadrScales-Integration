# Troubleshooting — Sadr Scales Integration

This page covers the basic **Sadr Scales 5.2.1 / SQL Contract v1 / SDK 1.x** integration path.

## Start here

Before debugging application code:

1. Make sure Sadr Scales 5.2.1 or a later Contract-v1-compatible release has started successfully and completed its database migration/check.
2. Run `await client.ValidateAsync()` or `samples/SQL/00-validate-contract.sql` against the same database.
3. Test with synthetic/non-production data first.
4. Keep the destination sales cursor in your own durable storage.

## `SadrContractMismatchException`

The connected database does not match the frozen SQL Contract v1 shape expected by the SDK.

Check:

- the connection string points to the Sadr Scales database you intended;
- Sadr Scales has run its own schema migration/check;
- the database is not an old copied schema or partially upgraded backup;
- no third-party customization changed Contract v1 columns, keys or types.

Do not bypass contract validation by changing the SDK to accept an unknown schema.

## SQL login / database-open errors

Typical causes are incorrect server/database/user configuration or missing SQL permissions.

The SDK does not hide or rewrite the caller's connection string and does not silently weaken SQL encryption settings. Fix the deployment configuration instead of embedding credentials in source code.

## SQL certificate / encryption errors

Use a SQL Server TLS configuration appropriate for the deployment. Do not globally disable encryption just to make a production connection work.

`TrustServerCertificate=True` may be useful in a controlled development environment, but production security policy belongs to the integrator/customer environment.

## Timeouts or transient connection failures

The SDK has bounded automatic retry only at safe boundaries:

- opening a connection before a command starts;
- complete read-only contract validation;
- complete read-only sales queries.

Transaction-scoped item/group writes are **not automatically replayed** after execution begins because a lost response can make commit state ambiguous.

If a write fails after execution started, determine the actual database state before retrying it.

## PLU batch rejected before SQL access

`UpsertBatchAsync` intentionally validates the complete request first.

Common causes:

- more than 200 PLUs in one call;
- duplicate `PluNo` values inside the batch;
- invalid item data.

For larger imports, page the source application explicitly into batches of at most 200 unique PLUs.

## A PLU upsert reports `Unchanged`

This is expected when the semantic item values already match the database row. A semantic no-op preserves the SQL `rowversion` rather than creating an unnecessary update.

## Sales appear duplicated in the destination

Use destination-side idempotency. The preferred Contract v1 duplicate key is:

`(DeviceNo, FID, SubID)`

Persist the destination data first. Advance your durable cursor only after that persistence succeeds.

## Sales IDs contain gaps

Gaps are valid. Never assume IDs are contiguous.

Read with:

```sql
WHERE ID > @LastProcessedId
ORDER BY ID ASC
```

and persist the last successfully imported ID as the destination cursor.

## No new sales are returned

Check:

- the destination cursor value;
- that you are connected to the expected Sadr Scales database;
- that Sadr Scales has actually accepted the sales;
- that the SQL account has `SELECT` permission on `dbo.SADR_Logs`.

Do not update or delete `SADR_Logs` to manage the cursor.

## Need direct scale protocol information?

Direct PLUS/LSG/Aclas/device wire protocols are not part of this public SDK. External software integrates with Sadr Scales through the public SQL Contract/SDK so device-model communication remains owned by the Sadr Scales runtime.

## Still stuck?

When opening a GitHub issue, include:

- Sadr Scales version;
- SDK version;
- .NET/runtime version;
- SQL Server version;
- the failing public API call;
- exception type/message and sanitized stack trace;
- whether `ValidateAsync()` succeeds.

Never post connection strings, credentials, customer data, packet captures or private/proprietary files.
