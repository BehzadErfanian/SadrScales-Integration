# طراحی Contract و Domain مستقل از Transport — Phase 2

**وضعیت:** پیش‌نویس Phase 2 برای بازبینی مالک/نگهدارنده  
**تاریخ:** 2026-08-19  
**Sadr Scales baseline:** `5.2.1`  
**Sadr Scales source commit:** `1048749f52faba35e69464b64983e772c1c857e3`  
**Integration baseline:** `7690e2a622201cc9b5f0f112c6d6099d44c4233e`  
**Phase 1 audit:** `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`

---

## 1. هدف Phase 2

هدف این فاز تعریف Contract عمومی ساده، کامل و قابل توسعه برای شرکت‌های ثالث است؛ بدون اینکه مصرف‌کننده مجبور شود ساختار داخلی Sadr Scales یا پروتکل ترازوها را بداند.

این سند چهار چیز را مشخص می‌کند:

1. Domain عمومی Integration؛
2. Public API سطح بالا؛
3. SQL Contract قابل استفاده با Sadr Scales 5.2.1؛
4. مرز عملیات‌هایی که برای اجرا روی خود ترازو به یک کانال مدیریت‌شده در Sadr Scales نیاز دارند.

این سند پیاده‌سازی نیست و `v1.0.0` و `SQL Contract v1` منتشرشده را تغییر نمی‌دهد.

---

## 2. اصل کلیدی: Domain با Transport یکی نیست

مفهوم‌های عمومی ثابت می‌مانند:

```text
Store / Branch
Scale
Scale Status
Item Group
Item / PLU
Scale Assignment
Hot Key
Sales Feed
Structured Invoice
Invoice Acknowledgement
Reports
Device Commands
```

اما روش دسترسی می‌تواند تغییر کند:

```text
Integration Domain
        │
        ├── SQL               ← روش اصلی قابل استفاده با 5.2.1
        ├── Future Local API
        ├── Future REST/API
        ├── Future XML
        └── Future Webhook / other
```

برنامه‌نویس نباید با تغییر Transport مجبور شود معنی `Scale`, `Item` یا `Invoice` را دوباره یاد بگیرد.

---

## 3. منظور از Runtime چیست؟

در این اسناد، **Runtime** یعنی خود پردازش در حال اجرای Sadr Scales و اجزای داخلی آن که مسئول این کارها هستند:

```text
Scale connections
Registry
Heartbeat
Queues
Send/Get operations
Reconnect
In-flight operations
Device capability checks
```

### نکته مهم برای 5.2.1

Runtime در نسخه 5.2.1 **API عمومی برای نرم‌افزار ثالث نیست**.

پس شرکت ثالث در Integration فعلی:

```text
Third-party software
        ↓
       SQL
        ↓
Sadr Scales Database
```

کار می‌کند.

اگر در آینده Local Service یا REST Gateway اضافه شود، آن کانال می‌تواند عملیات فعال Runtime را به شکل مدیریت‌شده در اختیار Integration قرار دهد.

---

## 4. Public API سطح بالا

API باید بر اساس مفهوم تجاری باشد:

```csharp
sadr.Connection
sadr.Stores
sadr.ItemGroups
sadr.Items
sadr.Scales
sadr.ScaleAssignments
sadr.HotKeys
sadr.Sales
sadr.Invoices
sadr.Reports
sadr.Commands
```

نام جدول‌هایی مثل `SADR_Total`, `SADR_ScaleItemClass` یا `SADR_KeyAssignment` جزئیات Storage هستند و برای استفاده عادی نباید لازم باشند.

---

# 5. Domain عمومی

## 5.1 Store / Branch

```csharp
public sealed class SadrStore
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
```

**Source of truth:** `dbo.SADR_Store`

قواعد:

- `Code` هویت Store است؛
- `Code = "0"` شعبه پیش‌فرض است؛
- `Name` فقط نام نمایشی است؛
- Upsert مجاز است؛
- ارتباط Scale با Store بر اساس `StoreCode` است.

---

## 5.2 Item Group

```csharp
public sealed class SadrItemGroup
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
```

**Source of truth:** `dbo.SADR_ItemClass`

Group مستقل از assignment گروه به Scale است.

---

## 5.3 Item / PLU

هویت عمومی کالا `PluNo` است.

قواعد:

- `PluNo = 0` نامعتبر است؛
- Insert/Update عمومی است؛
- حذف کالا soft-delete است؛
- Batch باید bounded و atomic باشد؛
- internal sync/delivery state عمومی نیست؛
- Price history فقط Read عمومی است.

---

## 5.4 Scale Definition

