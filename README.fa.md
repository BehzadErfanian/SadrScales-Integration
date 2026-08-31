<div dir="rtl" align="right">

# یکپارچه‌سازی Sadr Scales

[![SDK CI](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml)
[![Public Repository Guard](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml)

**مخزن عمومی رسمی اتصال نرم‌افزارهای فروشگاهی، ERP و حسابداری به Sadr Scales.**

ارائه و نگهداری توسط **Tozin Sadr و Behzad Erfanian**.

[English](README.md) · [شروع سریع](docs/fa/getting-started.md) · [قابلیت‌ها](docs/fa/capabilities.md) · [نمونه‌های SQL](samples/SQL/README.md) · [برنامه نمونه](samples/csharp/SadrScales.Integration.SampleApp/README.md) · [پشتیبانی](SUPPORT.md) · [امنیت](SECURITY.md)

## وضعیت

- نسخه عمومی پایدار Integration: **`v1.1.0`**
- مبنای ثابت Contract بانک/SDK: **Sadr Scales `5.2.1`**
- نسخه پایدار فعلی نرم‌افزار Sadr Scales: **`5.3`**
- سازگاری با Sadr Scales 5.3: **در مرحله نهایی تست Package و Vendor Rehearsal**
- Target SDK: `netstandard2.0`
- مصرف Package تأییدشده: .NET Framework 4.8 و .NET جدید
- مجوز: MIT

`v1.1.0` نسخه Vendor-Ready پایدار فعلی است. Contract عمومی SQL/SDK آن عمداً روی سطح یکپارچه‌سازی تأییدشده Sadr Scales 5.2.1 ثابت مانده است؛ ارتقای خود نرم‌افزار Sadr Scales به 5.3 به‌معنی تغییر خودکار Contract نیست. کار فعلی، تست Clean-room از روی Package منتشرشده و کنترل سازگاری روی Sadr Scales 5.3 پیش از ارائه بعدی به شرکت‌های نرم‌افزاری است.

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

سطح ثابت Integration مربوط به Sadr Scales 5.2.1 شامل این موارد است:

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

نسخه `v1.1.0` با این Gateها ساخته و منتشر شده است:

- Build/Test/Pack SDK؛
- SQL Server Integration Tests؛
- Build برنامه WinForms Sample؛
- مصرف واقعی Package روی .NET Framework 4.8؛
- Vendor Acceptance از روی Package و بدون ProjectReference؛
- Public Repository Guard؛
- Release Bundle validation.

تست فعلی بعد از Release از روی خود Package منتشرشده و Sadr Scales 5.3 انجام می‌شود و قرار نیست Contract ثابت 1.1 را بی‌دلیل گسترش دهد.

## مرز امنیتی

این مخزن عمداً پروتکل مستقیم ترازو، Packet خام، Capture شبکه، کلید خصوصی، داده واقعی مشتری، Firmware خصوصی و Runtime Command دلخواه را منتشر نمی‌کند.

Commandهای مدیریت‌شده تایپ‌شده، Service/REST، Webhook/realtime و قابلیت‌های Gateway جزو معماری آینده **Sadr Scales 5.4+** هستند و داخل Contract ثابت SQL/SDK نسخه 1.1 قرار ندارند.

## مجوز

MIT License. فایل‌های [LICENSE](LICENSE) و [NOTICE.md](NOTICE.md) را ببینید.

**Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.**

</div>
