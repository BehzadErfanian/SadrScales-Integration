# طراحی Contract و Domain مستقل از Transport — Phase 2

**وضعیت:** پیش‌نویس نهایی برای بازبینی مالک/نگهدارنده  
**تاریخ:** 2026-08-19  
**Sadr Scales baseline بررسی‌شده:** `5.2.1`  
**Sadr Scales source commit:** `1048749f52faba35e69464b64983e772c1c857e3`  
**Integration baseline:** `7690e2a622201cc9b5f0f112c6d6099d44c4233e`  
**Phase 1 audit:** `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`

---

## 1. هدف

هدف این فاز تعریف یک Contract عمومی **کامل، ساده، قابل یادگیری و قابل توسعه** برای شرکت‌های ثالث است؛ به شکلی که مصرف‌کننده مجبور نباشد ساختار داخلی Sadr Scales یا پروتکل ترازوها را بداند.

این سند طراحی است و هنوز پیاده‌سازی جدید ایجاد نمی‌کند.

`v1.0.0` و `SQL Contract v1` منتشرشده تغییر نمی‌کنند و Tag آن‌ها ثابت می‌ماند.

---

## 2. اصل اصلی: Domain از Transport جدا است

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
Device Commands
```

روش دسترسی می‌تواند در طول زمان تغییر کند:

```text
Integration Domain
        │
        ├── SQL                    ← روش فعلی و اصلی
        ├── Managed SQL Command    ← مسیر پیشنهادی برای عملیات Runtime
        ├── Future Local API
        ├── Future REST/API
        ├── Future XML
        └── Future Webhook / other
```

تغییر Transport نباید معنی `Scale`, `Item`, `Invoice` یا `SendItems` را عوض کند.

---

## 3. Runtime دقیقاً چیست؟

در این پروژه Runtime یعنی خود پردازش در حال اجرای Sadr Scales که مسئول این موارد است:

```text
Scale connections
Registry
Heartbeat
Reconnect
Queues
In-flight operations
Send/Get operations
Model capability checks
License-gated scale lifecycle
```

### وضعیت در 5.2.1

Sadr Scales 5.2.1 API عمومی Runtime برای شرکت ثالث ندارد.

پس نرم‌افزار ثالث فعلاً از SQL استفاده می‌کند:

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

این محدودیت نباید باعث شود عملیات Runtime را با UPDATE/DELETE ناقص روی جدول‌های داخلی تقلید کنیم.

---

## 4. سه نوع عملیات عمومی

### 4.1 Data SQL

عملیاتی که مستقیماً روی Contract داده قابل انجام‌اند:

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
```

### 4.2 Managed SQL Command

عملیاتی که باید توسط خود Sadr Scales اجرا شوند:

```text
Add Scale
Update Scale runtime-sensitive fields
Delete Scale
Enable/Disable Scale when runtime lifecycle is affected
Send Items
Get Items
Send HotKeys
Get HotKeys
Get Sales from device
Send/Get Specification
Set Date/Time
Salesmen operations
Text operations
Print Format
Barcode Format
Paper Type
```

برای این عملیات، جهت معماری vNext یک **SQL Command Channel رسمی** تعریف می‌شود تا شرکت ثالث همچنان فقط به SQL نیاز داشته باشد ولی منطق Runtime دور زده نشود.

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

- `Code` هویت پایدار Store است؛
- `Code = "0"` شعبه پیش‌فرض است؛
- `Name` هویت نیست و فقط نام نمایشی است؛
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
- حذف عمومی به شکل soft-delete انجام می‌شود؛
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

### نکته مهم 5.2.1

Runtime 5.2.1 هر ۱۰ ثانیه DB را برای ترازوهای جدید بررسی می‌کند، Registry را Reload می‌کند و برای Scale جدید Connection Check را شروع می‌کند.

