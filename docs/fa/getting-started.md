<div dir="rtl" align="right">

# شروع سریع — Sadr Scales Integration

این مسیر برای یکپارچه‌سازی معمول POS/ERP با **Sadr Scales 5.2.1 / SQL Contract v1** است.

## ۱. معماری

```text
POS / ERP / Accounting
        ↓
Sadr Scales SQL Contract v1
        ↓
Sadr Scales Runtime
        ↓
ترازوهای پشتیبانی‌شده
```

نرم‌افزار شما با SQL Server مورد استفاده Sadr Scales کار می‌کند. مدیریت Session، Retry، Registry، تفاوت مدل‌ها و ارتباط مستقیم با ترازو بر عهده Sadr Scales باقی می‌ماند.

## ۲. قبل از Integration

1. Sadr Scales 5.2.1 را اجرا کنید تا Schema migration/check خود برنامه کامل شود.
2. روی محیط تست، [`samples/SQL/00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql) را اجرا کنید.
3. Credential واقعی را داخل Source یا فایل نمونه قرار ندهید.

## ۳. سه Object پایه

- `dbo.SADR_ItemClass` — گروه کالا؛ Read/Insert/Update.
- `dbo.SADR_Item` — کالا/PLU؛ Read/Insert/Update.
- `dbo.SADR_Logs` — فروش؛ فقط Read.

Registry، Mapping و Structured Sales جزو مسیر پایه نیستند.

## ۴. کالا

- گروه را قبل از کالا بسازید.
- `PluNo` باید یکتا و غیرصفر باشد.
- `PluNo` هویت Contract v1 است؛ از `ID`/`IDitem` به‌عنوان شناسه Integration استفاده نکنید.
- `TimeStamp/rowversion` را ننویسید.
- برای غیرفعال‌سازی معمول، `DeleteFlag` را به حذف فیزیکی ترجیح دهید.
- برای نمونه امن و Dry-run، [`01-upsert-item.sql`](../../samples/SQL/01-upsert-item.sql) را ببینید.

## ۵. فروش

الگوی پایه:

```sql
SELECT TOP (@BatchSize) *
FROM dbo.SADR_Logs
WHERE ID > @LastProcessedId
ORDER BY ID ASC;
```

اما در کد واقعی ستون‌ها را صریح انتخاب کنید؛ نمونه کامل در [`02-read-sales-incremental.sql`](../../samples/SQL/02-read-sales-incremental.sql) قرار دارد.

قواعد مصرف:

- Cursor را در دیتابیس نرم‌افزار مقصد نگهدارید.
- ابتدا فروش را در مقصد Commit کنید، سپس Cursor را جلو ببرید.
- `(DeviceNo, FID, SubID)` را برای جلوگیری از Duplicate در مقصد استفاده کنید.
- `SADR_Logs` را Update/Delete نکنید.
- وجود Gap در `ID` طبیعی است؛ دنبال `ID = قبلی + 1` نباشید.

## ۶. ادامه

- [SQL Contract v1](sql-contract-v1.md)
- [Contract Freeze Record](../CONTRACT_V1_FREEZE.md)
- [Regression Checklist](../CONTRACT_V1_REGRESSION_CHECKLIST.md)
- [راهنمای جامع 5.2.1](../reference/README.md)
- [مرز امنیتی](../SECURITY_BOUNDARY.md)

C# SDK در M2 ساخته می‌شود. تا انتشار SDK، همین Contract و Sampleهای Repository مرجع رسمی Integration هستند.

</div>
