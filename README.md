# Sadr Scales Integration

[![SDK CI](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml)
[![Public Repository Guard](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml)

**Official public integration toolkit for POS, ERP and accounting software connecting to Sadr Scales.**

Provided and maintained by **Tozin Sadr and Behzad Erfanian**.

[فارسی](README.fa.md) · [Getting Started](docs/en/getting-started.md) · [Capabilities](docs/en/capabilities.md) · [Raw SQL Recipes](samples/SQL/README.md) · [Developer Sample](samples/csharp/SadrScales.Integration.SampleApp/README.md) · [Releases](https://github.com/BehzadErfanian/SadrScales-Integration/releases) · [Support](SUPPORT.md) · [Security](SECURITY.md)

## Status

- Stable public SDK release: **`v1.1.0`**
- Package: **`SadrScales.Integration.1.1.0`**
- Accepted Sadr Scales contract baseline: **`5.2.1`**
- Sadr Scales **`5.3`** application publication is in progress; the supported SQL/SDK contract remains backward-compatible.
- Final clean-room vendor rehearsal and the new website Developer Guide are intentionally scheduled **after Sadr Scales 5.3 is published**.
- SDK target: `netstandard2.0`
- Proven package consumers: .NET Framework 4.8 and modern .NET
- License: MIT

`v1.1.0` is the current frozen public SDK package. GitHub release assets are already published. The next documentation gate is a clean-room, package-only vendor rehearsal against the released Sadr Scales 5.3 environment; the official website Developer Guide will be finalized only after that rehearsal passes.

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

## v1.1.0 capabilities

The approved integration surface includes Stores, Items/PLUs, Price History, Scales, Scale Group Assignments, per-scale Item Mapping, HotKey templates, Item/HotKey AutoSend resend requests, incremental Sales Feed, filtered Sales Query/summaries, structured Invoice lookup, idempotent Invoice ACK, and Daily/Scale/Item reports.

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

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

Never commit production credentials.

## Non-C# / Raw SQL

C# is optional. The supported language-independent recipes are under [`samples/SQL`](samples/SQL/README.md). Do not invent writes against internal Sadr tables/columns that are not part of the documented recipes.

## Demo Data safety

DemoLab is intentionally **not** part of the production SDK API. Never initialize the Demo marker on a customer or production database.

## Release quality

The `v1.1.0` package release passed the repository build/test/package/public-repository gates. Before the website Developer Guide is promoted as final, a clean-room package-only vendor rehearsal against the released Sadr Scales application is required.

## Security boundary

This repository intentionally excludes direct scale protocols, raw packets, packet captures, private keys, customer production data, private firmware/vendor material and arbitrary Runtime commands.

The typed per-scale Integration Command Mailbox belongs to the future Sadr Scales **5.4 architecture track** (or the next unused two-part release if 5.4 is consumed by an intervening public release). It is not part of SDK `v1.1.0`.

## Release integrity

GitHub Release `v1.1.0` is the authoritative public package release. NuGet package SHA-256:

```text
2baa100d6cf3125c75edbb7e99e1d15ff3e99d0bcd52534180ebe3f29d9d359f
```

## License

MIT License. See [LICENSE](LICENSE) and [NOTICE.md](NOTICE.md).

**Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.**
