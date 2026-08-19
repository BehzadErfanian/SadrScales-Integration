# طراحی Contract و Domain مستقل از Transport — Phase 2

**وضعیت:** پیش‌نویس نهایی برای بازبینی مالک/نگهدارنده  
**تاریخ:** 2026-08-19  
**Sadr Scales baseline بررسی‌شده:** `5.2.1`  
**Sadr Scales source commit:** `1048749f52faba35e69464b64983e772c1c857e3`  
**Integration baseline:** `7690e2a622201cc9b5f0f112c6d6099d44c4233e`  
**Phase 1 audit:** `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`

---

## 1. هدف

هدف این فاز تعریف Contract عمومی **کامل، ساده، قابل یادگیری و قابل توسعه** برای شرکت‌های ثالث است؛ به شکلی که مصرف‌کننده مجبور نباشد ساختار داخلی Sadr Scales یا پروتکل ترازوها را بداند.

این سند طراحی است و هنوز پیاده‌سازی جدید ایجاد نمی‌کند.

`v1.0.0` و `SQL Contract v1` منتشرشده تغییر نمی‌کنند و Tag آن‌ها ثابت می‌ماند. توسعه بعدی به‌صورت additive انجام می‌شود.

---

## 2. اصل اصلی: Domain از روش اتصال جدا است

مفهوم‌های عمومی Integration عبارت‌اند از:

```text
Connection
Store / Branch
Scale
Scale Status
Item Group
Item / PLU
Scale Group Assignment
Scale Item Assignment
HotKey Template
Sales Feed
Structured Invoice
Invoice Acknowledgement
Reports
Automatic Resend Request
Device Commands
```

روش دسترسی می‌تواند در طول زمان تغییر کند:

```text
Integration Domain
        │
        ├── SQL                         ← روش فعلی و اصلی برای 5.2.1
        ├── Future Sadr Integration Service
        ├── Future REST/API
        ├── Future XML
        └── Future Webhook / other
```

تغییر Transport نباید معنی `Scale`, `Item`, `Invoice` یا `SendItems` را عوض کند.

---

## 3. وضعیت واقعی Integration در Sadr Scales 5.2.1

در 5.2.1 نرم‌افزار ثالث API مستقیمی به Runtime ندارد و مسیر اصلی Integration از SQL Server است:

```text
Third-party software
        ↓
       SQL
        ↓
Sadr Scales Database
        ↑
Sadr Scales Runtime
        ↕
      Scale
```

Runtime یعنی بخش در حال اجرای Sadr Scales که اتصال ترازو، Registry، Heartbeat، Queue، Reconnect، Send/Get و Capabilityهای مدل را مدیریت می‌کند.

تا جایی که رفتار مورد نیاز با SQL به‌صورت **صحیح و قابل پشتیبانی** انجام می‌شود، Contract عمومی SQL آن را ارائه می‌کند. قابلیت‌هایی که برای اجرای درست به Runtime orchestration نیاز دارند، در آینده از Service ارائه می‌شوند؛ نه با ساختن Command Queue مصنوعی داخل SQL.

---

## 4. مرز سه‌گانه قابلیت‌ها

### 4.1 SQL Data / Controlled SQL Operations

این قابلیت‌ها در SQL Integration عمومی قرار می‌گیرند:

```text
Connection/schema validation
Store read/upsert
Item Group read/upsert
Item read/upsert/soft-delete
Price history read
Static Scale read
Scale Online/Offline status read
Scale group assignment
Scale item assignment
HotKey template
Sales feed/query/summary
Structured Invoice lookup
Invoice ACK
Reports
Request Item Resend
Request HotKey Resend where current model/runtime supports auto HotKey send
```

### 4.2 Future Sadr Integration Service

این قابلیت‌ها نباید با SQL خام تقلید شوند و در آینده از Service تایپ‌شده ارائه می‌شوند:

```text
Safe Add / Update / Delete Scale lifecycle
Immediate Send Items
Immediate Get Items
Immediate Send HotKeys
Immediate Get HotKeys
Get Sales directly from device
Send/Get Specification
Set Date/Time
Salesmen operations
Text operations
Print Format
Barcode Format
Paper Type
Richer Runtime status / progress / current activity
```

