# طراحی Contract و Domain مستقل از Transport — Phase 2

**وضعیت:** پیش‌نویس کامل Phase 2 برای بازبینی مالک/نگهدارنده  
**تاریخ:** 2026-08-19  
**Sadr Scales baseline:** `5.2.1`  
**Sadr Scales source commit:** `1048749f52faba35e69464b64983e772c1c857e3`  
**Integration baseline:** `7690e2a622201cc9b5f0f112c6d6099d44c4233e`  
**Phase 1 audit:** `.github/maintainers/INTEGRATION_SURFACE_AUDIT_5.2.1_FA.md`

---

## 1. هدف Phase 2

Phase 1 مشخص کرد چه قابلیت‌هایی در Sadr Scales 5.2.1 واقعاً وجود دارند و کدام‌ها Safe Data، Managed Runtime یا Internal هستند.

Phase 2 پاسخ چهار سؤال است:

1. Domain عمومی Integration از دید شرکت ثالث چیست؟
2. Public API سطح بالا چه شکلی دارد؟
3. Direct SQL دقیقاً در چه محدوده‌ای امن و پشتیبانی‌شده است؟
4. چه عملیات‌هایی فقط از Managed Runtime عبور می‌کنند؟

این سند **پیاده‌سازی نیست**. هیچ Packet، پروتکل خصوصی، capture، firmware داخلی، کلید یا Runtime source را عمومی نمی‌کند.

`v1.0.0` و `SQL Contract v1` همچنان frozen هستند. نام/شماره Contract بعدی تا زمان پذیرش نهایی این طراحی و برنامه سازگاری عمداً تعیین نمی‌شود و در این سند با `vNext` نامیده می‌شود.

---

## 2. اصول قطعی طراحی

### 2.1 Domain قبل از Transport

مصرف‌کننده باید با مفهوم‌های تجاری کار کند، نه با نام جدول یا جزئیات Registry.

```text
Integration Domain
        │
        ├── Safe Data Transport
        │      ├── SQL
        │      └── Future API/other
        │
        └── Managed Runtime Transport
               ├── Local Runtime API
               └── Future remote/runtime gateway
```

Transport می‌تواند عوض شود؛ معنی `Scale`, `Item`, `Invoice` یا `SendItems` نباید عوض شود.

### 2.2 هیچ fallback خطرناک

اگر عملیاتی Runtime لازم دارد و Runtime channel در دسترس نیست:

- SDK نباید یواشکی Direct SQL انجام دهد؛
- نباید `SADR_Scale.Status` را جای Live Status جا بزند؛
- نباید command را به raw table mutation تبدیل کند؛
- باید خطای روشن `CapabilityNotAvailable` یا معادل عمومی آن برگردد.

### 2.3 Public API بر اساس business concept

جهت API سطح بالا:

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

نام‌هایی مثل `SADR_ScaleItemClass`, `LastSendItem`, `ItemSyncState`, `Registry` یا `LableStatus` نباید جزو API عادی مصرف‌کننده باشند.

### 2.4 Source of truth باید صریح باشد

هر مدل عمومی دقیقاً یک source of truth دارد. اگر Core برای سازگاری قدیمی representation دیگری نگه می‌دارد، آن representation به عنوان مدل اصلی عمومی معرفی نمی‌شود.

### 2.5 Command typed است، نه generic protocol passthrough

API عمومی باید این شکل را داشته باشد:

```csharp
await sadr.Commands.SendItemsAsync(...);
await sadr.Commands.GetSalesAsync(...);
await sadr.Commands.SetDateTimeAsync(...);
```

و نه این شکل:

```csharp
await sadr.Commands.ExecuteRawAsync(commandId, payload);
```

Raw command/packet passthrough مرز مالکیت فکری و ایمنی را می‌شکند و جزو Public API نیست.

---

## 3. Domain عمومی vNext

### 3.1 Store / Branch

```csharp
public sealed class SadrStore
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
```

**Source of truth:** `dbo.SADR_Store`.

قواعد:

- `Code` هویت پایدار Store است؛
- `Code = "0"` شعبه پیش‌فرض Core است؛
- `Name` فقط label است و هویت نیست؛
- Upsert مجاز است؛
- Delete عمومی در طراحی پایه ارائه نمی‌شود چون Scale می‌تواند FK به Store داشته باشد؛
- نسبت Scale به Store با `StoreCode` بیان می‌شود، نه با `SADR_Scale.StoreName`.

### 3.2 Item Group

