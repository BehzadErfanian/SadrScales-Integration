# گروه‌های ترازو، نگاشت کالا و کلیدهای سریع

این راهنما مربوط به سطح افزوده‌ی `1.1.0` برای Sadr Scales `5.2.1` است.

سه مفهوم این بخش جدا هستند:

```text
ScaleAssignments
گروه‌های کالایی وابسته به یک ترازو

ScaleMappings
نگاشت اختصاصی PLU و ItemCode و در صورت نیاز جایگاه کلید سریع روی همان ترازو

HotKeys
الگوی کلیدهای سریع در سطح گروه کالا
```

## گروه‌های یک ترازو

خواندن گروه‌ها:

```csharp
var groups = await client.ScaleAssignments.GetGroupsAsync(scaleId);
```

جایگزینی کامل گروه‌ها:

```csharp
var result = await client.ScaleAssignments.ReplaceGroupsAsync(
    scaleId,
    new[] { "FOOD", "FRUIT" });
```

نتیجه یکی از این سه حالت است:

```text
NotFound
Unchanged
Replaced
```

حداقل یک گروه معتبر لازم است. اگر واقعاً تغییری رخ دهد، وضعیت ارسال خودکار کالا برای همان ترازو بازنشانی می‌شود تا در چرخه بعدی دوباره بررسی شود. اگر فهرست جدید با وضعیت فعلی برابر باشد، نتیجه `Unchanged` است و درخواست ارسال تازه‌ای ساخته نمی‌شود.

## نگاشت اختصاصی کالا برای یک ترازو

نمونه:

```csharp
var map = new SadrScaleItemMap
{
    PluNo = 1001,
    ItemCode = 1,
    PageNo = 0,
    KeyNo = 1
};
```

خواندن:

```csharp
var mappings = await client.ScaleMappings.GetAsync(scaleId);
```

جایگزینی کامل:

```csharp
var result = await client.ScaleMappings.ReplaceAsync(
    scaleId,
    new[]
    {
        new SadrScaleItemMap { PluNo = 1001, ItemCode = 1 },
        new SadrScaleItemMap { PluNo = 1002, ItemCode = 2, PageNo = 0, KeyNo = 1 }
    });
```

کپی نگاشت از یک ترازو به ترازو دیگر:

```csharp
var result = await client.ScaleMappings.CopyAsync(sourceScaleId, destinationScaleId);
```

قواعد اصلی:

- `PluNo` باید مثبت و در کاتالوگ موجود باشد.
- `ItemCode` در همان ترازو باید مثبت و یکتا باشد.
- یک PLU در نگاشت همان ترازو دوبار تکرار نمی‌شود.
- یک جایگاه `PageNo + KeyNo` دوبار استفاده نمی‌شود.
- `PageNo` و `KeyNo` یا هر دو خالی هستند یا هر دو مقدار دارند.
- جایگاه کلید سریع باید با چیدمان ثبت‌شده‌ی همان ترازو سازگار باشد.
- اگر نگاشت مبدأ با چیدمان مقصد سازگار نباشد، Copy هیچ تغییری در مقصد ایجاد نمی‌کند.

تغییر واقعی Mapping باعث بازنشانی وضعیت ارسال خودکار کالا و کلید سریع همان ترازو می‌شود. `Replaced` فقط یعنی تنظیمات در SQL ثبت شده؛ به معنی دریافت آنی توسط ترازو نیست.

## الگوی کلید سریع در سطح گروه

خواندن کلیدهای کاربر:

```csharp
var hotKeys = await client.HotKeys.GetGroupAsync("FOOD");
```

جایگزینی کامل:

```csharp
var result = await client.HotKeys.ReplaceGroupAsync(
    "FOOD",
    new[]
    {
        new SadrHotKey { PageNo = 0, KeyNo = 1, PluNo = 1001 },
        new SadrHotKey { PageNo = 0, KeyNo = 2, PluNo = 1002 }
    });
```

SDK فقط کلیدهای کاربر با `PluNo > 0` را مدیریت می‌کند. ردیف‌های داخلی که مقدار PLU آن‌ها صفر یا منفی است در API عمومی نمایش داده نمی‌شوند و هنگام Replace هم حذف نمی‌شوند.

اگر Template یک گروه واقعاً تغییر کند، وضعیت ارسال کلید سریع فقط برای ترازوهایی بازنشانی می‌شود که به همان گروه وابسته هستند.

## تفاوت HotKey گروه با Mapping ترازو

HotKey گروه یک الگوی مشترک است:

```text
Group FOOD
Page 0 / Key 1 -> PLU 1001
```

Mapping ترازو مخصوص همان ترازو است:

```text
Scale 03
PLU 1001 -> ItemCode 15 -> Page 0 / Key 3
```

این دو مفهوم در SDK عمداً جدا نگه داشته شده‌اند.

## روند پیشنهادی برای نرم‌افزار ثالث

```text
1. وضعیت فعلی را بخوان
2. فهرست کامل جدید را در نرم‌افزار خودت بساز
3. یک Replace انجام بده
4. نتیجه را بررسی کن
5. اگر Replaced بود، چرخه AutoSend خود Sadr Scales انتقال بعدی را انجام می‌دهد
```

بهتر است به‌جای چند `INSERT` و `DELETE` جدا، از Replace اتمیک استفاده شود تا وضعیت نیمه‌کاره در دیتابیس باقی نماند.

## نمونه SQL

برای نرم‌افزارهایی که از SDK سی‌شارپ استفاده نمی‌کنند:

```text
samples/SQL/06-assignments-mapping-hotkeys.sql
```

نمونه به‌صورت پیش‌فرض فقط خواندنی است و Write نیاز به فعال‌سازی صریح دارد.

## مرز نسخه 5.2.1

این بخش فقط قابلیت‌هایی را پوشش می‌دهد که با SQL در 5.2.1 قابل انجام و تست هستند. فرمان‌های فوری دستگاه، وضعیت اجرای Runtime و Command Mailbox جزو این Contract نیستند. Command Mailbox برای Sadr Scales 5.3 برنامه‌ریزی شده است.
