<div dir="rtl" align="right">

# یکپارچه‌سازی Sadr Scales

[![SDK CI](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml)
[![Public Repository Guard](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml)

**مخزن عمومی رسمی اتصال نرم‌افزارهای فروشگاهی، ERP و حسابداری به Sadr Scales.**

ارائه و نگهداری توسط **Tozin Sadr و Behzad Erfanian**.

[English](README.md) · [شروع سریع](docs/fa/getting-started.md) · [قابلیت‌ها](docs/fa/capabilities.md) · [نمونه‌های SQL](samples/SQL/README.md) · [برنامه نمونه](samples/csharp/SadrScales.Integration.SampleApp/README.md) · [انتشارها](https://github.com/BehzadErfanian/SadrScales-Integration/releases) · [پشتیبانی](SUPPORT.md) · [امنیت](SECURITY.md)

## وضعیت

- نسخه عمومی پایدار SDK: **`v1.1.0`**
- بسته: **`SadrScales.Integration.1.1.0`**
- مبنای قرارداد تأییدشده Sadr Scales: **`5.2.1`**
- انتشار برنامه Sadr Scales **`5.3`** در حال تکمیل است و قرارداد SQL/SDK سازگاری عقب‌رو را حفظ می‌کند.
- تمرین Clean-room نهایی شرکت نرم‌افزاری و راهنمای جدید برنامه‌نویسان سایت عمداً **بعد از انتشار Sadr Scales 5.3** انجام می‌شود.
- Target SDK: `netstandard2.0`
- مصرف Package تأییدشده: .NET Framework 4.8 و .NET جدید
- مجوز: MIT

`v1.1.0` بسته عمومی فعلی و Freeze‌شده SDK است و فایل‌های Release آن روی GitHub منتشر شده‌اند. مرحله بعدی مستندات، اجرای Vendor Rehearsal فقط از روی Package در محیط Sadr Scales 5.3 منتشرشده است. راهنمای رسمی سایت فقط بعد از سبز شدن همان تست نهایی می‌شود.

## از اینجا شروع کنید

```text
۱. شروع سریع را بخوانید
۲. قرارداد بانک را بررسی کنید
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

## قابلیت‌های v1.1.0

سطح تأییدشده شامل شعبه‌ها، گروه کالا و کالا/PLU، تاریخچه قیمت، ترازوها، تخصیص گروه، Mapping هر ترازو، HotKey، درخواست AutoSend، Sales Feed افزایشی، Sales Query و Summary، دریافت ساختاریافته فاکتور، ACK صریح و Idempotent و گزارش‌های روزانه/ترازو/کالا است.

نقشه کامل در [صفحه قابلیت‌ها](docs/fa/capabilities.md) قرار دارد.

## قانون مهم فاکتور

```text
دریافت فاکتور
→ ذخیره در مقصد
→ Commit موفق مقصد
→ ACK در Sadr Scales
```

Lookup هیچ‌وقت خودکار ACK نمی‌کند. فاکتور ACK شده همچنان کامل با `AlreadyRead` برمی‌گردد تا بازیابی و ورود دوباره ممکن باشد.

## برنامه نمونه اجرایی

[`samples/csharp/SadrScales.Integration.SampleApp`](samples/csharp/SadrScales.Integration.SampleApp/README.md)

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.SampleApp
```

Credential واقعی را داخل Source قرار ندهید.

## مسیر غیر C# / Raw SQL

استفاده از C# اجباری نیست. نمونه‌های مستقل از زبان در [`samples/SQL`](samples/SQL/README.md) قرار دارند. برای قابلیت‌هایی که مستند نشده‌اند، Write تازه روی جدول‌ها یا ستون‌های داخلی اختراع نکنید.

## ایمنی Demo Data

DemoLab جزو API تولیدی SDK نیست. Demo marker را روی دیتابیس مشتری یا Production فعال نکنید.

## Gate کیفیت Release

بسته `v1.1.0` Gateهای Build/Test/Package/Public Repository را گذرانده است. قبل از اینکه راهنمای برنامه‌نویسان سایت نهایی اعلام شود، Vendor Rehearsal از روی Package و در برابر Sadr Scales منتشرشده نیز باید PASS شود.

## مرز امنیتی

این مخزن عمداً پروتکل مستقیم ترازو، Packet خام، Capture شبکه، کلید خصوصی، داده واقعی مشتری، Firmware خصوصی و Runtime Command دلخواه را منتشر نمی‌کند.

Integration Command Mailbox تایپ‌شده برای هر ترازو به مسیر معماری آینده **Sadr Scales 5.4** منتقل شده است؛ اگر 5.4 برای یک انتشار عمومی میانی مصرف شود، این کار به اولین نسخه دو بخشی آزاد بعدی منتقل می‌شود. این قابلیت جزو SDK `v1.1.0` نیست.

## صحت انتشار

GitHub Release `v1.1.0` مرجع عمومی بسته است. SHA-256 بسته NuGet:

```text
2baa100d6cf3125c75edbb7e99e1d15ff3e99d0bcd52534180ebe3f29d9d359f
```

## مجوز

MIT License. فایل‌های [LICENSE](LICENSE) و [NOTICE.md](NOTICE.md) را ببینید.

**Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.**

</div>