```csharp
public sealed class SadrItemGroup
{
    public string Code { get; set; }
    public string Name { get; set; }
}
```

**Source of truth:** `dbo.SADR_ItemClass`.

Group یک مفهوم مستقل تجاری است. مدیریت گروه و اختصاص گروه به ترازو دو API جدا هستند.

### 3.3 Item / PLU

مدل عمومی Item باید از Contract فعلی رشد کند، ولی identity اصلی همان `PluNo` باقی می‌ماند.

قواعد پایه:

- `PluNo` صفر مجاز نیست؛
- حذف، **soft delete** با semantic عمومی است، نه physical row delete؛
- batch write bounded و atomic باقی می‌ماند؛
- internal delivery/sync fields بخشی از Item عمومی نیستند؛
- Price history read model می‌تواند عمومی شود، اما نوشتن مستقیم PriceLog عمومی نیست.

### 3.4 Scale Definition

Scale static definition و Scale live status دو مدل جدا هستند.

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

Static read می‌تواند از DB بیاید. Add/Update/Delete/Enable/Disable باید Managed Runtime باشد.

### 3.5 Scale Status

```csharp
public enum SadrScaleConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Disconnecting,
    Faulted
}

public sealed class SadrScaleStatus
{
    public int ScaleId { get; set; }
    public SadrScaleConnectionState ConnectionState { get; set; }
    public string CurrentActivity { get; set; }
    public string LastActivityResult { get; set; }
    public int? ProgressCurrent { get; set; }
    public int? ProgressTotal { get; set; }
    public int? ProgressPercent { get; set; }
    public string ProgressDetail { get; set; }
    public DateTime? LastConnectedAtUtc { get; set; }
    public DateTime? LastDisconnectedAtUtc { get; set; }
    public DateTime? LastActivityAtUtc { get; set; }
    public DateTime? LastErrorAtUtc { get; set; }
    public string LastErrorMessage { get; set; }
}
```

**Source of truth:** `ScaleRegistry + ScaleRuntimeState`.

`dbo.SADR_Scale.Status` Public Live Status نیست و fallback محسوب نمی‌شود.

### 3.6 Scale Group Assignment

یک ترازو می‌تواند چند گروه داشته باشد.

```csharp
public sealed class SadrScaleGroupAssignment
{
    public int ScaleId { get; set; }
    public IReadOnlyList<string> GroupCodes { get; set; }
}
```

**Canonical source:** `dbo.SADR_ScaleItemClass`.

`dbo.SADR_Scale.ItemClassCode` فقط compatibility/legacy representation است و public source of truth نیست.

قواعد:

- حداقل یک Group لازم است؛
- duplicate group رد می‌شود؛
- همه Groupها باید موجود باشند؛
- تغییر assignment باید internal send-state لازم را reset کند؛
- مصرف‌کننده نباید `LastSendItem` را خودش مدیریت کند.

### 3.7 Scale Item Assignment

`SADR_ScaleItemMap` یک mapping مختص ترازو است و با Group HotKey Template یکی نیست.

```csharp
public sealed class SadrScaleItemAssignment
{
    public int ScaleId { get; set; }
    public int PluNo { get; set; }
    public string ItemCode { get; set; }
    public SadrHotKeyPosition HotKeyPosition { get; set; }
}
```

قواعد:

- یک PLU در mapping یک ترازو تکراری نیست؛
- ItemCode تکراری نیست؛
- اگر HotKeyPosition دارد، موقعیت تکراری مجاز نیست؛
- layout باید با `HotKeyCountPerPage` و `HotKeyPageCount` ترازو سازگار باشد؛
- write از Managed Configuration API انجام می‌شود؛ raw table mutation توصیه نمی‌شود.

### 3.8 HotKey Template

`dbo.SADR_KeyAssignment` یک **template سطح گروه** است:

```csharp
public sealed class SadrHotKeyTemplateEntry
{
    public string GroupCode { get; set; }
    public int PageNo { get; set; }
    public int KeyNo { get; set; }
    public int PluNo { get; set; }
}
```

این مفهوم از `ScaleItemAssignment` جداست:

```text
Item Group
   └── HotKey Template        -> SADR_KeyAssignment

Scale
   └── Item Assignment/Map    -> SADR_ScaleItemMap
         └── optional position
```

`HotKeys` مدیریت template/layout data را انجام می‌دهد؛ `Commands.SendHotKeysAsync` انتقال به device است.

### 3.9 Sales Feed

