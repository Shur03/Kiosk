# KioskSkytel

KioskSkytel нь **.NET 8 WPF** технологи дээр хөгжүүлсэн өөртөө үйлчлэх киоск систем юм. Энэхүү систем нь хэрэглэгчдэд утасны дугаар сонгох, нэгж болон дата багц худалдан авах, төлбөр төлөх зэрэг үйлчилгээг автоматжуулсан хэлбэрээр үзүүлэх зориулалттай.

---

## Үндсэн боломжууд

- 📱 Утасны дугаар хайх, сонгох
- 📦 Дата болон нэгжийн багц худалдах
- 💰 Төлбөр төлөх
- 🗺️ Үйлчилгээний байршил өөрчлөх хүсэлт илгээх
- 👤 Шинэ төхөөрөмж, хэрэглэгчийн хүсэлт илгээх

---

## Технологийн стек

| Layer             | Technology              |
| ----------------- | ----------------------- |
| Front-end         | WPF (.NET 8)            |
| Back-end          | ASP.NET Core / .NET     |
| Database          | PostgreSQL              |
| ORM / Data Access | Npgsql                  |
| UI Framework      | Material Design in XAML |
| Architecture      | MVVM                    |
| Language          | C#                      |

---

## Solution Structure

```text
KioskSkytel
│
├── KioskSkytel/
│   ├── App.xaml
│   ├── MainWindow.xaml
│   └── Application Entry Point
│
├── KioskApp.Core/
│   ├── Interfaces
│   ├── Business Logic
│   └── Shared Components
│
├── KioskApp.Models/
│   ├── Users
│   ├── Accounts
│   ├── Bundles
│   ├── Cards
│   ├── IdCardInfo
│   └── Payment Models
│
├── KioskApp.Services/
│   ├── Repository Layer
│   ├── Database Services
│   ├── Payment Services
│   └── External API Integration
│
└── KioskApp.UI/
    ├── Views
    ├── ViewModels
    ├── UserControls
    └── Helpers
```

---

## Өгөгдлийн сангийн бүтэц (ERD)

Системийн үндсэн entity-үүд:

### Users

Хэрэглэгчийн мэдээлэл хадгална.

| Field           | Type    |
| --------------- | ------- |
| id              | int     |
| first_name      | varchar |
| last_name       | varchar |
| register_number | varchar |

---

### Accounts

Хэрэглэгчийн үйлчилгээний данс.

| Field          | Type    |
| -------------- | ------- |
| id             | int     |
| user_id        | int     |
| service_type   | int     |
| account_number | varchar |

**Relationship**

```text
User (1) ---- (N) Accounts
```

---

### Payment Invoices

Төлбөрийн нэхэмжлэлүүд.

```text
Account (1) ---- (N) PaymentInvoices
```

---

### Payment Transactions

Хийгдсэн төлбөрийн гүйлгээ.

```text
Invoice (1) ---- (N) Transactions
```

---

### Bundles

Дата болон нэгжийн багцууд.

| Field       | Type    |
| ----------- | ------- |
| id          | int     |
| title       | varchar |
| description | text    |
| category    | int     |

---

### Account Bundles

Дансанд идэвхжүүлсэн багцууд.

```text
Account (1) ---- (N) AccountBundles
Bundle  (1) ---- (N) AccountBundles
```

---

### Cards

Нэгжийн картын мэдээлэл.

| Field       | Type    |
| ----------- | ------- |
| id          | int     |
| title       | varchar |
| price       | numeric |
| duration    | varchar |
| data_gb     | bigint  |
| unit_amount | int     |

---

### Phone Numbers

Худалдаанд байгаа утасны дугаарууд.

| Field    | Type    |
| -------- | ------- |
| id       | int     |
| number   | varchar |
| category | varchar |

---

## Database Relationships

```text
Users
 └── Accounts
       ├── PaymentInvoices
       │      └── PaymentTransactions
       │
       └── AccountBundles
               └── Bundles

Cards
PhoneNumbers
```

---

## Суурилуулах

### Шаардлага

- .NET SDK 8.0+
- PostgreSQL 14+
- Visual Studio 2022 эсвэл түүнээс дээш

---

## Build хийх

### Visual Studio

```bash
1. KioskSkytel.sln эсвэл KioskSkytel.slnx файлыг нээнэ
2. Restore NuGet Packages
3. Build Solution
```

### Command Line

```bash
git clone <repository-url>

cd KioskSkytel

dotnet restore

dotnet build
```

---

## Ажиллуулах

```bash
dotnet run --project KioskSkytel
```

эсвэл Visual Studio дээр:

```text
Set Startup Project → KioskSkytel
Press F5
```

---

## NuGet Packages

```xml
Npgsql
MaterialDesignThemes
MaterialDesignColors
CommunityToolkit.Mvvm
Microsoft.Extensions.DependencyInjection
Microsoft.Extensions.Configuration
```

---

## Архитектур

Систем нь MVVM загвар ашигладаг.

```text
View
 ↓
ViewModel
 ↓
Service
 ↓
Repository
 ↓
PostgreSQL
```

Энэ бүтэц нь:

- UI болон бизнес логикийг салгах
- Unit Test хийх боломжийг нэмэгдүүлэх
- Засвар үйлчилгээ хийхэд хялбар байх
- Hardware integration хийхэд тохиромжтой байх давуу талтай

---

## Цаашид хөгжүүлэлт хйигдэх

- QR төлбөрийн интеграц
- Карт уншигч (Card Reader) интеграц
- Иргэний үнэмлэх уншигч интеграц
- Хурууны хээ уншигч интеграц
- Receipt Printer дэмжлэг
- Remote Monitoring Dashboard
- Transaction Analytics

---

Shur Yeruult
