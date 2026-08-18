<div dir="rtl" align="right">

# یکپارچه‌سازی Sadr Scales

[![SDK CI](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/sdk-ci.yml)
[![Public Repository Guard](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml/badge.svg)](https://github.com/BehzadErfanian/SadrScales-Integration/actions/workflows/public-repo-guard.yml)

**مخزن عمومی رسمی قرارداد SQL، ابزار توسعه C# و نمونه‌های اتصال نرم‌افزارهای فروشگاهی، ERP و حسابداری به Sadr Scales.**

**ارائه و نگهداری توسط Tozin Sadr و Behzad Erfanian.**

[English](README.md) · [شروع سریع](docs/fa/getting-started.md) · [رفع اشکال](docs/fa/troubleshooting.md) · [چک‌لیست آمادگی بهره‌برداری](docs/PRODUCTION_READINESS_CHECKLIST.md) · [قرارداد SQL v1](docs/fa/sql-contract-v1.md) · [سازگاری](docs/COMPATIBILITY.md) · [پشتیبانی](SUPPORT.md) · [امنیت](SECURITY.md)

---

## وضعیت

کار مهندسی **`v1.0.0`** و آماده‌سازی Release در مرحله بازبینی نهایی است. Tag پایدار عمداً تا بازبینی تنظیمات امنیتی مالک Repository منتشر نمی‌شود.

مبنای عمومی نسخه 1:

| بخش | مبنا |
|---|---|
| Sadr Scales | `5.2.1` یا نسخه جدیدتری که صریحاً با SQL Contract v1 سازگار اعلام شده باشد |
| قرارداد عمومی بانک | **SQL Contract v1** |
| SDK زبان C# | `SadrScales.Integration 1.0.0` |
| Target SDK | `netstandard2.0` |
| SQL Provider | `Microsoft.Data.SqlClient 7.0.2` |
| .NET Framework | مصرف واقعی Package روی `net48` در CI |
| تست SQL | SQL Server 2022 با Schema و داده ساختگی Contract v1 |

## چرا از این مسیر یکپارچه‌سازی کنیم؟

نرم‌افزار شما با **Sadr Scales** یکپارچه می‌شود، نه با پروتکل اختصاصی مستقیم هر مدل ترازو.

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration / SQL Contract v1
        ↓
Sadr Scales Runtime
        ↓
PLUS / LSG / Aclas / ترازوهای پشتیبانی‌شده
```

مدیریت Session، Retry/Reconnect، Registry، تفاوت مدل‌ها و ارتباط مستقیم با دستگاه بر عهده Runtime خود Sadr Scales باقی می‌ماند.

## مسیر پنج‌دقیقه‌ای C#

### ۱. ابتدا Contract بانک را کنترل کنید

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

اگر Schema سازگار نیست، Integration را با دورزدن Validation ادامه ندهید.

### ۲. گروه و کالا/PLU را ثبت یا به‌روزرسانی کنید

```csharp
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);
```

برای انتقال گروهی محدود:

```csharp
SadrItemBatchWriteResult result = await client.Items.UpsertBatchAsync(items);
```

هر Batch حداکثر **۲۰۰ PLU یکتا** دارد، پیش از SQL کامل بررسی می‌شود و در یک Transaction اتمیک Commit می‌شود. انتقال بزرگ‌تر باید توسط نرم‌افزار شما صفحه‌بندی شود.

### ۳. فروش پذیرفته‌شده را افزایشی بخوانید

```csharp
SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

نرم‌افزار مقصد مالک State پایدار خودش است:

1. فروش را در بانک خودش ثبت کند؛
2. ثبت مقصد را Commit کند؛
3. بعد `batch.LastReadId` را به‌عنوان Cursor بعدی ذخیره کند.

برای جلوگیری از تکرار، کلید پیشنهادی `(DeviceNo, FID, SubID)` است. فاصله داشتن `ID`های فروش طبیعی است.

## Quick Start اجرایی

نمونه C# واقعی که در CI Build می‌شود و به‌صورت پیش‌فرض فقط خواندنی است:

[`samples/csharp/SadrScales.Integration.QuickStart`](samples/csharp/SadrScales.Integration.QuickStart/README.md)

Connection String فقط از `SADR_SCALES_CONNECTION_STRING` خوانده می‌شود:

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.QuickStart
```

Connection String واقعی را داخل Source Code قرار ندهید.

## نصب Package

فایل‌های SDK پایدار از طریق **GitHub Releases** منتشر می‌شوند. پس از دانلود `SadrScales.Integration.1.0.0.nupkg` در یک پوشه محلی:

```bash
dotnet add package SadrScales.Integration --version 1.0.0 --source <download-folder>
```

Release همچنین Symbol Package، DLL و XML Documentation، بسته Developer Kit، راهنمای رسمی فارسی و SHA-256 همه فایل‌ها را دارد.

## مسیر Raw SQL

برای Integration استفاده از C# اجباری نیست. نمونه‌های مستقل از زبان در [`samples/SQL`](samples/SQL/README.md) وجود دارند:

- کنترل Schema؛
- Upsert امن گروه و PLU با Rollback پیش‌فرض؛
- خواندن افزایشی و فقط‌خواندنی فروش.

این مسیر برای Java، Python، Node.js، PHP و سایر زبان‌ها نیز مبنای اولیه است تا Wrapper اختصاصی آن زبان ساخته شود.

## SQL Contract v1

سطح عمومی پایه عمداً کوچک نگه داشته شده است:

- `dbo.SADR_ItemClass` — گروه کالا؛ SELECT / INSERT / UPDATE؛
- `dbo.SADR_Item` — کالا/PLU؛ SELECT / INSERT / UPDATE؛ شناسه عمومی `PluNo` است؛
- `dbo.SADR_Logs` — فروش پذیرفته‌شده؛ فقط **SELECT**.

قواعد مهم:

- `rowversion` را ننویسید؛
- از `ID`/`IDitem` قدیمی به‌عنوان هویت عمومی PLU استفاده نکنید؛
- برای Ack یا Cursor جدول `SADR_Logs` را Update/Delete نکنید؛
- Cursor پایدار و Idempotency متعلق به نرم‌افزار مقصد است.

Registry، Mapping، جزئیات داخلی فاکتور ساخت‌یافته و Runtime State تا زمانی که Contract عمومی جداگانه‌ای تصویب نشود، سطح پیشرفته/کنترل‌شده باقی می‌مانند.

## رفتار ارتباط و Retry

SDK فقط در مرزهایی که Replay امن است Retry خودکار دارد:

- بازکردن Connection پیش از شروع Command؛
- اجرای کامل و فقط‌خواندنی Contract Validation؛
- اجرای کامل و فقط‌خواندنی Sales Read.

Writeهای Transactional گروه و کالا پس از شروع اجرا **به‌صورت خودکار Replay نمی‌شوند**؛ چون با گم‌شدن پاسخ ممکن است وضعیت Commit مبهم شود.

## تحویل برای بهره‌برداری واقعی

پیش از فعال‌کردن اتصال در محیط مشتری، [چک‌لیست آمادگی بهره‌برداری](docs/PRODUCTION_READINESS_CHECKLIST.md) را کامل کنید. این چک‌لیست کنترل نسخه و Contract، امنیت دسترسی بانک، قواعد کالا، Cursor و جلوگیری از ثبت تکراری فروش، تست Restart/Rollback و تحویل عملیاتی را پوشش می‌دهد.

## سازگاری و پایداری API

نسخه `1.0.0` اولین خط پایدار API است. نسخه‌های `1.x` از Semantic Versioning پیروی می‌کنند و نسخه SQL Contract جداگانه مدیریت می‌شود.

جزئیات در [سیاست سازگاری API](docs/API_COMPATIBILITY.md) ثبت شده است.

## سندهای اصلی

- [شروع سریع](docs/fa/getting-started.md)
- [راهنمای رفع اشکال](docs/fa/troubleshooting.md)
- [چک‌لیست آمادگی بهره‌برداری](docs/PRODUCTION_READINESS_CHECKLIST.md)
- [SQL Contract v1](docs/fa/sql-contract-v1.md)
- [سیاست سازگاری API](docs/API_COMPATIBILITY.md)
- [طراحی SDK v1](docs/SDK_DESIGN_V1.md)
- [سازگاری نسخه‌ها](docs/COMPATIBILITY.md)
- [مرز امنیتی](docs/SECURITY_BOUNDARY.md)
- [هویت راهنمای رسمی فارسی](docs/reference/README.md)
- [سیاست پشتیبانی](SUPPORT.md)
- [راهنمای مشارکت](CONTRIBUTING.md)

## پشتیبانی و امنیت

برای ایرادهای قابل بازتولید SDK یا Contract که اطلاعات حساس ندارند از GitHub Issues استفاده کنید. پیش از انتشار اطلاعات مربوط به مشتری، [SUPPORT.md](SUPPORT.md) را بخوانید.

گزارش‌های امنیتی باید طبق [SECURITY.md](SECURITY.md) ارسال شوند و نباید در Issue عمومی عادی منتشر شوند.

پروتکل مستقیم دستگاه، Capture شبکه، کلید خصوصی، Credential، اطلاعات مشتری، Firmware/Vendor material و Source یا زیرساخت داخلی Sadr Scales اجازه ورود به این Repository عمومی را ندارند.

## Gateهای Release

قبل از ساخت Draft Release پایدار، Automation این موارد را کنترل می‌کند:

- مرز امنیتی Repository عمومی و وجود فایل‌های الزامی حاکمیتی/Release؛
- Restore/Build/Test SDK؛
- ساختار NuGet Package، مشخصات هر دو ارائه‌دهنده، مجوز MIT و Source Link/Repository metadata؛
- Quick Start اجرایی C#؛
- تست واقعی SQL Server 2022؛
- مصرف واقعی Package در .NET Framework 4.8؛
- ساخت Release bundle و SHA-256؛
- Hash راهنمای رسمی Integration.

GitHub Release پس از این مراحل همچنان **Draft** باقی می‌ماند تا بازبینی انسانی انجام شود.

## مجوز و ارائه‌دهندگان

SDK عمومی `SadrScales-Integration` و محتوای عمومی مشمول فایل [LICENSE](LICENSE) با مجوز **MIT** منتشر می‌شوند.

**Copyright (c) 2026 Tozin Sadr and Behzad Erfanian.**

این مجوز شامل Source خصوصی Sadr Scales، پروتکل‌های اختصاصی ترازو، Firmware، کلیدهای خصوصی، اطلاعات مشتری یا سایر موارد خارج از این Repository نمی‌شود. برای مرزبندی کامل [NOTICE.md](NOTICE.md) را ببینید.

</div>
