# Backlog

Priority meanings: **P0** blocks safe public release; **P1** needed for v1.0; **P2** useful after v1.0.

## P0 — Public release administration / security

- [ ] Confirm public software license with the company before first SDK release; no `LICENSE` is published until approval.
- [ ] Complete `docs/GITHUB_SECURITY_ADMIN_CHECKLIST.md` in GitHub Settings.
- [ ] Upload approved Integration Guide PDF/checksum as GitHub Release assets before public v1.0 distribution.

## P1 — Contract / docs

- [x] Freeze SQL Contract v1 after 5.2.1 source/schema verification.
- [x] Publish Contract v1 freeze record and regression checklist.
- [x] Complete Persian/English Quick Start and contract docs.
- [x] Add executable SQL samples with synthetic values and expected results.
- [x] Produce and page-by-page QA the official Integration Guide PDF.
- [x] Record official PDF SHA-256 in repository reference docs.
- [ ] Add troubleshooting matrix for common SQL integration failures.

## P1 — SDK

- [ ] Decide .NET target framework(s) and SqlClient provider.
- [ ] Define `SadrScalesClient` public API.
- [ ] Implement schema validator.
- [ ] Implement item-group API.
- [ ] Implement item/PLU API and bounded batch operation.
- [ ] Implement incremental `SADR_Logs` sales reader.
- [ ] Add bounded transient-SQL retry policy.
- [ ] Add tests against a safe test schema.
- [ ] Add C# console sample.

## P1 — Release

- [ ] GitHub Actions build/test for SDK.
- [ ] Package and checksums.
- [ ] Release notes template.
- [ ] Create `v1.0.0` GitHub Release.
- [ ] Update website developer landing page to point to repository and release.

## P2 — Advanced / language examples

- [ ] Evaluate optional structured invoice helpers as explicitly advanced APIs.
- [ ] Python.
- [ ] Node.js.
- [ ] Java.
- [ ] PHP.

## P2 / Future generation

- [ ] Evaluate configurable no-code connector.
- [ ] Design REST/Webhook Integration Gateway as a separately versioned contract.