پس Scale جدیدی که در DB ظاهر شود قابل شناسایی است؛ اما **این به معنی مجاز بودن INSERT خام عمومی نیست**.

مسیر داخلی Add Scale علاوه بر DB شامل Validation، Duplicate check، License authorization، Registry reload و Connection check است. بنابراین Public SDK نباید برای Add/Update/Delete صرفاً جدول `SADR_Scale` را خام تغییر دهد.

---

## 5.5 Scale Status در SQL Integration

Sadr Scales هنگام Online/Offline شدن Scale مقدار `dbo.SADR_Scale.Status` را به‌روزرسانی می‌کند.

پس SQL Contract عمومی وضعیت پایه:

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

API:

```csharp
var status = await sadr.Scales.GetStatusAsync(scaleId, ct);
```

**Source of truth برای SQL transport:** `dbo.SADR_Scale.Status`

اطلاعات غنی‌تر مثل Progress، CurrentActivity و LastError در 5.2.1 SQL Contract نیستند و بعداً می‌توانند با Service/API افزوده شوند.

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

Write عمومی از SQL مجاز است، ولی فقط از API کنترل‌شده با semantic زیر:

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

مصرف‌کننده نباید `LastSendItem` را خودش مدیریت کند؛ SDK/SQL adapter این کار را انجام می‌دهد.

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
- پس از تغییر Mapping، `LastSendItem` باید توسط adapter reset شود.

API:

```csharp
await sadr.ScaleAssignments.SetItemsAsync(scaleId, assignments, ct);
await sadr.ScaleAssignments.CopyItemsAsync(sourceScaleId, targetScaleId, ct);
```

این عملیات **Data SQL کنترل‌شده** است، نه Device Command.

---

## 5.8 HotKey Template

`dbo.SADR_KeyAssignment` یک Template در سطح Group است.

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
   └── HotKey Template         → SADR_KeyAssignment

Scale
   └── Scale Item Assignment   → SADR_ScaleItemMap
         └── optional Page/Key
```

Write عمومی از SQL مجاز است، اما باید از API کنترل‌شده انجام شود.

برای Upsert معمولی، RowVersion خود جدول تغییر را قابل مشاهده می‌کند.

برای Replace/Delete که ممکن است Row حذف شود، adapter باید `LastSendKey` ترازوهای تحت تأثیر را reset کند تا حذف نیز در ارسال بعدی گم نشود.

`HotKeys.SetTemplateAsync` تعریف داده است؛ `Commands.SendHotKeysAsync` انتقال همین داده به Device است و یک مفهوم جدا محسوب می‌شود.

---

# 6. Sales Feed و Structured Invoice دو مسیر مستقل‌اند

## 6.1 Sales Feed

```csharp
var batch = await sadr.Sales.ReadAfterAsync(lastProcessedId, pageSize, ct);
```

قواعد:

- `SADR_Logs` Read-only است؛
- مقصد Cursor خودش را durable نگه می‌دارد؛
- gap در ID مجاز است؛
- SDK رکوردهای `SADR_Logs` را Ack/Update نمی‌کند.

---

## 6.2 Structured Invoice / TotalBarcode

TotalBarcode در Core:

```text
25 + ScaleID(D3) + FID(D9)
```

Lookup:

```csharp
var byBarcode = await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);
var byIdentity = await sadr.Invoices.GetAsync(scaleId, fid, ct);
```

Data از این ساختار ساخته می‌شود:

```text
SADR_Total
    +
SADR_Detail
    +
