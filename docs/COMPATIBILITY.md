# Compatibility

## Stable v1 compatibility baseline

| SDK | Sadr Scales | Public contract | Consumer/runtime evidence | Status |
|---|---:|---|---|---|
| `1.0.0` | `5.2.1` | SQL Contract v1 | modern .NET via `netstandard2.0` Quick Start | CI verified |
| `1.0.0` | `5.2.1` | SQL Contract v1 | real .NET Framework 4.8 NuGet-package consumer | CI verified |
| `1.x` | later Sadr Scales release explicitly verified as compatible | SQL Contract v1 | must remain covered by release CI | compatibility-by-evidence |

A product version number alone does not automatically prove Contract v1 compatibility. A later Sadr Scales version must be explicitly validated/documented before it is added to the supported matrix.

## .NET Framework 4.8 verification

The SDK library targets `netstandard2.0`. Compatibility is not claimed from framework tables alone: CI builds the actual `SadrScales.Integration 1.0.0` NuGet package and consumes that package from a separate `net48` application on Windows Server 2022.

Release gate:

- package restore into a real `net48` Console application: PASS;
- consumer build with warnings treated as errors: PASS;
- consumer runtime execution under .NET Framework 4.8: PASS;
- SDK assembly loads through the package dependency graph: PASS;
- `Microsoft.Data.SqlClient` dependency graph loads: PASS.

The net48 smoke consumer intentionally does not need a production SQL Server. SQL behavior is independently covered by the disposable SQL Server 2022 integration-test job using synthetic Contract v1 schema/data.

## Modern .NET verification

The executable C# Quick Start targets modern .NET and is restored/built by CI from repository source. It validates the public usage shape without embedding credentials.

## SQL Server verification

CI provisions a disposable SQL Server 2022 container and exercises the real SDK against a synthetic Contract v1 schema/data set.

The SQL-backed suite validates contract behavior, item/group writes, semantic no-op behavior, batch aggregate results and rollback behavior without using customer data.

## Public API compatibility

`1.0.0` establishes the first stable SDK API baseline. See [`API_COMPATIBILITY.md`](API_COMPATIBILITY.md).

Key rules:

1. SDK and SQL Contract versions are separate.
2. SDK `1.x` minor/patch releases must not intentionally break supported public 1.x API consumers.
3. A genuinely breaking SDK change requires a major SDK version.
4. A genuinely breaking public database change requires a new SQL Contract version.
5. Runtime/framework claims stay backed by executable package-consumer/build/runtime gates.

## Strong-name status

SDK `1.0.0` is not strong-name signed. The supported net48 package consumer does not require it. Strong naming is treated as an assembly-identity compatibility choice rather than a security feature; a future change requires a concrete supported-consumer need and explicit compatibility decision.

## Advanced surfaces

Registry, Mapping, structured-sales internals, runtime state and direct device protocols remain outside the basic public Contract v1/SDK compatibility promise unless explicitly promoted by a future public contract decision.
