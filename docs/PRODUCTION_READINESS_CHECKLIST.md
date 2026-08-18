# Production Readiness Checklist

Use this checklist before a POS, ERP or accounting integration is enabled in a customer environment.

## 1. Version and contract

- [ ] Record the deployed Sadr Scales version.
- [ ] Record the Integration SDK/package version or commit SHA.
- [ ] Confirm the Sadr Scales release is documented as SQL Contract v1 compatible.
- [ ] Run `SadrScalesClient.ValidateAsync()` or the Contract v1 SQL validation sample successfully.
- [ ] Stop deployment if Contract validation fails; do not bypass schema mismatches.

## 2. Database access and security

- [ ] Use a dedicated SQL identity appropriate to the required Contract v1 operations.
- [ ] Keep production connection strings outside source control.
- [ ] Do not log passwords, tokens or full production connection strings.
- [ ] Confirm network/firewall access is limited to what the integration actually needs.
- [ ] Confirm backups/recovery for the destination application's own data and sales cursor.

## 3. Item/group integration

- [ ] Create referenced item groups before dependent PLUs.
- [ ] Treat `PluNo` as the public Contract v1 PLU identity.
- [ ] Do not use legacy `ID`/`IDitem` as the public item identity.
- [ ] Never write SQL `rowversion`/`TimeStamp` columns.
- [ ] For SDK batch writes, keep each call at or below 200 unique PLUs.
- [ ] Page larger imports explicitly in the destination application.
- [ ] Test insert, update, unchanged/no-op and failed-batch rollback behavior with synthetic data.

## 4. Sales integration

- [ ] Read accepted sales incrementally; the public sales path is read-only.
- [ ] Store the destination cursor in durable destination-owned state.
- [ ] Persist/commit destination sales before advancing the cursor.
- [ ] Use `(DeviceNo, FID, SubID)` as the preferred destination duplicate-protection key.
- [ ] Tolerate gaps in source `ID` values.
- [ ] Never update/delete `SADR_Logs` to acknowledge or manage a consumer cursor.
- [ ] Test process restart after reading a batch but before advancing the cursor.
- [ ] Test duplicate replay/idempotency behavior.

## 5. Failure and recovery tests

- [ ] Test SQL connection interruption during a read operation.
- [ ] Test application restart with an existing durable cursor.
- [ ] Test an invalid/mismatched Contract schema and confirm fail-fast behavior.
- [ ] Test a write failure inside an atomic PLU batch and confirm full rollback.
- [ ] Confirm transaction-scoped writes are not blindly replayed after ambiguous failures.

## 6. Operational handoff

- [ ] Record the integration owner/contact at the software vendor.
- [ ] Record the customer deployment identifier without publishing customer data in GitHub.
- [ ] Record the exact SDK/Sadr Scales versions used in production.
- [ ] Keep a sanitized reproduction procedure for support.
- [ ] Review `SUPPORT.md`, `SECURITY.md` and the troubleshooting guide with the implementation team.

## 7. Go-live decision

Production go-live should proceed only when Contract validation succeeds, destination-side idempotency/cursor persistence has been tested, rollback/restart scenarios pass, and no sensitive information is embedded in code or public logs.
