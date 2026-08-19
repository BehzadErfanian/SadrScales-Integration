<div dir="rtl" align="right">

# شعبه‌ها و کاتالوگ کالا

این راهنما مربوط به قابلیت‌های Vendor-Ready در **Sadr Scales 5.2.1 / SadrScales.Integration 1.1.0** است.

## ۱. شعبه‌ها

هویت عمومی شعبه `StoreCode` است؛ نام شعبه شناسه نیست.

```csharp
var stores = await client.Stores.GetAllAsync();
var store = await client.Stores.GetAsync("S1");

var result = await client.Stores.UpsertAsync(new SadrStore
{
    StoreCode = "S1",
    StoreName = "شعبه یک",
    Descriptions = "فروشگاه نمونه"
});
```

نتیجه Upsert یکی از این موارد است:

```text
Inserted
Updated
Unchanged
```

Sadr Scales یک شعبه پیش‌فرض با کد `0` دارد. نرم‌افزار ثالث نباید نام شعبه را به‌جای `StoreCode` به‌عنوان هویت نگه دارد.

## ۲. گروه کالا

```csharp
var groups = await client.ItemGroups.GetAllAsync();
var group = await client.ItemGroups.GetAsync("G1");
await client.ItemGroups.UpsertAsync(groupToSave);
```

هویت عمومی گروه `ItemClassCode` است.

## ۳. خواندن کالاها

خواندن معمولی فقط کالاهای فعال را برمی‌گرداند:

```csharp
var activeItems = await client.Items.GetAllAsync();
```

برای دیدن کالاهای حذف منطقی‌شده:

```csharp
var allItems = await client.Items.GetAllAsync(includeDeleted: true);
```

خواندن مستقیم یک PLU حتی اگر حذف منطقی شده باشد همچنان ممکن است:

```csharp
var item = await client.Items.GetAsync(pluNo);
```

این رفتار برای بررسی، بازیابی و همگام‌سازی دوباره مهم است.

## ۴. حذف کالا

حذف عمومی کالا در 1.1.0 **فیزیکی نیست**:

```csharp
var result = await client.Items.SoftDeleteAsync(pluNo);
```

نتیجه:

```text
Deleted
AlreadyDeleted
NotFound
```

`SoftDeleteAsync` مقدار `DeleteFlag` را 1 می‌کند و ردیف کالا در دیتابیس باقی می‌ماند. مسیرهای عادی ارسال کالا در Sadr Scales 5.2.1 کالاهای دارای `DeleteFlag != 0` را فعال در نظر نمی‌گیرند.

برای برگرداندن یک کالا، داده کامل آن را بخوانید، مقدارهای موردنظر را اصلاح کنید، `DeleteFlag = 0` قرار دهید و Upsert کنید.

**نکته مهم:** هنگام ویرایش کالای موجود فقط با یک شیء خالی و چند فیلد Upsert نکنید؛ ابتدا کالا را بخوانید تا تنظیمات چاپ، بارکد، Tare و متن‌های استفاده‌نشده ناخواسته صفر نشوند. Sample App همین الگوی امن را نشان می‌دهد.

## ۵. تاریخچه قیمت

در Contract فعلی، Price History فقط‌خواندنی است:

```csharp
var history = await client.Items.GetPriceHistoryAsync(pluNo, 100);
var recent = await client.Items.GetRecentPriceHistoryAsync(100);
```

فیلدها شامل PLU، بارکد، نام کالا، قیمت قبلی، قیمت جدید، زمان و کاربر/منبع ثبت‌شده هستند.

در 1.1.0 هیچ قانون جدیدی تعریف نشده که هر `Items.UpsertAsync` الزاماً یک ردیف `SADR_PriceLog` بسازد؛ سورس 5.2.1 چنین رفتار عمومی و قطعی‌ای را به‌عنوان Contract ثابت نمی‌کند.

## ۶. نمونه SQL

برای مصرف‌کننده‌های غیر C#:

- [`samples/SQL/01-upsert-item.sql`](../../samples/SQL/01-upsert-item.sql) — Upsert گروه و کالا.
- [`samples/SQL/05-catalog.sql`](../../samples/SQL/05-catalog.sql) — خواندن شعبه/گروه/کالا، حذف منطقی و تاریخچه قیمت.

نمونه `05-catalog.sql` به‌صورت پیش‌فرض فقط‌خواندنی است و تا زمانی که `@ApplyChanges = 1` نشود تغییری اعمال نمی‌کند.

## ۷. مرز مسئولیت

این APIها داده‌های کاتالوگ Sadr Scales را مدیریت می‌کنند. ارسال واقعی و فوری کالا به ترازو یک مفهوم جداست. در 5.2.1 فقط می‌توان از Resend Request برای چرخه AutoSend استفاده کرد؛ Commandهای فوری برای 5.3 برنامه‌ریزی شده‌اند.

</div>