Service باید منطق Validation، License، Registry، Connection state، Busy state، Model capability و Runtime sequencing را از خود Sadr Scales استفاده کند.

### 4.3 Internal / Do Not Expose

```text
Raw device packets
Protocol opcodes
PCAP/captures
Protocol implementation
Internal queue structures
Registry internals
ItemSyncState internals
Migration/repair internals
Private keys/secrets
Firmware internals
Arbitrary SQL execution
Arbitrary Runtime command execution
```

---

# 5. Domain عمومی SQL

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

- `Code` هویت پایدار Store است؛
- `Code = "0"` شعبه پیش‌فرض است؛
- `Name` فقط نام نمایشی است؛
- Upsert عمومی است؛
- ارتباط Scale با Store با `StoreCode` تعریف می‌شود؛
- Delete عمومی در Contract پایه ارائه نمی‌شود تا FK و رفتار Scale مبهم نشود.

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

مدیریت Group و اختصاص Group به Scale دو مفهوم جدا هستند.

---

## 5.3 Item / PLU

هویت اصلی کالا `PluNo` است.

قواعد:

- `PluNo = 0` نامعتبر است؛
- Insert/Update عمومی است؛
- حذف عمومی soft-delete است؛
- Batch bounded و atomic است؛
- stateهای داخلی ارسال/Sync جزو مدل عمومی Item نیستند؛
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

Static data از `dbo.SADR_Scale` خوانده می‌شود.

### مرز lifecycle

Runtime 5.2.1 هر ۱۰ ثانیه DB را برای ترازوهای جدید بررسی می‌کند، Registry را Reload می‌کند و برای Scale جدید Connection Check را شروع می‌کند. بنابراین Scale جدیدی که در DB ظاهر شود قابل شناسایی است.

با این حال مسیر داخلی Add/Update/Delete فقط SQL نیست و Validation، Duplicate check، License authorization، Registry reload، Connection lifecycle و cleanup را هم درگیر می‌کند.

پس Public SDK نباید Add/Update/Delete Scale را با چند `INSERT/UPDATE/DELETE` خام پیاده کند. این عملیات در آینده از Sadr Integration Service ارائه می‌شوند.

---

## 5.5 Scale Status در SQL Integration

Sadr Scales هنگام Online/Offline شدن Scale مقدار `dbo.SADR_Scale.Status` را به‌روزرسانی می‌کند.

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

API هدف:

```csharp
var status = await sadr.Scales.GetStatusAsync(scaleId, ct);
```

**Source of truth برای SQL transport:** `dbo.SADR_Scale.Status`

اطلاعات غنی‌تر مثل Progress، CurrentActivity و LastError در SQL Contract فعلی نیستند و بعداً از Service قابل ارائه‌اند.

---

## 5.6 Scale Group Assignment

یک ترازو می‌تواند چند Group داشته باشد.

```csharp
public sealed class SadrScaleGroupAssignment
{
    public int ScaleId { get; set; }
    public IReadOnlyList<string> GroupCodes { get; set; }
}
```

**Canonical source:** `dbo.SADR_ScaleItemClass`

`dbo.SADR_Scale.ItemClassCode` فقط compatibility قدیمی است و source اصلی Multi-Group نیست.

Write عمومی باید semantic کنترل‌شده زیر را داشته باشد:

```text
Validate ScaleId
Validate at least one distinct Group
Validate referenced Groups
BEGIN TRANSACTION
Delete existing Scale groups
Insert complete replacement set
Reset SADR_Scale.LastSendItem = 0
COMMIT
```

Reset کردن `LastSendItem` در اینجا بخشی از عملیات assignment است تا AutoSend بعدی تغییر Group را از دست ندهد.

---

## 5.7 Scale Item Assignment

```csharp
public sealed class SadrScaleItemAssignment
{
    public int ScaleId { get; set; }
    public int PluNo { get; set; }
    public int ItemCode { get; set; }
    public SadrHotKeyPosition HotKeyPosition { get; set; }
}
```

**Source:** `dbo.SADR_ScaleItemMap`

قواعد:

