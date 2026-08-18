# برنامه جامع Sadr Scales Integration Platform

**وضعیت:** مرجع قطعی طراحی و مسیر کار

**تاریخ پذیرش:** 2026-08-18

**دامنه:** SadrScales-Integration و محیط‌های تست/نمونه مرتبط با آن

---

## 1. هدف سند

این فایل مرجع اصلی تصمیم‌ها و مسیر توسعه پروژه Integration است. تاریخچه گفتگو، حافظه چت یا توضیح شفاهی منبع حقیقت نیست.

اگر بین این سند و برداشت قبلی از پروژه اختلافی وجود داشت، تا زمان ثبت تصمیم جدید، این سند مبنا است.

هر تغییر معماری یا محصولی مهم باید ابتدا در همین سند یا یک تصمیم رسمی مرتبط ثبت شود.

---

## 2. تعریف محصول

SadrScales-Integration نباید صرفاً یک SQL SDK یا مجموعه‌ای از Queryها باشد.

هدف، ساخت یک **Integration Platform ساده، کامل، قابل یادگیری و قابل تست** برای اتصال نرم‌افزارهای POS، ERP، حسابداری و سایر نرم‌افزارهای تجاری به Sadr Scales است.

برنامه‌نویس شرکت ثالث باید بتواند بدون آشنایی با ساختار داخلی Sadr Scales بفهمد:

1. چطور متصل شود؛
2. چه قابلیت‌هایی در اختیار دارد؛
3. چطور شعبه، ترازو، گروه، کالا، نگاشت و سایر داده‌های عمومی را مدیریت کند؛
4. چطور وضعیت‌ها و فروش را بخواند؛
5. چطور فاکتور کامل را با بارکد تجمیعی یا شناسه منطقی دریافت کند؛
6. چطور عملیات پشتیبانی‌شده روی ترازو را درخواست کند؛
7. چطور کد واقعی نمونه را اجرا، تغییر و تست کند؛
8. چطور بدون داشتن دستگاه فیزیکی یک محیط کامل آزمایشی بسازد.

---

## 3. اولویت‌های محصول

ترتیب اولویت‌ها قطعی است:

1. **کامل بودن قابلیت‌ها**
2. **سادگی استفاده و سادگی Repository**
3. **راهنمای کامل، کوتاه، روشن و مرحله‌به‌مرحله**
4. **نمونه کد واقعی و قابل اجرا**
5. **قابلیت تست آسان و تکرارپذیر**
6. **امنیت و حفظ مرزهای مالکیت فکری پروتکل دستگاه‌ها**

هیچ طراحی‌ای صرفاً به دلیل تمیز بودن مهندسی پذیرفته نیست اگر برنامه‌نویس بیرونی برای استفاده از آن سردرگم شود.

---

## 4. اصل تجربه برنامه‌نویس

یک توسعه‌دهنده جدید باید از صفحه اول Repository در چند دقیقه بفهمد:

```text
START HERE
   ↓
What can I do?
   ↓
Choose integration method
   ↓
Run Quick Start
   ↓
Open Sample App
   ↓
Use Reference only when needed
```

مستندات نگهداری پروژه، تصمیم‌های داخلی، Backlog، Work Log، Release Engineering و موارد مشابه نباید در مسیر عادی مصرف‌کننده قرار بگیرند.

Repo عمومی باید از دید مصرف‌کننده ساده و کم‌تعداد باشد؛ مستندات نگهدارنده‌ها در مسیر مشخص Maintainers نگهداری می‌شوند.

---

## 5. مدل مفهومی مستقل از روش اتصال

مفاهیم اصلی Integration نباید به SQL قفل شوند.

مدل مفهومی مورد انتظار:

```text
Store / Branch
Scale
Scale Status
Item Group
Item / PLU
Scale Assignment
Hot Key / Key Mapping
Sales Feed
Structured Invoice
Invoice Detail
Reports
Device Commands
```

SQL فقط یکی از روش‌های اتصال است.

معماری آینده باید امکان این مسیرها را بدون تغییر مفهوم اصلی فراهم کند:

