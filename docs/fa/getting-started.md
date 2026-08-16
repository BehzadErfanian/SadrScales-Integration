<div dir="rtl" align="right">

# شروع سریع — Sadr Scales Integration

> وضعیت: Draft برای مرحله Foundation. نمونه اجرایی SDK در M2/M3 اضافه می‌شود.

## ۱. ابتدا معماری را درست انتخاب کنید

نرم‌افزار شما به SQL Server مورد استفاده Sadr Scales متصل می‌شود. خود Sadr Scales ارتباط با ترازو، Retry، Session و تفاوت مدل‌های دستگاه را مدیریت می‌کند.

```text
نرم‌افزار فروشگاهی / ERP
        ↓
SQL Contract v1
        ↓
Sadr Scales
        ↓
ترازو
```

## ۲. سه Object پایه

برای Integration عمومی v1:

- `dbo.SADR_ItemClass` — گروه کالا؛
- `dbo.SADR_Item` — کالا/PLU؛
- `dbo.SADR_Logs` — فروش؛ فقط خواندنی.

## ۳. قواعدی که از ابتدا رعایت کنید

- Queryها Parameterized باشند.
- `PluNo` صفر نباشد و یکتا باشد.
- گروه کالا قبل از کالا وجود داشته باشد.
- `rowversion/TimeStamp` دستی نوشته نشود.
- `SADR_Logs` را Update/Delete نکنید.
- Cursor فروش را در نرم‌افزار خودتان نگه دارید.
- Credential واقعی داخل سورس یا Log قرار ندهید.

## ۴. مستندات بعدی

- [قرارداد SQL v1](sql-contract-v1.md)
- [راهنمای جامع 5.2.1](../reference/README.md)
- [مرز امنیتی](../SECURITY_BOUNDARY.md)

## ۵. SDK

SDK زبان C# هنوز در مرحله طراحی است. تا زمان انتشار اولین Release، قرارداد SQL همین Repository مرجع رسمی است.

</div>