- PLU تکراری برای یک Scale رد می‌شود؛
- ItemCode تکراری برای یک Scale رد می‌شود؛
- Page/Key تکراری رد می‌شود؛
- Layout باید با `HotKeyCountPerPage` و `HotKeyPageCount` همان Scale سازگار باشد؛
- جایگزینی Mapping باید atomic باشد؛
- پس از تغییر Mapping، `LastSendItem` باید reset شود.

API هدف:

```csharp
await sadr.ScaleAssignments.SetItemsAsync(scaleId, assignments, ct);
await sadr.ScaleAssignments.CopyItemsAsync(sourceScaleId, targetScaleId, ct);
```

---

## 5.8 HotKey Template

`dbo.SADR_KeyAssignment` یک Template در سطح Group است و با Mapping اختصاصی Scale یکی نیست.

```csharp
public sealed class SadrHotKeyTemplateEntry
{
    public string GroupCode { get; set; }
    public int PageNo { get; set; }
    public int KeyNo { get; set; }
    public int PluNo { get; set; }
}
```

```text
Item Group
   └── HotKey Template         → SADR_KeyAssignment

Scale
   └── Scale Item Assignment   → SADR_ScaleItemMap
         └── optional Page/Key
```

Write عمومی از SQL مجاز است، اما باید validation و semantic لازم را حفظ کند.

برای Replace/Delete که حذف Row ممکن است از طریق timestamp عادی قابل تشخیص نباشد، `LastSendKey` ترازوهای تحت تأثیر reset می‌شود تا AutoSend بعدی تغییر را دوباره پردازش کند.

---

# 6. Request Resend در SQL — تصمیم قطعی

Sadr Scales 5.2.1 از `LastSendItem` و `LastSendKey` برای تشخیص وضعیت آخرین AutoSend استفاده می‌کند.

بنابراین Integration عمومی دو عملیات روشن ارائه می‌کند:

```csharp
await sadr.Items.RequestResendAsync(scaleId, ct);
await sadr.HotKeys.RequestResendAsync(scaleId, ct);
```

### 6.1 Request Item Resend

معنی عمومی:

```text
RequestItemResend(scaleId)
→ SADR_Scale.LastSendItem = 0
```

مسیر Raw SQL برای مصرف‌کننده غیر C# نیز مستند می‌شود:

```sql
UPDATE dbo.SADR_Scale
SET LastSendItem = 0
WHERE ScaleID = @ScaleId;
```

### 6.2 Request HotKey Resend

معنی عمومی:

```text
RequestHotKeyResend(scaleId)
→ SADR_Scale.LastSendKey = 0
```

مسیر Raw SQL:

```sql
UPDATE dbo.SADR_Scale
SET LastSendKey = 0
WHERE ScaleID = @ScaleId;
```

### 6.3 این عملیات «ارسال فوری» نیست

Reset کردن این دو مقدار فقط AutoSend را Pending می‌کند.

در 5.2.1 AutoSend به‌صورت دوره‌ای اجرا می‌شود و Scale فقط وقتی Target است که:

```text
Scale.Used == true
Scale.IsConnected == true
Scale.AutoSendItems == true
```

چرخه نگهداری Runtime به‌طور پیش‌فرض AutoSend را هر ۲۰ ثانیه اجرا می‌کند.

پس Contract باید صریح بگوید:

```text
RequestResend
    ↓
Reset send watermark
    ↓
Wait for next eligible AutoSend cycle
    ↓
Runtime sends data if model/mode supports it
```

اگر Scale در لحظه Reset آفلاین باشد، درخواست در DB باقی می‌ماند و بعد از Online شدن و رسیدن چرخه مناسب قابل پردازش است.

### 6.4 محدودیت مدل برای HotKey

`LastSendKey = 0` یک trigger عمومی SQL برای AutoSend HotKey است، اما ارسال واقعی باید با Capability مدل هماهنگ باشد.

در Runtime 5.2.1:

- LSG/LSG_24D/TSG در AutoSend، HotKey را بعد از مرحله Item بررسی می‌کنند؛
- LS6 مسیر Auto HotKey دارد؛
- PLUS در AutoScaleDataSendCoordinator مسیر مستقل `hotKeySender.SendAuto` ندارد.

بنابراین SDK و Sample نباید برای مدلی که Auto HotKey مستقل ندارد موفقیت جعلی گزارش کنند. Capability/Documentation باید این تفاوت را روشن کند.