SADR_Logs when complementary data is required
```

JOIN داخلی نباید به مصرف‌کننده تحمیل شود.

---

# 7. Invoice Read Status و ACK

## 7.1 معنی `LableStatus`

```text
NULL / 0 → مقصد هنوز فاکتور را ACK نکرده
1        → مقصد قبلاً فاکتور را ACK کرده
```

این semantic در سطح کل Invoice است.

`SADR_Detail.ItemStatus` فعلاً جزو Invoice-level ACK عمومی نیست.

---

## 7.2 Lookup هیچ‌وقت Auto-Ack نمی‌کند

رفتار درست:

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

---

## 7.3 AlreadyRead هشدار است، نه Block

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

`AlreadyRead` فقط اطلاع می‌دهد که این بارکد قبلاً ACK شده است.

نرم‌افزار مقصد می‌تواند به کاربر هشدار بدهد و اگر رکورد قبلی اشتباهی حذف شده یا بازیابی لازم است، اجازه ثبت دوباره بدهد.

SDK نباید این تصمیم تجاری را Block کند.

---

## 7.4 ACK صریح و Idempotent

```csharp
await sadr.Invoices.AcknowledgeAsync(totalBarcode, ct);
await sadr.Invoices.AcknowledgeAsync(scaleId, fid, ct);
```

Result:

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

ACK باید transaction-safe و idempotent باشد.

---

# 8. Managed SQL Command Channel

## 8.1 چرا لازم است؟

شرکت ثالث امروز SQL دارد، اما بعضی عملیات فقط با تغییر DB کامل نمی‌شوند.

مثلاً مسیر واقعی Add Scale در Core شامل این موارد است:

```text
Input validation
Duplicate check
License authorization
DB persistence
Registry reload
Connection check
```

Update/Delete نیز Connection lifecycle و cleanup دارند.

Device Commandها هم به Queue، connection state، model capability و protocol runtime وابسته‌اند.

پس به جای Direct SQL mutation، Sadr Scales باید request را خودش اجرا کند.

---

## 8.2 معماری پیشنهادی

```text
Third-party app / SDK
        ↓
Typed SQL procedure
        ↓
Integration Command Queue
        ↓
Sadr Scales Runtime
        ↓
Validate / Execute
        ↓
Command Result
        ↓
Third-party app / SDK
```

این مدل باعث می‌شود شرکت ثالث همچنان فقط SQL Server را بشناسد.

---

## 8.3 Queue table جزئیات داخلی می‌ماند

برنامه‌نویس بیرونی نباید مستقیماً یک `CommandType` دلخواه و Payload خام داخل table بنویسد.

Public SQL Contract باید **Typed stored procedure** داشته باشد.

نمونه جهت نام‌گذاری، نه Schema نهایی:

```text
dbo.SADR_Integration_AddScale
dbo.SADR_Integration_UpdateScale
dbo.SADR_Integration_DeleteScale

dbo.SADR_Integration_SendItems
dbo.SADR_Integration_GetItems
dbo.SADR_Integration_SendHotKeys
dbo.SADR_Integration_GetHotKeys
dbo.SADR_Integration_GetSales

dbo.SADR_Integration_SetDateTime
...