```text
Integration Domain
       │
 ┌─────┼───────────────┐
 ↓     ↓               ↓
SQL   REST/API       XML / Webhook / other
```

قابلیت‌های جدید فقط زمانی به روش دیگری اضافه می‌شوند که Contract و رفتارشان روشن و قابل تست باشد.

---

## 6. Audit اجباری قابلیت‌ها قبل از بازطراحی

قبل از توسعه API جدید، باید سطح کامل Integration در Sadr Scales 5.2.1 از روی سورس نهایی، Schema واقعی و مستندات رسمی Audit شود.

هر قابلیت باید در یکی از سه دسته قرار بگیرد:

### A. Safe Data Contract

عملیاتی که می‌توانند به شکل ایمن از طریق Contract داده عمومی شوند، مانند Read/Write کنترل‌شده Store، Group، Item، Sales و سایر داده‌های مناسب.

### B. Managed Runtime Command

عملیاتی که صرف نوشتن مستقیم SQL برای آن‌ها کافی یا ایمن نیست و باید از مسیر Runtime مدیریت شوند، مانند عملیات Online ترازو، ارسال/دریافت داده دستگاه، reconnect، queue، مدل‌محوری و عملیات وابسته به وضعیت واقعی ترازو.

### C. Internal / Do Not Expose

رفتارها و جزئیاتی که عمومی شدن آن‌ها لازم یا مجاز نیست، مانند پیاده‌سازی پروتکل خصوصی، Packet format، captures، کلیدهای خصوصی و ابزارهای داخلی مهندسی.

تا پایان Audit، هیچ قابلیت جدیدی صرفاً بر اساس حدس یا نام جدول عمومی نمی‌شود.

---

## 7. پوشش مورد انتظار Integration

Audit باید حداقل این حوزه‌ها را بررسی و نتیجه دقیق هرکدام را ثبت کند:

- اتصال و Contract validation؛
- Store / Branch؛
- Scale registry و مشخصات ترازو؛
- Scale status و وضعیت اتصال؛
- افزودن، ویرایش، فعال/غیرفعال و حذف ترازو از مسیر مناسب؛
- Item Group؛
- Item / PLU؛
- Batch item operations؛
- چند گروه برای یک ترازو؛
- اختصاص کالا به ترازو؛
- Scale item mapping؛
- Hot key / key assignment؛
- فروش افزایشی و Cursor؛
- جست‌وجو و فیلتر فروش؛
- Summary و Reports؛
- Structured Invoice؛
- SADR_Total / SADR_Detail / SADR_Logs به عنوان ساختار مرتبط فاکتور؛
- دریافت فاکتور با TotalBarcode؛
- دریافت فاکتور با ScaleID + FID؛
- جلوگیری از Duplicate و رفتار Idempotent؛
- Ack/State فقط در صورتی که Contract واقعی آن را پشتیبانی کند؛
- عملیات ارسال کالا؛
- عملیات دریافت کالا؛
- ارسال/دریافت HotKey؛
- ارسال/دریافت تنظیمات؛
- دریافت فروش از دستگاه؛
- تاریخ/زمان؛
- فروشنده؛
- متن؛
- Print Format؛
- Barcode Format؛
- Paper Type؛
- فایل/firmware/label operations و تصمیم جداگانه درباره عمومی شدن آن‌ها؛
- هر قابلیت دیگری که در Runtime نهایی Sadr Scales وجود دارد و برای شرکت ثالث کاربرد عمومی دارد.

ماتریس Audit باید برای هر ردیف حداقل این ستون‌ها را داشته باشد:

```text
Capability
Business meaning
Source of truth
Read
Write
Runtime required?
Security/safety rule
Public API candidate
SQL sample needed?
SDK sample needed?
Sample App screen needed?
Simulator/Lab test needed?
```

---

## 8. طراحی Public API

API باید بر اساس مفهوم تجاری خوانا باشد، نه نام جدول‌های داخلی.

جهت طراحی مطلوب:

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

