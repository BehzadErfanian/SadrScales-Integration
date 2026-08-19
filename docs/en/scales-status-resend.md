# Scales, coarse status and AutoSend resend requests

This Vendor-Ready capability targets **Sadr Scales 5.2.1 / SadrScales.Integration 1.1.0**.

## Purpose

A POS/ERP integration can:

- read registered scales without knowing a device protocol;
- read the coarse SQL-visible connection status;
- request that items become pending for a later AutoSend cycle;
- request the same for HotKeys on models whose 5.2.1 automatic HotKey path supports it.

## Read registered scales

```csharp
var client = new SadrScalesClient(connectionString);

var scales = await client.Scales.GetAllAsync();
var scale = await client.Scales.GetAsync(3);
var status = await client.Scales.GetStatusAsync(3);
```

The public model exposes business-useful registration metadata such as Scale ID, IP, port, model, display name, store, enabled state and AutoSend configuration.

`PrimaryItemGroupCode` is the legacy primary value persisted on the scale row. Canonical multi-group assignment is exposed separately by the Scale Assignments capability; callers must not treat the two representations as the same contract.

## Coarse status

Sadr Scales 5.2.1 persists coarse connection state in `SADR_Scale.Status`. The SDK maps it to only:

```text
Online
Offline
Unknown
```

`Unknown` covers a missing scale and empty/unrecognized SQL values. Transient internal states such as Connecting, operation progress and last runtime error are not part of the 5.2.1 SQL contract.

## Request item resend

```csharp
SadrResendRequestResult result =
    await client.Scales.RequestItemResendAsync(scaleId);
```

The SQL implementation resets the internal item-send watermark so the data can be considered again by a later automatic send cycle.

Possible results:

```text
Requested
NotFound
```

**Requested does not mean the physical scale has received the data.** It means the SQL resend state was recorded. Actual transfer requires a later eligible AutoSend cycle and its normal runtime conditions.

## Request HotKey resend

```csharp
SadrResendRequestResult result =
    await client.Scales.RequestHotKeyResendAsync(scaleId);
```

Possible results:

```text
Requested
NotFound
UnsupportedModel
```

For the 5.2.1 automatic HotKey path, registered model categories `LSG`, `LSG_24D`, `TSG` and `LS6` are supported. `PLUS` returns `UnsupportedModel` and its key-send watermark is left unchanged; the SDK never reports false success.

## Resend request versus immediate command

```text
RequestItemResend / RequestHotKeyResend
= make data pending for a later AutoSend cycle

Immediate SendItems / SendHotKeys
= runtime command with the result of that specific operation
```

Immediate device commands are outside the current 5.2.1 SQL surface. A typed Command Mailbox is planned for Sadr Scales 5.3.

## Raw SQL

Non-C# consumers can review:

[`samples/SQL/04-scale-status-resend.sql`](../../samples/SQL/04-scale-status-resend.sql)

The sample is read-only by default. It changes resend state only after `@ApplyResend = 1` is explicitly selected.

## Safety rules

- Use parameterized SQL from application code.
- Do not display `Requested` as physical-device transfer success.
- Keep protocol packets/opcodes outside the public integration boundary.
- Use future Command/Service capabilities when an external application needs a precise immediate-operation result.
