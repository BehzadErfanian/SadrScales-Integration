# Compatibility

## Current compatibility baseline

| Integration repository / SDK | Sadr Scales | Public contract | Consumer runtime | Status |
|---|---:|---|---|---|
| pre-1.0 | 5.2.1 | SQL Contract v1 | modern .NET via `netstandard2.0` | CI verified |
| pre-1.0 | 5.2.1 | SQL Contract v1 | .NET Framework 4.8 | **package restore/build/runtime verified** |
| 1.x (planned) | 5.2.1+ within verified compatibility | SQL Contract v1 | declared per release | Planned |

## .NET Framework 4.8 verification

The SDK library targets `netstandard2.0`. Compatibility is not claimed from framework tables alone: CI builds the actual SDK NuGet package and consumes that package from a separate `net48` application on Windows Server 2022.

Validated package-consumer gate:

- generated package: `SadrScales.Integration 0.1.0-alpha.1`;
- package restore into a real `net48` Console application: PASS;
- consumer build: PASS with **0 warnings / 0 errors** and warnings treated as errors;
- consumer runtime execution under .NET Framework 4.8: PASS;
- loaded SDK assembly: `SadrScales.Integration, Version=0.1.0.0`;
- loaded provider assembly from the restored dependency graph: `Microsoft.Data.SqlClient, Version=7.0.0.0`.

The compatibility smoke application intentionally does not connect to SQL Server. SQL behavior is independently covered by the disposable SQL Server 2022 integration-test job, which exercises the real Contract v1 schema/data path.

## Rules

1. A public SDK release must declare the Sadr Scales versions against which it was verified.
2. Runtime/framework support claims must be backed by an actual consumer build/runtime gate, not only by target-framework compatibility tables.
3. A Sadr Scales update that does not change the public contract should not force a new contract version.
4. A genuinely new public interface must be separately versioned and must not silently redefine Contract v1.
5. Registry/Mapping/structured-sales features remain advanced/controlled unless explicitly promoted by a future public contract decision.