Feed افزایشی همچنان read-only است.

```csharp
SadrSalesBatch ReadAfterAsync(long lastProcessedId, int pageSize, ...)
```

قواعد D-016 حفظ می‌شوند:

- destination cursor را خودش durable نگه می‌دارد؛
- gap در ID مجاز است؛
- SDK cursor مقصد را در DB Sadr ذخیره نمی‌کند؛
- duplicate business detection باید از identity فاکتور/رکورد استفاده کند، نه فرض contiguous ID.

### 3.10 Structured Invoice

```csharp
public sealed class SadrInvoice
{
    public int ScaleId { get; set; }
    public int Fid { get; set; }
    public string TotalBarcode { get; set; }
    public int ReceiptNo { get; set; }
    public DateTime? SaleDateTime { get; set; }
    public IReadOnlyList<SadrInvoiceLine> Lines { get; set; }
}
```

Public lookup:

```csharp
await sadr.Invoices.GetAsync(scaleId, fid, ct);
await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);
```

Source data از رابطه `SADR_Total + SADR_Detail + SADR_Logs` ساخته می‌شود، اما مصرف‌کننده لازم نیست join داخلی را بداند.

### 3.11 Reports

Public reports باید business-oriented باشند:

```csharp
sadr.Reports.Sales.DailyAsync(...)
sadr.Reports.Sales.ByScaleAsync(...)
sadr.Reports.Sales.ByItemAsync(...)
```

همچنین query/summary عمومی Sales حفظ می‌شود.

---

## 4. تصمیم‌های Gapهای Phase 1

### G-01 — Store identity و Scale relation

**پیشنهاد Phase 2:**

- `SADR_Store.StoreCode` هویت Store است؛
- `SADR_Scale.StoreCode` رابطه Scale -> Store است؛
- `SADR_Scale.StoreName` legacy/display field است و source of truth نیست؛
- ایجاد ترازو بدون Store مشخص، Store `"0"` می‌گیرد؛
- تغییر Store یک Scale از Public API باید Managed Runtime/Managed Configuration باشد تا Registry و persistence هماهنگ بمانند؛
- Direct SQL consumer حق ندارد فقط `StoreName` را برای تغییر شعبه دستکاری کند.

دلیل: Core برای Store `"0"` مقدار پیش‌فرض دارد و FK Scale به StoreCode است، در حالی که runtime create/update فعلی StoreCode را به عنوان request first-class دریافت نمی‌کند.

### G-02 — Primary Group در برابر Multi-Group

**پیشنهاد Phase 2:**

- Public Domain مفهوم `PrimaryGroup` ندارد؛
- `SADR_ScaleItemClass` source of truth اختصاص چندگروهی است؛
- `SADR_Scale.ItemClassCode` compatibility field باقی می‌ماند و API جدید به آن وابسته نمی‌شود؛
- `ScaleCreateRequest` عمومی به جای یک `Group` می‌تواند `GroupCodes` داشته باشد و حداقل یک مقدار لازم است؛
- adapter به Runtime 5.2.1 می‌تواند اولین Group را برای legacy create استفاده کند و سپس assignment کامل را ثبت کند؛ این جزئیات عمومی نمی‌شود.

### G-03 — Mapping در برابر HotKey

**پیشنهاد Phase 2:** دو مفهوم عمومی جدا:

1. `ScaleItemAssignment` — assignment مختص Scale، با optional key position؛
2. `HotKeyTemplate` — template سطح Item Group.

`Commands.SendHotKeysAsync` سومین مفهوم است: انتقال data به device.

بنابراین Data definition، per-scale mapping و device command با هم یکی نمی‌شوند.

### G-04 — Invoice Ack

**پیشنهاد Phase 2:** `Invoices.AcknowledgeAsync` در Contract پایه **وجود نداشته باشد**.

دلیل:

- تصمیم پذیرفته‌شده D-016 می‌گوید destination مالک cursor/state خودش است؛
- `LableStatus` و `ItemStatus` در 5.2.1 برای state داخلی Core استفاده می‌شوند و Contract عمومی چندمصرف‌کننده تعریف نشده؛
- write کردن این fieldها از SDK می‌تواند با pipeline داخلی Sadr تداخل کند.

قاعده عمومی:

```text
Read sales/invoice
→ destination commits its own business transaction
→ destination advances its own cursor
```

اگر در آینده Ack مشترک لازم شد، باید Contract مستقل با owner/lease/idempotency semantics طراحی شود.