---

# 7. Sales Feed و Structured Invoice دو مسیر مستقل‌اند

## 7.1 Sales Feed

```csharp
var batch = await sadr.Sales.ReadAfterAsync(lastProcessedId, pageSize, ct);
```

قواعد:

- `SADR_Logs` Read-only است؛
- مقصد Cursor خودش را durable نگه می‌دارد؛
- gap در ID مجاز است؛
- SDK رکوردهای `SADR_Logs` را Ack/Update نمی‌کند.

---

## 7.2 Structured Invoice / TotalBarcode

TotalBarcode در Core:

```text
25 + ScaleID(D3) + FID(D9)
```

Lookup:

```csharp
var byBarcode = await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);
var byIdentity = await sadr.Invoices.GetAsync(scaleId, fid, ct);
```

داده فاکتور از این ساختار ساخته می‌شود:

```text
SADR_Total
    +
SADR_Detail
    +
SADR_Logs when complementary data is required
```

JOIN داخلی نباید به مصرف‌کننده تحمیل شود.

---

# 8. Invoice Read Status و ACK

## 8.1 معنی `LableStatus`

```text
NULL / 0 → مقصد هنوز فاکتور را ACK نکرده
1        → مقصد قبلاً فاکتور را ACK کرده
```

این semantic در سطح کل Invoice است.

`SADR_Detail.ItemStatus` فعلاً جزو Invoice-level ACK عمومی نیست.

### 8.2 Lookup هیچ‌وقت Auto-Ack نمی‌کند

```text
Lookup invoice
      ↓
Return full invoice + read status
      ↓
Destination saves invoice
      ↓
Destination transaction COMMIT succeeds
      ↓
Destination explicitly ACKs invoice
```

اگر Read یا Save ناقص بماند، فاکتور نباید اشتباهی خوانده‌شده علامت بخورد.

### 8.3 AlreadyRead هشدار است، نه Block

```csharp
public enum SadrInvoiceLookupStatus
{
    FoundUnread,
    AlreadyRead,
    NotFound
}
```

در هر دو حالت زیر Invoice کامل برمی‌گردد:

```text
FoundUnread → full invoice
AlreadyRead → full invoice + warning
```

`AlreadyRead` فقط اطلاع می‌دهد که این فاکتور قبلاً ACK شده است. نرم‌افزار مقصد می‌تواند در صورت نیاز بازیابی یا ثبت مجدد را به کاربر اجازه دهد.

### 8.4 ACK صریح و Idempotent

```csharp
await sadr.Invoices.AcknowledgeAsync(totalBarcode, ct);
await sadr.Invoices.AcknowledgeAsync(scaleId, fid, ct);
```

```csharp
public enum SadrInvoiceAckStatus
{
    Acknowledged,
    AlreadyAcknowledged,
    NotFound
}
```

رفتار:

```text
Not found        → NotFound
LableStatus != 1 → set 1 → Acknowledged
LableStatus == 1 → no harmful mutation → AlreadyAcknowledged
```

---

# 9. Future Sadr Integration Service

SQL Contract را فقط تا جایی گسترش می‌دهیم که رفتار واقعی Sadr Scales را درست و قابل پیش‌بینی ارائه کند.

قابلیت‌های Runtime که SQL برای آن‌ها کافی نیست، در آینده از یک Service تایپ‌شده ارائه می‌شوند.

مدل مفهومی:

```text
Third-party app / SDK
        ↓
Sadr Integration Service
        ↓
Sadr Scales Runtime
        ↓
Validation / License / Registry / Connection / Capability
        ↓
Scale
```

جزئیات Transport سرویس در فاز پیاده‌سازی جداگانه انتخاب می‌شود؛ ممکن است Local API و بعداً REST باشد. Domain عمومی نباید به این انتخاب وابسته شود.

### 9.1 قابلیت‌های Service

حداقل جهت آینده:

```text
Scale lifecycle
- Add Scale
- Update Scale
- Delete Scale
- Enable/Disable where orchestration is required

Items
- SendItems immediately
- GetItems
- DeleteAllItems on device

HotKeys
- SendHotKeys immediately
- GetHotKeys

Sales
- GetSales directly from device

Settings
- Send/Get Specification
- SetDateTime
- Salesmen operations where supported
- Text operations where supported
- PrintFormat operations where supported
- BarcodeFormat operations where supported
- PaperType operations where supported

Runtime visibility
- richer status
- current activity
- progress
- last error/result
```