dbo.SADR_Integration_GetCommandResult
```

هر request یک `CommandId` برمی‌گرداند.

SDK جزئیات polling/result table را مخفی می‌کند.

---

## 8.4 Command state عمومی

```csharp
public enum SadrCommandState
{
    Pending,
    Running,
    Succeeded,
    Failed,
    Cancelled
}
```

Result عمومی حداقل:

```csharp
public sealed class SadrCommandResult
{
    public Guid CommandId { get; set; }
    public SadrCommandState State { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public int? ScaleId { get; set; }
}
```

Protocol code/packet نباید در Contract اصلی result باشد.

---

## 8.5 Idempotency

Commandهایی که ممکن است دوباره submit شوند باید `RequestId` یا Idempotency Key داشته باشند تا retry شبکه باعث اجرای دوباره خطرناک نشود.

قاعده:

```text
Same RequestId
→ same logical command
→ no duplicate destructive execution
```

برای command غیرقابل replay، timeout نباید باعث retry کور شود.

---

# 9. مرز Writeهای SQL

| Capability | Read SQL | Direct/Controlled SQL Write | Managed SQL Command |
|---|---:|---:|---:|
| Connection/schema | Yes | No | No |
| Store | Yes | Upsert | No |
| Item Group | Yes | Upsert | No |
| Item / PLU | Yes | Upsert + soft delete | No |
| Price History | Yes | No | No |
| Static Scale | Yes | **No raw lifecycle write** | Add/Update/Delete |
| Scale Status | Yes | No | No |
| Scale Groups | Yes | Replace atomic + reset send state | No |
| Scale Item Mapping | Yes | Replace/copy atomic + validation | No |
| HotKey Template | Yes | Controlled upsert/replace + reset when needed | No |
| Sales Feed | Yes | No | No |
| Structured Invoice | Yes | ACK only | No |
| Reports | Yes | No | No |
| Device operations | No | No | **Yes** |

---

# 10. Public API هدف

```csharp
var sadr = new SadrScalesClient(options);

await sadr.Connection.ValidateAsync(ct);

await sadr.Stores.UpsertAsync(store, ct);
await sadr.ItemGroups.UpsertAsync(group, ct);
await sadr.Items.UpsertAsync(item, ct);
await sadr.Items.UpsertBatchAsync(items, ct);
await sadr.Items.DeleteAsync(pluNo, ct); // soft delete

var scales = await sadr.Scales.GetAllAsync(ct);
var scale = await sadr.Scales.GetAsync(scaleId, ct);
var status = await sadr.Scales.GetStatusAsync(scaleId, ct);

await sadr.ScaleAssignments.SetGroupsAsync(scaleId, groupCodes, ct);
await sadr.ScaleAssignments.SetItemsAsync(scaleId, assignments, ct);
await sadr.ScaleAssignments.CopyItemsAsync(sourceScaleId, targetScaleId, ct);

await sadr.HotKeys.SetTemplateAsync(groupCode, entries, ct);
var template = await sadr.HotKeys.GetTemplateAsync(groupCode, ct);

var feed = await sadr.Sales.ReadAfterAsync(cursor, 200, ct);
var query = await sadr.Sales.QueryAsync(filter, ct);
var summary = await sadr.Sales.GetSummaryAsync(filter, ct);

var invoice = await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);
var ack = await sadr.Invoices.AcknowledgeAsync(totalBarcode, ct);

var daily = await sadr.Reports.Sales.DailyAsync(range, ct);
var byScale = await sadr.Reports.Sales.ByScaleAsync(range, ct);
var byItem = await sadr.Reports.Sales.ByItemAsync(range, ct);
```

با وجود Managed SQL Command Channel:

```csharp
await sadr.Scales.AddAsync(createRequest, ct);
await sadr.Scales.UpdateAsync(updateRequest, ct);
await sadr.Scales.DeleteAsync(scaleId, ct);

await sadr.Commands.SendItemsAsync(scaleId, request, ct);
await sadr.Commands.GetItemsAsync(scaleId, request, ct);
await sadr.Commands.SendHotKeysAsync(scaleId, request, ct);
await sadr.Commands.GetSalesAsync(scaleId, request, ct);
await sadr.Commands.SetDateTimeAsync(scaleId, value, ct);
```

---

# 11. Capability discovery و سازگاری با 5.2.1

SDK باید بفهمد DB چه Contractی را پشتیبانی می‌کند.

مثلاً:

```csharp
var capabilities = await sadr.Connection.GetCapabilitiesAsync(ct);
```

5.2.1 فعلی می‌تواند این قابلیت‌ها را ارائه کند:

```text
Data SQL
Scale coarse Online/Offline status
Sales feed
Structured Invoice lookup
Invoice ACK after vNext SDK contract implementation if schema is compatible
Reports
Controlled data assignments where required schema exists
```

اما تا وقتی Sadr Scales Command Channel را نداشته باشد:

```text
Scale lifecycle commands
Device commands
```

باید `CapabilityNotAvailable` برگردانند؛ نه اینکه SDK آن‌ها را با SQL خام تقلید کند.

این محدودیت باید در Sample و Documentation کاملاً واضح باشد.

---

# 12. Device Commands عمومی

حداقل خانواده‌های مورد انتظار:

```text
Items
- SendItems
- GetItems
- DeleteAllItems

