# Vendor-Ready capabilities — Sadr Scales 5.2.1 / Integration 1.1.0

Use this page as the capability map for the additive `1.1.0` line.

## Catalog

- Stores: read / upsert
- Item Groups: read / upsert
- Items / PLUs: read / upsert / bounded batch / logical delete
- Price History: read-only

Guide: [Catalog](catalog.md)

## Scales

- registered-scale metadata
- coarse SQL status: `Online / Offline / Unknown`
- Item AutoSend resend request
- supported HotKey AutoSend resend request

Guide: [Scales, status and resend](scales-status-resend.md)

## Scale configuration

- canonical multi-group assignment
- per-scale PLU / ItemCode mapping
- optional per-scale HotKey position mapping
- group HotKey templates
- validated atomic Replace / Copy semantics

Guide: [Assignments, mapping and HotKeys](assignments-mapping-hotkeys.md)

## Sales and reports

- incremental Sales Feed with destination-owned cursor
- filtered / paged Sales Query
- shared summary: rows, invoices, price, weight, quantity
- Daily report
- By Scale report
- By Item report

Guide: [Sales Query and Reports](sales-query-reports.md)

## Structured invoices

- lookup by aggregate `TotalBarcode`
- lookup by `ScaleID + FID`
- complete header + details
- explicit idempotent ACK after destination commit
- acknowledged invoices remain fully readable with `AlreadyRead`

Guide: [Structured Invoice + ACK](structured-invoices.md)

## Demo and testing

- executable WinForms Developer Sample
- deterministic Demo Data with explicit Seed
- guarded Demo marker / reset flow
- package-only Vendor Acceptance CI

The DemoLab helper is not part of the production SDK API and cannot be used to bypass the supported 5.2.1 production contract.

## Raw SQL

C# is optional. Equivalent reference recipes for non-C# stacks are under [`samples/SQL`](../../samples/SQL/README.md).

## Not part of the 5.2.1 Vendor-Ready contract

- direct device protocol packets/opcodes
- arbitrary Runtime commands
- immediate device command completion/progress
- production scale lifecycle through raw SQL
- firmware/file transfer

A typed per-scale Command Mailbox is planned for Sadr Scales 5.3.
