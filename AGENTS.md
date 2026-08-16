# AGENTS.md — Project continuity rules

This repository must remain self-documenting. Chat history is never the source of truth.

## Before changing anything

Read, in this order:

1. `docs/PROJECT_STATUS.md`
2. `docs/DECISIONS.md`
3. `docs/ROADMAP.md`
4. `docs/BACKLOG.md`
5. `docs/WORK_LOG.md`
6. `docs/SECURITY_BOUNDARY.md`
7. the relevant contract/guide under `docs/`

## Mandatory after every accepted change

Update documentation in the same change:

- `docs/PROJECT_STATUS.md` — what changed, current state, next exact step;
- `docs/DECISIONS.md` — if a new architectural/product decision was accepted;
- `docs/BACKLOG.md` — close/add/update work items;
- `docs/WORK_LOG.md` — append meaningful session/handoff progress;
- `CHANGELOG.md` — for user/developer-visible changes;
- relevant contract/reference docs when behavior changed.

A feature or fix is not considered complete when its documentation is stale.

## Security boundary

This public repository must never receive proprietary scale protocols, packet captures, private keys, credentials, customer data, internal Sadr Scales source, private build/protection configuration or vendor-confidential material. See `SECURITY.md` and `docs/SECURITY_BOUNDARY.md`.

## Contract discipline

- SQL Contract v1 is the public integration contract for Sadr Scales 5.2.1.
- Do not expand the public contract by accident while implementing helpers.
- Registry/Mapping/structured-sales operations are advanced/controlled surfaces unless explicitly promoted by an accepted decision.
- Direct device protocol support belongs to the private Sadr Scales runtime, not this project.

## Future-chat handoff

A new chat/session should be able to continue by reading only the repository documents above. Never rely on undocumented agreements from a previous conversation.
