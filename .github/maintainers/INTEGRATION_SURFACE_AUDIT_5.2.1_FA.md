# ممیزی کامل سطح Integration در Sadr Scales 5.2.1

**وضعیت:** خروجی Phase 1 — مبنای طراحی Contract بعدی  
**تاریخ ممیزی:** 2026-08-18  
**Sadr Scales baseline:** `5.2.1`  
**Sadr Scales source commit:** `1048749f52faba35e69464b64983e772c1c857e3`  
**Integration baseline before redesign:** `7f2a190679b4384616e26e23c2174e8ac83ef759`

---

## 1. هدف

این سند فهرست قابلیت‌های واقعی Sadr Scales 5.2.1 را از دید اتصال نرم‌افزارهای بیرونی ثبت می‌کند و بین سه نوع سطح تفاوت می‌گذارد:

1. **Safe Data Contract** — داده‌ای که می‌توان برای مصرف‌کننده بیرونی Contract روشن و امن تعریف کرد؛
2. **Managed Runtime Command** — عملیاتی که باید از Runtime خود Sadr Scales عبور کند و Direct SQL برای آن کافی یا امن نیست؛
3. **Internal / Do Not Expose** — جزئیات پیاده‌سازی، State داخلی، Migration، Recovery یا داده‌های پروتکلی که نباید API عمومی شوند.

این سند **Contract جدید نیست**. نتیجه Audit است. هر API، SQL Contract یا Transport جدید در Phase 2 طراحی می‌شود.

---

## 2. منابع حقیقت

ممیزی بر مبنای Commit نهایی 5.2.1 انجام شده است، نه فایل SQL قدیمی یا برداشت از UI.

منابع اصلی:

- `SadrSQLConnection/Core/SadrSqlConnection.cs`
- `SadrSQLConnection/Reporting/SalesReporting.cs`
- `SadrSQLConnection/Models/HotKeyPosition.cs`
- `SadrSQLConnection/Services/UiDataReadService.cs`
- `SadrScales.Runtime/Registry/ScaleRegistry.cs`
- `SadrScales.Runtime/Registry/ScaleRuntimeState.cs`
- `SadrScales.Runtime/Services/ScaleManagementService.cs`
- `SadrScales.Runtime/Services/ScaleOperationApplicationService.cs`
- `SadrScales.Runtime/Services/DatabaseLifecycleService.cs`
- `SadrScales/SadrScales/UI/Main/MainForm.cs`
- راهنمای رسمی Integration Database نسخه 5.2.1 برای تطبیق مفاهیم عمومی.

### قاعده مهم Schema

Schema مؤثر، نتیجه اجرای migration/check خود Sadr Scales از طریق `CheckTables()` و `EnsureDatabaseSchema()` است. فایل اولیه `DB.sql` به تنهایی Contract نهایی محسوب نمی‌شود.

---

## 3. ماتریس قابلیت‌ها

