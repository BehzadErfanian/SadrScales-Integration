# Scale Assignments, Per-Scale Mapping and Group HotKeys

This guide covers the additive Vendor-Ready `1.1.0` SQL integration surface for Sadr Scales `5.2.1`.

The SDK deliberately keeps three concepts separate:

```text
ScaleAssignments  -> item groups assigned to one registered scale
ScaleMappings     -> per-scale PLU / ItemCode / optional HotKey position mapping
HotKeys           -> user-managed HotKey template owned by an item group
```

## Scale assignments

Read the canonical groups for one scale:

```csharp
var groups = await client.ScaleAssignments.GetGroupsAsync(scaleId);
```

Replace the complete assignment set atomically:

```csharp
var result = await client.ScaleAssignments.ReplaceGroupsAsync(
    scaleId,
    new[] { "FOOD", "FRUIT" });
```

The result is `NotFound`, `Unchanged`, or `Replaced`.

Rules:

- at least one valid existing item group is required;
- the supplied collection represents the complete new assignment set;
- an unchanged replacement does not create a new resend request;
- a real change resets the selected scale's Item AutoSend state so a later eligible AutoSend cycle re-evaluates the assigned catalog.

## Per-scale item mapping

A mapping entry can include a scale-specific item code and an optional HotKey position:

```csharp
var map = new SadrScaleItemMap
{
    PluNo = 1001,
    ItemCode = 1,
    PageNo = 0,
    KeyNo = 1
};
```

Read:

```csharp
var mappings = await client.ScaleMappings.GetAsync(scaleId);
```

Replace the complete mapping:

```csharp
var result = await client.ScaleMappings.ReplaceAsync(
    scaleId,
    new[]
    {
        new SadrScaleItemMap { PluNo = 1001, ItemCode = 1 },
        new SadrScaleItemMap { PluNo = 1002, ItemCode = 2, PageNo = 0, KeyNo = 1 }
    });
```

Copy one scale's mapping to another scale:

```csharp
var result = await client.ScaleMappings.CopyAsync(sourceScaleId, destinationScaleId);
```

Validation includes:

- positive existing PLU numbers;
- positive unique per-scale ItemCodes;
- unique PLUs per scale;
- unique PageNo + KeyNo positions;
- PageNo and KeyNo must be both null or both populated;
- populated HotKey positions must fit the target scale's persisted HotKey layout;
- an incompatible copy leaves the destination unchanged.

A real mapping change resets both Item and HotKey AutoSend state for the affected scale. `Replaced` means the SQL configuration was committed; it does not mean the physical device has already received the data.

## Group HotKey templates

Read user-managed keys for one group:

```csharp
var hotKeys = await client.HotKeys.GetGroupAsync("FOOD");
```

Replace the complete user-managed template:

```csharp
var result = await client.HotKeys.ReplaceGroupAsync(
    "FOOD",
    new[]
    {
        new SadrHotKey { PageNo = 0, KeyNo = 1, PluNo = 1001 },
        new SadrHotKey { PageNo = 0, KeyNo = 2, PluNo = 1002 }
    });
```

### Internal/system HotKey rows

Some model-specific database rows can use zero or negative PLU values for internal behavior. They are not public business HotKeys.

The public API therefore manages only rows with `PluNo > 0`:

- `GetGroupAsync` does not expose internal/system rows;
- `ReplaceGroupAsync` does not delete them;
- third-party software should not modify them.

A real user-template change resets HotKey AutoSend state only for scales canonically assigned to that group.

## Group HotKeys are not scale mappings

A group HotKey template is shared group configuration:

```text
Group FOOD
Page 0 / Key 1 -> PLU 1001
```

A scale mapping is specific to one scale:

```text
Scale 03
PLU 1001 -> ItemCode 15 -> Page 0 / Key 3
```

Keeping them separate avoids hiding model/layout constraints inside a global group template.

## AutoSend semantics

These SQL APIs do not directly execute a device protocol operation.

```text
Replace scale assignments -> reset Item resend state
Replace scale mapping     -> reset Item + HotKey resend state
Replace group HotKeys     -> reset HotKey resend state for assigned scales
```

Actual transfer occurs during a later eligible Sadr Scales AutoSend cycle.

## Recommended vendor flow

```text
1. Read current configuration.
2. Build the complete desired replacement in the destination application.
3. Call one Replace operation.
4. Inspect NotFound / Unchanged / Replaced.
5. If Replaced, allow Sadr Scales AutoSend to perform the later device transfer.
```

Avoid ad-hoc sequences of direct INSERT/DELETE statements. Atomic replace prevents partially applied configuration from remaining in the database.

## Raw SQL

Non-C# consumers can use the guarded reference recipe:

```text
samples/SQL/06-assignments-mapping-hotkeys.sql
```

It is read-only by default. Writes require explicit `@ApplyChanges = 1` plus the specific replacement flag.

## 5.2.1 boundary

This guide covers operations that are safely available through the current SQL integration surface. Immediate device commands, Runtime completion/progress and the planned per-scale Command Mailbox are outside the 5.2.1 Vendor-Ready contract. The Command Mailbox is planned for Sadr Scales 5.3.