### G-05 — Live Status

**پیشنهاد Phase 2:** فقط Runtime source معتبر است.

```csharp
await sadr.Scales.GetStatusAsync(scaleId, ct);
```

اگر Runtime transport configure نشده باشد، operation با CapabilityNotAvailable fail می‌شود. SQL Status fallback ممنوع است.

### G-06 — Runtime Command Channel

**پیشنهاد Phase 2:** typed Managed Runtime API با commandهای مشخص و capability negotiation.

Minimum public commands:

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

Specification / Settings
- SendSpecification
- GetSpecification
- SetDateTime
- Salesmen operations where model supports
- Text operations where model supports
- PrintFormat operations where model supports
- BarcodeFormat operations where model supports
- PaperType operations where model supports
```

هر command باید قبل از اجرا بررسی کند:

- Scale exists;
- Scale enabled;
- Runtime transport available;
- device/model capability supports command;
- connection/operation state اجازه اجرا می‌دهد؛
- cancellation/timeout policy مشخص است.

### G-07 — File/Firmware/Label transfer

**پیشنهاد Phase 2:** در Public default surface قرار نگیرد.

این عملیات `Managed Runtime` هستند ولی به دلیل سطح خطر و مالکیت فکری، تا Security/API review جداگانه public نمی‌شوند.

این تصمیم قابلیت Core را حذف نمی‌کند؛ فقط Public surface فعلی را محدود می‌کند.

---

## 5. Public API سطح بالا

هدف readability:

```csharp
var sadr = new SadrScalesClient(options);

await sadr.Connection.ValidateAsync(ct);

await sadr.Stores.UpsertAsync(store, ct);
var stores = await sadr.Stores.GetAllAsync(ct);

await sadr.ItemGroups.UpsertAsync(group, ct);
await sadr.Items.UpsertAsync(item, ct);
await sadr.Items.UpsertBatchAsync(items, ct);
await sadr.Items.DeleteAsync(pluNo, ct); // soft delete

var scales = await sadr.Scales.GetAllAsync(ct);
var scale = await sadr.Scales.GetAsync(scaleId, ct);
var status = await sadr.Scales.GetStatusAsync(scaleId, ct); // runtime required

await sadr.Scales.AddAsync(createRequest, ct);       // runtime required
await sadr.Scales.UpdateAsync(updateRequest, ct);   // runtime required
await sadr.Scales.DeleteAsync(scaleId, ct);         // runtime required

await sadr.ScaleAssignments.SetGroupsAsync(scaleId, groupCodes, ct);
await sadr.ScaleAssignments.SetItemsAsync(scaleId, assignments, ct);
await sadr.ScaleAssignments.CopyItemsAsync(sourceScaleId, targetScaleId, ct);

await sadr.HotKeys.SetTemplateAsync(groupCode, entries, ct);
var template = await sadr.HotKeys.GetTemplateAsync(groupCode, ct);

var sales = await sadr.Sales.ReadAfterAsync(cursor, 200, ct);
var query = await sadr.Sales.QueryAsync(filter, ct);
var summary = await sadr.Sales.GetSummaryAsync(filter, ct);

var invoice1 = await sadr.Invoices.GetAsync(scaleId, fid, ct);
var invoice2 = await sadr.Invoices.GetByBarcodeAsync(totalBarcode, ct);

var daily = await sadr.Reports.Sales.DailyAsync(range, ct);
var byScale = await sadr.Reports.Sales.ByScaleAsync(range, ct);
var byItem = await sadr.Reports.Sales.ByItemAsync(range, ct);

await sadr.Commands.SendItemsAsync(scaleId, request, ct);
await sadr.Commands.GetSalesAsync(scaleId, request, ct);
await sadr.Commands.SetDateTimeAsync(scaleId, value, ct);
```

### 5.1 Capability discovery

برای جلوگیری از حدس درباره transport/model:

```csharp
SadrIntegrationCapabilities capabilities =
    await sadr.Connection.GetCapabilitiesAsync(ct);
