# Contributing

Thank you for improving Sadr Scales Integration.

## Before contributing

Read:

- `SECURITY.md`
- `docs/SECURITY_BOUNDARY.md`
- `docs/DECISIONS.md`
- `docs/PROJECT_STATUS.md`

## Rules

1. Keep changes inside the public Integration boundary.
2. Do not add direct device protocols, packet captures, vendor-confidential material, private Sadr Scales source or secrets.
3. Use synthetic data in tests and samples.
4. Keep public contract changes explicit and versioned.
5. Add/update tests for implementation changes.
6. Update project documentation in the same pull request.
7. Run `pwsh ./tools/Validate-PublicRepository.ps1` before submitting.
