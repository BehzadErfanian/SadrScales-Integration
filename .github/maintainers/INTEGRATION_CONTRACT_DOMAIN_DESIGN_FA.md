# طراحی Contract و Domain مستقل از Transport — Phase 2

**وضعیت:** دامنه‌ی 5.2.1 برای Vendor-Ready Baseline تثبیت شده؛ Command Mailbox به Sadr Scales 5.3 منتقل شده است  
**تاریخ:** 2026-08-19  
**Sadr Scales baseline بررسی‌شده:** `5.2.1`  
**Sadr Scales source commit:** `1048749f52faba35e69464b64983e772c1c857e3`  
**Integration baseline:** `7690e2a622201cc9b5f0f112c6d6099d44c4233e`  
**Phase 1 audit:** `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`

---

## 1. هدف این فاز

هدف Phase 2 این است که برای شرکت‌های نرم‌افزاری یک Contract عمومی **کامل، ساده، پایدار و قابل تست** تعریف شود، بدون اینکه مصرف‌کننده مجبور باشد ساختار داخلی Sadr Scales یا پروتکل ترازوها را بداند.

اصل تجاری مهم این فاز:

> شرکت‌های نرم‌افزاری باید یک‌بار برای بازبینی و به‌روزرسانی Integration دعوت شوند؛ بنابراین قبل از Outreach عمومی باید یک Vendor-Ready Stable Baseline داشته باشیم و از انتشار چندمرحله‌ای و تغییر مداوم Contract جلوگیری کنیم.

`v1.0.0` و `SQL Contract v1` منتشرشده تغییر نمی‌کنند و Tag آن‌ها ثابت می‌ماند. توسعه‌ی بعدی additive است.

---

## 2. مسیر نسخه‌ها

### اکنون — Sadr Scales 5.2.1

تمرکز فوری فقط روی کامل‌کردن قابلیت‌هایی است که **همین نسخه واقعاً و قابل اتکا از SQL پشتیبانی می‌کند**:

```text
Third-party software
        ↓
   Integration SDK / SQL
        ↓
Sadr Scales Database
        ↑
Sadr Scales 5.2.1 Runtime
        ↕
      Scale
```

این Baseline باید قبل از نامه/دعوت عمومی به شرکت‌های نرم‌افزاری کامل، مستند، تست‌شده و Stable شود.

### بعد — Sadr Scales 5.3

Command Mailbox رسمی در خود Sadr Scales 5.3 اضافه می‌شود تا عملیات Runtime از طریق یک Contract پایدار و بدون افشای پروتکل قابل فراخوانی شوند.

```text
Third-party software
        ↓
Integration SDK / SQL
        ↓
SADR Integration Command Mailbox
        ↓
Sadr Scales 5.3 Runtime
        ↓
Validated Device Operation
        ↓
Scale
```

### آینده — Service / REST / سایر Transportها

Service و REST می‌توانند بعداً روی **همین Domain و Command semantics** سوار شوند. Service نباید Domain دیگری ایجاد کند.

---

## 3. Domain عمومی مستقل از Transport

