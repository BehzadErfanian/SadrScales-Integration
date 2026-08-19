<div dir="rtl" align="right">

# یکپارچه‌سازی Sadr Scales

[![SDK CI](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml)
[![Public Repository Guard](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml)

**مخزن عمومی رسمی اتصال نرم‌افزارهای فروشگاهی، ERP و حسابداری به Sadr Scales.**

ارائه و نگهداری توسط **Tozin Sadr و Behzad Erfanian**.

[English](README.md) · [شروع سریع](docs/fa/getting-started.md) · [قابلیت‌ها](docs/fa/capabilities.md) · [نمونه‌های SQL](samples/SQL/README.md) · [برنامه نمونه](samples/csharp/SadrScales.Integration.SampleApp/README.md) · [پشتیبانی](SUPPORT.md) · [امنیت](SECURITY.md)

## وضعیت

- نسخه عمومی پایدار: **`v1.0.0`**
- نسخه افزوده‌ی Vendor-Ready بعدی: **`1.1.0`**
- مبنای Sadr Scales: **`5.2.1`**
- Target SDK: `netstandard2.0`
- مصرف Package تأییدشده: .NET Framework 4.8 و .NET جدید
- مجوز: MIT

نسخه `v1.0.0` ثابت و تغییرناپذیر است. خط `1.1.0` فقط پس از تکمیل سطح Vendor-Ready 5.2.1، برنامه نمونه، محافظ Demo Data و تست Vendor Acceptance از روی Package Freeze می‌شود.

## از اینجا شروع کنید

```text
۱. شروع سریع را بخوانید
۲. Contract بانک را Validate کنید
۳. صفحه قابلیت‌ها را ببینید
۴. برنامه نمونه را اجرا کنید
۵. اگر C# ندارید از نمونه‌های Raw SQL استفاده کنید
۶. فقط در صورت نیاز سراغ سندهای مرجع بروید
```

## معماری

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration یا SQL مستندشده
        ↓
دیتابیس / Runtime نرم‌افزار Sadr Scales
        ↓
PLUS / LSG / Aclas / ترازوهای پشتیبانی‌شده
```

نرم‌افزار شما با **Sadr Scales** یکپارچه می‌شود، نه با پروتکل اختصاصی مستقیم هر ترازو. ارتباط مستقیم، Retry/Reconnect و تفاوت مدل‌ها در اختیار Sadr Scales باقی می‌ماند.

## شروع پنج‌دقیقه‌ای C#

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();

await client.Stores.UpsertAsync(store);
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

برای همگام‌سازی پیوسته فروش:

```csharp
SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

ابتدا داده را در مقصد ذخیره و Commit کنید و فقط بعد از آن `batch.LastReadId` را به‌عنوان Cursor جدید نگه دارید.

## قابلیت‌های Vendor-Ready 1.1

سطح تأییدشده Sadr Scales 5.2.1 شامل این موارد است:

- شعبه‌ها؛
- گروه‌های کالا و کالا/PLU؛
- خواندن تاریخچه قیمت؛
- ترازوهای ثبت‌شده و وضعیت `Online / Offline / Unknown`؛
- تخصیص گروه‌های کالا به ترازو؛
- Mapping اختصاصی هر ترازو؛
- HotKeyهای گروه؛
- درخواست ارسال مجدد خودکار کالا/HotKey؛
- Sales Feed افزایشی؛
- Sales Query و Summary؛
- دریافت فاکتور با TotalBarcode یا ScaleID + FID؛
- ACK صریح و Idempotent؛
- گزارش روزانه، بر اساس ترازو و بر اساس کالا.

نقشه کامل در [صفحه قابلیت‌ها](docs/fa/capabilities.md) است.

## قانون مهم فاکتور

```text
دریافت فاکتور
→ ذخیره در مقصد
→ Commit موفق مقصد
→ ACK در Sadr Scales
```

Lookup هیچ‌وقت خودکار ACK نمی‌کند. فاکتور ACK شده همچنان کامل با `AlreadyRead` برمی‌گردد تا بازیابی و ورود دوباره ممکن باشد.

## برنامه نمونه اجرایی

مرجع اصلی:

[`samples/csharp/SadrScales.Integration.SampleApp`](samples/csharp/SadrScales.Integration.SampleApp/README.md)

این برنامه Invoices، Scales، Catalog، Assignments/Mapping/HotKeys، Sales/Reports و Demo Data محافظت‌شده را نشان می‌دهد.

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

Credential واقعی را داخل Source قرار ندهید.

## مسیر غیر C# / Raw SQL

استفاده از C# اجباری نیست. نمونه‌های مستقل از زبان در [`samples/SQL`](samples/SQL/README.md) قرار دارند.

برای قابلیت‌هایی که مستند نشده‌اند، Write تازه روی جدول‌ها یا ستون‌های داخلی اختراع نکنید.

## ایمنی Demo Data

DemoLab جزو API تولیدی SDK نیست. Generate/Reset فقط روی دیتابیس واضح غیرتولیدی، با Schema سازگار، داده تجاری خالی، تأیید دوباره نام دقیق دیتابیس و Demo marker معتبر مجاز است.

Demo marker را روی دیتابیس مشتری یا Production فعال نکنید.

## Gate کیفیت Release

نسخه Vendor-Ready باید این موارد را سبز داشته باشد:

- Build/Test/Pack SDK؛
- SQL Server Integration Tests؛
- Build برنامه WinForms Sample؛
- مصرف واقعی Package روی .NET Framework 4.8؛
- Vendor Acceptance از روی Package و بدون ProjectReference؛
- Public Repository Guard؛
- Release Bundle smoke test.

بعد از Freeze نسخه `1.1.0 RC` فقط اصلاح Bug، Security و Compatibility مجاز است تا نامه به شرکت‌های نرم‌افزاری ارسال شود.

## مرز امنیتی

این مخزن عمداً پروتکل مستقیم ترازو، Packet خام، Capture شبکه، کلید خصوصی، داده واقعی مشتری، Firmware خصوصی و Runtime Command دلخواه را منتشر نمی‌کند.

Command Mailbox تایپ‌شده برای هر ترازو در **Sadr Scales 5.3** برنامه‌ریزی شده و جزو انتشار فعلی 5.2.1 نیست.

## مجوز

MIT License. فایل‌های [LICENSE](LICENSE) و [NOTICE.md](NOTICE.md) را ببینید.

**Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.**

</div>
