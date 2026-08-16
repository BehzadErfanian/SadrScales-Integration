# Sadr Scales Integration

[![Sadr](assets/sadr-logo.png)](https://sadrgroup.ir)

**Official public integration toolkit and SQL contract for connecting POS, ERP and accounting software to Sadr Scales.**

[فارسی](README.fa.md) · [Project Status](docs/PROJECT_STATUS.md) · [Roadmap](docs/ROADMAP.md) · [Security](SECURITY.md)

---

## Status

This repository is currently in **Foundation / pre-1.0** stage. The public SQL integration contract for **Sadr Scales 5.2.1** is defined as **SQL Contract v1**. The first SDK and language samples are planned and tracked in the project roadmap.

## What this project is

`SadrScales-Integration` is the public developer repository for software vendors that need to integrate POS, ERP and accounting systems with Sadr Scales without implementing proprietary scale protocols.

The integration boundary is intentionally:

```text
POS / ERP / Accounting
        ↓
Sadr Scales SQL Contract v1
        ↓
Sadr Scales Runtime
        ↓
Supported scale models
```

Sadr Scales remains responsible for device sessions, retries, reconnects, protocol differences and communication with the scales.

## Public Contract v1

The public contract allows an external application to:

- create/update item groups;
- create/update PLUs/items;
- read sales incrementally;
- use stable consumer-side cursors;
- inspect the documented schema contract;
- use controlled advanced schema areas when required and explicitly understood.

Public SQL Contract v1 is centered on:

- `dbo.SADR_ItemClass`
- `dbo.SADR_Item`
- read-only `dbo.SADR_Logs`

Registry, internal synchronization state, device sessions and proprietary communication protocols are not part of the basic public contract.

## Documentation

- [Persian Quick Start](docs/fa/getting-started.md)
- [English Quick Start](docs/en/getting-started.md)
- [SQL Contract v1 — Persian](docs/fa/sql-contract-v1.md)
- [SQL Contract v1 — English](docs/en/sql-contract-v1.md)
- [Full Persian Integration & Database Guide](docs/reference/README.md)
- [Compatibility](docs/COMPATIBILITY.md)
- [Project status](docs/PROJECT_STATUS.md) — current work and exact next step.
- [Roadmap](docs/ROADMAP.md)
- [Backlog](docs/BACKLOG.md)
- [Decision log](docs/DECISIONS.md)
- [Security boundary](docs/SECURITY_BOUNDARY.md) — what may and may not be public.

## Planned SDK

The v1.0 C# client should reduce integration to a small, explicit API while keeping the complete source code public. A target usage shape is:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.Items.UpsertAsync(item);

var sales = await client.Sales.ReadAfterAsync(lastProcessedId);
```

This API is **not frozen yet**. See the roadmap before building against pre-1.0 source.

## Samples

Sample areas are prepared for C#, SQL, Python, Node.js, Java and PHP. Executable samples will be added after Contract v1 validation is frozen.

## Releases

Release binaries should be published through **GitHub Releases**, not committed into the main branch. A release is expected to include the SDK package/DLL, XML docs where applicable, sample bundle, technical guide, changelog and SHA-256 checksums.

## Security boundary

This repository intentionally excludes direct device protocols, packet captures, private keys, credentials, customer data, proprietary firmware/vendor material and internal Sadr Scales release infrastructure. Read [SECURITY.md](SECURITY.md) before contributing.

## License

No open-source license has been granted yet. The project intends to use a permissive license, but the exact license requires explicit company approval before public SDK distribution. Until a `LICENSE` file is added, all rights remain reserved. See [NOTICE.md](NOTICE.md).
