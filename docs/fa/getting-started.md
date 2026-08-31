<div dir="rtl" align="right">

# شروع سریع — Sadr Scales Integration

این کوتاه‌ترین مسیر پشتیبانی‌شده برای شرکت‌های نرم‌افزاری است که از **SadrScales.Integration `v1.1.0`** استفاده می‌کنند.

> Contract عمومی ثابت SQL/SDK بر اساس Sadr Scales `5.2.1` تعریف شده است. Sadr Scales `5.3` نسخه پایدار فعلی نرم‌افزار است و تست نهایی سازگاری/Vendor Rehearsal با همین Contract بدون تغییر در حال انجام است.

## ۱. مرز ارتباط را درست ببینید

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration یا SQL مستندشده
        ↓
دیتابیس / Runtime نرم‌افزار Sadr Scales
        ↓
ترازوهای پشتیبانی‌شده
```

نرم‌افزار شما پروتکل مستقیم PLUS، LSG یا Aclas را پیاده‌سازی نمی‌کند. مالک ارتباط مستقیم با ترازو، Session/Reconnect و تفاوت مدل‌ها همچنان Sadr Scales است.

## ۲. پیش‌نیازها

- Package نسخه `SadrScales.Integration 1.1.0`.
- نصب Sadr Scales سازگار با Contract 5.2.1؛ نسخه 5.3 هدف فعلی تست سازگاری است.
- دسترسی به SQL Server همان نصب Sadr Scales.
- برای C#: .NET Framework 4.8 یا .NET جدید با توان مصرف `netstandard2.0`.

ابتدا Sadr Scales را یک‌بار اجرا کنید تا بررسی/آماده‌سازی Schema خود برنامه کامل شود.

برای یادگیری و تست Acceptance از دیتابیس آزمایشی استفاده کنید و Credential واقعی را داخل Source قرار ندهید.

## ۳. قبل از هر کاری Contract را کنترل کنید

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

برای زبان‌های غیر C# می‌توانید [`00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql) را اجرا کنید.

اگر Schema سازگار نیست، Integration را با دورزدن Validation ادامه ندهید و روی جدول‌ها/ستون‌های داخلی Write مستندنشده نسازید.

## ۴. قابلیت‌های تأییدشده را ببینید

صفحه [قابلیت‌های Vendor-Ready](capabilities.md) را ببینید.

سطح ثابت 1.1 شامل این موارد است:
- شعبه، گروه کالا و کالا/PLU؛
- تاریخچه قیمت؛
- ترازوهای ثبت‌شده و وضعیت `Online / Offline / Unknown`؛
- Assignment، Mapping اختصاصی و HotKey گروه؛
- درخواست ارسال مجدد کالا/HotKey؛
- Sales Feed افزایشی؛
- Sales Query، Summary و Reports؛
- دریافت فاکتور با TotalBarcode یا ScaleID + FID؛
- ACK صریح و Idempotent و بازیابی کامل `AlreadyRead`.

## ۵. از Package منتشرشده استفاده کنید

اگر Package نسخه `v1.1.0` را در یک پوشه محلی دانلود کرده‌اید:

```bash
dotnet add package SadrScales.Integration --version 1.1.0 --source <download-folder>
```

شروع معمول:

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();

await client.Stores.UpsertAsync(store);
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

پروژه Package-only Vendor Acceptance داخل مخزن عمداً هیچ `ProjectReference` به SDK ندارد و مسیر واقعی مصرف Package را تست می‌کند.

## ۶. برنامه نمونه اجرایی را اجرا کنید

مرجع اصلی:
[`samples/csharp/SadrScales.Integration.SampleApp`](../../samples/csharp/SadrScales.Integration.SampleApp/README.md)

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

Quick Start کوچک‌تر و فقط‌خواندنی هم در [`samples/csharp/SadrScales.Integration.QuickStart`](../../samples/csharp/SadrScales.Integration.QuickStart/README.md) قرار دارد.

## ۷. قانون مهم فاکتور

```text
دریافت فاکتور
→ ذخیره در نرم‌افزار مقصد
→ Commit موفق مقصد
→ ACK فاکتور در Sadr Scales
```

Lookup هیچ‌وقت خودکار ACK نمی‌کند. ACK باید Idempotent باشد. فاکتور ACK شده همچنان با `AlreadyRead` و همه جزئیات برمی‌گردد تا Recovery/Re-import ممکن بماند.

## ۸. قانون مهم Sales Feed

`Sales.ReadAfterAsync` یک Feed با Cursor تحت مالکیت نرم‌افزار مقصد است:
1. ردیف‌های بعد از Cursor ذخیره‌شده را بخوانید؛
2. در مقصد ذخیره کنید؛
3. Transaction مقصد را Commit کنید؛
4. فقط بعد از آن Cursor جدید را ثبت کنید.

برای جلوگیری از Duplicate از `(DeviceNo, FID, SubID)` استفاده کنید. `Sales.QueryAsync` برای جست‌وجو/گزارش است و جای Feed/Cursor را نمی‌گیرد.

## ۹. مسیر غیر C# / Raw SQL

نمونه‌های تأییدشده در [`samples/SQL`](../../samples/SQL/README.md) قرار دارند و Contract SQL را بدون نیاز به C# نشان می‌دهند.

روی جدول‌ها و ستون‌های داخلی که بخشی از Contract/Recipe مستند نیستند Write تازه اختراع نکنید.

## ۱۰. ایمنی Demo Data

Demo Data از Contract تولیدی SDK جداست. برنامه نمونه فقط وقتی اجازه Generate/Reset می‌دهد که دیتابیس واضح غیرتولیدی باشد، Schema لازم موجود باشد، داده تجاری خالی باشد، نام دقیق دیتابیس دوباره تأیید شود و Demo marker معتبر وجود داشته باشد.

Demo marker را روی دیتابیس مشتری یا Production فعال نکنید.

## ۱۱. فقط در صورت نیاز سراغ مرجع بروید

- [قابلیت‌ها](capabilities.md)
- [کاتالوگ](catalog.md)
- [ترازو، وضعیت و ارسال مجدد](scales-status-resend.md)
- [تخصیص، Mapping و HotKey](assignments-mapping-hotkeys.md)
- [فاکتور ساختاریافته و ACK](structured-invoices.md)
- [جست‌وجوی فروش و گزارش‌ها](sales-query-reports.md)
- [نمونه‌های Raw SQL](../../samples/SQL/README.md)
- [رفع اشکال](troubleshooting.md)
- [Security](../../SECURITY.md)

پروتکل مستقیم ترازو، Packet خام، کلیدهای خصوصی و Runtime Command دلخواه عمداً خارج از این مخزن عمومی هستند.

Commandهای مدیریت‌شده، Service/REST و Webhook/realtime جزو معماری آینده **Sadr Scales 5.4+** هستند و داخل Contract ثابت SQL/SDK نسخه 1.1 قرار ندارند.

</div>