| قابلیت | مفهوم تجاری | منبع حقیقت | Read | Write | نیاز Runtime | دسته‌بندی Audit | API پیشنهادی در Phase 2 | Sample/Lab |
|---|---|---|---:|---:|---:|---|---|---:|
| بررسی اتصال DB | سالم بودن Connection | SQL Server | بله | خیر | خیر | Safe Data | `Connection.ValidateAsync` | بله |
| بررسی Schema | سازگاری DB با Sadr Scales | Effective migrated schema | بله | خیر | خیر | Safe Data | `Connection.ValidateAsync` | بله |
| شعبه | Store/Branch | `SADR_Store` | بله | بله | خیر | Safe Data | `Stores` | بله |
| گروه کالا | Item group | `SADR_ItemClass` | بله | بله | خیر | Safe Data | `ItemGroups` | بله |
| کالا/PLU | Business item | `SADR_Item` | بله | بله | خیر | Safe Data | `Items` | بله |
| حذف کالا | حذف منطقی کالا | `SADR_Item.DeleteFlag` | بله | بله | خیر | Safe Data با semantic مشخص | `Items.DeleteAsync` | بله |
| تاریخچه قیمت | Audit قیمت | `SADR_PriceLog` | قابل ارائه | خیر برای مصرف‌کننده | خیر | Safe Read / Internal Write | `Items.PriceHistory` یا `Reports` | بله |
| فهرست ترازوها | تنظیمات ثبت‌شده | `SADR_Scale` | بله | — | خیر برای Read | Safe Data Read | `Scales.Get...` | بله |
| افزودن ترازو | ثبت + آماده‌سازی Runtime | ScaleManagementService | بله | بله | **بله** | Managed Runtime | `Scales.AddAsync` | بله |
| ویرایش ترازو | تغییر تنظیمات ثبت‌شده + Runtime | ScaleManagementService | بله | بله | **بله** | Managed Runtime | `Scales.UpdateAsync` | بله |
| حذف ترازو | Disconnect + drain + cleanup + DB | ScaleManagementService | — | بله | **بله** | Managed Runtime | `Scales.DeleteAsync` | بله |
| وضعیت Live ترازو | Connected/Connecting/Faulted و activity | `ScaleRegistry` + `ScaleRuntimeState` | بله | خیر | **بله** | Managed Runtime Read | `Scales.GetStatusAsync` | بله |
| Hardware info | MAC/Version/model discovered | Runtime/registry | بله | داخلی | بله | Read candidate / Internal Write | داخل `ScaleInfo` | بله |
| چند گروه برای ترازو | Item-class assignment | `SADR_ScaleItemClass` | بله | بله | اثر روی send state | Managed configuration | `ScaleAssignments.Groups` | بله |
| نگاشت کالا به ترازو | PluNo/ItemCode/Page/Key | `SADR_ScaleItemMap` | بله | بله | اثر روی send state/layout | Managed configuration | `ScaleAssignments.Items` | بله |
| کپی نگاشت | انتقال mapping بین دو ترازو | DB + validation | — | بله | اثر روی send state/layout | Managed configuration | `ScaleAssignments.CopyAsync` | بله |
| HotKey position | PageNo/KeyNo | `SADR_KeyAssignment` + layout | بله | بله | اثر روی send state | Managed configuration | `HotKeys` | بله |
| ارسال کالا به ترازو | Device command | Runtime coordinators | — | device | **بله** | Managed Runtime Command | `Commands.SendItemsAsync` | بله |
| دریافت کالا از ترازو | Device command | Runtime coordinators | device | — | **بله** | Managed Runtime Command | `Commands.GetItemsAsync` | بله |
| ارسال HotKey | Device command | Runtime coordinators | — | device | **بله** | Managed Runtime Command | `Commands.SendHotKeysAsync` | بله |
| دریافت HotKey | Device command | Runtime coordinators | device | — | **بله** | Managed Runtime Command | `Commands.GetHotKeysAsync` | بله |
| حذف همه PLU دستگاه | Device command، مدل‌محور | Runtime | — | device | **بله** | Managed Runtime Command | `Commands.DeleteAllItemsAsync` | بله |
| دریافت فروش از ترازو | Device command | Runtime sales pipeline | device | DB | **بله** | Managed Runtime Command | `Commands.GetSalesAsync` | بله |
| Sales feed | جریان افزایشی فروش | `SADR_Logs` | بله | خیر | خیر | Safe Data | `Sales.ReadAfterAsync` | بله |
| جست‌وجوی فروش | تاریخ/ترازو/PLU/FID | `SADR_Logs` | بله | خیر | خیر | Safe Data | `Sales.QueryAsync` | بله |
| خلاصه فروش | تعداد رکورد/فاکتور/مبلغ/وزن/تعداد | `SADR_Logs` | بله | خیر | خیر | Safe Data | `Sales.GetSummaryAsync` | بله |
| گزارش روزانه | Daily sales report | Reporting layer | بله | خیر | خیر | Safe Data | `Reports.Sales.DailyAsync` | بله |
| گزارش ترازو | Scale sales report | Reporting layer | بله | خیر | خیر | Safe Data | `Reports.Sales.ByScaleAsync` | بله |
| گزارش کالا | Item sales report | Reporting layer | بله | خیر | خیر | Safe Data | `Reports.Sales.ByItemAsync` | بله |
| فاکتور ساخت‌یافته | Header + Detail + feed | `SADR_Total` + `SADR_Detail` + `SADR_Logs` | بله | داخلی هنگام دریافت | خیر برای lookup | Safe Data Read | `Invoices` | بله |
| lookup با TotalBarcode | دریافت فاکتور کامل | Structured sales tables | بله | خیر | خیر | Safe Data | `Invoices.GetByBarcodeAsync` | بله |
| lookup با ScaleID + FID | هویت منطقی فاکتور | Structured sales tables | بله | خیر | خیر | Safe Data | `Invoices.GetAsync(scaleId,fid)` | بله |
| Ack/processing فاکتور | وضعیت مصرف مقصد | `LableStatus` / `ItemStatus` | بله | نیاز Contract دقیق | خیر | **نیازمند تصمیم Phase 2** | `Invoices.AcknowledgeAsync` فقط پس از تعریف semantics | بله |
| ارسال Specification | Device command | Runtime | — | device | بله | Managed Runtime Command | `Commands.SendSpecificationAsync` | بله |
| دریافت Specification | Device command | Runtime | device | — | بله | Managed Runtime Command | `Commands.GetSpecificationAsync` | بله |
| فروشنده | تنظیم فروشنده دستگاه | Runtime/model capability | — | device | بله | Managed Runtime Command | `Commands.Salesmen...` | بله |
| Text | ارسال/دریافت متن | Runtime | دوطرفه | دوطرفه | بله | Managed Runtime Command | `Commands.Text...` | بله |
| Date/Time | تنظیم زمان دستگاه | Runtime | — | device | بله | Managed Runtime Command | `Commands.SetDateTimeAsync` | بله |
| Print format | تنظیم فرمت چاپ | Runtime/model capability | — | device | بله | Managed Runtime Command | `Commands.PrintFormat...` | بله |
| Barcode format | تنظیم فرمت بارکد | Runtime/model capability | — | device | بله | Managed Runtime Command | `Commands.BarcodeFormat...` | بله |
| Paper type | تنظیم کاغذ | Runtime/model capability | — | device | بله | Managed Runtime Command | `Commands.PaperType...` | بله |
| File/Firmware/Label transfer | انتقال فایل مدل‌محور | Runtime | دوطرفه/ارسال | device | بله | **Managed but security review required** | تصمیم Phase 2 | فقط Lab کنترل‌شده |

