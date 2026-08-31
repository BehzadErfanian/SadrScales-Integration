# Project Status

**Updated:** 2026-08-31

## Current public release

- Stable release: **`v1.1.0`**.
- Source/tag target: `d79fe6b359f25e93d12edfc4970777a4c3d06efc`.
- SDK package: `SadrScales.Integration.1.1.0.nupkg`.
- Published package SHA256: `2baa100d6cf3125c75edbb7e99e1d15ff3e99d0bcd52534180ebe3f29d9d359f`.
- SDK target: `netstandard2.0`.
- Proven package consumers: .NET Framework 4.8 and modern .NET.

## Contract identity

The public 1.1 SQL/SDK contract remains frozen against the validated **Sadr Scales 5.2.1** integration surface.

Sadr Scales **5.3** is now the current Stable application release. A post-release clean-room/compatibility rehearsal is in progress to prove that the frozen 1.1 contract continues to operate correctly on 5.3. The application version change does not silently redefine or expand the public Integration contract.

## Completed 1.1 Vendor-Ready scope

- Stores / Item Groups / Items/PLUs.
- bounded atomic item batch writes and logical delete behavior.
- Price History read.
- registered Scales and coarse `Online / Offline / Unknown` status.
- Scale Group Assignments.
- per-scale Item Mapping.
- group HotKey templates.
- Item/HotKey resend requests through approved Sadr AutoSend behavior.
- incremental Sales Feed with destination-owned cursor.
- Sales Query / Summary / typed reports.
- structured Invoice lookup by TotalBarcode and ScaleID + FID.
- explicit idempotent Invoice ACK.
- complete `AlreadyRead` recovery.
- guarded DemoLab.
- bilingual docs, executable samples and Raw SQL recipes.

## Accepted release quality evidence

The 1.1 release line has passed:
- SDK build/test/package;
- NuGet package metadata/shape/Source Link validation;
- disposable SQL Server 2022 integration tests;
- WinForms Developer Sample build;
- .NET Framework 4.8 package consumer;
- package-only Vendor Acceptance with no SDK ProjectReference;
- Public Repository Guard;
- release bundle/manifest/checksum validation.

## Current post-release phase

The current goal is not another feature expansion. It is a **human vendor rehearsal using the published package bytes** plus Sadr Scales 5.3, Developer Simulator and POS Integration Lab.

The internal management test plan is resumable and risk-based. It verifies:
- exact released artifact identity;
- package-only clean-room consumption;
- 5.3 schema/SDK compatibility;
- T-Plus and LSG invoice/ACK flows;
- restart/failure/duplicate recovery;
- public-documentation usability;
- exact vendor-safe delivery bundle;
- one serious external software-vendor acceptance.

## Documentation state

This branch corrects stale pre-release wording that still described `v1.0.0` as current and `1.1.0` as upcoming.

The full Developer Guide and external website Developer page should be finalized after the internal 5.3/package rehearsal so wording reflects the tested vendor experience rather than assumptions.

## Future architecture

Future typed managed commands, Service/REST, realtime/Webhook and Integration Gateway capabilities belong to **Sadr Scales 5.4+** architecture (or the next unused public version if 5.4 is consumed first by maintenance work).

They should extend the existing Integration Domain rather than create competing business concepts.

## Security boundary

This public repository intentionally excludes private Sadr runtime source, direct scale wire protocols, packets/captures/reverse-engineering material, private keys/credentials, customer production data and vendor-confidential material.

## Exact next step

Complete the published-package clean-room/5.3 compatibility rehearsal, incorporate any documentation friction, then finalize the Developer Guide and prepare the first serious external vendor acceptance.
