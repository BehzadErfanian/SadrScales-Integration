# Sadr Scales Integration

**Official public integration toolkit and SQL contract for connecting POS, ERP and accounting software to Sadr Scales.**

[فارسی](README.fa.md) · [Getting Started](docs/en/getting-started.md) · [SQL Contract v1](docs/en/sql-contract-v1.md) · [Roadmap](docs/ROADMAP.md) · [Security](SECURITY.md)

---

## Repository status

This repository is currently in **Foundation / pre-1.0** stage. The public SQL integration contract for **Sadr Scales 5.2.1** is defined as **SQL Contract v1**. The first SDK and language samples are planned and tracked in the project roadmap.

| Component | Status |
|---|---|
| SQL Contract v1 | Defined for Sadr Scales 5.2.1 |
| Full Persian technical guide | Available in `docs/reference/` |
| C# integration SDK | Planned for v1.0 |
| Raw SQL samples | Planned |
| Python / Node.js / Java / PHP samples | Planned |
| GitHub Releases | Planned |
| REST / Webhook gateway | Future generation; not part of 5.2.1 / Contract v1 |

## What this repository is for

Software vendors should integrate with **Sadr Scales**, not reimplement the proprietary wire protocol of each scale model.

```text
POS / ERP / Accounting
        ↓
Sadr Scales SQL Contract v1 / Integration SDK
        ↓
Sadr Scales Runtime
        ↓
Supported scale models
```

The public contract allows an external application to:

- create/update item groups and PLUs;
- read accepted sales from Sadr Scales;
- use structured invoice data in controlled scenarios;
- validate the expected database schema;
- keep its own durable sales cursor/state;
- integrate without knowing the direct communication protocol of PLUS, LSG, Aclas or other devices.

## Quick contract summary

Public SQL Contract v1 is centered on:

- `dbo.SADR_ItemClass` — item groups, read/write;
- `dbo.SADR_Item` — PLU/item master, read/write;
- `dbo.SADR_Logs` — accepted sales feed, **read-only**.

Registry, internal synchronization state, device sessions and proprietary communication protocols are not part of the basic public contract.

See [SQL Contract v1](docs/en/sql-contract-v1.md) and the [Persian contract](docs/fa/sql-contract-v1.md).

## Documentation

- [Project status](docs/PROJECT_STATUS.md) — current work and exact next step.
- [Roadmap](docs/ROADMAP.md) — planned milestones through v1.0 and beyond.
- [Decision log](docs/DECISIONS.md) — accepted architectural/product decisions.
- [Backlog](docs/BACKLOG.md) — actionable work items.
- [Work log](docs/WORK_LOG.md) — chronological engineering/handoff record.
- [GitHub bootstrap](docs/GITHUB_SETUP.md) — safe first-push instructions.
- [Security boundary](docs/SECURITY_BOUNDARY.md) — what may and may not be public.
- [Compatibility](docs/COMPATIBILITY.md) — Sadr Scales / Contract / SDK compatibility.
- [Release policy](docs/RELEASE_POLICY.md) — versioning and GitHub Release contents.
- [Full Persian Integration & Database Guide](docs/reference/README.md).

## Planned developer experience

The v1.0 C# client should reduce integration to a small, explicit API while keeping the complete source code public. A target usage shape is:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.Items.UpsertAsync(item);

var sales = await client.Sales.ReadAfterAsync(lastProcessedId, batchSize: 100);
```

The final API is **not frozen yet**. API design and target frameworks are tracked in the roadmap and decision log.

## Releases

Release binaries should be published through **GitHub Releases**, not committed into the main branch. A release is expected to include the SDK package/DLL, XML docs where applicable, sample bundle, technical guide, changelog and SHA-256 checksums.

## Security

This repository intentionally excludes direct device protocols, packet captures, private keys, credentials, customer data, proprietary firmware/vendor material and internal Sadr Scales release infrastructure. Read [SECURITY.md](SECURITY.md) before contributing.

## License

No open-source license has been granted yet. The project intends to use a permissive license, but the exact license requires explicit company approval before public SDK distribution. Until a `LICENSE` file is added, all rights remain reserved. See [NOTICE.md](NOTICE.md).
