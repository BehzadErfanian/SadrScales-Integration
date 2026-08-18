# SDK API Compatibility Policy

This policy applies to the public `SadrScales.Integration` SDK beginning with stable `1.0.0`.

## Versioning layers

Two versions are intentionally separate:

1. **SDK version** — Semantic Versioning for the C# package/public API.
2. **SQL Contract version** — the database integration contract exposed by Sadr Scales.

SDK `1.x` is built for **SQL Contract v1**. A new SDK minor/patch release does not by itself create a new SQL contract.

## SDK Semantic Versioning

After `1.0.0`:

- **PATCH** (`1.0.x`) — compatible fixes, diagnostics, documentation or package corrections.
- **MINOR** (`1.x.0`) — backward-compatible public SDK additions.
- **MAJOR** (`2.0.0`) — intentional breaking public SDK behavior/API changes.

For the `1.x` line, existing supported public API members are not intentionally removed, renamed or changed incompatibly in a minor/patch release.

## SQL Contract compatibility

SQL Contract v1 is independently frozen for its documented basic surface.

Breaking changes to the public database contract require an explicitly new SQL Contract version. Existing Contract v1 consumers must not be silently reinterpreted as a different incompatible schema.

A later Sadr Scales release may still be compatible with Contract v1. Compatibility must be explicitly validated/documented rather than inferred only from the Sadr Scales product version.

## Package validation

The SDK enables .NET package validation during pack/build hardening.

`1.0.0` establishes the first stable API baseline. After the stable package exists, later `1.x` release engineering should compare the candidate package against the previous stable/baseline package and treat unexpected API-compatibility findings as release blockers until reviewed.

## Runtime compatibility claims

A target framework declaration alone is not considered sufficient evidence.

The v1 release gates include:

- real package restore/build/runtime in a `.NET Framework 4.8` consumer application;
- modern .NET Quick Start build;
- real SQL Server 2022 integration tests using synthetic Contract v1 data.

Compatibility claims should remain backed by executable CI evidence.

## Transaction/retry compatibility

The following behaviors are part of the v1 contract and should not be silently weakened:

- sales reads are read-only and destination-cursor-owned;
- transaction-scoped writes are not automatically replayed after execution begins;
- bounded automatic retry is limited to safe connection/read boundaries;
- `UpsertBatchAsync` has an explicit maximum of 200 unique PLUs per atomic call;
- semantic no-op item writes remain distinguishable from real updates.

## Strong-name identity

SDK `1.0.0` is intentionally **not strong-name signed**. The current tested consumer surface does not require it, and a strong-name identity would introduce a long-lived signing-key/assembly-identity compatibility commitment.

This is not a security downgrade: strong names are an assembly identity mechanism, not a publisher-trust/security boundary. If a future supported .NET Framework consumer has a concrete strong-name requirement, that change must be evaluated as an explicit compatibility decision rather than added casually.

## Deprecation

When feasible, a public API that needs replacement should first be documented/deprecated with a supported alternative before removal in a future major release.

## Source of truth

The public GitHub repository, tagged release source, release notes and this policy define the supported SDK surface. Proprietary device wire protocols and internal Sadr Scales runtime APIs are outside this compatibility promise.
