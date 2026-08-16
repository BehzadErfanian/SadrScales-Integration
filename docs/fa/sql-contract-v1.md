<div dir="rtl" align="right">

# Sadr Scales SQL Integration Contract v1 — 5.2.1

این سند خلاصه قرارداد عمومی SQL است. شرح کامل Schema، Registry، Mapping، Structured Sales و نمونه‌های C# در [راهنمای جامع](../reference/README.md) قرار دارد.

## Objectهای عمومی پایه

### ورودی کالا

- `dbo.SADR_ItemClass` — گروه کالا؛ Read/Insert/Update.
- `dbo.SADR_Item` — کالا/PLU؛ Read/Insert/Update، حذف منطقی ترجیح دارد.

### خروجی فروش

- `dbo.SADR_Logs` — فروش‌های پذیرفته‌شده؛ **Read-only** برای نرم‌افزار ثالث.

## قواعد اجباری

1. `PluNo <> 0`.
2. `PluNo` یکتا است.
3. `ItemClassCode` باید پیش از کالا وجود داشته باشد.
4. `TimeStamp` / `rowversion` را دستی Insert یا Update نکنید.
5. Queryها Parameterized و عملیات چندجدولی Transactional باشند.
6. برای حذف/غیرفعال‌سازی کالا، `DeleteFlag` را به حذف فیزیکی ترجیح دهید.
7. `SADR_Logs` را UPDATE یا DELETE نکنید.
8. Cursor فروش در State نرم‌افزار مقصد نگهداری شود؛ الگوی پایه `ID` صعودی است.
9. UPDATE بی‌دلیل همه کالاها در هر Cycle توصیه نمی‌شود، حتی اگر Semantic Fingerprint تغییر واقعی را تشخیص دهد.
10. Registry ترازو، Mapping داخلی و Stateهای Runtime خارج از مسیر پایه Public Contract v1 هستند.

## فیلدهای اصلی کالا

```text
ItemClassCode
DeleteFlag
PluNo
ItemCode
IndexBarcode
PluName
PluUnit
UnitPrice
PrintFormat
BarFormat
BarFlags
PluCost
ShelfDate
ShelfDatePrint
SaleDatePrint
SaleTimePrint
OnlyTare
Tare
TaxRate
Text1..Text7
```

`TaxNo` و `SendFlag` در Schema فعلی وجود دارند، اما مسیر پایه Contract v1 به نوشتن آنها وابسته نیست.

## فیلدهای فروش قابل خواندن

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

`LastProcessedId` را فقط پس از ثبت موفق رکوردها در سیستم مقصد جلو ببرید.

## Structured Sales

`SADR_Total` و `SADR_Detail` در راهنمای جامع مستند شده‌اند و برای سناریوهای ساخت‌یافته/کنترل‌شده قابل استفاده‌اند. `LableStatus` و `ItemStatus` وضعیت پردازش همان نرم‌افزار مقصد این نصب هستند و Cursor داخلی دریافت ترازو نیستند.

## Compatibility

Contract v1 برای Sadr Scales 5.2.1 مستند شده است. REST/Webhook عمومی جزو این Contract نیست.

</div>
