# Sadr Scales Integration

[![SDK CI](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml)
[![Public Repository Guard](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml)

**Official public integration toolkit, SQL contract, C# SDK and samples for connecting POS, ERP and accounting software to Sadr Scales.**

**Provided and maintained by Tozin Sadr and Behzad Erfanian.**

[فارسی](README.fa.md) · [Stable v1.0.0 Release](https://github.com/BehzadErfanian/SadrScales-Integration/releases/tag/v1.0.0) · [Getting Started](docs/en/getting-started.md) · [Troubleshooting](docs/en/troubleshooting.md) · [Production Readiness](docs/PRODUCTION_READINESS_CHECKLIST.md) · [SQL Contract v1](docs/en/sql-contract-v1.md) · [Compatibility](docs/COMPATIBILITY.md) · [Support](SUPPORT.md) · [Security](SECURITY.md)

---

## Status

**Stable public release: `v1.0.0`.**

The supported public baseline is:

| Component | v1 baseline |
|---|---|
| Sadr Scales | `5.2.1` or later version explicitly verified as SQL Contract v1 compatible |
| Public database contract | **SQL Contract v1** |
| C# SDK | `SadrScales.Integration 1.0.0` |
| SDK target | `netstandard2.0` |
| SQL provider | `Microsoft.Data.SqlClient 7.0.2` |
| .NET Framework | real `net48` package restore/build/runtime gate |
| SQL validation | disposable SQL Server 2022 integration suite |
| License | MIT |

`v1.0.0` was published from exact commit `a6bccc7c13a8afba29b6860869d2a942b1231803` after protected CI, branch protection, release-bundle verification and SHA-256 validation.

## Why integrate here?

Your software integrates with **Sadr Scales**, not with the proprietary wire protocol of each scale model.

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration / SQL Contract v1
        ↓
Sadr Scales Runtime
        ↓
PLUS / LSG / Aclas / supported scales
```

Sadr Scales keeps ownership of device sessions, retry/reconnect, model differences, Registry and direct scale communication.

## Five-minute C# path

### 1. Validate the database contract

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

A schema mismatch is a stop condition. Do not bypass contract validation.

### 2. Create/update groups and PLUs

```csharp
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

For bounded bulk work:

```csharp
SadrItemBatchWriteResult result = await client.Items.UpsertBatchAsync(items);
```

A batch contains at most **200 unique PLUs**, is fully validated before SQL access and commits atomically. Larger imports are paged explicitly by your application.

### 3. Read accepted sales incrementally

```csharp
SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

Your application owns durable import state:

1. persist sales in your destination;
2. commit destination data;
3. only then persist `batch.LastReadId` as the next cursor.

Use `(DeviceNo, FID, SubID)` as the preferred destination duplicate-protection key. Source `ID` values may contain gaps.

## Executable Quick Start

A build-validated, read-only-by-default C# sample is included:

[`samples/csharp/SadrScales.Integration.QuickStart`](samples/csharp/SadrScales.Integration.QuickStart/README.md)

It reads the connection string only from `SADR_SCALES_CONNECTION_STRING`.

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.QuickStart
```

Never commit a real production connection string.

## Package installation

Stable SDK artifacts are distributed through [GitHub Releases](https://github.com/BehzadErfanian/SadrScales-Integration/releases/tag/v1.0.0).

After downloading `SadrScales.Integration.1.0.0.nupkg` to a local folder:

```bash
dotnet add package SadrScales.Integration --version 1.0.0 --source <download-folder>
```

The stable release also contains:

- symbol package (`.snupkg`);
- compiled DLL/XML documentation bundle;
- Developer Kit ZIP;
- official Persian Integration & Database Guide for Sadr Scales 5.2.1;
- release manifest;
- SHA-256 checksums;
- release notes.

## Raw SQL path

C# is not required. Language-independent Contract v1 SQL samples are available in [`samples/SQL`](samples/SQL/README.md):

- schema validation;
- safe item-group/PLU upsert with rollback by default;
- read-only incremental sales query.

This is the starting point for Java, Python, Node.js, PHP or other stacks until language-specific wrappers are added.

## Public SQL Contract v1

The basic public contract intentionally stays small:

- `dbo.SADR_ItemClass` — item groups; SELECT / INSERT / UPDATE;
- `dbo.SADR_Item` — PLU/item master; SELECT / INSERT / UPDATE; `PluNo` is the public identity;
- `dbo.SADR_Logs` — accepted sales feed; **SELECT only**.

Important rules:

- never write SQL `rowversion`;
- do not use legacy `ID`/`IDitem` as the public PLU identity;
- do not update/delete `SADR_Logs` for acknowledgement or cursor management;
- destination software owns its own durable cursor and idempotency.

Registry, Mapping, structured-invoice internals and runtime state remain controlled surfaces unless a future public contract explicitly promotes them.

## Reliability behavior

The SDK retries only where replay is safe:

- connection opening before a command starts;
- complete read-only contract validation;
- complete read-only sales reads.

Transaction-scoped item/group writes are deliberately **not automatically replayed** after execution begins because a lost response can make commit state ambiguous.

## Production handoff

Before enabling an integration in a customer environment, complete the [Production Readiness Checklist](docs/PRODUCTION_READINESS_CHECKLIST.md). It covers version/contract verification, database security, PLU rules, sales cursor/idempotency, restart/rollback testing and operational handoff.

## Compatibility and API stability

`1.0.0` establishes the first stable SDK API line. `1.x` follows Semantic Versioning and SQL Contract versioning remains separate.

Read [SDK API Compatibility Policy](docs/API_COMPATIBILITY.md) for the compatibility promise, package-validation policy and strong-name decision.

## Documentation

- [Getting Started](docs/en/getting-started.md)
- [Troubleshooting](docs/en/troubleshooting.md)
- [Production Readiness Checklist](docs/PRODUCTION_READINESS_CHECKLIST.md)
- [SQL Contract v1](docs/en/sql-contract-v1.md)
- [SDK API Compatibility Policy](docs/API_COMPATIBILITY.md)
- [SDK Design v1](docs/SDK_DESIGN_V1.md)
- [Compatibility matrix](docs/COMPATIBILITY.md)
- [Security boundary](docs/SECURITY_BOUNDARY.md)
- [Official Persian guide identity](docs/reference/README.md)
- [Support policy](SUPPORT.md)
- [Contributing](CONTRIBUTING.md)

## Support and security

Use sanitized GitHub Issues for public reproducible SDK/Contract problems. Read [SUPPORT.md](SUPPORT.md) before posting customer-specific material.

Security-sensitive reports must follow [SECURITY.md](SECURITY.md) and must not be posted in a normal public issue.

This public repository intentionally excludes direct device protocols, packet captures, private keys, credentials, customer data, proprietary firmware/vendor material and internal Sadr Scales runtime/release infrastructure.

## Release quality and repository protection

The stable release was validated through:

- Public Repository Guard;
- SDK build/tests and executable Quick Start;
- NuGet package/license/provider/Source Link validation;
- SQL Server 2022 integration tests;
- real .NET Framework 4.8 package consumer;
- protected tag release workflow;
- official Integration Guide SHA-256 verification;
- release manifest and full asset SHA-256 verification.

GitHub host controls include Secret Scanning, Push Protection, Dependabot alerts/security updates, Private Vulnerability Reporting and CodeQL default setup for C#. `main` is protected with required validated checks, conversation resolution, admin enforcement, and force-push/deletion disabled.

## License and providers

The public `SadrScales-Integration` SDK and repository materials covered by [LICENSE](LICENSE) are distributed under the **MIT License**.

**Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.**

This public license does not publish or license private Sadr Scales runtime source, proprietary device protocols, firmware, private keys, customer data or other material outside this repository. See [NOTICE.md](NOTICE.md).
