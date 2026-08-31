# Sadr Scales Integration

[![SDK CI](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml)
[![Public Repository Guard](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml)

**Official public integration toolkit for POS, ERP and accounting software connecting to Sadr Scales.**

Provided and maintained by **Tozin Sadr and Behzad Erfanian**.

[فارسی](README.fa.md) · [Getting Started](docs/en/getting-started.md) · [Capabilities](docs/en/capabilities.md) · [Raw SQL Recipes](samples/SQL/README.md) · [Developer Sample](samples/csharp/SadrScales.Integration.SampleApp/README.md) · [Support](SUPPORT.md) · [Security](SECURITY.md)

## Status

- Stable public Integration release: **`v1.1.0`**
- Frozen SQL/SDK contract baseline: **Sadr Scales `5.2.1`**
- Current Sadr Scales application release: **`5.3`**
- Sadr Scales 5.3 compatibility: **final vendor-rehearsal validation in progress**
- SDK target: `netstandard2.0`
- Proven package consumer: .NET Framework 4.8 and modern .NET
- License: MIT

`v1.1.0` is the current Vendor-Ready release. Its public SQL/SDK contract remains intentionally pinned to the validated Sadr Scales 5.2.1 integration surface; advancing the Sadr Scales application to 5.3 does not silently redefine that contract. The current post-release work is a published-package clean-room and compatibility rehearsal against Sadr Scales 5.3 before the next external vendor outreach.

## Start here

```text
1. Read Getting Started
2. Validate the SQL contract
3. Review Capabilities
4. Run the Developer Sample
5. Use Raw SQL Recipes only when you do not use the C# SDK
6. Open detailed reference docs only when needed
```

## Architecture

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration or documented SQL
        ↓
Sadr Scales database/runtime
        ↓
PLUS / LSG / Aclas / supported scales
```

Your software integrates with **Sadr Scales**, not with proprietary device wire protocols. Sadr Scales keeps ownership of sessions, retry/reconnect, model differences and direct scale communication.

## Five-minute C# start

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();

await client.Stores.UpsertAsync(store);
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

For continuous sales synchronization:

```csharp
SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

Persist destination data first, commit it, and only then persist `batch.LastReadId` as your new cursor.

## Vendor-Ready 1.1 capabilities

The frozen Sadr Scales 5.2.1 integration surface includes:

- Stores;
- Item Groups and Items/PLUs;
- Price History read;
- registered Scales and coarse `Online / Offline / Unknown` state;
- Scale Group Assignments;
- per-scale Item Mapping;
- group HotKey templates;
- Item/HotKey AutoSend resend requests;
- incremental Sales Feed;
- filtered Sales Query and summaries;
- structured Invoice lookup by TotalBarcode or ScaleID + FID;
- explicit idempotent Invoice ACK;
- Daily / Scale / Item reports.

See the [Capabilities map](docs/en/capabilities.md).

## Structured invoice rule

```text
Read invoice
→ Save destination data
→ Commit destination transaction
→ ACK source invoice
```

Lookup never ACKs automatically. An acknowledged invoice remains fully readable with `AlreadyRead`, so recovery/re-import is possible.

## Executable Developer Sample

Use the WinForms reference application:

[`samples/csharp/SadrScales.Integration.SampleApp`](samples/csharp/SadrScales.Integration.SampleApp/README.md)

It demonstrates Invoices, Scales, Catalog, Assignments/Mapping/HotKeys, Sales/Reports and guarded Demo Data.

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

Never commit production credentials.

## Non-C# / Raw SQL

C# is optional. The supported language-independent recipes are under [`samples/SQL`](samples/SQL/README.md).

Do not invent writes against internal Sadr tables/columns that are not part of the documented recipes.

## Demo Data safety

DemoLab is intentionally **not** part of the production SDK API. Demo generation/reset requires a clearly non-production database, compatible schema, empty business data, exact database-name confirmation and a valid Demo marker.

Never initialize the Demo marker on a customer or production database.

## Release quality

The `v1.1.0` Vendor-Ready release gate includes:

- SDK build/test/package validation;
- disposable SQL Server integration tests;
- executable WinForms Sample build;
- real .NET Framework 4.8 package consumer;
- package-only external-developer Vendor Acceptance flow;
- Public Repository Guard;
- release-bundle validation.

The post-release compatibility rehearsal uses the published package bytes and Sadr Scales 5.3 rather than expanding the frozen 1.1 contract.

## Security boundary

This repository intentionally excludes direct scale protocols, raw packets, packet captures, private keys, customer production data, private firmware/vendor material and arbitrary Runtime commands.

Typed managed commands, Service/REST, Webhook/realtime and related Gateway capabilities are future **Sadr Scales 5.4+** architecture work. They are outside the current frozen 1.1 SQL/SDK contract.

## License

MIT License. See [LICENSE](LICENSE) and [NOTICE.md](NOTICE.md).

**Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.**
