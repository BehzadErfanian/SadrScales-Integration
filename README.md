# Sadr Scales Integration

**Official public integration toolkit and SQL contract for connecting POS, ERP and accounting software to Sadr Scales.**

[فارسی](README.fa.md) · [Getting Started](docs/en/getting-started.md) · [SQL Contract v1](docs/en/sql-contract-v1.md) · [SDK Design](docs/SDK_DESIGN_V1.md) · [Security](SECURITY.md)

---

## Repository status

This repository is **pre-1.0**. The basic public integration contract for **Sadr Scales 5.2.1** is frozen as **SQL Contract v1**, and the first C# SDK foundation is now implemented and validated by CI.

| Component | Status |
|---|---|
| SQL Contract v1 | Frozen basic surface for Sadr Scales 5.2.1 |
| Persian technical guide | 34-page official PDF prepared and QA'd; release asset pending |
| C# integration SDK | Pre-1.0 foundation builds, tests and packs successfully |
| Raw SQL samples | Available |
| Python / Node.js / Java / PHP samples | Planned |
| GitHub Releases | Not published yet |
| REST / Webhook gateway | Future generation; not part of 5.2.1 / Contract v1 |

## What this repository is for

Software vendors integrate with **Sadr Scales**, rather than reimplementing the proprietary wire protocol of each scale model.

```text
POS / ERP / Accounting
        ↓
Sadr Scales SQL Contract v1 / Integration SDK
        ↓
Sadr Scales Runtime
        ↓
Supported scale models
```

The basic public contract allows an external application to:

- create/update item groups and PLUs;
- validate the expected Contract v1 database schema;
- read accepted sales incrementally from Sadr Scales;
- keep its own durable sales cursor/state;
- integrate without knowing the direct communication protocol of PLUS, LSG, Aclas or other devices.

Registry, Mapping, structured invoice data and runtime state are documented as advanced/controlled areas rather than part of the basic SDK path.

## Quick contract summary

Public SQL Contract v1 is centered on:

- `dbo.SADR_ItemClass` — item groups, SELECT / INSERT / UPDATE;
- `dbo.SADR_Item` — PLU/item master, SELECT / INSERT / UPDATE; `PluNo` is the public identity;
- `dbo.SADR_Logs` — accepted sales feed, **SELECT only**.

The destination application persists imported sales first, then advances its own durable cursor. The basic SDK never updates or deletes `SADR_Logs` for acknowledgement.

See [SQL Contract v1](docs/en/sql-contract-v1.md), the [Persian contract](docs/fa/sql-contract-v1.md), and the [Contract Freeze record](docs/CONTRACT_V1_FREEZE.md).

## C# SDK — pre-1.0

The current foundation targets `netstandard2.0` and uses `Microsoft.Data.SqlClient`. Its basic API is intentionally small:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);

SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

Current foundation features:

- Contract v1 schema validation;
- parameterized, transactional item-group upsert;
- parameterized, transactional semantic PLU upsert;
- read-only incremental sales batches;
- explicit caller-owned connection and destination-cursor behavior;
- unit tests and GitHub Actions restore/build/test/pack validation.

The API is still **pre-1.0** and may change before the first stable release. See [SDK Design v1](docs/SDK_DESIGN_V1.md).

## SQL samples

Executable synthetic samples are available in [`samples/SQL`](samples/SQL/README.md):

- Contract schema validation;
- safe item-group/PLU upsert with rollback by default;
- read-only incremental sales query.

## Documentation

- [Getting Started](docs/en/getting-started.md)
- [SQL Contract v1](docs/en/sql-contract-v1.md)
- [SDK Design v1](docs/SDK_DESIGN_V1.md)
- [Project status](docs/PROJECT_STATUS.md)
- [Roadmap](docs/ROADMAP.md)
- [Decision log](docs/DECISIONS.md)
- [Compatibility](docs/COMPATIBILITY.md)
- [Security boundary](docs/SECURITY_BOUNDARY.md)
- [Full Persian guide release identity](docs/reference/README.md)

## Releases

Stable binaries/packages are published through **GitHub Releases**, not committed as binary clutter to `main`. The first stable release is not published yet. The intended release includes the SDK package/DLL, XML docs, samples, official technical guide, changelog and SHA-256 checksums.

## Security

This repository intentionally excludes direct device protocols, packet captures, private keys, credentials, customer data, proprietary firmware/vendor material and internal Sadr Scales release infrastructure. Read [SECURITY.md](SECURITY.md) before contributing.

## License

No open-source license has been granted yet. The exact public software license requires explicit company approval before stable SDK distribution. Until a `LICENSE` file is added, all rights remain reserved. See [NOTICE.md](NOTICE.md).
