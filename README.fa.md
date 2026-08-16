<div dir="rtl" align="right">

# یکپارچه‌سازی Sadr Scales

**مخزن عمومی رسمی برای قرارداد یکپارچه‌سازی، ابزار توسعه و نمونه‌های اتصال نرم‌افزارهای فروشگاهی، ERP و حسابداری به Sadr Scales.**

[English](README.md) · [شروع سریع](docs/fa/getting-started.md) · [قرارداد SQL v1](docs/fa/sql-contract-v1.md) · [طراحی SDK](docs/SDK_DESIGN_V1.md) · [امنیت](SECURITY.md)

---

## وضعیت فعلی

این مخزن هنوز **پیش از نسخه 1.0** است. سطح پایه قرارداد عمومی برای **Sadr Scales 5.2.1** با نام **SQL Contract v1** Freeze شده و پایه نخست SDK زبان C# نیز ساخته شده و از Build/Test/Pack خودکار عبور کرده است.

| بخش | وضعیت |
|---|---|
| SQL Contract v1 | سطح پایه برای Sadr Scales 5.2.1 Freeze شده |
| راهنمای جامع فنی فارسی | PDF رسمی ۳۴ صفحه‌ای آماده و بازبینی شده؛ انتشار در Release باقی مانده |
| SDK زبان C# | پایه پیش از 1.0 ساخته شده و CI آن سبز است |
| نمونه‌های SQL | موجود |
| Python / Node.js / Java / PHP | برنامه‌ریزی‌شده |
| GitHub Releases | هنوز منتشر نشده |
| REST / Webhook | نسل بعد؛ خارج از 5.2.1 و Contract v1 |

## هدف این Repository

نرم‌افزار فروشگاهی، ERP یا حسابداری با **Sadr Scales** یکپارچه می‌شود و لازم نیست پروتکل اختصاصی ارتباط مستقیم هر مدل ترازو را دوباره پیاده‌سازی کند.

```text
POS / ERP / Accounting
        ↓
Sadr Scales SQL Contract v1 / Integration SDK
        ↓
Sadr Scales Runtime
        ↓
مدل‌های پشتیبانی‌شده ترازو
```

مسیر عمومی پایه برای این کارهاست:

- ثبت و ویرایش گروه کالا و PLU؛
- کنترل Schema مورد انتظار Contract v1؛
- خواندن افزایشی فروش‌های پذیرفته‌شده توسط Sadr Scales؛
- نگهداری Cursor/State پایدار فروش در نرم‌افزار مقصد؛
- یکپارچه‌سازی بدون نیاز به شناخت پروتکل مستقیم PLUS، LSG، Aclas و سایر دستگاه‌ها.

Registry، Mapping، اطلاعات ساخت‌یافته فاکتور و Runtime State به‌عنوان بخش‌های پیشرفته/کنترل‌شده مستند می‌شوند و جزو مسیر پایه SDK نیستند.

## خلاصه قرارداد عمومی v1

مسیر پایه روی سه Object است:

- `dbo.SADR_ItemClass` — گروه کالا، SELECT / INSERT / UPDATE؛
- `dbo.SADR_Item` — کالا/PLU، SELECT / INSERT / UPDATE؛ هویت عمومی `PluNo` است؛
- `dbo.SADR_Logs` — Feed فروش پذیرفته‌شده، فقط **SELECT**.

نرم‌افزار مقصد ابتدا فروش را در دیتابیس خودش Commit می‌کند و بعد Cursor پایدار خودش را جلو می‌برد. SDK پایه برای Ack یا Cursor، `SADR_Logs` را Update/Delete نمی‌کند.

جزئیات در [قرارداد SQL v1](docs/fa/sql-contract-v1.md)، [نسخه انگلیسی](docs/en/sql-contract-v1.md) و [سند Freeze قرارداد](docs/CONTRACT_V1_FREEZE.md) آمده است.

## SDK زبان C# — پیش از 1.0

پایه فعلی روی `netstandard2.0` ساخته شده و از `Microsoft.Data.SqlClient` استفاده می‌کند. API پایه عمداً کوچک نگه داشته شده است:

```csharp
var client = new SadrScalesClient(connectionString);

await client.ValidateAsync();
await client.ItemGroups.UpsertAsync(group);
await client.Items.UpsertAsync(item);

SadrSalesBatch batch = await client.Sales.ReadAfterAsync(lastProcessedId, 100);
```

قابلیت‌های پایه فعلی:

- کنترل Schema قرارداد v1؛
- Upsert پارامتری و Transactional گروه کالا؛
- Upsert پارامتری و Semantic کالا/PLU؛
- خواندن افزایشی و فقط‌خواندنی فروش؛
- مالکیت Connection String و Cursor مقصد توسط نرم‌افزار مصرف‌کننده؛
- Unit Test و GitHub Actions برای Restore/Build/Test/Pack.

API هنوز **پیش از 1.0** است و تا Release پایدار امکان اصلاح دارد. جزئیات در [طراحی SDK v1](docs/SDK_DESIGN_V1.md) ثبت شده است.

## نمونه‌های SQL

نمونه‌های اجرایی با داده ساختگی در [`samples/SQL`](samples/SQL/README.md) موجودند:

- کنترل Schema قرارداد؛
- Upsert امن گروه/کالا با Rollback پیش‌فرض؛
- خواندن افزایشی و فقط‌خواندنی فروش.

## سندهای اصلی

- [شروع سریع](docs/fa/getting-started.md)
- [قرارداد SQL v1](docs/fa/sql-contract-v1.md)
- [طراحی SDK v1](docs/SDK_DESIGN_V1.md)
- [وضعیت پروژه](docs/PROJECT_STATUS.md)
- [نقشه‌راه](docs/ROADMAP.md)
- [تصمیم‌ها](docs/DECISIONS.md)
- [سازگاری نسخه‌ها](docs/COMPATIBILITY.md)
- [مرز امنیتی](docs/SECURITY_BOUNDARY.md)
- [مشخصات راهنمای جامع فارسی آماده Release](docs/reference/README.md)

## Releaseها

DLL/Package پایدار و فایل‌های انتشار از طریق **GitHub Releases** منتشر می‌شوند و به‌صورت Binary در Branch اصلی انباشته نمی‌شوند. نسخه پایدار اول هنوز منتشر نشده است.

## امنیت

پروتکل مستقیم دستگاه، Capture شبکه، کلید خصوصی، Credential، اطلاعات مشتری، Firmware/Vendor material و زیرساخت داخلی Build/Release نرم‌افزار اصلی اجازه ورود به این مخزن عمومی را ندارند. قبل از Contribution فایل [SECURITY.md](SECURITY.md) را بخوانید.

## مجوز

هنوز مجوز عمومی نهایی برای SDK صادر نشده است. نوع مجوز باید پیش از انتشار پایدار به‌صورت رسمی تأیید شود. تا زمان اضافه‌شدن فایل `LICENSE`، همه حقوق محفوظ است. [NOTICE.md](NOTICE.md) را ببینید.

</div>
