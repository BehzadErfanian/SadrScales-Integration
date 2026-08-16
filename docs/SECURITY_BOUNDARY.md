# Public / Private Security Boundary

This document is normative for this repository.

## Public — allowed

- SQL Contract v1 table/field/query documentation required for supported integration.
- Public C# Integration SDK implementation.
- Safe, synthetic sample applications in supported languages.
- Public compatibility/versioning information.
- Public schema validation and SQL examples.
- Public Integration & Database Guide approved for software vendors.
- Public GitHub Release artifacts built from this repository.
- General Sadr Scales architecture at the boundary level: external software → Sadr Scales → supported scale.

## Private — forbidden

### Device communication

- direct PLUS protocol frames/state machine implementation;
- direct LSG protocol frames/state machine implementation;
- Aclas or other proprietary device wire protocol details not already intentionally public;
- packet captures, Wireshark files and raw vendor network traces;
- reverse-engineering notes that expose device protocol internals.

### Security / licensing

- license private keys;
- diagnostic private/client keys not intentionally public;
- signing certificates/private material;
- secrets, access tokens, passwords or real connection strings;
- obfuscation/protection secrets or private build credentials.

### Vendor / customer confidentiality

- firmware not explicitly redistributable;
- vendor-confidential manuals/SDKs;
- real customer databases, sales records or personal information;
- customer IPs, usernames, passwords or support bundles.

### Private product internals

- private Sadr Scales runtime source copied wholesale;
- internal release/protection pipeline details that are unnecessary for Integration SDK consumers;
- unpublished vulnerability details.

## Review rule

When uncertain whether a file is safe, **do not commit it**. Document the need first, then explicitly classify it as public before adding it.

The validator blocks common sensitive file types, but tooling is a guardrail, not permission to ignore this policy.
