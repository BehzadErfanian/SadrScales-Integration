<div dir="rtl" align="right">

# ترازوها، وضعیت اتصال و درخواست ارسال دوباره

این بخش برای **Sadr Scales 5.2.1 / SadrScales.Integration 1.1.0** در حال آماده‌سازی است.

## هدف

نرم‌افزار فروشگاهی یا ERP بتواند بدون شناخت پروتکل ترازو:

- فهرست ترازوهای ثبت‌شده را بخواند؛
- وضعیت کلی اتصال هر ترازو را ببیند؛
- درخواست کند کالاها در چرخه خودکار بعدی دوباره ارسال شوند؛
- در مدل‌های پشتیبانی‌شده، همین درخواست را برای کلیدهای سریع ثبت کند.

## خواندن ترازوها

در C#:

```csharp
var client = new SadrScalesClient(connectionString);

var scales = await client.Scales.GetAllAsync();
var scale = await client.Scales.GetAsync(3);
var status = await client.Scales.GetStatusAsync(3);
```

مدل عمومی ترازو شامل اطلاعاتی مانند شماره ترازو، IP، Port، مدل، نام، شعبه، وضعیت، فعال/غیرفعال بودن و تنظیمات AutoSend است.

`PrimaryItemGroupCode` فقط مقدار اصلی/قدیمی ذخیره‌شده روی ردیف ترازو است. نگاشت چندگروهی رسمی در بخش Scale Assignments ارائه می‌شود؛ نرم‌افزار مقصد نباید این دو مفهوم را یکی فرض کند.

## وضعیت اتصال

Sadr Scales 5.2.1 وضعیت کلی را در `SADR_Scale.Status` نگه می‌دارد. SDK آن را فقط به سه مقدار عمومی تبدیل می‌کند:

```text
Online
Offline
Unknown
```

`Unknown` یعنی مقدار SQL خالی، ناشناخته یا ترازو پیدا نشده است. وضعیت‌های لحظه‌ای داخلی مانند Connecting، Progress یا Last Error جزو Contract فعلی SQL نیستند.

## درخواست ارسال دوباره کالا

```csharp
SadrResendRequestResult result =
    await client.Scales.RequestItemResendAsync(scaleId);
```

این عملیات در سطح SQL مقدار داخلی آخرین ارسال کالا را Reset می‌کند تا داده در چرخه خودکار بعدی دوباره قابل ارسال باشد.

نتیجه‌ها:

```text
Requested
NotFound
```

**Requested به معنی دریافت موفق توسط ترازو نیست.** فقط یعنی درخواست در وضعیت SQL ثبت شده است. ارسال واقعی زمانی انجام می‌شود که چرخه AutoSend به ترازو برسد و شرایط لازم مانند فعال بودن، اتصال و تنظیمات ارسال خودکار برقرار باشد.

## درخواست ارسال دوباره کلید سریع

```csharp
SadrResendRequestResult result =
    await client.Scales.RequestHotKeyResendAsync(scaleId);
```

نتیجه‌ها:

```text
Requested
NotFound
UnsupportedModel
```

در 5.2.1 مسیر AutoSend کلید سریع برای مدل‌های ثبت‌شده `LSG`، `LSG_24D`، `TSG` و `LS6` پشتیبانی می‌شود. برای `PLUS` این API `UnsupportedModel` برمی‌گرداند و وضعیت ارسال را تغییر نمی‌دهد؛ SDK موفقیت ساختگی نشان نمی‌دهد.

## تفاوت Resend Request با دستور فوری

```text
RequestItemResend / RequestHotKeyResend
= درخواست برای چرخه AutoSend بعدی

SendItems / SendHotKeys فوری
= عملیات Runtime و نتیجه واقعی همان عملیات
```

دستورهای فوری جزو SQL Integration فعلی 5.2.1 نیستند. مسیر Command Mailbox برای Sadr Scales 5.3 برنامه‌ریزی شده است.

## نمونه SQL

برای مصرف‌کننده‌های غیر C#:

[`samples/SQL/04-scale-status-resend.sql`](../../samples/SQL/04-scale-status-resend.sql)

نمونه به‌صورت پیش‌فرض فقط‌خواندنی است و تا زمانی که `@ApplyResend = 1` نشود هیچ Watermarkی را تغییر نمی‌دهد.

## نکته ایمنی

در برنامه واقعی:

- SQL را با پارامتر اجرا کنید؛
- نتیجه `Requested` را «ارسال موفق به ترازو» نمایش ندهید؛
- برای کنترل دقیق نتیجه انتقال، منتظر قابلیت Command/Service آینده بمانید؛
- مستقیم Packet، Opcode یا پروتکل ترازو را از این Integration انتظار نداشته باشید.

</div>
