<div dir="rtl" align="right">

# شروع سریع — Sadr Scales Integration

این کوتاه‌ترین مسیر پشتیبانی‌شده برای شرکت‌های نرم‌افزاری است که با **Sadr Scales 5.2.1** یکپارچه می‌شوند.

> نسخه عمومی پایدار فعلی `v1.0.0` است. خط افزوده‌ی Vendor-Ready `1.1.0` پیش از نامه‌ی بعدی به شرکت‌های نرم‌افزاری در حال Freeze و تست نهایی است.

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

نرم‌افزار شما پروتکل مستقیم PLUS، LSG یا Aclas را پیاده‌سازی نمی‌کند. مالک ارتباط مستقیم با ترازو همچنان Sadr Scales است.

## ۲. پیش‌نیازها

- Sadr Scales `5.2.1` یا نسخه جدیدتری که صریحاً سازگار اعلام شده باشد.
- دسترسی به SQL Server همان نصب Sadr Scales.
- برای C#: .NET Framework 4.8 یا .NET جدید با توان مصرف `netstandard2.0`.

ابتدا Sadr Scales را یک‌بار اجرا کنید تا بررسی و به‌روزرسانی Schema خود برنامه کامل شود.

## ۳. قبل از هر کاری Contract را کنترل کنید

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

برای زبان‌های غیر C# می‌توانید [`00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql) را اجرا کنید.

اگر Schema سازگار نیست، Integration را با دورزدن Validation ادامه ندهید.

## ۴. ببینید چه قابلیت‌هایی در اختیار شماست

صفحه [قابلیت‌های Vendor-Ready](capabilities.md) را ببینید.

سطح تأییدشده فعلی 5.2.1 شامل این موارد است:

- شعبه، گروه کالا و کالا/PLU؛
- ترازوهای ثبت‌شده و وضعیت Online/Offline؛
- تخصیص گروه، Mapping اختصاصی و HotKey گروه؛
- درخواست ارسال مجدد خودکار کالا و HotKey؛
- Feed افزایشی فروش؛
- Query فروش و گزارش‌های تایپ‌شده؛
- دریافت فاکتور ساختاریافته و ACK صریح.

## ۵. برنامه نمونه اجرایی را اجرا کنید

مرجع اصلی اجرایی اینجاست:

[`samples/csharp/SadrScales.Integration.SampleApp`](../../samples/csharp/SadrScales.Integration.SampleApp/README.md)

این برنامه جریان‌های اصلی نسخه Vendor-Ready و Demo Data محافظت‌شده را به‌صورت قابل مشاهده نشان می‌دهد.

Connection String را بدون قرار دادن رمز داخل Source تنظیم کنید:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

Quick Start کوچک‌تر و فقط‌خواندنی هم در [`samples/csharp/SadrScales.Integration.QuickStart`](../../samples/csharp/SadrScales.Integration.QuickStart/README.md) باقی مانده است.

## ۶. مسیر Package در C#

پس از دانلود Package نسخه Release در یک پوشه محلی:

```bash
dotnet add package SadrScales.Integration --version <release-version> --source <download-folder>
```

شروع معمول:

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();

await client.Stores.UpsertAsync(store);
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

## ۷. قانون مهم فاکتور

```text
دریافت فاکتور
→ ذخیره در نرم‌افزار مقصد
→ Commit موفق مقصد
→ ACK فاکتور در Sadr Scales
```

Lookup هیچ‌وقت خودکار ACK نمی‌کند. فاکتور ACK شده همچنان با `AlreadyRead` و همه‌ی جزئیات برمی‌گردد تا بازیابی یا ورود دوباره ممکن باشد.

## ۸. قانون مهم Sales Feed

`Sales.ReadAfterAsync` برای همگام‌سازی افزایشی است و Cursor را خود نرم‌افزار مقصد نگه می‌دارد.

1. ردیف‌های بعد از Cursor را بخوانید؛
2. آن‌ها را در مقصد ذخیره کنید؛
3. Transaction مقصد را Commit کنید؛
4. فقط بعد از آن Cursor جدید را ثبت کنید.

برای جلوگیری از Duplicate از `(DeviceNo, FID, SubID)` استفاده کنید. `Sales.QueryAsync` برای جست‌وجو و گزارش است و جای Feed/Cursor را نمی‌گیرد.

## ۹. مسیر غیر C# / Raw SQL

نمونه‌های تأییدشده در [`samples/SQL`](../../samples/SQL/README.md) قرار دارند و همان قابلیت‌های SQL فعلی 5.2.1 را بدون نیاز به C# نشان می‌دهند.

روی جدول‌ها و ستون‌های داخلی که در این مسیر مستند نشده‌اند Write تازه اختراع نکنید.

## ۱۰. ایمنی Demo Data

Demo Data از Contract تولیدی SDK جداست. برنامه نمونه فقط وقتی اجازه Generate/Reset می‌دهد که دیتابیس نام واضح غیرتولیدی داشته باشد، Schema لازم موجود باشد، داده تجاری خالی باشد، نام دقیق دیتابیس دوباره تأیید شود و Demo marker معتبر وجود داشته باشد.

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

</div>
