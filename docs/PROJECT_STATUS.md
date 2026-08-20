# Project Status — SadrScales-Integration

**Last updated:** 2026-08-20  
**Phase:** Vendor-Ready 5.2.1 — `1.1.0` RC + Integration Lab completion  
**Current stable release:** `v1.0.0`  
**Next Vendor-Ready package version:** `1.1.0`  
**Supported Sadr Scales baseline:** `5.2.1`

## Canonical references

1. `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md`
2. `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`
3. `.github/maintainers/INTEGRATION_CONTRACT_DOMAIN_DESIGN_FA.md`
4. `docs/DECISIONS.md`
5. this file

Chat history is not the project source of truth.

## Business objective

Deliver an Integration Platform that a real software vendor can understand, run and validate without needing proprietary protocol knowledge or physical scale hardware for the normal development loop.

Current execution sequence:

```text
Freeze/verify 1.1.0 RC
        ↓
Complete private Internal Scale Simulator behavior
        ↓
Extract approved safe Public Scale Emulator
        ↓
Complete POS/Retail Integration Lab
        ↓
Run one serious external vendor acceptance
        ↓
Feed validated future requirements into Sadr Scales 5.3
```

Sadr Scales 5.3 remains the next runtime/service evolution, but the current vendor-lab chain should be closed first so the near-complete Integration work is not left unfinished.

## Frozen 5.2.1 integration scope

```text
Connection/schema validation
Stores
Item Groups
Items / PLUs
Price History read
Static Scale read
Online / Offline status
Scale Group Assignment
Scale Item Mapping
Group HotKey Template
Item AutoSend resend request
supported HotKey AutoSend resend request
Sales Feed
Sales Query / Summary
Structured Invoice lookup
Invoice ACK
Daily / Scale / Item Reports
```

## Completed Vendor-Ready slices

### Slice 1 — Structured Invoice + ACK
Merged and green. Includes lookup by TotalBarcode and ScaleID+FID, full details, explicit idempotent ACK, `FoundUnread / AlreadyRead / NotFound`, Raw SQL, docs, Sample and SQL/net48 tests.

### Slice 2 — Scales + Status + Resend
Merged and green. Includes scale metadata/status and Item/HotKey AutoSend resend requests without exposing internal watermarks.

### Slice 3 — Stores + Catalog
Merged on PR #17 and green. Includes Store/Group/Item reads and writes, bounded batch, logical delete, Price History read, Raw SQL, docs and Sample.

### Slice 4 — Assignments + Mapping + HotKeys
Merged on PR #18 and green. Includes canonical multi-group assignment, validated per-scale mapping/Copy, user group HotKey templates, system-row preservation and correct resend-state behavior.

### Slice 5 — Sales Query + Reports
Merged on PR #19. Main merge commit: `230f73938181a6eb8ebe06a44832e08a8c502e7e`.

Includes:
- existing incremental `Sales.ReadAfterAsync` Feed unchanged;
- newest-first filtered/paged `Sales.QueryAsync`;
- complete-filter Summary;
- distinct invoice count by `(DeviceNo, FID)`;
- Today / Saturday-week / Persian-month helpers;
- typed Daily / Scale / Item reports;
- Raw SQL, docs, Sample, SQL tests and net48 coverage.

### Slice 6 — Demo Data + Vendor Acceptance + RC
Merged on PR #20. Main head after merge was verified as `634910288455c1da1f940d085888728afd17fc6e` during the management review on 2026-08-20.

Includes:
- guarded deterministic DemoLab;
- synthetic test Stores / Groups / Items / Scales / Assignments / Mappings / HotKeys / Sales / Invoices;
- package-only `SadrScales.Integration.VendorAcceptance` with no SDK ProjectReference;
- external-developer flow through catalog/config/status/sales/invoice/ACK/recovery;
- vendor-acceptance CI gate;
- simplified English/Persian vendor path.

## RC freeze rule

The `1.1.0` public contract/naming is frozen after the RC merge. Before vendor outreach, only these changes are permitted:

