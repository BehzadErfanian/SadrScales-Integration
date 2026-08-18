# AGENTS.md — Project continuity rules

This repository must remain self-documenting. Chat history and assistant memory are never the source of truth.

## Canonical product and architecture plan

Before any new product, architecture, API, repository-structure, sample, simulator or Integration Lab work, read first:

1. `.github/maintainers/INTEGRATION_PLATFORM_MASTER_PLAN_FA.md`

This is the canonical statement of the accepted Integration Platform vision, priorities, coding rules, testing model, simulator/public-emulator split, education workflow and execution phases.

If an older project document conflicts with the accepted direction recorded in the master plan, do not silently follow the older wording. Resolve the conflict explicitly in the same change.

## Current-state references

After the master plan, read only the references relevant to the task:

1. `docs/PROJECT_STATUS.md`
2. `docs/DECISIONS.md`
3. `docs/ROADMAP.md`
4. `docs/BACKLOG.md`
5. `docs/WORK_LOG.md`
6. `docs/SECURITY_BOUNDARY.md`
7. the relevant contract/guide under `docs/`

These files describe current/history state. They do not override the canonical future-product direction unless the master plan is formally updated.

## Mandatory before implementation

- Do not implement a new public Integration capability before its business meaning, source of truth and safety boundary are understood.
- Complete the Integration Surface Audit before the planned large Public API/repository redesign.
- Classify capabilities as Safe Data Contract, Managed Runtime Command, or Internal/Do Not Expose.
- Do not guess behavior from table names or old schema scripts when the final Sadr Scales source/runtime is available.
- Preserve the educational workflow: understand the contract, predict behavior, design, implement, test, debug, review and explain.

## Coding and usability rules

For new Integration code:

- public APIs require clear XML documentation;
- non-obvious behavior must explain the reason/contract, not merely restate code;
- C# files with multiple logical sections use clear `#region` blocks;
- regions must not be used to justify oversized multi-responsibility classes;
- repository and API usability for an external developer is a first-class acceptance criterion;
- every public feature must include appropriate tests, documentation and samples;
- Sample App demo data must support both random data and deterministic seeded data where applicable.

## Mandatory after every accepted change

Update documentation in the same change as appropriate:

- the canonical master plan when a project-wide accepted rule/direction changes;
- `docs/PROJECT_STATUS.md` for current state and next exact step;
- `docs/DECISIONS.md` for accepted architectural/product decisions not already sufficiently represented by the master plan;
- `docs/BACKLOG.md` for work-item state;
- `docs/WORK_LOG.md` for meaningful session/handoff progress;
- `CHANGELOG.md` for user/developer-visible changes;
- relevant contract/reference docs when behavior changes.

A feature or fix is not complete when its documentation is stale.

## Security boundary

This public repository must never receive proprietary scale protocols, packet captures, reverse-engineering notes, private keys, credentials, customer data, internal Sadr Scales source, private build/protection configuration or vendor-confidential material.

The internal engineering protocol simulator and the future public scale emulator are different products. Direct protocol implementation remains private; the future public emulator distributes only the approved protected behavior/runtime surface.

See `SECURITY.md`, `docs/SECURITY_BOUNDARY.md` and the master plan.

## Release discipline

- `v1.0.0` is an immutable published release of the existing Basic SQL Contract generation.
- Never move or reuse the `v1.0.0` tag.
- Do not choose the next public version number until the next Contract/API compatibility design is complete.

## Future-session handoff

A new session must be able to recover the intended direction by reading the master plan plus the relevant current-state references. Never rely on undocumented agreements from a previous conversation.
