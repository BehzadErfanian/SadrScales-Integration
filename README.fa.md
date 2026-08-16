<div dir="rtl" align="right">

# یکپارچه‌سازی Sadr Scales

**مخزن عمومی رسمی برای قرارداد یکپارچه‌سازی، ابزار توسعه و نمونه‌های اتصال نرم‌افزارهای فروشگاهی، ERP و حسابداری به Sadr Scales.**

[English](README.md) · [شروع سریع](docs/fa/getting-started.md) · [قرارداد SQL v1](docs/fa/sql-contract-v1.md) · [نقشه‌راه](docs/ROADMAP.md) · [امنیت](SECURITY.md)

---

## وضعیت فعلی

این مخزن در مرحله **Foundation / پیش از نسخه 1.0** قرار دارد. قرارداد عمومی نسخه **Sadr Scales 5.2.1** با نام **SQL Contract v1** مشخص شده است. SDK و نمونه‌های چندزبان در مراحل بعدی همین نقشه‌راه ساخته می‌شوند.

| بخش | وضعیت |
|---|---|
| SQL Contract v1 | مشخص‌شده برای Sadr Scales 5.2.1 |
| راهنمای جامع فنی فارسی | موجود در `docs/reference/` |
| SDK زبان C# | برنامه‌ریزی‌شده برای v1.0 |
| نمونه SQL | برنامه‌ریزی‌شده |
| Python / Node.js / Java / PHP | برنامه‌ریزی‌شده |
| GitHub Releases | برنامه‌ریزی‌شده |
| REST / Webhook | نسل بعد؛ خارج از 5.2.1 و Contract v1 |

## هدف این Repository

نرم‌افزار فروشگاهی یا حسابداری باید با **Sadr Scales** یکپارچه شود و نباید برای هر مدل ترازو پروتکل اختصاصی ارتباط با دستگاه را دوباره پیاده‌سازی کند.

```text
POS / ERP / Accounting
        ↓
Sadr Scales SQL Contract v1 / Integration SDK
        ↓
Sadr Scales Runtime
        ↓
مدل‌های پشتیبانی‌شده ترازو
```

این قرارداد برای این کارهاست:

- ثبت و ویرایش گروه کالا و PLU؛
- خواندن فروش‌های پذیرفته‌شده توسط Sadr Scales؛
- استفاده کنترل‌شده از اطلاعات ساخت‌یافته فاکتور؛
- کنترل Schema مورد انتظار؛
- نگهداری Cursor/State فروش در نرم‌افزار مقصد؛
- یکپارچه‌سازی بدون نیاز به شناخت پروتکل مستقیم PLUS، LSG، Aclas و سایر دستگاه‌ها.

## خلاصه قرارداد عمومی v1

مسیر پایه روی سه Object است:

- `dbo.SADR_ItemClass` — گروه کالا، Read/Write؛
- `dbo.SADR_Item` — کالا/PLU، Read/Write؛
- `dbo.SADR_Logs` — Feed فروش پذیرفته‌شده، **Read-only**.

Registry ترازو، State داخلی همگام‌سازی، Session دستگاه‌ها و پروتکل مستقیم ترازو جزو قرارداد عمومی پایه نیستند.

جزئیات در [قرارداد SQL v1](docs/fa/sql-contract-v1.md) و [راهنمای جامع فارسی](docs/reference/README.md) آمده است.

## سندهای اصلی پروژه

برای اینکه ادامه پروژه به حافظه چت وابسته نباشد، این فایل‌ها مرجع هستند:

- [وضعیت پروژه](docs/PROJECT_STATUS.md): آخرین وضعیت و قدم بعدی دقیق.
- [نقشه‌راه](docs/ROADMAP.md): مراحل برنامه‌ریزی‌شده تا v1.0 و بعد از آن.
- [تصمیم‌ها](docs/DECISIONS.md): تصمیم‌های معماری و محصول که نهایی شده‌اند.
- [Backlog](docs/BACKLOG.md): کارهای قابل انجام و اولویت آنها.
- [گزارش روند کار](docs/WORK_LOG.md): ثبت زمانی تصمیم‌ها و کارهای انجام‌شده.
- [راه‌اندازی GitHub](docs/GITHUB_SETUP.md): روش امن برای اولین Push و عمومی‌کردن Repo.
- [مرز امنیتی](docs/SECURITY_BOUNDARY.md): چه چیزی اجازه انتشار دارد و چه چیزی ندارد.
- [سازگاری نسخه‌ها](docs/COMPATIBILITY.md).
- [سیاست Release](docs/RELEASE_POLICY.md).

## تجربه نهایی موردنظر برای C#

هدف SDK این است که شرکت نرم‌افزاری مجبور نباشد Query و Transaction و Cursor را از صفر پیاده کند. شکل هدف API چیزی در این حدود است:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.Items.UpsertAsync(item);

var sales = await client.Sales.ReadAfterAsync(lastProcessedId, batchSize: 100);
```

API نهایی هنوز Freeze نشده و طراحی آن در Roadmap ثبت شده است.

## Releaseها

DLL/Package و Sample نهایی داخل Branch اصلی نگهداری نمی‌شوند و از طریق **GitHub Releases** منتشر خواهند شد. هر Release باید Hash، Change Log، راهنما و بسته نمونه داشته باشد.

## امنیت

پروتکل مستقیم دستگاه، Capture شبکه، کلید خصوصی، Credential، اطلاعات مشتری، Firmware/Vendor material و زیرساخت داخلی Build/Release نرم‌افزار اصلی اجازه ورود به این مخزن عمومی را ندارند. قبل از هر Contribution فایل [SECURITY.md](SECURITY.md) را بخوانید.

## مجوز

هنوز مجوز متن‌باز نهایی برای این پروژه صادر نشده است. هدف، انتخاب یک مجوز آزاد و ساده برای استفاده شرکت‌های نرم‌افزاری است، اما نوع مجوز باید پیش از انتشار SDK پایدار به‌صورت رسمی تأیید شود. تا زمان اضافه‌شدن فایل `LICENSE`، همه حقوق محفوظ است. [NOTICE.md](NOTICE.md) را ببینید.

</div>