- bug fixes;
- security fixes;
- compatibility fixes;
- documentation corrections that do not change the frozen contract.

New Lab/Emulator tooling must not casually expand or break the frozen SQL contract.

## Current P0 follow-up — complete the developer test environment

The canonical master plan already separates these products and phases. They are now the immediate execution priority.

### 1. Developer Sample App
Role:
- public educational source;
- simple SDK learning/copy-paste path;
- only approved public APIs;
- not a protocol/debug tool.

### 2. Private Internal Scale Simulator
Role:
- internal engineering/protocol regression tool;
- complete supported PLUS/LSG observable behavior needed by production and Lab tests;
- cross-check against physical/protocol truth;
- retain private stress/fault/debug capabilities;
- freeze an approved subset for public extraction.

Truth order:

`Physical device/capture -> protocol docs -> production -> private simulator`.

### 3. Public Scale Emulator
Role:
- separate vendor-facing virtual scale product;
- derived only after private behavior is validated/frozen;
- allow realistic development without physical hardware;
- no proprietary protocol source/captures/packet-debug surfaces;
- protected/obfuscated/signed binary boundary where required.

### 4. POS/Retail Simulator
Role:
- simulate the destination software side;
- exercise Sales Feed, Query/Reports, invoice lookup, destination Save+Commit, ACK, restart, duplicate, cursor and recovery behavior;
- complement the educational Sample App rather than replace it.

### 5. One-PC Integration Lab
Target topology:

```text
Developer Sample / POS Simulator
        ↓
SadrScales Integration SDK / SQL Contract
        ↓
SadrScales test environment
        ↓
Public Scale Emulator
```

Target outcome: a vendor can validate the supported integration workflow without a physical scale.

### 6. External Developer Acceptance
Choose one serious software vendor, deliver the Lab/package, observe setup friction and real usage, then classify feedback as:
- `1.1.x` bug/security/compatibility/documentation;
- future Sadr Scales `5.3` / SadrPlatform capability.

## SDK / Integration evolution

The Integration Domain is intentionally not defined as “SQL forever”.

Current generation:
- SQL Contract;
- C# SDK;
- Raw SQL reference;
- Developer Sample;
- Vendor Acceptance;
- Integration Lab.

Future additive evolution may include:
- typed managed per-scale commands / Command Mailbox;
- Sadr Scales Service / REST transport;
- realtime events;
- Webhook/adapters;
- later SadrPlatform transports.

The same business concepts should remain recognizable across transports. Avoid parallel competing domain models for SQL and REST.

## Important integration decisions

- SQL is the current third-party transport for Sadr Scales 5.2.1.
- `SADR_Scale.Status` is the supported coarse SQL status source.
- invoice lookup never auto-ACKs;
- destination Save/Commit happens before ACK;
- ACK sets `SADR_Total.LableStatus = 1` and is idempotent;
- `AlreadyRead` never blocks full invoice recovery;
- Sales Feed cursor belongs to the destination application;
- Sales Query is separate from Feed synchronization;
- public HotKey APIs preserve internal zero/negative system rows;
- DemoLab is not a production SDK surface;
- production Scale lifecycle and arbitrary Runtime commands are not exposed through 5.2.1 raw SQL.

## Sadr Scales 5.3 follow-up

A typed per-scale Command Mailbox remains planned for 5.3. Runtime owns validation, licensing, connection/busy state, model capability and protocol execution. Service/REST should later become another transport over the same Integration Domain rather than a competing command model.

## Security boundary

The public repository and vendor-facing tooling must not expose proprietary device protocols, raw packets/captures, reverse-engineering notes, private keys, customer production data, private firmware/vendor material or arbitrary raw protocol execution.

## Exact next step

1. Confirm the final live `1.1.0` publication/release/package evidence.
2. Audit/complete the private SadrScaleSimulator capability matrix required by the Lab.
3. Freeze the approved public emulator behavior subset.
4. Build the Public Scale Emulator + POS/Retail Lab.
5. Run first serious external vendor acceptance.
