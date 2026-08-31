# Roadmap

**Updated:** 2026-08-31

## M0 — Public repository foundation — complete
- [x] Public repository, bilingual landing pages, security boundary and continuity docs.
- [x] Support/contribution/code-of-conduct files.
- [x] CODEOWNERS and hardened Issue/PR intake.
- [x] Dependency/security maintenance policy.

## M1 — Contract v1 / public documentation — complete
- [x] Freeze Contract v1 against effective Sadr Scales 5.2.1 schema/behavior.
- [x] Bilingual contract docs, SQL samples and regression guidance.
- [x] Getting Started and troubleshooting paths.
- [x] production-readiness/go-live checklist.
- [x] stable public Guide/release evidence for the original v1 line.

## M2 — C# Integration SDK v1 — complete
- [x] `netstandard2.0` SDK.
- [x] contract validator and semantic Stores/Groups/Items operations.
- [x] bounded atomic PLU batch writes.
- [x] retry/reliability rules.
- [x] Sales Feed, Query, Summary and reports.
- [x] scale metadata/status/resend.
- [x] Assignments / Mapping / HotKeys.
- [x] structured Invoice + explicit idempotent ACK + `AlreadyRead` recovery.
- [x] disposable SQL Server integration suite.
- [x] .NET Framework 4.8 package consumer.
- [x] C# Quick Start + WinForms Developer Sample.
- [x] Raw SQL language-neutral path.
- [x] guarded DemoLab.
- [x] NuGet validation/Source Link/repository metadata.
- [x] automated release bundles/manifest/checksums.
- [x] Public Repository Guard and protected release flow.

## M3 — Stable `v1.0.0` publication — complete / historical
The original stable release remains immutable historical evidence.

## M4 — Vendor-Ready `v1.1.0` — complete / current stable
- [x] additive Vendor-Ready contract surface completed.
- [x] package-only Vendor Acceptance with no SDK ProjectReference.
- [x] PR/release-engineering hardening.
- [x] exact accepted source `d79fe6b359f25e93d12edfc4970777a4c3d06efc`.
- [x] tag and publish `v1.1.0`.
- [x] stable NuGet package published in release assets.
- [x] current public package SHA256 `2baa100d6cf3125c75edbb7e99e1d15ff3e99d0bcd52534180ebe3f29d9d359f`.

The frozen public SQL/SDK contract remains the validated Sadr Scales 5.2.1 integration surface.

## M5 — Post-release Sadr Scales 5.3 compatibility / vendor rehearsal — current

Sadr Scales 5.3 is now the Stable application release. The next credibility gate is a human rehearsal using the **published 1.1.0 package bytes** rather than a source-built substitute.

- [ ] published-package clean-room restore/build/run;
- [ ] frozen contract validation on a safe Sadr Scales 5.3 database;
- [ ] Store/Group/Item/Scale/Sales smoke on 5.3;
- [ ] T-Plus structured invoice / TotalBarcode / ACK / `AlreadyRead` path;
- [ ] LSG structured invoice / TotalBarcode / ACK / `AlreadyRead` path;
- [ ] targeted POS restart/failure/duplicate recovery regression;
- [ ] public-documentation-only usability walk;
- [ ] record all setup/docs friction before next vendor outreach.

This is a compatibility/usability delta. It does not silently rename or expand the frozen 1.1 contract.

## M6 — Developer documentation and external vendor acceptance — next

After the internal 5.3/package rehearsal:
- [ ] finalize current Developer Guide from tested setup/flows;
- [ ] update the Sadr website Developers page;
- [ ] assemble exact vendor-safe package/docs/simulator/sample set;
- [ ] run a security/public-boundary audit;
- [ ] select one serious external software vendor;
- [ ] measure time/questions/friction through first package build, contract validation, item, Sales Feed, invoice and correct ACK;
- [ ] classify findings as current bug/compatibility/docs vs future architecture requirement.

## M7 — Current contract maintenance

Until a separately approved contract/version change:
- [ ] accept bug/security/compatibility fixes;
- [ ] accept documentation/sample/package corrections that do not redefine behavior;
- [ ] preserve SemVer/API compatibility policy;
- [ ] keep package/release/security evidence current;
- [ ] do not expose direct scale wire protocols.

## M8 — Future Sadr Scales 5.4+ transport evolution

Only after accepted SadrScales architecture decisions:
- [ ] typed managed commands / Command Mailbox;
- [ ] Service/REST transport;
- [ ] realtime/Webhook adapters where justified;
- [ ] extend the same Integration Domain rather than creating competing SQL/REST concepts;
- [ ] update samples/tests for accepted future transports.

If the public `5.4` product version is consumed first by a maintenance release, this architecture stream follows the next unused SadrScales public version.

## M9 — Multi-language reference examples — future
- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.
- [ ] language-neutral SQL type/null mapping table.

Until wrappers exist, non-C# developers use the documented SQL Contract and executable SQL samples.

## M10 — Distribution/public engineering improvements — future
- [ ] evaluate NuGet.org publication and package ownership policy.
- [ ] evaluate immutable GitHub Releases after confirming compatibility with the verification workflow.
- [ ] add a second trusted maintainer when operationally appropriate.
- [ ] keep dependency/security/compatibility evidence current.

## Non-goals
No-code connectors, direct wire-protocol publication and speculative REST/Webhook contracts are not current work. Future transports must follow accepted SadrScales/SadrPlatform architecture, not precede it.
