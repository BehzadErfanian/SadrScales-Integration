# Stores and item catalog

This guide covers Vendor-Ready catalog capabilities for **Sadr Scales 5.2.1 / SadrScales.Integration 1.1.0**.

## Stores

`StoreCode` is the stable public store/branch identity; the display name is not the identity.

```csharp
var stores = await client.Stores.GetAllAsync();
var store = await client.Stores.GetAsync("S1");

var result = await client.Stores.UpsertAsync(new SadrStore
{
    StoreCode = "S1",
    StoreName = "Branch One",
    Descriptions = "Example branch"
});
```

Store upsert returns `Inserted`, `Updated` or `Unchanged`.

Sadr Scales has a default store with code `0`.

## Item groups

```csharp
var groups = await client.ItemGroups.GetAllAsync();
var group = await client.ItemGroups.GetAsync("G1");
await client.ItemGroups.UpsertAsync(groupToSave);
```

`ItemClassCode` is the public item-group identity.

## Read items

The normal catalog read returns active items only:

```csharp
var activeItems = await client.Items.GetAllAsync();
```

Include logically deleted rows explicitly when required:

```csharp
var allItems = await client.Items.GetAllAsync(includeDeleted: true);
```

A single PLU lookup still returns a logically deleted row:

```csharp
var item = await client.Items.GetAsync(pluNo);
```

This keeps recovery and inspection possible.

## Logical item delete

The public delete operation in 1.1.0 is not a physical SQL DELETE:

```csharp
var result = await client.Items.SoftDeleteAsync(pluNo);
```

Possible results:

```text
Deleted
AlreadyDeleted
NotFound
```

The operation sets `DeleteFlag = 1` and keeps the row. Normal active-item reads hide that row, matching the 5.2.1 active-item semantics.

To reactivate an item, read the complete existing model, make the intended changes, set `DeleteFlag = 0`, and upsert it.

**Important:** when editing an existing PLU, do not construct a mostly empty `SadrItem` and overwrite the row. Read the current item first so print, barcode, tare and text settings that are not being edited are preserved. The Developer Sample demonstrates this safe pattern.

## Price history

Price History is read-only in the current contract:

```csharp
var history = await client.Items.GetPriceHistoryAsync(pluNo, 100);
var recent = await client.Items.GetRecentPriceHistoryAsync(100);
```

Entries expose PLU, captured barcode/name, previous price, new price, timestamp and recorded user/source.

The 1.1.0 SDK does not invent a requirement that every external `Items.UpsertAsync` must create a `SADR_PriceLog` row; Sadr Scales 5.2.1 does not establish that as a stable public contract.

## Raw SQL

For non-C# consumers:

- [`samples/SQL/01-upsert-item.sql`](../../samples/SQL/01-upsert-item.sql) — item group + PLU upsert.
- [`samples/SQL/05-catalog.sql`](../../samples/SQL/05-catalog.sql) — stores/groups/items, logical delete and read-only price history.

`05-catalog.sql` is read-only by default and performs sanctioned writes only after `@ApplyChanges = 1` is explicitly selected.

## Responsibility boundary

Catalog persistence and physical device transfer are different concerns. In 5.2.1, a resend request can make data eligible for a later AutoSend cycle; immediate typed device commands are planned for Sadr Scales 5.3.
