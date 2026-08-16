# Compatibility

## Current compatibility baseline

| Integration repository | Sadr Scales | Public contract | Status |
|---|---:|---|---|
| pre-1.0 | 5.2.1 | SQL Contract v1 | Documentation foundation |
| 1.x (planned) | 5.2.1+ within verified compatibility | SQL Contract v1 | Planned |

## Rules

1. A public SDK release must declare the Sadr Scales versions against which it was verified.
2. A Sadr Scales update that does not change the public contract should not force a new contract version.
3. A genuinely new public interface (for example a future REST/Webhook gateway) must be separately versioned and must not silently redefine Contract v1.
4. Registry/Mapping/structured-sales features are advanced/controlled unless explicitly promoted into a future public contract decision.
5. Compatibility claims must be backed by tests or explicit source/schema verification, not assumption.
