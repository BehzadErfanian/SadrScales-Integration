<div dir="rtl" align="right">

# Sadr Scales SQL Integration Contract v1 — 5.2.1

**وضعیت:** Basic public surface frozen — 2026-08-16

این سند قرارداد عمومی و کوتاه SQL برای اتصال نرم‌افزارهای POS، ERP و حسابداری به Sadr Scales 5.2.1 است. قرارداد بر اساس **Schema مؤثر پس از اجرای migration/check خود Sadr Scales 5.2.1** اعتبارسنجی شده است؛ فایل SQL قدیمی یا Databaseای که هنوز migration برنامه روی آن اجرا نشده، مبنای Contract نیست.

شرح کامل Registry، Mapping، Structured Sales و سایر بخش‌های کنترل‌شده در [راهنمای جامع](../reference/README.md) قرار دارد. سند Freeze فنی نیز در [CONTRACT_V1_FREEZE.md](../CONTRACT_V1_FREEZE.md) ثبت شده است.

## سطح عمومی پایه v1

### `dbo.SADR_ItemClass` — گروه کالا

دسترسی عمومی: **Read / Insert / Update**.

ستون‌های قرارداد:

- `ItemClassCode varchar(50)` — کلید اصلی و اجباری؛
- `ItemClassName nvarchar(100)` — اختیاری؛
- `Descriptions nvarchar(150)` — اختیاری.

رکورد پیش‌فرض با کد `0` توسط Sadr Scales نگهداری می‌شود و نباید بدون migration کنترل‌شده حذف شود.

### `dbo.SADR_Item` — کالا / PLU

دسترسی عمومی: **Read / Insert / Update**. در Integration عادی، `DeleteFlag` به حذف فیزیکی ترجیح دارد.

هویت عمومی کالا در Contract v1، `PluNo` است:

- `PluNo <> 0`؛
- `PluNo` یکتا است؛
- `ItemClassCode` باید از قبل در `SADR_ItemClass` وجود داشته باشد؛
- `TimeStamp` / `rowversion` را هرگز دستی Insert یا Update نکنید.

فیلدهای مسیر پایه کالا:

```text
ItemClassCode
PluNo
PluUnit
UnitPrice
PrintFormat
PluCost
BarFormat
BarFlags
ItemCode
IndexBarcode
Tare
ShelfDate
ShelfDatePrint
SaleDatePrint
SaleTimePrint
OnlyTare
TaxRate
PluName
Text1..Text7
DeleteFlag
```

نکات مهم:

- `ID` و `IDitem` فیلدهای Legacy Schema هستند و هویت Contract v1 محسوب نمی‌شوند.
- `TaxNo` و `SendFlag` در Schema فعلی وجود دارند، اما مسیر پایه Contract v1 به نوشتن آن‌ها وابسته نیست؛ در حالت عادی Default دیتابیس را حفظ کنید.
- `SADR_ItemSyncState` داخلی است و نرم‌افزار ثالث نباید آن را تغییر دهد.
- حتی با وجود Semantic Change Tracking خود Sadr Scales، نوشتن دوباره همه کالاها در هر Cycle بدون تغییر واقعی توصیه نمی‌شود.

### `dbo.SADR_Logs` — خروجی فروش

دسترسی عمومی: **Read-only**.

نکات Schema که برای Integration مهم‌اند:

- `ID` از نوع `int IDENTITY` و Cursor پیشنهادی مصرف‌کننده است؛
- `DeviceNo` در Schema فعلی از نوع `int` و شناسه ترازو است؛
- `(DeviceNo, FID, SubID)` یکتا است و کلید مناسب جلوگیری از ثبت تکراری در مقصد محسوب می‌شود؛
- `FID`، `SubID` و `PLU` صفر نیستند.

فیلدهای قابل خواندن:

```text
ID
DeviceNo
Identify
DateTime
FID
SID
Salesman
SubID
TotalPrice
PLU
Class
Dept
Amount
Unit
LogType
Tax
Text1..Text4
UnitPrice
CoFID
PLUName
```

## الگوی خواندن افزایشی فروش

```sql
SELECT TOP (@BatchSize)
    ID, DeviceNo, Identify, [DateTime], FID, SID,
    Salesman, SubID, TotalPrice, PLU, Class, Dept,
    Amount, Unit, LogType, Tax,
    Text1, Text2, Text3, Text4,
    UnitPrice, CoFID, PLUName
FROM dbo.SADR_Logs
WHERE ID > @LastProcessedId
ORDER BY ID ASC;
```

قواعد مصرف فروش:

1. `LastProcessedId` را در State پایدار نرم‌افزار مقصد نگهداری کنید.
2. ابتدا Batch را در POS/ERP ثبت و Commit کنید؛ سپس Cursor مقصد را جلو ببرید.
3. Import باید Idempotent باشد؛ `(DeviceNo, FID, SubID)` کلید پیشنهادی تشخیص تکرار است.
4. `SADR_Logs` را برای Ack یا Cursor، UPDATE یا DELETE نکنید.
5. فرض نکنید IDها حتماً پیوسته‌اند؛ قرارداد فقط `ID > Cursor` را تضمین می‌کند.

## بخش‌های Advanced / Controlled

Registry ترازو، Store، Mapping، کلیدهای سریع، `SADR_Total` و `SADR_Detail` در راهنمای جامع مستند شده‌اند، اما جزو مسیر پایه Public Contract v1 نیستند. `LableStatus` و `ItemStatus` وضعیت پردازش نرم‌افزار مقصد همین نصب هستند و Cursor دریافت ترازو نیستند.

## قواعد عمومی

- Queryها باید Parameterized باشند.
- عملیات چندجدولی مرتبط باید Transactional باشند.
- Retry خطاهای موقت SQL محدود و Bounded باشد؛ Retry نامحدود مجاز نیست.
- Credential و داده واقعی مشتری وارد Source، Sample، Log یا Repository نشود.
- REST/Webhook عمومی جزو Sadr Scales 5.2.1 و Contract v1 نیست.

## نمونه‌های اجرایی

نمونه‌های SQL با داده ساختگی در [`samples/SQL`](../../samples/SQL/README.md) قرار دارند. ابتدا `00-validate-contract.sql` را روی Database تستی که migration خود Sadr Scales روی آن اجرا شده است اجرا کنید.

</div>