مفهوم‌های عمومی عبارت‌اند از:

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
Device Command
```

Transport می‌تواند تغییر کند، اما معنی این مفاهیم نباید عوض شود.

---

# 4. Vendor-Ready SQL Surface برای 5.2.1

قابلیت‌هایی که در Baseline فعلی باید کامل شوند:

```text
Connection / schema validation
Store read / upsert
Item Group read / upsert
Item read / upsert / bounded batch / soft-delete
Price history read
Static Scale read
Scale Online / Offline status read
Scale Group Assignment
Scale Item Mapping
HotKey Template
Request Item Resend
Request HotKey Resend where supported
Sales Feed
Sales Query / Summary
Structured Invoice lookup
Invoice ACK
Reports
```

این فهرست Scope فوری است. چیزی که نیازمند تغییر Runtime 5.2.1 است، نباید انتشار Vendor-Ready را معطل کند.

---

## 4.1 Store / Branch

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
- ارتباط Scale با Store بر اساس `StoreCode` است.

---

## 4.2 Item Group

```csharp
public sealed class SadrItemGroup
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
```

**Source of truth:** `dbo.SADR_ItemClass`

Group و اختصاص Group به Scale دو مفهوم جدا هستند.

---

## 4.3 Item / PLU

هویت اصلی کالا `PluNo` است.

قواعد:

- `PluNo = 0` نامعتبر است؛
- Insert/Update عمومی است؛
- حذف عمومی soft-delete است؛
- Batch bounded و atomic است؛
- `SADR_ItemSyncState` و stateهای داخلی Sync عمومی نیستند؛
- Price history فقط Read عمومی است.

---

## 4.4 Scale Definition و Status

Static Scale از `dbo.SADR_Scale` خوانده می‌شود.

Status عمومی SQL:

```text
Online
Offline
Unknown
```

**Source of truth:** `dbo.SADR_Scale.Status`

Sadr Scales 5.2.1 خودش Online/Offline را در DB ثبت می‌کند؛ بنابراین این وضعیت برای نرم‌افزار ثالث قابل اتکاست.

Add/Update/Delete کامل Scale در 5.2.1 فقط یک SQL write ساده نیست و Validation، License، Registry، Connection و Cleanup را درگیر می‌کند؛ بنابراین Lifecycle کامل Scale جزو Vendor-Ready SQL 5.2.1 نیست.

---

## 4.5 Scale Group Assignment

Canonical source:

```text
dbo.SADR_ScaleItemClass
```

یک Scale می‌تواند چند Group داشته باشد.

Replace باید atomic باشد و بعد از تغییر:

```text
SADR_Scale.LastSendItem = 0
```

شود تا AutoSend تغییر Assignment را از دست ندهد.

`SADR_Scale.ItemClassCode` فقط compatibility قدیمی است و source اصلی Multi-Group نیست.

---

## 4.6 Scale Item Mapping

Source:

```text
dbo.SADR_ScaleItemMap
```

قواعد:

- PLU تکراری برای یک Scale رد شود؛
- ItemCode تکراری رد شود؛
- Page/Key تکراری رد شود؛
- Layout با `HotKeyCountPerPage` و `HotKeyPageCount` همان Scale سازگار باشد؛
- Replace/Copy atomic باشد؛
- بعد از تغییر Mapping، `LastSendItem = 0` شود.

---

## 4.7 HotKey Template

Source:

```text
dbo.SADR_KeyAssignment
```

HotKey Template در سطح Group است و با Mapping اختصاصی Scale یکی نیست.

```text
Item Group
   └── HotKey Template

Scale
   └── Scale Item Mapping
         └── optional Page/Key
