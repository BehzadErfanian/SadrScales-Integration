# Release Policy

## Versioning

The Integration project uses Semantic Versioning for public SDK/releases:

- MAJOR: breaking public SDK/contract behavior.
- MINOR: backward-compatible public functionality.
- PATCH: backward-compatible fixes/documentation/package corrections.

The SQL contract version is tracked separately. SDK `1.x` does not imply SQL Contract v1 will change on every SDK release.

## GitHub Release contents

A stable release should contain, as applicable:

- compiled Integration SDK (`.dll` and/or package);
- package metadata / XML documentation;
- source tag;
- sample bundle;
- official Integration & Database Guide PDF;
- concise release notes;
- `SHA256SUMS.txt`.

## Stable-release gates

- public-repository security validator passes;
- all automated tests pass;
- no secrets/customer data/vendor-confidential material;
- compatibility matrix updated;
- `PROJECT_STATUS.md`, `CHANGELOG.md`, `ROADMAP.md` and `BACKLOG.md` updated;
- guide/contract matches implemented behavior;
- release assets are generated from the tagged public source where possible.