مصرف‌کننده SDK نباید برای عملیات عادی مجبور باشد نام جدول‌های داخلی یا جزئیات Registry را بداند.

Raw SQL برای زبان‌ها و محیط‌هایی که SDK مستقیم ندارند می‌تواند مستند شود، اما باید Contract روشن و محدود داشته باشد.

---

## 9. قواعد کدنویسی

این قواعد برای تمام کدهای جدید Integration اجباری‌اند:

### 9.1 خوانایی

- کلاس‌ها تک‌مسئولیتی و کوتاه بمانند؛
- متدها نام روشن و رفتار قابل پیش‌بینی داشته باشند؛
- از abstraction غیرضروری خودداری شود؛
- استفاده از SDK برای برنامه‌نویس بیرونی از ساختار داخلی ساده‌تر باشد، نه پیچیده‌تر.

### 9.2 Commenting

- همه APIهای public باید XML documentation روشن داشته باشند؛
- کلاس‌های مهم، مدل‌ها، Contractها و منطق غیر بدیهی باید توضیح شفاف داشته باشند؛
- Comment باید دلیل، Contract، محدودیت یا خطر را توضیح دهد؛
- Comment نباید فقط همان کد را با کلمات دیگر تکرار کند.

### 9.3 Regions

فایل‌های C# باید در صورت وجود چند بخش منطقی با Regionهای روشن تقسیم شوند، برای نمونه:

```text
#region Construction
#region Public API
#region Validation
#region SQL Commands
#region Mapping
#region Helpers
```

Region جایگزین شکستن کلاس بیش‌ازحد بزرگ نیست.

### 9.4 جلوگیری از شلوغی

اگر یک کلاس، فایل یا پوشه شروع به انجام چند مسئولیت کرد، قبل از ادامه باید refactor شود.

---

## 10. Definition of Done برای هر قابلیت

هیچ Feature جدیدی صرفاً با «کد کار می‌کند» کامل محسوب نمی‌شود.

هر قابلیت عمومی باید در صورت مرتبط بودن این خروجی‌ها را داشته باشد:

1. Contract / behavior definition؛
2. SDK/API؛
3. Unit tests؛
4. Integration tests؛
5. Documentation؛
6. Code sample؛
7. Executable Sample coverage؛
8. Failure/recovery test؛
9. Security/safety review؛
10. Lab/Simulator scenario در صورت وابستگی به Runtime/Device.

---

## 11. Sample App رسمی توسعه‌دهنده

یک Sample App واقعی و ساده باید بخشی از محصول باشد.

هدف آن فقط Demo ظاهری نیست؛ باید **کد مرجع قابل کپی و یادگیری** باشد و فقط از API عمومی رسمی استفاده کند.

پیشنهاد اولیه صفحه‌ها:

```text
Connection
Scales
Stores
Groups
Items
Assignments
Hot Keys
Sales
Invoices
Reports
Commands
Demo Data
```

هر صفحه باید:

- ورودی حداقلی و روشن داشته باشد؛
- نتیجه را در Grid یا نمایش مناسب نشان دهد؛
- خطا را قابل فهم نمایش دهد؛
- کد پشت عملیات کوتاه و آموزشی باشد؛
- رفتار واقعی SDK را نشان دهد.

---

## 12. Demo Data Generator

Sample App باید امکان ایجاد داده آزمایشی آماده داشته باشد تا توسعه‌دهنده بدون ورود دستی ده‌ها رکورد رفتار سیستم را ببیند.

قابلیت‌های مورد انتظار:

```text
Generate Demo Stores
Generate Demo Groups
Generate Demo Items
Generate Demo Scales
Generate Demo Assignments
Generate Demo Hot Keys
Reset Demo Data
```

دو حالت لازم است:

### Random Demo

برای دیدن سریع رفتار و تنوع داده.

### Seeded Demo

برای تست تکرارپذیر، مثال:

```text
Seed = 12345
```

Seed ثابت باید همیشه همان مجموعه داده را بسازد تا Debug و Regression قابل تکرار باشند.