Raw protocol passthrough عمومی ممنوع است.

---

# 10. مرز Capabilityها

| Capability | SQL 5.2.1 / vNext SQL | Future Service |
|---|---:|---:|
| Connection/schema validate | Yes | Optional |
| Store | Read/Upsert | Optional |
| Item Group | Read/Upsert | Optional |
| Item / PLU | Read/Upsert/Soft delete | Optional |
| Price History | Read | Optional |
| Static Scale | Read | Add/Update/Delete |
| Coarse Scale Status | Online/Offline read | richer runtime state |
| Scale Groups | Controlled replace + `LastSendItem=0` | Optional |
| Scale Item Mapping | Controlled replace/copy + reset | Optional |
| HotKey Template | Controlled write + reset when needed | Optional |
| Request Item Resend | `LastSendItem=0` | immediate send later |
| Request HotKey Resend | `LastSendKey=0` where supported | immediate send later |
| Sales Feed | Read | Optional |
| Structured Invoice | Read + ACK | Optional |
| Reports | Read | Optional |
| Direct Device Operations | No | **Yes** |
| Firmware/File/Label transfer | No public default | Security/API review first |

---

# 11. Public API هدف

```csharp
var sadr = new SadrScalesClient(options);

await sadr.Connection.ValidateAsync(ct);

await sadr.Stores.UpsertAsync(store, ct);
await sadr.ItemGroups.UpsertAsync(group, ct);
await sadr.Items.UpsertAsync(item, ct);
await sadr.Items.UpsertBatchAsync(items, ct);
await sadr.Items.DeleteAsync(pluNo, ct);

var scales = await sadr.Scales.GetAllAsync(ct);
var status = await sadr.Scales.GetStatusAsync(scaleId, ct);

await sadr.ScaleAssignments.SetGroupsAsync(scaleId, groupCodes, ct);
await sadr.ScaleAssignments.SetItemsAsync(scaleId, assignments, ct);
await sadr.ScaleAssignments.CopyItemsAsync(sourceScaleId, targetScaleId, ct);

await sadr.HotKeys.SetTemplateAsync(groupCode, entries, ct);

await sadr.Items.RequestResendAsync(scaleId, ct);
await sadr.HotKeys.RequestResendAsync(scaleId, ct);

var feed = await sadr.Sales.ReadAfterAsync(cursor, 200, ct);
var query = await sadr.Sales.QueryAsync(filter, ct);
var summary = await sadr.Sales.GetSummaryAsync(filter, ct);

var invoice = await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);
var ack = await sadr.Invoices.AcknowledgeAsync(totalBarcode, ct);

var daily = await sadr.Reports.Sales.DailyAsync(range, ct);
var byScale = await sadr.Reports.Sales.ByScaleAsync(range, ct);
var byItem = await sadr.Reports.Sales.ByItemAsync(range, ct);
```

وقتی Service اضافه شود، APIهای Domain جدید به همین Client افزوده می‌شوند؛ برای مثال:

```csharp
await sadr.Scales.AddAsync(createRequest, ct);
await sadr.Commands.SendItemsAsync(scaleId, ct);
await sadr.Commands.GetSalesAsync(scaleId, ct);
await sadr.Commands.SetDateTimeAsync(scaleId, value, ct);
```

مصرف‌کننده نباید پروتکل یا Transport داخلی Service را بداند.

---

# 12. Capability discovery

SDK باید بتواند تفاوت قابلیت‌های SQL فعلی و Service آینده را گزارش کند.

```csharp
var capabilities = await sadr.Connection.GetCapabilitiesAsync(ct);
```

5.2.1 فعلی می‌تواند بعد از تکمیل vNext SQL SDK این موارد را ارائه کند:

```text
Data SQL
Coarse Scale Online/Offline status
Scale Groups / Mapping / HotKey data management
Item/HotKey AutoResend request by send-state reset
Sales Feed
Structured Invoice lookup
Invoice ACK
Reports
```

