# Contributing

Thank you for improving Sadr Scales Integration.

This repository is intentionally public and has a strict boundary around what may be contributed.

## Before contributing

Read:

- `README.md`
- `SECURITY.md`
- `docs/SECURITY_BOUNDARY.md`
- `docs/DECISIONS.md`
- `docs/API_COMPATIBILITY.md`
- `docs/PROJECT_STATUS.md`

## Public-boundary rules

1. Keep changes inside the public Integration SDK / SQL Contract boundary.
2. Do not add direct device protocols, packet captures, vendor-confidential material, private Sadr Scales source, customer data, credentials or secrets.
3. Use synthetic or sanitized data in tests, examples and issue reports.
4. Keep public SQL Contract changes explicit and separately versioned.
5. Do not silently expand Registry/Mapping/structured-sales internals into the public contract.
6. Transaction-scoped write behavior must not gain automatic replay without an explicit design/compatibility decision.

## Engineering requirements

Implementation changes should include or update appropriate tests. Before opening a pull request, run the relevant gates:

```powershell
pwsh ./tools/Validate-PublicRepository.ps1

dotnet restore tests/SadrScales.Integration.Tests/SadrScales.Integration.Tests.csproj
dotnet build tests/SadrScales.Integration.Tests/SadrScales.Integration.Tests.csproj --configuration Release --no-restore
dotnet test tests/SadrScales.Integration.Tests/SadrScales.Integration.Tests.csproj --configuration Release --no-build

dotnet restore samples/csharp/SadrScales.Integration.QuickStart/SadrScales.Integration.QuickStart.csproj
dotnet build samples/csharp/SadrScales.Integration.QuickStart/SadrScales.Integration.QuickStart.csproj --configuration Release --no-restore
```

SQL Server integration and .NET Framework 4.8 package-consumer validation also run in GitHub Actions. A contributor does not need to reproduce those hosted environments locally unless the change specifically requires it.

## Pull requests

A pull request should:

- explain the problem and scope;
- keep unrelated refactoring out of the change;
- include tests for behavioral changes;
- update developer documentation when behavior or public API changes;
- preserve Semantic Versioning/API compatibility rules;
- contain no sensitive/private material;
- pass SDK CI and Public Repository Guard.

## Licensing of contributions

The repository is distributed under the MIT License. By submitting a contribution, you agree that your contribution may be distributed under the repository's MIT License.

Project copyright/provider identity remains documented in `LICENSE`, `NOTICE.md` and package metadata.

## Security reports

Do not use a normal pull request or public issue to disclose a vulnerability or sensitive material. Follow `SECURITY.md`.