---

## 4. یافته‌های قطعی مهم

### 4.1 وجود جدول به معنی مجاز بودن Direct SQL نیست

`SADR_Scale` بهترین مثال است. مسیر واقعی افزودن ترازو فقط INSERT نیست. Runtime این مراحل را انجام می‌دهد:

```text
Validate request
→ License authorization
→ Duplicate registry check
→ Cleanup detached operational state
→ Database insert
→ Reload registry
→ Start connection check
```

پس `INSERT INTO SADR_Scale` نباید API عمومی توصیه‌شده برای افزودن ترازو باشد.

همین اصل درباره حذف/ویرایش ترازو و هر عملیاتی که روی Runtime فعال اثر دارد برقرار است.

### 4.2 وضعیت واقعی Online در Runtime است

`SADR_Scale.Status` می‌تواند یک وضعیت ثبت‌شده/نمایشی باشد، اما حقیقت Live برای API آینده باید از `ScaleRegistry` و `ScaleRuntimeState` گرفته شود.

Runtime علاوه بر اتصال این اطلاعات را دارد:

- `Disconnected / Connecting / Connected / Disconnecting / Faulted`
- Current activity
- Last activity result
- progress current/total/percent/detail
- last connected/disconnected/activity/error timestamps
- last error message

پس `Scales.GetStatusAsync()` باید Managed Runtime Read باشد.

### 4.3 حذف کالا Semantic است، نه Physical Delete

مسیر معمول حذف کالا در 5.2.1 با `DeleteFlag = 1` انجام می‌شود. API عمومی آینده نباید به مصرف‌کننده `DELETE FROM SADR_Item` را آموزش دهد.

### 4.4 Item delivery state داخلی است

`SADR_ItemSyncState` وضعیت تحویل/ارسال Semantic هر کالا به هر ترازو را نگهداری می‌کند. این جدول Business entity شرکت ثالث نیست.

- Direct write عمومی: ممنوع؛
- raw table API: ممنوع؛
- در آینده می‌توان فقط یک **derived read model** برای نمایش sync/delivery status طراحی کرد، اگر نیاز واقعی وجود داشته باشد.

### 4.5 HotKey دو مفهوم جدا دارد

1. **Configuration/assignment** — چه کالا در چه Page/Key قرار دارد؛
2. **Device send/retrieve** — واقعاً ارسال/دریافت تنظیمات کلید به/از ترازو.

اولی داده پیکربندی کنترل‌شده است؛ دومی Runtime command است. این دو نباید در API قاطی شوند.