```csharp
public sealed class SadrScale
{
    public int ScaleId { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public string Model { get; set; }
    public string DeviceName { get; set; }
    public string StoreCode { get; set; }
    public bool AutoSendItems { get; set; }
    public bool AutoGetSales { get; set; }
    public bool IsEnabled { get; set; }
    public int HotKeyCountPerPage { get; set; }
    public int HotKeyPageCount { get; set; }
}
```

Static scale data از `dbo.SADR_Scale` خوانده می‌شود.

Lifecycle واقعی Scale مثل افزودن/حذف/تغییراتی که Registry و اتصال فعال را درگیر می‌کنند نباید با چند SQL خام ناقص تقلید شود. برای این عملیات در نسخه‌های بعدی باید Managed Command Channel طراحی شود.

---

## 5.5 Scale Status در SQL Integration فعلی

این قسمت با تصمیم مالک اصلاح شده است.

Sadr Scales 5.2.1 هنگام اتصال و قطع ارتباط، `dbo.SADR_Scale.Status` را به‌روزرسانی می‌کند.

پس برای Integration مبتنی بر SQL فعلی، Status عمومی قابل پشتیبانی این است:

```csharp
public enum SadrScaleConnectionStatus
{
    Unknown,
    Offline,
    Online
}

public sealed class SadrScaleStatus
{
    public int ScaleId { get; set; }
    public SadrScaleConnectionStatus ConnectionStatus { get; set; }
}
```

API پیشنهادی:

```csharp
var status = await sadr.Scales.GetStatusAsync(scaleId, ct);
```

**Source of truth برای SQL transport:** `dbo.SADR_Scale.Status`

### مرز این Contract

در SQL Contract فعلی فقط وضعیت coarse اتصال ارائه می‌شود:

```text
Online
Offline
Unknown / invalid value
```

Stateهای غنی‌تر داخلی مانند:

```text
Connecting
Current Activity
Progress
Last Error
Operation Busy
```

در 5.2.1 Contract عمومی SQL نیستند.

اگر در آینده Service/API عمومی ساخته شود، می‌توان بدون شکستن مدل اصلی Scale، یک مدل وضعیت پیشرفته‌تر اضافه کرد.

---

## 5.6 Scale Group Assignment

یک ترازو می‌تواند چند گروه داشته باشد.

```csharp
public sealed class SadrScaleGroupAssignment
{
    public int ScaleId { get; set; }
    public IReadOnlyList<string> GroupCodes { get; set; }
}
```

**Canonical source:** `dbo.SADR_ScaleItemClass`

`dbo.SADR_Scale.ItemClassCode` برای compatibility قدیمی باقی می‌ماند، ولی source اصلی Multi-Group نیست.

تغییر assignment باید از API کنترل‌شده انجام شود تا validation و reset stateهای لازم در یک نقطه انجام شوند.

---

## 5.7 Scale Item Assignment

```csharp
public sealed class SadrScaleItemAssignment
{
    public int ScaleId { get; set; }
    public int PluNo { get; set; }
    public string ItemCode { get; set; }
    public SadrHotKeyPosition HotKeyPosition { get; set; }
}
```

**Storage:** `dbo.SADR_ScaleItemMap`

قواعد:

- PLU تکراری در یک Scale مجاز نیست؛
- ItemCode تکراری در یک Scale مجاز نیست؛
- موقعیت کلید سریع تکراری مجاز نیست؛
- Page/Key باید با Layout همان Scale سازگار باشد.

---

## 5.8 HotKey Template

`dbo.SADR_KeyAssignment` الگوی کلید سریع در سطح Group است و با Scale Item Mapping یکی نیست.

```csharp
public sealed class SadrHotKeyTemplateEntry
{
    public string GroupCode { get; set; }
    public int PageNo { get; set; }
    public int KeyNo { get; set; }
    public int PluNo { get; set; }
}
```

مدل ذهنی:

```text
Item Group
   └── HotKey Template

Scale
   └── Scale Item Assignment
         └── optional Page/Key
```

---

# 6. Sales Feed و Structured Invoice دو مسیر جدا هستند

## 6.1 Sales Feed

Feed برای Sync پیوسته است:

```csharp
var batch = await sadr.Sales.ReadAfterAsync(lastProcessedId, pageSize, ct);
```

قواعد:

- `SADR_Logs` Read-only است؛
- مقصد Cursor خودش را نگه می‌دارد؛
- gap در ID مجاز است؛
- SDK رکوردهای `SADR_Logs` را Ack یا Update نمی‌کند.

این مسیر برای «هر فروش جدیدی که آمده» مناسب است.