```

برای Replace/Delete، `LastSendKey` ترازوهای تحت تأثیر باید reset شود تا حذف Row در AutoSend گم نشود.

---

# 5. AutoSend Resend Request در 5.2.1

این دو قابلیت جزو Contract رسمی SQL فعلی می‌شوند.

## 5.1 Request Item Resend

```csharp
await sadr.Items.RequestResendAsync(scaleId, ct);
```

Semantic:

```text
SADR_Scale.LastSendItem = 0
```

Raw SQL:

```sql
UPDATE dbo.SADR_Scale
SET LastSendItem = 0
WHERE ScaleID = @ScaleId;
```

## 5.2 Request HotKey Resend

```csharp
await sadr.HotKeys.RequestResendAsync(scaleId, ct);
```

Semantic:

```text
SADR_Scale.LastSendKey = 0
```

Raw SQL:

```sql
UPDATE dbo.SADR_Scale
SET LastSendKey = 0
WHERE ScaleID = @ScaleId;
```

## 5.3 معنی موفقیت

موفقیت این APIها فقط یعنی **درخواست AutoSend در DB ثبت شد**.

این عملیات ارسال فوری نیست و ادعا نمی‌کند ترازو داده را دریافت کرده است. انتقال در چرخه AutoSend بعدی و در صورت مناسب بودن وضعیت Scale انجام می‌شود.

---

# 6. Sales Feed و Structured Invoice

دو مسیر مستقل‌اند.

## 6.1 Sales Feed

```csharp
var batch = await sadr.Sales.ReadAfterAsync(cursor, pageSize, ct);
```

- `SADR_Logs` Read-only است؛
- مقصد Cursor خودش را نگه می‌دارد؛
- gap در ID مجاز است؛
- Feed Ack ندارد.

## 6.2 Structured Invoice

Lookup:

```csharp
await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);
await sadr.Invoices.GetAsync(scaleId, fid, ct);
```

TotalBarcode Core:

```text
25 + ScaleID(D3) + FID(D9)
```

داده‌ی Invoice از ساختار `SADR_Total + SADR_Detail` و در صورت نیاز اطلاعات مکمل `SADR_Logs` ساخته می‌شود. مصرف‌کننده نباید JOIN داخلی را خودش طراحی کند.

---

# 7. Invoice ACK

`SADR_Total.LableStatus` Contract عمومی ACK کل Invoice است:

```text
0 / NULL → هنوز ACK نشده
1        → قبلاً ACK شده
```

## 7.1 Lookup هیچ‌وقت Auto-ACK نمی‌کند

```text
Lookup
↓
Return full invoice + status
↓
Destination Save
↓
Destination Commit succeeds
↓
Explicit ACK
```

## 7.2 AlreadyRead Block نیست

```text
FoundUnread → full invoice
AlreadyRead → full invoice + informational warning
NotFound    → no invoice
```

فاکتور ACKشده باید دوباره قابل دریافت باشد تا بازیابی داده‌ی حذف‌شده یا گم‌شده ممکن باشد.

## 7.3 ACK idempotent است

```text
0 → 1 = Acknowledged
1 → 1 = AlreadyAcknowledged
missing = NotFound
```

`SADR_Detail.ItemStatus` فعلاً جزو ACK عمومی کل Invoice نیست.

---

# 8. Reports

Contract هدف باید قابلیت‌های واقعی Core را پوشش دهد:

```text
Sales Query by date / scale / PLU / FID
Paging
Record Count
Invoice Count
Total Price
Total Weight
Total Quantity
Daily Report
By Scale Report
By Item Report
```

---

# 9. Sadr Scales 5.3 — Command Mailbox

این قابلیت **عمداً از Vendor-Ready Baseline فعلی خارج است** و به 5.3 منتقل شده است.

هدف: نرم‌افزار ثالث بتواند یک Command تایپ‌شده ثبت کند و Sadr Scales خودش آن را با Runtime واقعی اجرا و نتیجه را گزارش کند، بدون افشای پروتکل Device.

## 9.1 مدل اولیه

برای هر Scale یک Mailbox/Row پایدار در نظر گرفته می‌شود؛ در هر لحظه فقط یک Command فعال برای همان Scale.

فیلدهای مفهومی:

```text
ScaleID
RequestId
CommandCode
CommandFlags / typed parameters
Status
RequestedAt
StartedAt
CompletedAt
ResultCode
ResultMessage
RowVersion
```

State machine اولیه:

```text
Idle
→ Pending
→ Running
→ Succeeded
```

یا:

```text
Pending
→ Running
→ Failed / Rejected
```

Reasonهای عمومی می‌توانند شامل مواردی مثل `Offline`, `Disabled`, `Unsupported`, `Busy`, `LicenseDenied`, `Timeout` باشند.

## 9.2 Command Domain

نمونه خانواده‌ها:

```text
Items
- SendItems
- DeleteAllItems
- RetrieveItems

HotKeys
- SendHotKeys
- RetrieveHotKeys

Sales
- RetrieveSales