```

Capability باید حداقل فرق این‌ها را نشان دهد:

- Data transport available؛
- Runtime transport available؛
- Live status available؛
- Scale lifecycle commands available؛
- device command families available؛
- current contract version؛
- supported baseline/version info.

نباید private protocol name/packet detail را افشا کند.

---

## 6. SQL Contract vNext — مرز کامل و امن

### 6.1 Direct SQL safe surface

| Domain | SQL object | Read | Write | Public rule |
|---|---|---:|---:|---|
| Connection/schema | effective migrated schema | Yes | No | validate only |
| Store | `SADR_Store` | Yes | Upsert | no public delete in base contract |
| Item Group | `SADR_ItemClass` | Yes | Upsert | referential rules enforced |
| Item | `SADR_Item` | Yes | Upsert + soft delete | no physical delete |
| Price History | `SADR_PriceLog` | Yes | No | read-only public history |
| Scale static definition | `SADR_Scale` | Yes | No direct write | lifecycle/config mutation goes managed |
| Sales Feed | `SADR_Logs` | Yes | No | destination cursor-owned |
| Structured Invoice | `SADR_Total` + `SADR_Detail` + feed relation | Yes | No | SDK/domain hides joins |
| Reports | reporting queries | Yes | No | read-only |

### 6.2 Managed configuration surface

این بخش ممکن است در implementation داخلی از SQL استفاده کند، اما **Raw SQL Contract عمومی نیست**:

| Domain | Internal storage | Public operation |
|---|---|---|
| Scale -> Store | `SADR_Scale.StoreCode` | managed update |
| Scale group assignment | `SADR_ScaleItemClass` | `SetGroupsAsync` |
| Scale item mapping | `SADR_ScaleItemMap` | `SetItemsAsync`, `CopyItemsAsync` |
| HotKey template | `SADR_KeyAssignment` | `HotKeys.SetTemplateAsync` |

دلیل: این تغییرها send-state/layout/Runtime consistency دارند و مصرف‌کننده نباید internal fields لازم برای consistency را خودش مدیریت کند.

### 6.3 Explicitly internal SQL surface

این object/fieldها Public Contract نیستند:

```text
SADR_ItemSyncState
SADR_Scale.LastSendItem
SADR_Scale.LastSendKey
SADR_Scale.Status as live truth
SADR_Total.LableStatus as destination Ack
SADR_Detail.ItemStatus as destination Ack
schema migration/repair internals
backup/restore/drop database operations
recovery/quarantine state
protocol-specific persistence details
```

وجود این‌ها در DB به معنی permission عمومی برای read/write contract نیست.

---

## 7. Managed Runtime Command Boundary

### 7.1 Runtime owns

Runtime مالک این concernها است:

```text
connection lifecycle
registry synchronization
model/capability check
queue/in-flight coordination
heartbeat interaction
send/get operation sequencing
progress
cancellation/timeout
reconnect
sales receive orchestration
safe delete/drain/cleanup
license gate for managed scale lifecycle
```

Public Integration فقط request و result typed می‌بیند.

### 7.2 Operation result

فرم عمومی پیشنهادی:

```csharp
public sealed class SadrOperationResult
{
    public bool Succeeded { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public int? ScaleId { get; set; }
}
```

برای operation طولانی:

```csharp
public sealed class SadrOperationProgress
{
    public int? Current { get; set; }
    public int? Total { get; set; }
    public int? Percent { get; set; }
    public string Detail { get; set; }
}
```

Transport ممکن است internally operation ID داشته باشد، اما consumer برای عملیات ساده مجبور به مدیریت queue/registry داخلی نیست.

### 7.3 Error families

Public errors باید domain-oriented باشند:

```text
ValidationFailed
NotFound
Duplicate
Conflict
CapabilityNotAvailable
RuntimeUnavailable
ScaleOffline
OperationBusy
TimedOut
Cancelled
DatabaseUnavailable
DatabaseContractMismatch
LicenseRestricted
UnexpectedFailure
```

SQL error number یا protocol error code می‌تواند diagnostic detail باشد، نه contract اصلی.

---

## 8. Compatibility با v1.0.0

### 8.1 v1 frozen می‌ماند

این طراحی هیچ رفتار published در `v1.0.0` را عوض نمی‌کند.

```text
v1.0.0
- ItemGroups.Upsert
- Items.Upsert / UpsertBatch
- Sales.ReadAfter
- SQL Contract v1
```

همچنان معتبرند.

### 8.2 vNext باید additive migration path داشته باشد

مصرف‌کننده v1 نباید برای مهاجرت مجبور شود code path فعلی را یک‌باره دور بریزد.

جهت مهاجرت:

```csharp
// v1 style remains valid
client.Items.UpsertAsync(...)
client.Sales.ReadAfterAsync(...)

// vNext adds domains
client.Stores...
client.Scales...
client.Invoices...
client.Commands...
```

اگر signature یا semantic موجود شکسته شود، release باید major/API contract decision جدا داشته باشد.

---

## 9. Transport behavior matrix

| Capability | Direct SQL | Managed Runtime | Future REST/API |
|---|---:|---:|---:|
| Connection/schema validate | Yes | Optional | Yes |
| Store CRUD bounded | Yes | No required | Yes |
| Item Group | Yes | No required | Yes |
| Item/PLU | Yes | No required for DB write | Yes |
| Static Scale read | Yes | Yes | Yes |
| Add/Update/Delete Scale | No | **Required** | REST may proxy Runtime |
| Live Scale Status | No | **Required** | REST may proxy Runtime |
| Scale Group Assignment | No raw contract | Managed config | Yes |
| Scale Item Mapping | No raw contract | Managed config | Yes |
| HotKey Template | No raw contract | Managed config | Yes |
| Sales Feed | Yes | Optional | Yes |
| Sales Query/Summary/Reports | Yes | Optional | Yes |
| Structured Invoice read | Yes | Optional | Yes |
| Device Commands | No | **Required** | REST may proxy Runtime |
| Firmware/File/Label transfer | No | Protected/internal pending review | Not public by default |

---

## 10. Idempotency و retry

قواعد v1 حفظ و تعمیم می‌شوند:

### Safe reads

Bounded retry مجاز است.

### SQL writes

- validate before execution؛
- transaction scoped؛
- automatic replay بعد از شروع transaction ممنوع؛
- upsert semantic باید deterministic باشد.

### Runtime commands

Commandی که به device ارسال شده نباید بعد از timeout به شکل کور automatic replay شود مگر خود operation Contract صریح idempotent باشد.

در implementation هر command باید یکی از این policyها را داشته باشد:

```text
ReadOnly / SafeRetry
IdempotentWrite / ControlledRetry
NonReplayableCommand
```

این policy internal implementation detail می‌تواند باشد، ولی رفتار public باید قابل پیش‌بینی باشد.

---

## 11. Security boundary

Public Integration نباید موارد زیر را ارائه کند:

```text
raw device packet
protocol command bytes
capture/pcap
vendor protocol documentation
private signing/licensing keys
firmware internals
customer production data
internal diagnostics passwords/secrets
arbitrary SQL execution API
arbitrary runtime reflection/command execution
```

Runtime transport باید authentication/authorization مستقل داشته باشد؛ طراحی جزئی آن مربوط به implementation/security phase بعدی است.

---

## 12. نتیجه پیشنهادی Phase 2

با پذیرش این سند، تصمیم‌های معماری زیر بسته می‌شوند:

1. Domain از Transport مستقل است؛
2. SQL فقط Safe Data surface را مستقیم پوشش می‌دهد؛
3. Scale lifecycle و Live Status Runtime-only هستند؛
4. Store relation با `StoreCode` تعریف می‌شود؛
5. Multi-group با `SADR_ScaleItemClass` canonical می‌شود؛
6. Scale mapping و Group HotKey template دو مفهوم جدا هستند؛
7. Public invoice Ack حذف می‌شود و destination cursor مالک processing state می‌ماند؛
8. Runtime command API typed است و raw protocol passthrough ندارد؛
9. firmware/file/label در public default surface قرار نمی‌گیرد تا review جدا؛
10. `v1.0.0` frozen و migration path additive باقی می‌ماند.

---

## 13. Gate خروج از Phase 2

Phase 2 فقط وقتی Complete است که مالک/نگهدارنده این موارد را تأیید کند:

- [ ] Domain model؛
- [ ] Store/Scale relation؛
- [ ] Multi-group source of truth؛
- [ ] Mapping/HotKey split؛
- [ ] عدم Public Ack؛
- [ ] Live Status runtime-only؛
- [ ] Managed Runtime command boundary؛
- [ ] SQL safe surface؛
- [ ] v1 compatibility rule؛
- [ ] firmware/file/label exclusion pending separate review.

بعد از پذیرش، قدم بعدی implementation planning است؛ نه توسعه بدون ترتیب.

پیشنهاد ترتیب اجرای بعدی:

```text
Phase 3A — Public Domain Models + interfaces/client shape
Phase 3B — Expanded Safe SQL implementation
Phase 3C — Managed Runtime transport contract
Phase 3D — Samples + Developer Sample App
Phase 3E — Simulator/Emulator/Integration Lab
```

نام دقیق فازهای بعدی باید با Master Plan هماهنگ بماند و در زمان شروع implementation ثبت شود.