HotKeys
- SendHotKeys
- GetHotKeys

Sales
- GetSales

Settings
- SendSpecification
- GetSpecification
- SetDateTime
- Salesmen operations where supported
- Text operations where supported
- PrintFormat operations where supported
- BarcodeFormat operations where supported
- PaperType operations where supported
```

هر command قبل از اجرا باید Scale existence، enabled state، connection state، busy state و model capability را بررسی کند.

Raw protocol passthrough عمومی ممنوع است.

---

# 13. Firmware / File / Label transfer

این قابلیت‌ها در Core وجود دارند، ولی در Public default surface قرار نمی‌گیرند تا Security/API review جداگانه انجام شود.

این تصمیم قابلیت Core را حذف نمی‌کند.

---

# 14. Retry و Transaction

### Read SQL

Bounded retry مجاز است.

### Data SQL Write

```text
Validate before execution
Bounded transaction
No blind replay after transaction begins
Atomic commit/rollback
```

### Invoice ACK

Idempotent است و retry کنترل‌شده مجاز است.

### Runtime Command

هر command باید policy مشخص داشته باشد:

```text
SafeRetry
IdempotentControlledRetry
NonReplayable
```

Timeout هیچ‌وقت به معنی retry کور command غیرقابل replay نیست.

---

# 15. نتیجه Phase 2 تا این نقطه

تصمیم‌های تثبیت‌شده در طراحی:

1. Domain از Transport مستقل است؛
2. SQL روش اصلی Integration فعلی است؛
3. `SADR_Scale.Status` منبع عمومی Online/Offline در SQL است؛
4. Store با `StoreCode` شناخته می‌شود؛
5. Multi-Group با `SADR_ScaleItemClass` canonical است؛
6. Scale Mapping و Group HotKey Template دو مفهوم جدا هستند؛
7. Group/Mapping/HotKey writeها از SQL ممکن‌اند ولی semantic و transaction آن‌ها باید در SDK کنترل شود؛
8. Structured Invoice با TotalBarcode و `ScaleID + FID` قابل lookup است؛
9. Lookup auto-ACK نمی‌کند؛
10. `AlreadyRead` فاکتور را Block نمی‌کند و Data کامل برمی‌گردد؛
11. Invoice ACK صریح، بعد از Commit مقصد و idempotent است؛
12. Scale lifecycle و Device Commands نباید با raw SQL mutation پیاده شوند؛
13. جهت vNext، Managed SQL Command Channel راه پیشنهادی برای اجرای این عملیات توسط خود Sadr Scales است؛
14. protocol implementation خصوصی باقی می‌ماند؛
15. `v1.0.0` frozen است و vNext باید additive باشد.

---

# 16. Gate خروج از Phase 2

قبل از شروع implementation باید مالک/نگهدارنده این موارد را تأیید کند:

- [ ] Data SQL boundary؛
- [ ] Store/Scale relation؛
- [ ] Multi-group semantics؛
- [ ] Mapping write semantics؛
- [ ] HotKey write semantics؛
- [x] SQL Online/Offline status؛
- [x] Structured Invoice lookup؛
- [x] Explicit Invoice ACK؛
- [x] AlreadyRead returns full data؛
- [ ] Managed SQL Command Channel direction؛
- [ ] Scale lifecycle through managed command؛
- [ ] Device command families؛
- [ ] firmware/file/label exclusion pending separate review.

بعد از پذیرش، تصمیم‌ها در `docs/DECISIONS.md` به‌عنوان Accepted ثبت می‌شوند و سپس implementation planning شروع می‌شود.