---

## 6.2 Structured Invoice / Aggregate Barcode

این مسیر برای دریافت یک فاکتور مشخص است.

Barcode فاکتور در Core به شکل زیر ساخته می‌شود:

```text
25 + ScaleID(D3) + FID(D9)
```

Lookup عمومی:

```csharp
var result = await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);

var result2 = await sadr.Invoices.GetAsync(scaleId, fid, ct);
```

داده فاکتور از رابطه زیر ساخته می‌شود:

```text
SADR_Total
    +
SADR_Detail
    +
SADR_Logs when complementary data is required
```

مصرف‌کننده نباید JOIN داخلی را خودش طراحی کند.

---

# 7. Invoice Read Status و ACK — تصمیم اصلاح‌شده Phase 2

این بخش با تصمیم صریح مالک پروژه جزو Contract عمومی است.

## 7.1 معنی `LableStatus`

در `SADR_Total`:

```text
LableStatus = 0 / NULL  → فاکتور هنوز توسط نرم‌افزار مقصد Ack نشده
LableStatus = 1         → فاکتور قبلاً Ack / خوانده شده
```

این Contract در سطح **کل فاکتور** است.

`SADR_Detail.ItemStatus` در Contract پایه Invoice Ack تغییر داده نمی‌شود، مگر بعداً semantic مستقل و مشخصی برای Ack خط‌به‌خط تعریف شود.

---

## 7.2 Lookup هیچ‌وقت Auto-Ack نمی‌کند

این اشتباه است:

```text
Get invoice
→ automatically mark as read
```

رفتار درست:

```text
Lookup invoice
      ↓
Return full invoice + current read status
      ↓
Destination saves/commits invoice successfully
      ↓
Destination explicitly ACKs invoice
```

اگر نرم‌افزار مقصد قبل از Commit خراب شود، فاکتور هنوز خوانده‌شده علامت نمی‌خورد.

---

## 7.3 نتیجه Lookup باید همیشه Data را برگرداند

مدل پیشنهادی:

```csharp
public enum SadrInvoiceLookupStatus
{
    FoundUnread,
    AlreadyRead,
    NotFound
}

public sealed class SadrInvoiceLookupResult
{
    public SadrInvoiceLookupStatus Status { get; set; }
    public SadrInvoice Invoice { get; set; }
}
```

### فاکتور جدید

```text
Status = FoundUnread
Invoice = full invoice
```

### فاکتوری که قبلاً ACK شده

```text
Status = AlreadyRead
Invoice = full invoice
```

یعنی **AlreadyRead مانع برگشت Data نمی‌شود.**

هدف این است که POS بتواند بفهمد فاکتور قبلاً مصرف شده، ولی در صورت نیاز همچنان اطلاعات کامل آن را ببیند.

### فاکتور ناموجود

```text
Status = NotFound
Invoice = null
```

---

## 7.4 ACK صریح

API پیشنهادی:

```csharp
var ack = await sadr.Invoices.AcknowledgeAsync(totalBarcode, ct);
```

یا با identity منطقی:

```csharp
var ack = await sadr.Invoices.AcknowledgeAsync(scaleId, fid, ct);
```

رفتار:

```text
Find exact logical invoice
        ↓
If not found → NotFound
        ↓
If LableStatus != 1
        ↓
Set LableStatus = 1
        ↓
Return Acknowledged
```

اگر قبلاً `LableStatus = 1` بوده:

```text
Return AlreadyAcknowledged
No harmful second mutation
```

پس Ack باید **idempotent** باشد.

مدل نتیجه:

```csharp
public enum SadrInvoiceAckStatus
{
    Acknowledged,
    AlreadyAcknowledged,
    NotFound
}
```

---

## 7.5 ترتیب درست POS

```text
1. Scan / receive TotalBarcode
2. GetByBarcode
3. If FoundUnread or AlreadyRead → receive complete invoice
4. Save invoice in POS transaction
5. Commit POS transaction
6. Acknowledge invoice in Sadr
```

اگر مرحله 6 به دلیل قطع SQL شکست خورد، POS می‌تواند Ack را دوباره تلاش کند چون عملیات Ack idempotent است.

---

## 7.6 Cursor و Invoice ACK یکی نیستند

این دو را نباید مخلوط کنیم:

```text
Sales Feed
→ destination-owned cursor

Structured Invoice Lookup
→ SADR_Total.LableStatus acknowledgement
```

یک شرکت ممکن است یکی یا هر دو روش را استفاده کند.

---

# 8. Reports

API عمومی گزارش‌ها باید business-oriented باشد:

```csharp
sadr.Reports.Sales.DailyAsync(...)
sadr.Reports.Sales.ByScaleAsync(...)
sadr.Reports.Sales.ByItemAsync(...)
```

همچنین:

```csharp
sadr.Sales.QueryAsync(...)
sadr.Sales.GetSummaryAsync(...)
```

---

# 9. SQL Contract vNext — سطح داده قابل پشتیبانی

| Domain | SQL object | Read | Write | Contract |
|---|---|---:|---:|---|
| Connection/schema | effective schema | Yes | No | Validate |
| Store | `SADR_Store` | Yes | Upsert | Public |
| Item Group | `SADR_ItemClass` | Yes | Upsert | Public |
| Item | `SADR_Item` | Yes | Upsert + soft delete | Public |
| Price History | `SADR_PriceLog` | Yes | No | Public read |
| Scale definition | `SADR_Scale` | Yes | Controlled | Public model |
| Scale online/offline status | `SADR_Scale.Status` | Yes | No by consumer | Public read |
| Scale group assignment | `SADR_ScaleItemClass` | Yes | Controlled | Public domain API |
| Scale item mapping | `SADR_ScaleItemMap` | Yes | Controlled | Public domain API |
| HotKey template | `SADR_KeyAssignment` | Yes | Controlled | Public domain API |
| Sales Feed | `SADR_Logs` | Yes | No | Public read |
| Structured Invoice | `SADR_Total` + `SADR_Detail` | Yes | Ack only | Public |
| Invoice Ack | `SADR_Total.LableStatus` | Yes | `0/NULL → 1` | Public controlled write |
| Reports | reporting queries | Yes | No | Public read |

`Controlled` یعنی مصرف‌کننده از API/Query رسمی مستندشده استفاده می‌کند و لازم نیست side-effectهای داخلی را خودش حدس بزند.

---

# 10. SQL surfaces که عمومی نیستند

این موارد public write contract نیستند:

```text
SADR_ItemSyncState
SADR_Scale.LastSendItem
SADR_Scale.LastSendKey
schema migration internals
backup/restore/drop operations
recovery/repair internals
protocol-specific state
```

همچنین:

```text
SADR_Detail.ItemStatus
```

تا وقتی semantic عمومی مستقل برای آن تعریف نشده، بخشی از Invoice-level ACK نیست.

---

# 11. Device Commands و عملیات فعال ترازو

عملیاتی مثل:

```text
Send Items to device
Get Items from device
Send/Get HotKeys
Get Sales from device
Send/Get Specification
Set Date/Time
Text
Salesman
Print Format
Barcode Format
Paper Type
```

با خواندن یا نوشتن ساده یک Business Table معادل نیستند؛ باید توسط خود Sadr Scales اجرا شوند چون اتصال واقعی ترازو در اختیار آن است.

در 5.2.1 نرم‌افزار ثالث کانال عمومی مستقیم برای این Runtime ندارد.

پس Phase 2 فقط Contract مفهومی این عملیات را تعریف می‌کند و در فاز پیاده‌سازی باید یکی از کانال‌های مدیریت‌شده طراحی شود، برای نمونه:

```text
SQL Command Queue consumed by Sadr Scales
or
Local Service/API
or
Future REST Gateway
```

انتخاب کانال نباید معنی command را عوض کند.

### اصل امنیتی

API عمومی typed است:

```csharp
await sadr.Commands.SendItemsAsync(...);
await sadr.Commands.GetSalesAsync(...);
await sadr.Commands.SetDateTimeAsync(...);
```

Raw packet / command bytes عمومی نمی‌شوند.

---

# 12. File / Firmware / Label transfer

Core می‌تواند عملیات file/firmware/label داشته باشد، اما به دلیل ریسک و مالکیت فکری، این‌ها تا Security/API review مستقل در Public default surface قرار نمی‌گیرند.

این تصمیم قابلیت Core را حذف نمی‌کند.

---

# 13. Public API نمونه پس از تکمیل vNext