### 4.6 فروش دو مسیر متفاوت دارد

#### Sales Feed
برای Sync پیوسته مقصد:

```text
SADR_Logs
ID > destination cursor
```

#### Structured Invoice Lookup
برای گرفتن کل فاکتور:

```text
SADR_Total  ← header
SADR_Detail ← lines
SADR_Logs   ← sale feed/detail evidence
```

هویت منطقی فاکتور:

```text
ScaleID / DeviceNo + FID
```

بارکد تجمیعی تولیدشده توسط 5.2.1:

```text
"25" + ScaleID(D3) + FID(D9)
```

Integration آینده باید هر دو مدل را درجه‌یک پشتیبانی کند.

### 4.7 Structured invoice در سه جدول Atomic ذخیره می‌شود

Runtime برای فاکتورهای LSG/PLUS کامل بودن داده را بین `SADR_Total`، `SADR_Logs` و `SADR_Detail` کنترل می‌کند و منطق duplicate/repair دارد.

مصرف‌کننده بیرونی نباید این repair logic یا deleteهای داخلی را تقلید کند.

### 4.8 PriceLog نتیجه عملیات کالا است

`SADR_PriceLog` برای ثبت تغییر قیمت وجود دارد و Core امکان Insert داخلی و عملیات نگهداری دارد.

برای Integration:

- Read history می‌تواند عمومی شود؛
- برنامه ثالث نباید آزادانه PriceLog جعلی Insert/Delete کند؛
- ثبت History باید پیامد عملیات رسمی تغییر Item باشد.

### 4.9 Reporting از ابتدا جزو قابلیت واقعی محصول است

Core 5.2.1 سه گزارش رسمی دارد:

- Daily
- Scale
- Item

و Filterهای تاریخ، ترازو، PLU، FID و Identify را پشتیبانی می‌کند. بنابراین Reports نباید در redesign یک Feature فرعی فرض شود.

---

## 5. سطوح Internal / Do Not Expose

موارد زیر نباید به عنوان API عمومی یا Raw SQL recipe عادی منتشر شوند:

- `SADR_ItemSyncState` write؛
- `LastSendItem` / `LastSendKey` و سایر send cursors داخلی؛
- hardware-info write از سمت شرکت ثالث؛
- protocol-specific PLU/data projection برای LSG/PLUS/Aclas/Vista؛
- raw protocol packet/frame/opcode؛
- invoice repair/delete-incomplete internals؛
- `DeleteTables` / `DeleteSalesTables`؛
- schema creation/migration internals؛
- `SADR_ViewPlu` / `SADR_ViewKey` به عنوان Contract سطح بالا؛
- manual creation/deletion of PriceLog history؛
- runtime registry mutation از بیرون؛
- private device protocol implementation؛
- fault-injection/protocol-debug internals مربوط به simulator مهندسی.

---

## 6. Gapها و نقاطی که قبل از Contract باید حل شوند

### GAP-01 — StoreCode در Scale management

Schema و read model ترازو `StoreCode` را دارند و `SADR_Store` نیز موجود است، اما `ScaleCreateRequest` در 5.2.1 `StoreCode` ندارد و `ScaleUpdateRequest` فقط `StoreName` را حمل می‌کند.

**اقدام Phase 2/Core review:** مشخص شود رابطه رسمی Scale ↔ Store باید بر اساس `StoreCode` چگونه باشد و آیا اصلاح Core لازم است.

### GAP-02 — Dual group representation

`SADR_Scale.ItemClassCode` در کنار `SADR_ScaleItemClass` وجود دارد. 5.2.1 قابلیت چندگروهی دارد، ولی legacy single group نیز در مدل ترازو باقی مانده است.

**اقدام:** Contract عمومی نباید هر دو را مستقل در اختیار مصرف‌کننده بگذارد؛ یک مدل دامنه واحد لازم است.

### GAP-03 — Scale item map در برابر HotKey assignment

`SADR_ScaleItemMap` شامل `PluNo/ItemCode/PageNo/KeyNo` است و `SADR_KeyAssignment` نیز وضعیت assignment کلید را نگهداری می‌کند.

**اقدام:** Phase 2 باید ownership و precedence این دو ساختار را در API سطح بالا روشن کند تا شرکت ثالث مجبور به فهم دو representation داخلی نشود.