تا وقتی Service اضافه نشده، قابلیت‌های مستقیم Runtime باید `CapabilityNotAvailable` بدهند؛ نه اینکه SDK آن‌ها را با SQL خام تقلید کند.

---

# 13. Retry و Transaction

### Read SQL

Bounded retry مجاز است.

### Controlled SQL Write

```text
Validate before execution
Bounded transaction
No blind replay after transaction begins
Atomic commit/rollback
```

### Invoice ACK

Idempotent است و retry کنترل‌شده مجاز است.

### Request Resend

Reset به صفر یک write کوچک و deterministic است. تکرار آن قبل از تکمیل AutoSend مضر نیست و فقط همان state pending را حفظ می‌کند.

### Future Service Command

هر command باید policy مشخص داشته باشد:

```text
SafeRetry
IdempotentControlledRetry
NonReplayable
```

Timeout هیچ‌وقت به معنی retry کور command غیرقابل replay نیست.

---

# 14. Security boundary

Public Integration نباید موارد زیر را ارائه کند:

```text
raw device packet
protocol command bytes
capture/pcap
vendor/private protocol documentation
private signing/licensing keys
firmware internals
customer production data
arbitrary SQL execution API
arbitrary runtime reflection/command execution
```

Public Scale Emulator آینده نیز رفتار ترازو را ارائه می‌کند، نه جزئیات پروتکل.

---

# 15. نتیجه Phase 2 تا این نقطه

تصمیم‌های تثبیت‌شده:

1. Domain از Transport مستقل است؛
2. SQL روش اصلی Integration فعلی 5.2.1 است؛
3. `SADR_Scale.Status` منبع عمومی Online/Offline در SQL است؛
4. Store با `StoreCode` شناخته می‌شود؛
5. Multi-Group با `SADR_ScaleItemClass` canonical است؛
6. Scale Mapping و Group HotKey Template دو مفهوم جدا هستند؛
7. Group/Mapping/HotKey writeها SQL-controlled هستند و reset state لازم جزو semantic آن‌هاست؛
8. `LastSendItem = 0` یک Request Item Resend رسمی SQL است؛
9. `LastSendKey = 0` یک Request HotKey Resend رسمی SQL برای مدل/مسیرهای پشتیبانی‌شده است؛
10. Request Resend ارسال فوری نیست و در AutoSend بعدی اجرا می‌شود؛
11. Structured Invoice با TotalBarcode و `ScaleID + FID` lookup می‌شود؛
12. Lookup auto-ACK نمی‌کند؛
13. `AlreadyRead` Data را Block نمی‌کند؛
14. Invoice ACK صریح، بعد از Commit مقصد و idempotent است؛
15. قابلیت‌های Runtime که SQL برای آن‌ها کافی نیست در آینده از Sadr Integration Service ارائه می‌شوند؛
16. Managed SQL Command Queue از جهت معماری حذف شده است؛
17. Raw protocol خصوصی باقی می‌ماند؛
18. `v1.0.0` frozen و vNext additive است.

---

# 16. Gate خروج از Phase 2

موارد تأییدشده:

- [x] SQL به‌عنوان روش فعلی 5.2.1؛
- [x] SQL Online/Offline status؛
- [x] Structured Invoice lookup؛
- [x] Explicit Invoice ACK؛
- [x] AlreadyRead returns full data؛
- [x] Item resend via `LastSendItem = 0`؛
- [x] HotKey resend via `LastSendKey = 0` با رعایت Capability مدل؛
- [x] Future Service به‌جای SQL Command Queue برای قابلیت‌های Runtime-only.

مواردی که قبل از Complete شدن Phase 2 باید یک مرور نهایی شوند:

- [ ] Store/Scale relation؛
- [ ] Multi-group semantics؛
- [ ] Mapping write semantics؛
- [ ] HotKey write semantics؛
- [ ] Scale lifecycle service boundary؛
- [ ] Device command families؛
- [ ] firmware/file/label exclusion pending separate review.

بعد از پذیرش نهایی، تصمیم‌ها در `docs/DECISIONS.md` ثبت می‌شوند، Phase 2 Complete می‌شود و implementation planning در Sliceهای کوچک و قابل تست شروع می‌شود.
