# GitHub Bootstrap

## Recommended repository settings

**Owner:** `BehzadErfanian` (until/unless an official Sadr organization is used)  
**Repository:** `SadrScales-Integration`  
**Description:** `Official public integration toolkit, SQL contract, SDK and samples for connecting POS/ERP/accounting software to Sadr Scales.`  
**Default branch:** `main`

## Safe publication sequence

For the first bootstrap, the safest sequence is:

1. Create the repository on GitHub without auto-generated README/license/gitignore.
2. Prefer **Private** during the first upload/review if possible.
3. Push the M0 foundation.
4. Verify `Public repository guard` passes.
5. Inspect Git history and repository files for sensitive material.
6. Enable GitHub security features available for the repository.
7. Change visibility to **Public** when the review is clean.

The end state is a public repository. The temporary private staging step only reduces the risk of publishing an accidental secret during bootstrap.

## Push from a prepared local repository

```bash
git remote add origin https://github.com/BehzadErfanian/SadrScales-Integration.git
git push -u origin main
```

If the repository was created with a README or other initial commit, do not force-push blindly. Reconcile the history first.

## After first push

Update:

- `docs/PROJECT_STATUS.md`
- `docs/WORK_LOG.md`
- `docs/BACKLOG.md`

with the repository URL, initial commit SHA and confirmed visibility/security settings.