### GAP-04 — Invoice Ack semantics

`LableStatus` و `ItemStatus` وجود دارند، اما قبل از ایجاد `AcknowledgeAsync` باید دقیقاً مشخص شود:

- owner این state چه نرم‌افزاری است؛
- Ack در چه نقطه‌ای مجاز است؛
- header-only یا header+items؛
- retry/idempotency؛
- رفتار چند مصرف‌کننده؛
- رابطه آن با cursor مستقل `SADR_Logs`.

تا این تصمیم بسته نشود، write این statusها Public API محسوب نمی‌شود.

### GAP-05 — Runtime command channel عمومی هنوز وجود ندارد

Core عملیات کامل دستگاه را دارد، اما SDK عمومی v1 فقط SQL data path را دارد.

**اقدام:** در Phase 2 یک Managed Runtime Boundary طراحی شود؛ Direct device protocol یا Direct SQL جایگزین آن نیست.

### GAP-06 — Live status با DB status نباید قاطی شود

API آینده باید Static Scale Configuration و Live Runtime State را جدا مدل کند.

### GAP-07 — Current v1 SDK coverage ناقص است

v1.0.0 مطابق Contract خودش صحیح است ولی فقط بخش کوچکی از Surface واقعی را پوشش می‌دهد. این Release تغییرناپذیر می‌ماند و redesign روی نسخه آینده انجام می‌شود.

---

## 7. Public Domain پیشنهادی برای Phase 2

این فقط خروجی Audit برای طراحی بعدی است، نه API نهایی:

```text
Connection
Stores
ItemGroups
Items
Scales
ScaleAssignments
HotKeys
Sales
Invoices
Reports
Commands
```

### اصل مهم

Domain بالا باید مستقل از Transport باشد:

```text
Business Domain
   ├── SQL adapter
   ├── future REST/API adapter
   ├── future XML adapter
   └── future Webhook/event adapter
```

SQL یک روش اتصال است، نه تعریف خود محصول.

---

## 8. پوشش Sample App مورد نیاز بر اساس Audit

Sample App آینده باید حداقل این مسیرها را قابل اجرا کند:

```text
Connection
Scales
Stores
Groups
Items
Assignments
Hot Keys
Sales Feed
Invoice Lookup
Reports
Commands
Demo Data
```

و Demo Data Generator باید Random و Seeded باشد تا بتوان سناریوهای زیر را بدون ورود دستی ساخت:

- چند شعبه؛
- چند گروه؛
- تعداد زیاد کالا؛
- چند ترازوی فرضی؛
- assignment/mapping؛
- hotkey layout؛
- داده‌های فروش آزمایشی در محیط Lab.

---

## 9. تست مورد نیاز بر اساس دسته‌بندی

### Safe Data Contract

```text
Unit
→ SQL Integration
→ SDK consumer
→ Sample App
```

### Managed Runtime Command

```text
Unit
→ Runtime integration
→ Internal protocol simulator
→ Public emulator/Lab
→ final real hardware cross-check
```

### Internal

فقط تست داخلی؛ هیچ Sample عمومی که implementation داخلی را لو بدهد ایجاد نمی‌شود.

---

## 10. نتیجه Phase 1

Audit نشان داد Sadr Scales 5.2.1 عملاً سطح Integration بسیار بزرگ‌تری از Contract عمومی v1 دارد.

مهم‌ترین تصمیم معماری برای ادامه:

> **Public Integration Platform نباید wrapper مستقیم جدول‌ها باشد. Data operations امن از Runtime commands جدا می‌شوند و API بر اساس مفهوم تجاری طراحی می‌شود.**

Phase 1 هیچ API یا Contract موجودی را تغییر نمی‌دهد.

---

## 11. Gate ورود به Phase 2

قبل از Implementation، در Phase 2 باید این موارد به ترتیب بسته شوند:

1. تصمیم StoreCode و رابطه Scale ↔ Store؛
2. مدل واحد Scale Groups؛
3. مدل واحد Assignments/HotKeys؛
4. Invoice lookup + Ack semantics؛
5. Static Scale vs Live Scale State؛
6. Managed Runtime Command boundary؛
7. Transport-independent domain contracts؛
8. Compatibility analysis و سپس تعیین شماره نسخه بعدی.

تا این Gateها بسته نشده‌اند، اضافه‌کردن Clientهای جدید به SDK ممنوع است.