Settings
- SendSpecification
- RetrieveSpecification
- SetDateTime
- Salesman
- Text
- PrintFormat
- BarcodeFormat
- PaperType
```

مثال:

```text
Command = SendItems
Flags   = ClearExisting
```

برای LSG می‌تواند به معنی ارسال فوری کالا با پاک‌کردن داده قبلی باشد، ولی Mapping واقعی به پروتکل داخل Sadr Scales خصوصی می‌ماند.

## 9.3 اصول Command Mailbox

- Command جدید نباید Command فعال همان Scale را overwrite کند؛
- `RequestId` باید نتیجه را به درخواست مشخص وصل کند؛
- Commandها typed هستند؛
- Raw packet/opcode/payload پروتکلی عمومی ممنوع است؛
- Result عمومی Business/Operational است، نه Packet-level؛
- Validation، License، Registry، Busy/Connection state و model capability داخل Sadr Scales اجرا می‌شود؛
- Service/API آینده می‌تواند Transport دوم همین Command Domain باشد.

Schema و عدد دقیق `CommandCode` در Phase طراحی 5.3 Freeze می‌شود، نه در Integration 5.2.1.

---

# 10. Public API هدف برای Vendor-Ready 5.2.1

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

await sadr.ScaleAssignments.SetGroupsAsync(scaleId, groups, ct);
await sadr.ScaleAssignments.SetItemsAsync(scaleId, assignments, ct);
await sadr.ScaleAssignments.CopyItemsAsync(sourceScaleId, targetScaleId, ct);

await sadr.HotKeys.SetTemplateAsync(groupCode, entries, ct);

await sadr.Items.RequestResendAsync(scaleId, ct);
await sadr.HotKeys.RequestResendAsync(scaleId, ct);

var feed = await sadr.Sales.ReadAfterAsync(cursor, 200, ct);
var invoice = await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);
var ack = await sadr.Invoices.AcknowledgeAsync(totalBarcode, ct);

var daily = await sadr.Reports.Sales.DailyAsync(range, ct);
```

این API هدف است؛ پیاده‌سازی باید Slice-by-Slice و با تست انجام شود.

---

# 11. Vendor-Ready Release Gate

قبل از ارسال نامه به شرکت‌های نرم‌افزاری، Baseline جدید باید همه موارد زیر را PASS کند:

1. Contract SQL نهایی 5.2.1 Freeze شده باشد؛
2. تمام قابلیت‌های Vendor-Ready بالا در SDK پوشش داده شده باشند؛
3. Raw SQL path برای مصرف‌کننده‌های غیر C# مستند باشد؛
4. Structured Invoice + ACK تست واقعی SQL داشته باشد؛
5. Scale status، Group/Mapping/HotKey و Resend semantics تست شده باشند؛
6. Reports و Sales Query تست شده باشند؛
7. WinForms Developer Sample واقعی قابلیت‌ها را نشان دهد؛
8. Demo Data seeded/reproducible و Production guard داشته باشد؛
9. Quick Start کوتاه و Full Guide کامل باشند؛
10. .NET Framework 4.8 consumer و SQL Server integration tests PASS باشند؛
11. API/Contract naming و compatibility Freeze شده باشند؛
12. یک Release Candidate به‌صورت end-to-end از دید شرکت ثالث اجرا شده باشد؛
13. بعد از این Gate، فقط bug/security/compatibility fix وارد Baseline Vendor Outreach شود.

هدف این Gate این است که شرکت‌های نرم‌افزاری یک بار Integration را بررسی و به‌روزرسانی کنند، نه اینکه چندبار مجبور به تطبیق با Contractهای ناپایدار شوند.

---

# 12. خارج از Scope فوری

موارد زیر Vendor Outreach فعلی را معطل نمی‌کنند:

```text
Sadr Scales 5.3 Command Mailbox implementation
Service / REST transport
Realtime Runtime progress API
Public Scale Emulator
Full Integration Lab
Advanced runtime device commands beyond 5.2.1 SQL
Firmware/File/Label public operations
```

این‌ها Roadmap بعدی‌اند و باید بدون شکستن Vendor-Ready Contract فعلی اضافه شوند.

---

# 13. Definition of Done هر قابلیت عمومی

هر قابلیت فقط وقتی Done است که این پنج جزء را داشته باشد:

```text
Contract
+ SDK/API
+ Documentation
+ Code Sample
+ Executable Sample
```

و تست‌پذیری آن نیز مشخص باشد.

---

# 14. قدم بعدی

بعد از Merge شدن Phase 2:

1. Capability Matrix نهایی را به Implementation Slices تبدیل کنیم؛
2. ابتدا SQL Contract و SDK فعلی را تا Vendor-Ready 5.2.1 کامل کنیم؛
3. سپس Sample App و Demo Data را کامل کنیم؛
4. End-to-End Vendor Acceptance Test اجرا کنیم؛
5. Release Candidate را Freeze کنیم؛
6. بعد از PASS کامل، Outreach/نامه به شرکت‌های نرم‌افزاری انجام شود؛
7. Command Mailbox جداگانه در برنامه Sadr Scales 5.3 پیاده‌سازی شود.
