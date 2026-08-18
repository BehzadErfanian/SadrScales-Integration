# Security Policy

## Scope

This is a **public integration repository**. Treat every committed byte, issue comment, pull request and attached log as public.

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
- customer databases, backups, sales data or personally identifiable customer information;
- internal Sadr Scales obfuscation/protection configuration or private build/release infrastructure;
- proprietary source files copied from the private Sadr Scales repository unless explicitly approved for public release.

## Allowed public content

- SQL Contract v1 and its documentation;
- public Integration SDK source;
- safe sample applications with synthetic data;
- public schema/query examples required by the contract;
- compatibility notes, changelog, roadmap and public support documentation;
- release binaries generated from this public repository;
- public technical guide approved for software vendors.

## Reporting a vulnerability

Do **not** open a normal public issue containing vulnerabilities, credentials, private keys, exploitable customer data or unpublished protocol/security material.

Preferred reporting order:

1. if GitHub shows a private **Report a vulnerability** option for this repository, use that private channel;
2. otherwise contact Tozin Sadr through its official support channel and provide only the minimum information required to reproduce the issue.

Official website: https://sadrgroup.ir/

A public issue may be used only after sensitive details have been removed and disclosure is safe.

## Secret exposure response

If a secret is accidentally committed or posted publicly, consider it compromised even if the content is later deleted.

1. rotate/revoke the credential or key first;
2. preserve the minimum evidence needed for investigation;
3. remove the exposed material from the repository/history where appropriate;
4. review whether customer or production systems were affected;
5. add or strengthen a prevention check when practical.

## Supported security boundary

The MIT license for this public repository does not publish or grant access to private Sadr Scales runtime source, proprietary device protocols, private keys, firmware or vendor-confidential material. See `NOTICE.md` and `docs/SECURITY_BOUNDARY.md`.