Demo generation نباید بدون کنترل ایمنی روی Database واقعی مشتری اجرا شود. محیط Demo/Test باید به روش قابل تشخیص محافظت شود.

---

## 13. استراتژی تست

پروژه باید بدون وابستگی دائمی به سخت‌افزار واقعی قابل تست باشد.

سطوح تست:

```text
Unit Tests
    ↓
SQL Integration Tests
    ↓
SDK Consumer Tests
    ↓
Sample App Tests
    ↓
Runtime Integration Tests
    ↓
Virtual Device End-to-End Tests
    ↓
Final Real Hardware Cross-check
```

سناریوهای Failure، Retry، Duplicate، Restart، Cursor persistence، Transaction rollback و Recovery باید قابل بازتولید باشند.

---

## 14. Internal Protocol Simulator در برابر Public Scale Emulator

این دو محصول یکی نیستند و نباید با هم مخلوط شوند.

### 14.1 Internal Protocol Simulator

Simulator فعلی ابزار مهندسی داخلی است و برای انتشار عمومی مناسب نیست.

نسخه داخلی باید ابتدا کامل شود و همه رفتارهای ارتباطی قابل اثبات LSG و PLUS را بر اساس مستندات پروتکل، Traceهای معتبر و تست سخت‌افزار واقعی پیاده‌سازی کند.

ابزارهای داخلی می‌توانند شامل موارد زیر باشند:

```text
Raw frame viewer
Protocol console
Manual packet operations
Fault injection
Malformed frames
Delayed/lost ACK
Disconnect/reconnect tests
Slow network
Stress test
Replay
Verbose protocol logs
Timing tests
```

این قابلیت‌ها برای مهندسی و تثبیت ارتباط هستند، نه برای مصرف‌کننده بیرونی.

### 14.2 ترتیب تکمیل Simulator داخلی

```text
LSG protocol completeness
        ↓
LSG simulator validation
        ↓
PLUS protocol completeness
        ↓
PLUS simulator validation
        ↓
Real hardware cross-check
        ↓
Freeze validated device behavior
```

فقط پس از این مرحله می‌توان Public Emulator را استخراج کرد.

### 14.3 Public Scale Emulator

Public Emulator باید شبیه **یک ترازوی واقعی مجازی** رفتار کند، نه ابزار Debug پروتکل.

قابلیت‌های عمومی نمونه:

```text
Select model
Scale number
Port / endpoint
Start / Stop
Connection state
Received items
Received hot keys
Visible settings
Create sale
Send sale
View stored sales
Natural reconnect
Reset virtual scale
```

موارد زیر نباید در نسخه عمومی در دسترس باشند:

```text
Raw packets
Protocol opcode details
Packet capture
Malformed-frame tools
ACK manipulation
Protocol test vectors
Reverse-engineering notes
Private timing/protocol diagnostics
```

---

## 15. مرز امنیتی Public Emulator

کد واقعی Protocol Engine نباید در Repository عمومی قرار بگیرد.

مدل مطلوب:

```text
Private source repository
        ↓
Validated protocol engine
        ↓
Protected / obfuscated signed binary
        ↓
Public Emulator distribution
```

Public Repository می‌تواند UI، Sampleها، Documentation و Integration code عمومی را داشته باشد، اما پیاده‌سازی پروتکل مستقیم دستگاه خصوصی می‌ماند.

Obfuscation جایگزین خصوصی ماندن Source نیست.

---

## 16. هدف نهایی Integration Lab

هدف بلندمدت این است که یک تیم نرم‌افزاری بتواند بدون ترازوی فیزیکی کل مسیر را روی یک سیستم اجرا کند:

```text
POS Simulator / Developer Sample
             ↓
SadrScales Integration
             ↓
Sadr Scales
             ↓
Protected Sadr Scale Emulator
```

محیط مطلوب:

```text
ONE PC
NO PHYSICAL SCALE
NO CUSTOMER DATABASE
NO SPECIAL HARDWARE
```

و در عین حال جریان واقعی زیر قابل مشاهده باشد:

```text
Demo Item
   ↓
Integration
   ↓
Sadr Scales
   ↓
Virtual Scale

Virtual Sale
   ↓
Sadr Scales
   ↓
Structured Sales Database
   ↓
Integration
   ↓
POS Sample / Simulator
```

---

## 17. نقش Sample App و POS Simulator

### Sample App

- Source عمومی و آموزشی؛
- نشان دادن روش صحیح استفاده از SDK؛
- مناسب برای Copy/Paste و یادگیری؛
- UI ساده با Grid و عملیات پایه؛
- بدون منطق مخفی یا shortcut داخلی.

### POS Simulator

- شبیه‌سازی رفتار نرم‌افزار فروشگاهی؛
- مناسب سناریوهای End-to-End و Failure؛
- امکان تست Scan TotalBarcode، دریافت Invoice، Commit، Restart، Duplicate و سایر رفتارهای مقصد؛
- مستقل از Sample App آموزشی.

---

## 18. مسیر آموزشی توسعه پروژه

این پروژه نباید با Vibe Coding توسعه داده شود.

برای هر بخش مهم، روند کار این است:

```text
Learn
  ↓
Understand the contract
  ↓
Predict behavior
  ↓
Design
  ↓
Implement
  ↓
Test
  ↓
Debug
  ↓
Review
  ↓
Teach-back / explain the design
```

قبل از تغییر مهم کد، مسئله و طراحی باید روشن باشد.

کد نوشته‌شده باید قابل توضیح Line-by-Line یا حداقل Responsibility-by-Responsibility باشد.

هدف فقط تولید خروجی نیست؛ مالک پروژه باید به معماری، Contract، تست و علت تصمیم‌ها مسلط باشد.

---

## 19. وضعیت Release فعلی

`v1.0.0` یک Release رسمی و تغییرناپذیر از Basic SQL Contract فعلی است.

Tag و دارایی‌های Release قبلی نباید جابه‌جا یا بازنویسی شوند.

بازطراحی کامل Integration Platform باید روی کار جدید انجام شود.

نسخه Release بعدی فقط پس از پایان Contract design تعیین می‌شود. تا قبل از آن تصمیمی درباره 1.1.0 یا 2.0.0 قطعی نیست.

---

## 20. ترتیب فازهای اجرایی

### Phase 0 — Canonical Plan

- [x] ثبت هدف، قوانین و مسیر اصلی در این سند.

### Phase 1 — Full Integration Surface Audit

- [ ] Audit کامل Sadr Scales 5.2.1؛
- [ ] ساخت ماتریس Capability؛
- [ ] دسته‌بندی Safe Data / Managed Runtime / Internal؛
- [ ] کشف Gapها و ناسازگاری‌های Core؛
- [ ] عدم تغییر Integration API تا پایان Audit.

### Phase 2 — Contract & Domain Design

- [ ] تعریف Domain مستقل از Transport؛
- [ ] تعیین Public API سطح بالا؛
- [ ] تعیین SQL Contract کامل و امن؛
- [ ] تعیین Managed Runtime Command boundary؛
- [ ] تعیین نسخه آینده پس از روشن شدن compatibility.

### Phase 3 — Repository Simplification

- [ ] طراحی START HERE؛
- [ ] کاهش فایل‌های جلوی چشم کاربر؛
- [ ] انتقال اسناد Maintainer؛
- [ ] ساخت مسیر روشن Getting Started / Capabilities / Recipes / Reference؛
- [ ] حذف Navigation گیج‌کننده و تکراری.

### Phase 4 — SDK Completion

- [ ] Store؛
- [ ] Group؛
- [ ] Item؛
- [ ] Scale؛
- [ ] Assignments / Mapping؛
- [ ] Hot Keys؛
- [ ] Sales؛
- [ ] Structured Invoices؛
- [ ] Reports؛
- [ ] Managed Commands؛
- [ ] تست و مستند برای هر حوزه.

### Phase 5 — Developer Sample App

