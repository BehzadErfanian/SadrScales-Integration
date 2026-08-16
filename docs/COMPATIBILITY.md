# Compatibility

## Current compatibility baseline

| Integration repository / SDK | Sadr Scales | Public contract | Consumer runtime | Status |
|---|---:|---|---|---|
| pre-1.0 | 5.2.1 | SQL Contract v1 | modern .NET via `netstandard2.0` | CI verified |
| pre-1.0 | 5.2.1 | SQL Contract v1 | .NET Framework 4.8 | compatibility gate in progress |
| 1.x (planned) | 5.2.1+ within verified compatibility | SQL Contract v1 | declared per release | Planned |

## .NET Framework compatibility rule

The SDK library targets `netstandard2.0`. Microsoft documents .NET Framework 4.8 as supporting .NET Standard 2.0, and recommends .NET Framework 4.7.2 or later for consuming .NET Standard 2.0 libraries.

`Microsoft.Data.SqlClient 7.0.2` also publishes direct .NET Framework 4.6.2+ and .NET Standard 2.0 package assets.

Before v1.0, CI must prove the packaged SDK can be restored, built and executed by a real `net48` consumer application on Windows. This test consumes the generated NuGet package rather than using a project reference.

## Rules

1. A public SDK release must declare the Sadr Scales versions against which it was verified.
2. Runtime/framework support claims must be backed by an actual consumer build/runtime gate, not only by target-framework compatibility tables.
3. A Sadr Scales update that does not change the public contract should not force a new contract version.
4. A genuinely new public interface must be separately versioned and must not silently redefine Contract v1.
5. Registry/Mapping/structured-sales features remain advanced/controlled unless explicitly promoted by a future public contract decision.
