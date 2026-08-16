# Backlog

Priority meanings: **P0** blocks safe public release; **P1** needed for v1.0; **P2** useful after v1.0.

## P0 — Foundation / security

- [x] Create public GitHub repository `BehzadErfanian/SadrScales-Integration`.
- [ ] Push M0 bootstrap.
- [ ] Run `tools/Validate-PublicRepository.ps1` in local and CI environments.
- [ ] Confirm the public software license with the company before the first SDK release; no LICENSE is published until approval.
- [ ] Enable GitHub security options available to the repository.
- [ ] Review repository history before first public announcement for accidental sensitive files.

## P1 — Contract / docs

- [ ] Freeze SQL Contract v1 after source verification.
- [ ] Export official Integration Guide PDF.
- [ ] Publish the guide PDF in GitHub Release/reference area.
- [ ] Complete English Quick Start and contract docs.
- [ ] Add sample SQL scripts with only synthetic values.
- [ ] Add troubleshooting matrix for common SQL integration failures.

## P1 — SDK

- [ ] Decide .NET target frameworks.
- [ ] Define `SadrScalesClient` API.
- [ ] Implement schema validator.
- [ ] Implement item-group API.
- [ ] Implement item/PLU API and batch operation.
- [ ] Implement incremental `SADR_Logs` sales reader.
- [ ] Implement optional structured invoice helpers.
- [ ] Add bounded transient-SQL retry policy.
- [ ] Add tests against a safe test schema.
- [ ] Add C# console sample.

## P1 — Release

- [ ] GitHub Actions build/test.
- [ ] Package and checksums.
- [ ] Release notes template.
- [ ] Create `v1.0.0` GitHub Release.
- [ ] Update website developer landing page to point to repository and release.

## P2 — Language examples

- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.

## P2 / Future generation

- [ ] Evaluate configurable no-code connector.
- [ ] Design REST/Webhook Integration Gateway as a separately versioned contract.
