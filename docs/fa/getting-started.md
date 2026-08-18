<div dir="rtl" align="right">

# شروع سریع — Sadr Scales Integration

این کوتاه‌ترین مسیر پشتیبانی‌شده برای **Sadr Scales 5.2.1 / SQL Contract v1 / SadrScales.Integration 1.x** است.

## ۱. معماری

```text
POS / ERP / Accounting
        ↓
SadrScales.Integration / SQL Contract v1
        ↓
Sadr Scales Runtime
        ↓
ترازوهای پشتیبانی‌شده
```

نرم‌افزار شما با Sadr Scales یکپارچه می‌شود. مدیریت Session، Retry، Registry، تفاوت مدل‌ها و ارتباط مستقیم با ترازو بر عهده Sadr Scales باقی می‌ماند.

## ۲. پیش‌نیازها

- Sadr Scales 5.2.1 یا نسخه جدیدتری که صریحاً با SQL Contract v1 سازگار اعلام شده باشد.
- دسترسی به SQL Server همان نصب Sadr Scales.
- برای SDK زبان C#: .NET Framework 4.8 یا Runtime جدید .NET که `netstandard2.0` را مصرف کند.

ابتدا Sadr Scales را یک‌بار اجرا کنید تا بررسی/به‌روزرسانی Schema خود برنامه کامل شود.

## ۳. اول Contract را کنترل کنید

در C#:

```csharp
var client = new SadrScalesClient(connectionString);
await client.ValidateAsync();
```

یا Validator فقط‌خواندنی SQL را اجرا کنید:

[`samples/SQL/00-validate-contract.sql`](../../samples/SQL/00-validate-contract.sql)

اگر Schema سازگار نیست، Integration را با دورزدن Validation ادامه ندهید.

## ۴. Quick Start اجرایی C# را تست کنید

نمونه‌ای که در CI Build می‌شود اینجاست:

[`samples/csharp/SadrScales.Integration.QuickStart`](../../samples/csharp/SadrScales.Integration.QuickStart/README.md)

این برنامه Connection String را فقط از `SADR_SCALES_CONNECTION_STRING` می‌خواند، Contract v1 را بررسی می‌کند و به‌صورت پیش‌فرض فقط یک خواندن فروش انجام می‌دهد.

```powershell
$env:SADR_SCALES_CONNECTION_STRING = "Server=...;Database=...;..."
dotnet run --project samples/csharp/SadrScales.Integration.QuickStart
```

Connection String واقعی را داخل Source Code قرار ندهید.

## ۵. استفاده از Package نسخه Release

پس از دانلود فایل NuGet از GitHub Release در یک پوشه محلی:

```bash
dotnet add package SadrScales.Integration --version 1.0.0 --source <download-folder>
```

سپس:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);

SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

## ۶. کالا و PLU

- گروه مرجع را قبل از کالا بسازید.
- `PluNo` باید یکتا و غیرصفر باشد.
- `PluNo` شناسه عمومی Contract v1 است؛ از `ID`/`IDitem` به‌عنوان شناسه Integration استفاده نکنید.
- `TimeStamp/rowversion` را ننویسید.
- `UpsertAsync` در صورت یکسان بودن داده‌های معنایی Update بی‌دلیل انجام نمی‌دهد.
- `UpsertBatchAsync` در هر فراخوانی حداکثر **200 PLU یکتا** را در یک Transaction اتمیک می‌پذیرد. انتقال‌های بزرگ‌تر باید در نرم‌افزار مقصد صفحه‌بندی شوند.
- Writeهای Transactional پس از شروع اجرا به‌صورت خودکار Replay نمی‌شوند.

نمونه SQL با Rollback پیش‌فرض: [`samples/SQL/01-upsert-item.sql`](../../samples/SQL/01-upsert-item.sql).

## ۷. فروش

SDK فروش‌های پذیرفته‌شده را افزایشی می‌خواند و برای Ack یا Cursor جدول `SADR_Logs` را Update/Delete نمی‌کند.

قواعد نرم‌افزار مقصد:

- Cursor را در محل پایدار خود نگه دارد؛
- ابتدا فروش را در مقصد ذخیره کند و بعد Cursor را جلو ببرد؛
- از `(DeviceNo, FID, SubID)` برای جلوگیری از Duplicate استفاده کند؛
- فاصله داشتن IDها را طبیعی بداند.

نمونه SQL: [`samples/SQL/02-read-sales-incremental.sql`](../../samples/SQL/02-read-sales-incremental.sql).

## ۸. اگر مشکلی پیش آمد

ابتدا [راهنمای رفع اشکال](troubleshooting.md) را ببینید.

سپس:

- [SQL Contract v1](sql-contract-v1.md)
- [طراحی SDK v1](../SDK_DESIGN_V1.md)
- [سازگاری](../COMPATIBILITY.md)
- [مرز امنیتی](../SECURITY_BOUNDARY.md)
- [راهنمای جامع فارسی](../reference/README.md)

پروتکل مستقیم PLUS، LSG، Aclas و سایر مدل‌ها عمداً خارج از سطح عمومی Integration است.

</div>
