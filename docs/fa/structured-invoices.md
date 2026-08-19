<div dir="rtl" align="right">

# فاکتور تجمیعی و تأیید دریافت

> وضعیت: قابلیت Vendor-Ready در حال آماده‌سازی برای انتشار بعد از `v1.0.0`.
> این سند Contract منتشرشده‌ی `SQL Contract v1` را تغییر نمی‌دهد.

## هدف

نرم‌افزار فروشگاهی، ERP یا حسابداری می‌تواند فاکتور کامل ثبت‌شده توسط Sadr Scales را با **بارکد تجمیعی** یا با `ScaleID + FID` بخواند.

خواندن فاکتور به‌تنهایی هیچ تغییری در وضعیت آن ایجاد نمی‌کند. نرم‌افزار مقصد فقط بعد از اینکه فاکتور را با موفقیت در دیتابیس خودش ذخیره و Commit کرد، باید ACK بفرستد.

## بارکد تجمیعی

فرمت Sadr Scales 5.2.1:

```text
25 + ScaleID(D3) + FID(D9)
```

نمونه:

```text
ScaleID = 12
FID     = 3456
TotalBarcode = 25012000003456
```

در C# لازم نیست این فرمت را دستی بسازید:

```csharp
string barcode = SadrInvoiceClient.BuildTotalBarcode(12, 3456);
```

## خواندن فاکتور

```csharp
var client = new SadrScalesClient(connectionString);

var result = await client.Invoices.GetByBarcodeAsync(totalBarcode);
```

یا:

```csharp
var result = await client.Invoices.GetAsync(scaleId, fid);
```

نتیجه یکی از این سه حالت است:

```text
FoundUnread  → فاکتور پیدا شده و هنوز ACK نشده است.
AlreadyRead  → فاکتور قبلاً ACK شده ولی داده کامل دوباره برمی‌گردد.
NotFound     → فاکتور پیدا نشد.
```

در هر دو حالت `FoundUnread` و `AlreadyRead`، مدل `SadrInvoice` به همراه همه Detailهای قابل استفاده برمی‌گردد.

## چرا Lookup خودش ACK نمی‌کند؟

ترتیب درست:

```text
Lookup از Sadr Scales
        ↓
دریافت فاکتور کامل
        ↓
BEGIN TRANSACTION در نرم‌افزار مقصد
        ↓
ذخیره Header + Detail
        ↓
COMMIT موفق مقصد
        ↓
ACK به Sadr Scales
```

اگر Lookup همان لحظه `LableStatus = 1` کند و ذخیره مقصد بعداً شکست بخورد، Sadr Scales به اشتباه فاکتور را خوانده‌شده حساب می‌کند. به همین دلیل Read و ACK دو عملیات مستقل‌اند.

## ACK

بعد از Commit موفق مقصد:

```csharp
SadrInvoiceAckStatus ack =
    await client.Invoices.AcknowledgeAsync(totalBarcode);
```

یا:

```csharp
SadrInvoiceAckStatus ack =
    await client.Invoices.AcknowledgeAsync(scaleId, fid);
```

حالت‌های نتیجه:

```text
Acknowledged          → LableStatus از unread به 1 تغییر کرد.
AlreadyAcknowledged   → قبلاً ACK شده بود؛ عملیات تکراری بی‌خطر است.
NotFound              → فاکتور پیدا نشد.
```

ACK در سطح کل فاکتور با `SADR_Total.LableStatus` انجام می‌شود.
`SADR_Detail.ItemStatus` در این Contract معنی ACK عمومی ندارد و نرم‌افزار مقصد نباید آن را برای این کار تغییر دهد.

## رفتار AlreadyRead

`AlreadyRead` خطای مسدودکننده نیست.

مثلاً ممکن است نرم‌افزار مقصد قبلاً فاکتور را گرفته و ACK کرده باشد ولی رکورد خودش بعداً به اشتباه حذف شده باشد. در این حالت اسکن دوباره باید:

1. هشدار بدهد که فاکتور قبلاً خوانده شده؛
2. همان فاکتور کامل را نمایش دهد؛
3. اجازه دهد نرم‌افزار/کاربر با آگاهی از وضعیت، بازیابی یا ورود دوباره را انجام دهد.

SDK تصمیم تجاری مقصد را Block نمی‌کند.

## نمونه Raw SQL

برای نرم‌افزارهایی که C# SDK استفاده نمی‌کنند:

[`samples/SQL/03-structured-invoice-lookup-ack.sql`](../../samples/SQL/03-structured-invoice-lookup-ack.sql)

نمونه به‌صورت پیش‌فرض فقط Read است و ACK فقط با فعال‌کردن صریح بخش مربوط انجام می‌شود.

## نکته Retry

Lookup یک عملیات فقط‌خواندنی است و می‌تواند از Retry محدود اتصال/Read استفاده کند.

ACK یک Write تراکنشی است. SDK فقط بازکردن Connection را پیش از شروع عملیات می‌تواند Retry کند؛ بعد از شروع Transaction، ACK به‌صورت کور Replay نمی‌شود.

</div>
