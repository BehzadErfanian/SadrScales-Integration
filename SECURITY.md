# Security Policy

## Scope

This is a **public integration repository**. Treat every committed byte as public.

Before submitting a commit or pull request, read `docs/SECURITY_BOUNDARY.md` and run:

```powershell
pwsh ./tools/Validate-PublicRepository.ps1
```

## Never publish here

- PLUS / LSG / Aclas or other direct scale wire-protocol implementation details;
- packet captures (`.pcap`, `.pcapng`) or vendor sniffing material;
- firmware, confidential vendor SDKs/manuals or reverse-engineering notes;
- private signing, license or diagnostic keys;
- `.pfx`, `.p12`, private `.pem`, `.key`, `.snk`, seed or recovery material;
- production credentials, connection strings containing real passwords, API tokens or secrets;
- customer databases, backups, sales data, personally identifiable customer information;
- internal Sadr Scales obfuscation/protection configuration or private build/release infrastructure;
- proprietary source files copied from the private Sadr Scales repository unless they were explicitly approved for public release.

## Allowed public content

- SQL Contract v1 and its documentation;
- public Integration SDK source;
- safe sample applications with synthetic data;
- public schema/query examples required by the contract;
- compatibility notes, changelog, roadmap and public support documentation;
- release binaries generated from this public repository;
- public technical guide approved for software vendors.

## Reporting a vulnerability

Do **not** open a public issue containing credentials, private keys, exploitable customer data, or unpublished protocol/security material. Contact Tozin Sadr through its official support channel and provide the minimum information necessary to reproduce the issue.

If a secret is accidentally committed, consider it compromised even if the commit is later removed. Rotate/revoke it first, then remove it from Git history.
