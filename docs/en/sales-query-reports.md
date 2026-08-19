# Sales Query and Reports

This guide covers the additive Vendor-Ready `1.1.0` read-only sales-query/report surface for Sadr Scales `5.2.1`.

## Feed vs query

Two sales APIs intentionally serve different jobs.

### Incremental integration feed

```csharp
var batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

Use this when another application continuously imports new `SADR_Logs` rows. It is ordered by increasing `ID` and the destination owns/persists its cursor.

### User/search query

```csharp
var page = await client.Sales.QueryAsync(filter);
```

Use this for UI searches, review screens and reports. It is ordered newest first by `DateTime DESC, ID DESC` and does not use or change the destination feed cursor.

Do not replace the feed cursor workflow with Query pagination.

## Query filter

```csharp
var filter = new SadrSalesQueryFilter
{
    StartDateInclusive = new DateTime(2026, 8, 1),
    EndDateExclusive = new DateTime(2026, 9, 1),
    ScaleId = 3,
    Plu = 1001,
    PageNumber = 1,
    PageSize = 200
};

var page = await client.Sales.QueryAsync(filter);
```

Optional filters:

- inclusive start date/time;
- exclusive end date/time;
- exact `Identify`;
- PLU;
- Scale ID / `DeviceNo`;
- FID.

Paging reproduces 5.2.1 behavior:

- page number is at least 1;
- page size is clamped to 50..2000;
- empty results report one page.

## Summary

Every query page contains a summary for the **complete filter**, not only the visible page:

```text
RecordCount
InvoiceCount
TotalPrice
TotalWeight
TotalQuantity
```

`InvoiceCount` is the distinct `(DeviceNo, FID)` count. Multiple item rows in one invoice therefore count as one invoice.

Weight/quantity semantics match Sadr Scales 5.2.1:

- unit codes `0`, `1` and `3` contribute to `TotalWeight`;
- unit code `2` contributes to `TotalQuantity`.

## Period helpers

Sadr Scales period semantics are available without forcing vendors to recalculate calendar boundaries:

```csharp
var range = SadrSalesPeriod.GetRange(
    SadrSalesPeriodPreset.CurrentMonth,
    DateTime.Today);

filter.StartDateInclusive = range.StartDateInclusive;
filter.EndDateExclusive = range.EndDateExclusive;
```

Presets:

- `Today` — the local Gregorian date;
- `CurrentWeek` — Saturday through Friday;
- `CurrentMonth` — the Persian-calendar month containing the reference date.

## Typed reports

All reports accept the same `SadrSalesQueryFilter` as `Sales.QueryAsync`.

### Daily

```csharp
var rows = await client.Reports.GetDailyAsync(filter);
```

Returns one aggregate per Gregorian sale date, newest first.

### By scale

```csharp
var rows = await client.Reports.GetByScaleAsync(filter);
```

Returns one aggregate per `DeviceNo` / Scale ID, highest total price first.

### By item

```csharp
var rows = await client.Reports.GetByItemAsync(filter);
```

Returns one aggregate per PLU, highest total price first. The report is capped at 5000 rows, matching Sadr Scales 5.2.1.

Each report row contains the same `SadrSalesSummary` fields used by Query.

## Read-only contract

Sales Query and Reports do not update `SADR_Logs`, invoice ACK state, feed cursors or any scale state.

They are safe to repeat and can be retried according to the SDK read policy.

## Raw SQL

Non-C# consumers can use the read-only reference:

```text
samples/SQL/07-sales-query-reports.sql
```

It demonstrates the same filters, summary, newest-first page and Daily/Scale/Item reports.

## 5.2.1 boundary

This surface reports sales already stored in Sadr Scales. Requesting a scale to fetch fresh sales from a physical device is a Runtime/device command and is outside the current 5.2.1 SQL contract. That class of operation is planned for the Sadr Scales 5.3 Command Mailbox.