- [ ] WinForms sample واقعی؛
- [ ] Grid-based examples؛
- [ ] Demo Data Generator؛
- [ ] Random + Seeded data؛
- [ ] Error/recovery examples؛
- [ ] فقط استفاده از Public API رسمی.

### Phase 6 — Internal Scale Simulator Completion

- [ ] تکمیل LSG بر اساس پروتکل معتبر؛
- [ ] cross-check با دستگاه واقعی؛
- [ ] تکمیل PLUS بر اساس پروتکل معتبر؛
- [ ] cross-check با دستگاه واقعی؛
- [ ] تثبیت state machine و رفتار ارتباطی؛
- [ ] حفظ ابزارهای stress/fault/protocol فقط در محیط داخلی.

### Phase 7 — Public Scale Emulator

- [ ] استخراج behavior تمیز از Simulator داخلی؛
- [ ] ساخت UI ساده شبیه ترازو؛
- [ ] حذف تمام ابزارهای protocol/debug داخلی؛
- [ ] Protection/obfuscation/signing engine؛
- [ ] Release binary بدون Source پروتکل.

### Phase 8 — POS Simulator & Integration Lab

- [ ] تکمیل POS Simulator برای رفتار مقصد؛
- [ ] اتصال Sample/POS → Integration → Sadr Scales → Emulator؛
- [ ] سناریوهای End-to-End؛
- [ ] سناریوهای restart/failure/duplicate/recovery؛
- [ ] راه‌اندازی کامل روی یک PC بدون Device.

### Phase 9 — External Developer Acceptance

- [ ] تست توسط فردی که پروژه را از قبل نمی‌شناسد؛
- [ ] اندازه‌گیری زمان First Successful Connection؛
- [ ] اندازه‌گیری زمان First Item Write؛
- [ ] اندازه‌گیری زمان First Invoice Read؛
- [ ] اصلاح نقاط سردرگم‌کننده؛
- [ ] Release نهایی نسل جدید Integration.

---

## 21. معیار پذیرش نهایی محصول

نسل جدید Integration فقط وقتی آماده ارائه رسمی است که یک توسعه‌دهنده بیرونی بتواند با راهنمای Repository و بدون توضیح شفاهی:

1. محیط را نصب/اجرا کند؛
2. Contract را Validate کند؛
3. شعبه، گروه، کالا و ترازو آزمایشی بسازد؛
4. داده را بخواند و تغییر دهد؛
5. وضعیت ترازو را ببیند؛
6. کالا را تا Virtual/Real Scale منتقل کند؛
7. فروش را تولید و دریافت کند؛
8. فاکتور کامل را با TotalBarcode بخواند؛
9. گزارش و Feed فروش را مصرف کند؛
10. Restart، Duplicate و Failureهای اصلی را تست کند؛
11. Sample code را بفهمد و در پروژه خودش استفاده کند؛
12. برای شروع توسعه به ترازوی فیزیکی وابسته نباشد.

---

## 22. مواردی که عمداً ممنوع یا خارج از Public Scope هستند

- انتشار Source پروتکل مستقیم LSG/PLUS/Aclas یا سایر دستگاه‌ها؛
- Packet capture یا reverse-engineering notes؛
- کلید خصوصی، signing secret، credential یا customer data؛
- تبدیل Public Emulator به protocol debugger؛
- وابسته کردن Domain عمومی به یک Transport خاص؛
- اضافه کردن قابلیت بدون تست، مستند و Sample؛
- افزودن فایل‌های متعدد مدیریتی به مسیر مصرف‌کننده؛
- تغییر یا جابه‌جایی Tag رسمی `v1.0.0`.

---

## 23. قانون ادامه کار

قدم بعدی بعد از پذیرش این سند:

> **Phase 1 — Full Integration Surface Audit**

تا زمانی که Audit کامل و بازبینی نشده است، بازطراحی Public API یا جابه‌جایی بزرگ ساختار Repository شروع نمی‌شود.

این ترتیب برای جلوگیری از دوباره‌کاری، ناقص ماندن قابلیت‌ها و Vibe Coding اجباری است.