```csharp
var sadr = new SadrScalesClient(options);

await sadr.Connection.ValidateAsync(ct);

await sadr.Stores.UpsertAsync(store, ct);
var stores = await sadr.Stores.GetAllAsync(ct);

await sadr.ItemGroups.UpsertAsync(group, ct);
await sadr.Items.UpsertAsync(item, ct);
await sadr.Items.UpsertBatchAsync(items, ct);
await sadr.Items.DeleteAsync(pluNo, ct);

var scales = await sadr.Scales.GetAllAsync(ct);
var scaleStatus = await sadr.Scales.GetStatusAsync(scaleId, ct);

await sadr.ScaleAssignments.SetGroupsAsync(scaleId, groupCodes, ct);
await sadr.ScaleAssignments.SetItemsAsync(scaleId, assignments, ct);

var sales = await sadr.Sales.ReadAfterAsync(cursor, 200, ct);
var query = await sadr.Sales.QueryAsync(filter, ct);
var summary = await sadr.Sales.GetSummaryAsync(filter, ct);

SadrInvoiceLookupResult invoice =
    await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);

if (invoice.Status == SadrInvoiceLookupStatus.FoundUnread ||
    invoice.Status == SadrInvoiceLookupStatus.AlreadyRead)
{
    SaveAndCommitInPos(invoice.Invoice);
    await sadr.Invoices.AcknowledgeAsync(totalBarcode, ct);
}

var daily = await sadr.Reports.Sales.DailyAsync(range, ct);
```

Device commands زمانی عملیاتی می‌شوند که Managed Command Channel اضافه شود.

---

# 14. Retry و Idempotency

## Read operations

Bounded retry مجاز است.

## Business SQL writes

- validation قبل از اجرا؛
- transaction در عملیات چندمرحله‌ای؛
- blind replay بعد از transaction مبهم ممنوع.

## Invoice Ack

Ack استثنائاً برای retry مناسب است چون semantic آن idempotent است:

```text
Desired final state = LableStatus 1
```

اگر بار اول موفق شده باشد و جواب به مقصد نرسیده باشد، بار دوم `AlreadyAcknowledged` برمی‌گرداند.

## Device commands

Blind replay ممنوع است مگر خود command صریحاً idempotent تعریف شده باشد.

---

# 15. Compatibility با v1.0.0

`v1.0.0` frozen باقی می‌ماند.

این APIها معتبر می‌مانند:

```text
ItemGroups.Upsert
Items.Upsert
Items.UpsertBatch
Sales.ReadAfter
SQL Contract v1
```

vNext قابلیت‌های جدید را additive اضافه می‌کند.

اگر تغییر شکستن API یا semantic لازم شد، تصمیم versioning مستقل گرفته می‌شود.

---

# 16. تصمیم‌های Phase 2 پس از اصلاح

در صورت تأیید مالک، این موارد قفل می‌شوند:

1. Domain مستقل از Transport است؛
2. SQL روش اصلی Integration قابل استفاده با Sadr Scales 5.2.1 است؛
3. `SADR_Scale.Status` منبع وضعیت Online/Offline برای SQL Integration فعلی است؛
4. richer runtime status مربوط به Service/API آینده است؛
5. Store با `StoreCode` شناخته می‌شود؛
6. Multi-group با `SADR_ScaleItemClass` canonical است؛
7. Scale mapping و Group HotKey template دو مفهوم جدا هستند؛
8. Sales Feed با destination cursor کار می‌کند؛
9. Structured Invoice lookup با TotalBarcode و ScaleID+FID عمومی است؛
10. Lookup همیشه invoice را برمی‌گرداند، حتی اگر قبلاً خوانده شده باشد؛
11. `AlreadyRead` یک status نتیجه است، نه مانع دریافت داده؛
12. Ack صریح و جدا از Lookup است؛
13. Ack موفق `SADR_Total.LableStatus = 1` می‌کند؛
14. Ack idempotent است؛
15. `SADR_Detail.ItemStatus` در Contract پایه Invoice Ack دست‌کاری نمی‌شود؛
16. Device Commands typed هستند و raw protocol عمومی نیست؛
17. اجرای Device Commands نیازمند Managed Command Channel در Sadr Scales است؛
18. firmware/file/label تا review جدا در public default surface نیست؛
19. `v1.0.0` تغییر نمی‌کند.

---

# 17. Gate خروج از Phase 2

Phase 2 وقتی Complete است که این موارد تأیید شوند:

- [ ] Domain model
- [ ] SQL-first behavior برای 5.2.1
- [ ] Scale Status از `SADR_Scale.Status`
- [ ] Store/Scale relation
- [ ] Multi-group source of truth
- [ ] Mapping/HotKey split
- [ ] Structured Invoice lookup
- [ ] `LableStatus` ACK semantics
- [ ] AlreadyRead + return-full-data behavior
- [ ] Sales cursor مستقل از Invoice Ack
- [ ] Managed Device Command boundary
- [ ] SQL safe surface
- [ ] v1 compatibility
- [ ] firmware/file/label exclusion pending separate review

بعد از پذیرش، وارد Implementation Planning می‌شویم؛ نه توسعه پراکنده.
