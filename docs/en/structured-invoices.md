# Structured invoices and explicit acknowledgement

> Status: Vendor-Ready capability being prepared for the release after `v1.0.0`.
> This document does not change the already-published `SQL Contract v1`.

## Purpose

A POS, ERP or accounting application can read a complete structured invoice persisted by Sadr Scales using either the aggregate barcode or the logical `ScaleID + FID` identity.

Reading never changes the invoice read state. The destination acknowledges the invoice only after its own persistence transaction has committed successfully.

## Aggregate barcode

Sadr Scales 5.2.1 format:

```text
25 + ScaleID(D3) + FID(D9)
```

Example:

```text
ScaleID = 12
FID     = 3456
TotalBarcode = 25012000003456
```

C# callers should use the SDK helper instead of rebuilding the format manually:

```csharp
string barcode = SadrInvoiceClient.BuildTotalBarcode(12, 3456);
```

## Lookup

```csharp
var client = new SadrScalesClient(connectionString);
var result = await client.Invoices.GetByBarcodeAsync(totalBarcode);
```

or:

```csharp
var result = await client.Invoices.GetAsync(scaleId, fid);
```

The lookup result is one of:

```text
FoundUnread  -> invoice exists and has not been ACKed yet
AlreadyRead  -> invoice was ACKed previously; complete data is still returned
NotFound     -> no matching structured invoice exists
```

Both `FoundUnread` and `AlreadyRead` return a complete `SadrInvoice` with its persisted detail rows.

## Why lookup never auto-ACKs

The safe destination sequence is:

```text
Lookup from Sadr Scales
        ↓
Receive complete invoice
        ↓
BEGIN TRANSACTION in destination
        ↓
Persist header + details
        ↓
COMMIT destination transaction
        ↓
ACK Sadr Scales
```

If lookup changed `LableStatus` immediately and destination persistence failed afterwards, Sadr Scales would incorrectly record the invoice as consumed. Read and ACK are therefore separate operations.

## ACK

After the destination commit succeeds:

```csharp
SadrInvoiceAckStatus ack =
    await client.Invoices.AcknowledgeAsync(totalBarcode);
```

or:

```csharp
SadrInvoiceAckStatus ack =
    await client.Invoices.AcknowledgeAsync(scaleId, fid);
```

ACK states:

```text
Acknowledged          -> invoice changed from unread to LableStatus = 1
AlreadyAcknowledged   -> invoice was already ACKed; the repeated operation is harmless
NotFound              -> no matching structured invoice exists
```

Invoice-level acknowledgement uses `SADR_Total.LableStatus`.
`SADR_Detail.ItemStatus` is not defined as a public ACK field by this contract and must not be modified for invoice acknowledgement.

## AlreadyRead recovery behavior

`AlreadyRead` is informational, not a data-access block.

A destination may have received and acknowledged an invoice earlier and later lost or deleted its own record by mistake. A later scan must still receive the complete invoice so the application or operator can make an informed recovery/re-import decision.

The SDK reports the fact that the invoice was read before, but it does not impose the destination's business decision.

## Raw SQL recipe

Non-C# integrations can use:

[`samples/SQL/03-structured-invoice-lookup-ack.sql`](../../samples/SQL/03-structured-invoice-lookup-ack.sql)

The sample is lookup-only by default. The ACK section runs only when explicitly enabled after destination commit.

## Retry boundary

Lookup is read-only and may use the configured bounded connection/read retry policy.

ACK is a transactional write. The SDK may retry opening the connection before the transaction starts, but it does not blindly replay the ACK transaction after execution begins.
