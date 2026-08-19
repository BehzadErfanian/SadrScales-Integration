# جست‌وجوی فروش و گزارش‌ها

این راهنما مربوط به سطح افزوده‌ی `1.1.0` برای Sadr Scales `5.2.1` است و فقط داده‌های فروش موجود در SQL را می‌خواند.

## تفاوت Feed با Query

دو مسیر فروش عمداً کاربرد متفاوت دارند.

### Feed افزایشی برای نرم‌افزار مقصد

```csharp
var batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

این مسیر برای نرم‌افزاری است که فروش‌ها را پیوسته وارد دیتابیس خودش می‌کند. ترتیب بر اساس `ID` صعودی است و Cursor را خود نرم‌افزار مقصد نگه می‌دارد.

### Query برای جست‌وجو و نمایش

```csharp
var page = await client.Sales.QueryAsync(filter);
```

این مسیر برای فرم جست‌وجو، بررسی فروش و گزارش است. نتیجه از جدیدترین فروش به قدیمی‌ترین فروش مرتب می‌شود و Cursor نرم‌افزار مقصد را تغییر نمی‌دهد.

پس Query جایگزین Feed و Cursor نیست.

## فیلتر فروش

```csharp
var filter = new SadrSalesQueryFilter
{
    StartDateInclusive = new DateTime(2026, 8, 1),
    EndDateExclusive = new DateTime(2026, 9, 1),
    ScaleId = 3,
    Plu = 1001,
    PageNumber = 1,
    PageSize = 200
};

var page = await client.Sales.QueryAsync(filter);
```

فیلترهای اختیاری:

- تاریخ و ساعت شروع، به‌صورت شامل؛
- تاریخ و ساعت پایان، به‌صورت غیرشامل؛
- مقدار دقیق `Identify`؛
- شماره PLU؛
- شماره ترازو / `DeviceNo`؛
- FID فاکتور.

صفحه‌بندی مطابق رفتار 5.2.1 است:

- شماره صفحه حداقل 1 است؛
- اندازه صفحه بین 50 تا 2000 محدود می‌شود؛
- نتیجه خالی یک صفحه گزارش می‌کند.

## جمع کل

هر نتیجه Query یک Summary دارد که مربوط به **کل فیلتر** است، نه فقط ردیف‌های صفحه فعلی:

```text
RecordCount
InvoiceCount
TotalPrice
TotalWeight
TotalQuantity
```

`InvoiceCount` بر اساس ترکیب یکتای `(DeviceNo, FID)` محاسبه می‌شود. بنابراین چند ردیف کالای یک فاکتور فقط یک فاکتور به شمار می‌آیند.

تفکیک وزن و تعداد مطابق 5.2.1:

- Unitهای `0`، `1` و `3` در `TotalWeight` جمع می‌شوند؛
- Unit برابر `2` در `TotalQuantity` جمع می‌شود.

## بازه‌های آماده

برای اینکه نرم‌افزار ثالث مجبور نباشد مرز هفته و ماه مورد استفاده Sadr Scales را دوباره پیاده کند:

```csharp
var range = SadrSalesPeriod.GetRange(
    SadrSalesPeriodPreset.CurrentMonth,
    DateTime.Today);

filter.StartDateInclusive = range.StartDateInclusive;
filter.EndDateExclusive = range.EndDateExclusive;
```

بازه‌ها:

- `Today` — همان روز؛
- `CurrentWeek` — هفته از شنبه تا جمعه؛
- `CurrentMonth` — ماه شمسی شامل تاریخ مرجع.

## گزارش‌های Typed

همه گزارش‌ها همان `SadrSalesQueryFilter` را می‌پذیرند.

### گزارش روزانه

```csharp
var rows = await client.Reports.GetDailyAsync(filter);
```

برای هر تاریخ فروش یک ردیف تجمیعی می‌دهد و جدیدترین تاریخ اول است.

### گزارش بر اساس ترازو

```csharp
var rows = await client.Reports.GetByScaleAsync(filter);
```

برای هر شماره ترازو یک ردیف می‌دهد و بیشترین مبلغ فروش در بالا قرار می‌گیرد.

### گزارش بر اساس کالا

```csharp
var rows = await client.Reports.GetByItemAsync(filter);
```

برای هر PLU یک ردیف تجمیعی می‌دهد و بیشترین مبلغ فروش در بالا قرار می‌گیرد. مطابق 5.2.1 حداکثر 5000 ردیف برمی‌گردد.

هر ردیف گزارش همان Summary شامل تعداد رکورد، تعداد فاکتور، مبلغ، وزن و تعداد را دارد.

## فقط خواندنی

Query و Reports هیچ‌کدام:

- `SADR_Logs` را تغییر نمی‌دهند؛
- ACK فاکتور را تغییر نمی‌دهند؛
- Cursor نرم‌افزار مقصد را تغییر نمی‌دهند؛
- وضعیت ترازو را تغییر نمی‌دهند.

بنابراین این عملیات Read-only هستند.

## نمونه SQL

برای نرم‌افزارهایی که از SDK سی‌شارپ استفاده نمی‌کنند:

```text
samples/SQL/07-sales-query-reports.sql
```

این فایل همان فیلترها، Summary، صفحه‌بندی و گزارش‌های روزانه/ترازو/کالا را فقط با SQL نشان می‌دهد.

## مرز نسخه 5.2.1

این قابلیت‌ها فروش‌هایی را گزارش می‌کنند که قبلاً داخل Sadr Scales ذخیره شده‌اند. فرمان «الان از خود ترازو فروش جدید بگیر» یک عملیات Runtime/Device است و جزو Contract SQL فعلی 5.2.1 نیست. این دسته عملیات برای Command Mailbox نسخه 5.3 برنامه‌ریزی شده است.
