# MTK - Mənzil-Tikinti Kooperativi İdarəetmə Sistemi

## 📋 Layihə Haqqında

**MTK** - Mənzil-Tikinti Kooperativləri üçün tam funksional kommunal haqq idarəetmə sistemidir. Sistem bina idarəetməsi, maliyyə şəffaflığı, ödəniş idarəetməsi və sakinlərlə qarşılıqlı əlaqəni asanlaşdırır.

### Əsas Məqsəd
Bir MTK (məsələn, "28 May MTK") üçün 3 binanın (Blok 1, 2, 3) tam idarəetməsi - kommunal haqq toplama, xərc idarəetməsi, işçi maaşları və tam maliyyə şəffaflığı.

---

## 🏗️ Texniki Arxitektura

### Arxitektura Paterni
- **Modular Monolith** - Mikroservislərin mürəkkəbliyi olmadan modulyar struktur
- **Domain-Driven Design (DDD)** - Bounded Context-lər ilə təşkil edilmiş
- **CQRS** - Command/Query ayrılması
- **Event-Driven Architecture** - Outbox/Inbox pattern ilə modullar arası əlaqə
- **Clean Architecture** - Hər modul: Domain → Application → Infrastructure → Presentation

### Texnologiyalar

```
Backend:
  - .NET 9.0 (C#)
  - ASP.NET Core Web API
  - Entity Framework Core 9.0 (PostgreSQL)
  - MediatR (CQRS pattern)
  - FluentValidation
  - Serilog (logging)

Authentication & Authorization:
  - Keycloak (Identity Provider)
  - JWT Bearer Tokens
  - Role-based Access Control (RBAC)

Database:
  - PostgreSQL 16+
  - Entity Framework Core Migrations

Background Jobs:
  - Hangfire (aylıq faktura generasiyası, keycloak sync)

API Documentation:
  - Swagger/OpenAPI 3.0
  - OAuth2 + PKCE flow
```

---

## 📂 Proyekt Strukturu

```
MTK/
├── src/
│   ├── Apps/
│   │   └── MTK.Api/                          # ASP.NET Core Web API
│   │
│   ├── Common/                                # Shared Kernel
│   │   ├── MTK.Common.Domain/                # Base entities, value objects
│   │   ├── MTK.Common.Application/           # CQRS abstractions, behaviors
│   │   ├── MTK.Common.Infrastructure/        # Outbox/Inbox, interceptors
│   │   └── MTK.Common.Presentation/          # Endpoints, middleware
│   │
│   └── Modules/                               # Business Modules
│       ├── Identity/                          # ✅ TAMAMLANMIŞ
│       │   ├── MTK.Modules.Identity.Domain/
│       │   ├── MTK.Modules.Identity.Application/
│       │   ├── MTK.Modules.Identity.Infrastructure/
│       │   ├── MTK.Modules.Identity.Presentation/
│       │   └── MTK.Modules.Identity.IntegrationEvents/
│       │
│       ├── Buildings/                         # 🚧 PLANLAŞDIRILMIŞ
│       │   ├── MTK.Modules.Buildings.Domain/
│       │   ├── MTK.Modules.Buildings.Application/
│       │   ├── MTK.Modules.Buildings.Infrastructure/
│       │   ├── MTK.Modules.Buildings.Presentation/
│       │   └── MTK.Modules.Buildings.IntegrationEvents/
│       │
│       ├── Billing/                           # 🚧 PLANLAŞDIRILMIŞ
│       ├── Finance/                           # 🚧 PLANLAŞDIRILMIŞ
│       ├── Expenses/                          # 🚧 PLANLAŞDIRILMIŞ
│       ├── Employees/                         # 🚧 PLANLAŞDIRILMIŞ
│       ├── Maintenance/                       # 🚧 PLANLAŞDIRILMIŞ
│       ├── Voting/                            # 🚧 PLANLAŞDIRILMIŞ
│       ├── Notifications/                     # 🚧 PLANLAŞDIRILMIŞ
│       └── Documents/                         # 🚧 PLANLAŞDIRILMIŞ
│
├── tests/                                     # Unit & Integration tests
├── docs/                                      # Documentation
└── MTK.sln                                    # Solution file
```

---

## 👥 İstifadəçi Rolları

### 1. SystemAdmin
- **İzahat:** Sistem administratoru
- **Hüquqlar:** Tam giriş - bütün modullar və əməliyyatlar
- **Keycloak Role:** `SystemAdmin`

### 2. BuildingManager (Komandant) ⭐
- **İzahat:** Bina idarəçisi - hər 3 binaya baxır
- **Hüquqlar:**
  - Mənzil idarəetməsi
  - Xərc qeydiyyatı
  - İşçilərə tapşırıq vermə
  - Maliyyə hesabatlarına baxış
  - Şikayət idarəetməsi
- **Keycloak Role:** `BuildingManager`

### 3. Accountant (Mühasib)
- **İzahat:** Maliyyə əməliyyatları
- **Hüquqlar:**
  - Maliyyə hesabatları
  - Ödəniş təsdiqi
  - Xərc qeydiyyatı
  - Budget idarəetməsi
- **Keycloak Role:** `Accountant`

### 4. ApartmentOwner (Mənzil Sahibi) ⭐⭐⭐
- **İzahat:** Bir və ya bir neçə mənzilin sahibi
- **Hüquqlar:**
  - Öz borcunu görür
  - Onlayn ödəniş edir
  - Ödəniş tarixçəsi
  - Şikayət yazır
  - Səsvermədə iştirak edir
  - Şəffaflıq dashboarduna baxır
- **Keycloak Role:** `ApartmentOwner`

### 5. Employee (İşçi)
- **İzahat:** Təmizlikçi, mühafizəçi, texniki işçilər
- **Hüquqlar:**
  - Tapşırıqları görür
  - Maaş məlumatına baxır
  - Tapşırıq tamamlandı işarəsi
- **Keycloak Role:** `Employee`

---

## 🏢 Domain Modulları

### 1️⃣ Identity Module ✅ (TAMAMLANMIŞ)

**Məqsəd:** İstifadəçi idarəetməsi, autentifikasiya, avtorizasiya

**Əsas Entity-lər:**
- `User` - İstifadəçilər
- `Role` - Rollar (SystemAdmin, BuildingManager, Accountant, ApartmentOwner, Employee)
- `Group` - Qruplar (Keycloak ilə sync)
- `AuditLog` - Audit jurnalı

**Xüsusiyyətlər:**
- ✅ Keycloak inteqrasiyası (OAuth2 + PKCE)
- ✅ JWT token autentifikasiyası
- ✅ Role-based authorization
- ✅ Keycloak sync job (background)
- ✅ Audit logging
- ✅ MTK rolları seed data

**Status:** Production-ready

---

### 2️⃣ Buildings Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** Bina, mənzil, qaraj və sahiblik idarəetməsi

**Əsas Entity-lər:**

```csharp
// Building (Bina)
public class Building
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // "Blok 1", "Blok 2", "Blok 3"
    public string Address { get; set; }
    public int FloorCount { get; set; }
    public ICollection<Apartment> Apartments { get; set; }
    public ICollection<Garage> Garages { get; set; }
}

// Apartment (Mənzil)
public class Apartment
{
    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public Guid OwnerId { get; set; }             // ⭐ Cari sahibi

    public string Number { get; set; }
    public int Floor { get; set; }
    public decimal Area { get; set; }             // m²

    // ⭐ Sahiblik tarixçəsi (Mənzil satışı, transfer)
    public ICollection<OwnershipHistory> OwnershipHistory { get; set; }

    public Building Building { get; set; }
    public Owner Owner { get; set; }
}

// OwnershipHistory (Sahiblik Tarixçəsi) ⭐ YENI
public class OwnershipHistory
{
    public Guid Id { get; set; }
    public Guid ApartmentId { get; set; }
    public Guid OwnerId { get; set; }

    public DateTime StartDate { get; set; }       // Sahiblik başlama
    public DateTime? EndDate { get; set; }        // Sahiblik bitmə (null = cari sahibi)

    public OwnershipTransferReason? TransferReason { get; set; }
    public string? Notes { get; set; }

    public Apartment Apartment { get; set; }
    public Owner Owner { get; set; }
}

public enum OwnershipTransferReason
{
    Purchase = 1,      // Satın alma
    Sale = 2,          // Satış
    Inheritance = 3,   // Miras
    Gift = 4,          // Bağışlama
    Other = 5          // Digər
}

// Owner (Sahiblik) ⭐ MƏRKƏZİ AGGREGATE
public class Owner
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }              // Identity module-a əlaqə

    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string IdentityNumber { get; set; }

    // ⭐ Bir sahibin ÇOXLU mənzili ola bilər
    public ICollection<Apartment> Apartments { get; set; }

    // ⭐ Bir sahibin ÇOXLU qarajı ola bilər
    public ICollection<Garage> Garages { get; set; }
}

// Garage (Qaraj)
public class Garage
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }             // ⭐ Owner-ə bağlı
    public Guid? BuildingId { get; set; }

    public string Number { get; set; }            // "G-12", "P-45"
    public GarageType Type { get; set; }          // OpenParking, CoveredGarage, Storage

    public Owner Owner { get; set; }
}

public enum GarageType
{
    OpenParking = 1,      // Açıq parking - 15 AZN/ay
    CoveredGarage = 2,    // Qapalı qaraj - 30 AZN/ay
    Storage = 3           // Anbarlıq - 12 AZN/ay
}
```

**Integration Events:**
- `OwnerCreatedIntegrationEvent` → Billing moduluna
- `ApartmentCreatedIntegrationEvent` → Billing moduluna
- `GarageAddedToOwnerIntegrationEvent` → Billing moduluna
- `ApartmentOwnershipTransferredIntegrationEvent` → Billing moduluna ⭐

**Sahiblik Transferi (Mənzil Satışı) - Business Rules:**

```csharp
// ⚠️ Vacib Qaydalar:

1. Transfer Şərti:
   ✅ Köhnə sahibin BÜTÜN borcları ödənilməlidir
   ✅ Transfer yalnız borc 0 AZN olanda mümkündür

2. Transfer Tarixi:
   ✅ Həmişə ayın 1-nə aid edilir
   ✅ Məsələn: 15 Martda satış = 1 Apreldə rəsmi transfer
   ✅ Mart fakturaları hələ də köhnə sahibə

3. Borc İzolasiyası:
   ✅ Köhnə sahibin borcu Owner-ə bağlıdır (mənzilə YOX)
   ✅ Yeni sahibi təmiz başlayır (0 AZN borc)

4. Sahiblik Tarixçəsi:
   ✅ Hər transfer qeyd olunur (OwnershipHistory)
   ✅ Tam şəffaflıq - kim, nə zaman, nə səbəbdən
```

**Nümunə Transfer Ssenarisi:**

```
Vəziyyət:
  - Kamran mənzili (Blok 2, №18) Rəşad'a satır
  - Kamran'ın 150 AZN borcu var (2 ay)
  - Satış tarixi: 15 Mart 2026

Addımlar:
  1. Kamran 150 AZN ödəyir → Borc = 0 AZN ✅
  2. Manager transfer sorğusu göndərir
  3. Sistem yoxlama edir:
     - Borc = 0? ✅
     - Transfer tarixi ayın 1-i? (15 Mart → 1 Aprel) ✅
  4. OwnershipHistory yenilənir:
     - Kamran: 2020-01-01 ~ 2026-03-15 (Satış)
     - Rəşad: 2026-03-15 ~ (aktiv)
  5. Apartment.OwnerId dəyişir: Kamran → Rəşad
  6. Integration Event: ApartmentOwnershipTransferredIntegrationEvent
     → Billing module (növbəti fakturalar Rəşad'a)
  7. Rəşad təmiz başlayır: 0 AZN borc ✅
```

---

### 3️⃣ Billing Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** Kommunal haqq hesablama, faktura generasiyası, ödəniş

**Əsas Entity-lər:**

```csharp
// ServiceRate (Tarif)
public class ServiceRate
{
    public Guid Id { get; set; }
    public decimal ApartmentRatePerSqm { get; set; }  // 0.50 AZN/m²

    // Qaraj növləri üçün fixed məbləğlər
    public decimal OpenParkingRate { get; set; }      // 15 AZN
    public decimal CoveredGarageRate { get; set; }    // 30 AZN
    public decimal StorageRate { get; set; }          // 12 AZN

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

// Invoice (Faktura) ⭐ PARTIAL PAYMENT SUPPORT
public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }         // "2026-03-001"

    public Guid ApartmentId { get; set; }             // Hər mənzil üçün ayrı
    public Guid OwnerId { get; set; }                 // ⭐ Snapshot - faktura yaradılma anında sahibi

    public DateTime BillingMonth { get; set; }

    // ⭐ Snapshot məlumatlar (transfer zamanı dəyişməz qalır)
    public string OwnerName { get; set; }             // O zamankı sahibin adı
    public string ApartmentNumber { get; set; }       // Mənzil nömrəsi
    public decimal ApartmentArea { get; set; }
    public decimal ApartmentCharge { get; set; }      // Area × Rate
    public decimal GarageCharges { get; set; }        // Owner-in bütün qarajları
    public decimal PreviousDebt { get; set; }
    public decimal TotalAmount { get; set; }

    // ⭐ Partial Payment Tracking
    public decimal PaidAmount { get; set; }           // Nə qədər ödənilib
    public decimal RemainingAmount => TotalAmount - PaidAmount;

    public InvoiceStatus Status { get; set; }         // Pending, PartiallyPaid, Paid, Overpaid
    public DateTime? FullyPaidAt { get; set; }

    // ⭐ Çoxlu ödəniş (junction table)
    public ICollection<InvoicePayment> InvoicePayments { get; set; }

    public Apartment Apartment { get; set; }
    public Owner Owner { get; set; }
}

public enum InvoiceStatus
{
    Pending = 1,         // Ödənilməyib
    PartiallyPaid = 2,   // Qismən ödənilib
    Paid = 3,            // Tam ödənilib
    Overpaid = 4,        // Artıq ödənilib (avans kimi qalır)
    Cancelled = 5        // Ləğv edilib
}

// ⚠️ QEYD: Invoice entity snapshot pattern istifadə edir
// Səbəb: Mənzil satılsa belə, köhnə fakturalar köhnə sahibə aid qalmalıdır

// Payment (Ödəniş) ⭐ BULK + ADVANCE + CASH SUPPORT
public class Payment
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; }         // "PAY-2026-03-001"
    public string TransactionId { get; set; }         // Bank transaction ID (onlayn üçün)

    public Guid OwnerId { get; set; }                 // Kim ödəyir
    public decimal TotalAmount { get; set; }          // Ümumi məbləğ

    public PaymentMethod Method { get; set; }         // Online, Cash, BankTransfer
    public PaymentStatus Status { get; set; }         // Pending, Success, Failed

    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // ⭐ Nağd ödəniş üçün
    public Guid? RecordedByUserId { get; set; }       // Komandant/Mühasib
    public string RecordedByName { get; set; }        // Snapshot: "Leyla Məmmədova"
    public DateTime? RecordedAt { get; set; }

    // ⭐ Çoxlu faktura (junction table)
    public ICollection<InvoicePayment> InvoicePayments { get; set; }

    public Owner Owner { get; set; }
}

public enum PaymentMethod
{
    Online = 1,          // Payriff, e-Manat
    Cash = 2,            // Nağd (mühasibə otağında)
    BankTransfer = 3     // Bank köçürməsi
}

public enum PaymentStatus
{
    Pending = 1,         // Gözləyir (onlayn ödəniş üçün)
    Success = 2,         // Uğurlu
    Failed = 3,          // Uğursuz
    Cancelled = 4        // Ləğv edilib
}

// InvoicePayment (Junction Table) ⭐ YENI
public class InvoicePayment
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }

    public decimal AllocatedAmount { get; set; }      // Bu fakturaya ayrılan məbləğ
    public DateTime AllocatedAt { get; set; }

    public Payment Payment { get; set; }
    public Invoice Invoice { get; set; }
}

// OwnerAccount (Avans/Credit Balance) ⭐ YENI
public class OwnerAccount
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public decimal CreditBalance { get; set; }        // Avans/Artıq məbləğ
    public DateTime LastUpdated { get; set; }

    public Owner Owner { get; set; }
    public ICollection<OwnerAccountTransaction> Transactions { get; set; }
}

// OwnerAccountTransaction (Avans Tarixçəsi) ⭐ YENI
public class OwnerAccountTransaction
{
    public Guid Id { get; set; }
    public Guid OwnerAccountId { get; set; }

    public TransactionType Type { get; set; }         // Credit, Debit
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }         // Əməliyyatdan sonra balans

    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Reference (opsional)
    public Guid? PaymentId { get; set; }
    public Guid? InvoiceId { get; set; }

    public OwnerAccount OwnerAccount { get; set; }
}

public enum TransactionType
{
    Credit = 1,      // Artır (avans ödəniş, artıq məbləğ)
    Debit = 2        // Azalır (avtomatik faktura ödənişi)
}
```

**Hesablama Məntiqi:**

```
Nümunə 1:
Owner: Kamran
- Mənzil: Blok 2, №18 (85 m²)
- Qaraj: G-12 (qapalı)

Hesablama:
  Mənzil: 85 × 0.50 = 42.50 AZN
  Qaraj:  1 × 30    = 30.00 AZN
  ────────────────────────────
  TOPLAM:            72.50 AZN

Nümunə 2:
Owner: Vəli
- Mənzil 1: Blok 1, №5 (120 m²)
- Mənzil 2: Blok 3, №34 (95 m²)
- Qaraj 1: G-08 (qapalı)
- Qaraj 2: P-21 (açıq)
- Anbarlıq: S-03

Hesablama:
  Mənzil 1: 120 × 0.50 = 60.00 AZN   (Faktura 1)
  Mənzil 2:  95 × 0.50 = 47.50 AZN   (Faktura 2)
  Qaraj 1:   30 AZN
  Qaraj 2:   15 AZN
  Anbarlıq:  12 AZN
  ─────────────────────────────────
  TOPLAM:           164.50 AZN (2 faktura)
```

---

**Xüsusi Ödəniş Ssenariləri:**

### **Ssenari 1: Avans Ödəniş (Advance Payment)**

```
15 Mart 2026:
  Kamran: "3 ay qabağa ödəyim"

  Mövcud Fakturalar:
    ✅ Mart 2026: 80 AZN (mövcud)
    ❌ Aprel 2026: YOX (hələ yaradılmayıb)
    ❌ May 2026: YOX (hələ yaradılmayıb)

  Kamran ödəyir: 240 AZN

  Sistem allocation:
    1. Mart fakturauna: 80 AZN → Invoice.Status = Paid ✅
    2. Qalan: 160 AZN → OwnerAccount.CreditBalance = 160 AZN

  1 Aprel (Hangfire job):
    - Aprel fakturaları generasiya olunur
    - Kamran'ın fakturaı: 80 AZN
    - OwnerAccount.CreditBalance = 160 AZN (kifayət edir)
    - Avtomatik allocation: 160 - 80 = 80 AZN qalır
    - Invoice.Status = Paid (avtomatik) ✅

  1 May:
    - May fakturaları generasiya
    - OwnerAccount.CreditBalance = 80 AZN
    - Avtomatik: 80 - 80 = 0 AZN
    - Invoice.Status = Paid ✅
    - OwnerAccount.CreditBalance = 0 AZN
```

**OwnerAccountTransaction Tarixçəsi:**
```
15.03.2026  Credit   +160.00 AZN  "Artıq ödəniş"         Balance: 160.00
01.04.2026  Debit     -80.00 AZN  "Aprel faktura #123"   Balance: 80.00
01.05.2026  Debit     -80.00 AZN  "May faktura #124"     Balance: 0.00
```

---

### **Ssenari 2: Qismən Ödəniş (Partial Payment)**

```
Kamran'ın borcu: 240 AZN (3 faktura)
  - Yanvar: 80 AZN
  - Fevral: 80 AZN
  - Mart: 80 AZN

Kamran ödəyir: 150 AZN

Allocation (FIFO - First In First Out):
  1. Yanvar: 80 AZN → Tam ödənilib ✅
     - Invoice.PaidAmount = 80 AZN
     - Invoice.Status = Paid

  2. Fevral: 70 AZN → Qismən ödənilib ⚠️
     - Invoice.PaidAmount = 70 AZN
     - Invoice.RemainingAmount = 10 AZN
     - Invoice.Status = PartiallyPaid

  3. Mart: 0 AZN → Ödənilməyib ❌
     - Invoice.PaidAmount = 0 AZN
     - Invoice.Status = Pending

InvoicePayment Records:
  - Payment #1 → Yanvar Invoice: 80 AZN
  - Payment #1 → Fevral Invoice: 70 AZN
```

---

### **Ssenari 3: Nağd Ödəniş (Cash Payment - Mühasib Qeydə Alır)**

```
Kamran mühasibə otağına gəlir, 80 AZN nağd verir

Mühasib (Leyla):
  1. Sistemə daxil olur (accountant role)
  2. "Nağd Ödəniş Qəbul Et" bölməsi
  3. Kamran'ı axtarır (Owner search)
  4. Ödənilməmiş fakturaları görür:
     - [✓] Mart 2026 - 80 AZN
     - [ ] Fevral 2026 - 80 AZN
  5. Məbləğ daxil edir: 80 AZN
  6. "Qəbz Çap Et və Təsdiq Et" düyməsi
  7. A4 qəbz print olunur, Kamran'a verilir

Sistemdə:
  Payment.Method = Cash
  Payment.Status = Success (dərhal təsdiq)
  Payment.RecordedByUserId = Leyla'nın ID
  Payment.RecordedByName = "Leyla Məmmədova" (snapshot)
  Payment.RecordedAt = 15.03.2026 14:30

  InvoicePayment.AllocatedAmount = 80 AZN
  Invoice.Status = Paid
  Invoice.FullyPaidAt = 15.03.2026 14:30
```

---

### **Ssenari 4: Bulk Ödəniş (Multiple Invoices)**

```
Owner Dashboard - Kamran seçir:

  [✓] Blok 1, №5  - Mart:   80 AZN
  [✓] Blok 2, №18 - Mart:   65 AZN
  [✓] Blok 1, №5  - Fevral: 80 AZN
  [ ] Blok 2, №18 - Fevral: 65 AZN

  Seçilmiş: 3 faktura
  Toplam: 225 AZN

  ["Seçilmişləri Ödə (225 AZN)"]

Sistem:
  1. Payment.TotalAmount = 225 AZN
  2. Payment Gateway redirect (Payriff)
  3. Callback alınır
  4. 3 InvoicePayment record yaradılır:
     - Payment → Invoice 1: 80 AZN
     - Payment → Invoice 2: 65 AZN
     - Payment → Invoice 3: 80 AZN
  5. Hər 3 faktura Status = Paid ✅
```

---

**Background Jobs:**
- `GenerateMonthlyInvoicesJob` - Hər ayın 1-i, saat 00:00 (Hangfire)
  - Fakturalar yaradılır
  - ⭐ OwnerAccount.CreditBalance > 0 olanlar avtomatik ödənilir
  - OwnerAccountTransaction yaradılır (Debit)

**Integration Events:**
- `InvoiceGeneratedIntegrationEvent` → Notifications
- `PaymentReceivedIntegrationEvent` → Finance, Documents
- `OwnerCreditAddedIntegrationEvent` → Notifications (avans balansı əlavə olundu)
- `OwnerCreditDeductedIntegrationEvent` → Owner Dashboard (real-time update)
- `BulkPaymentCompletedIntegrationEvent` → Notifications, Documents (çoxlu faktura ödənişi)
- `CashPaymentRecordedIntegrationEvent` → Finance (nağd ödəniş qeydə alındı)

**External Integrations:**
- Payriff Payment Gateway (test mode)
- e-Manat (future)

**Business Rules:**
- ✅ Avans ödəniş FIFO (First In First Out) allocation
- ✅ Partial payment dəstəyi (qismən ödəniş)
- ✅ Nağd ödəniş yalnız Accountant və BuildingManager rolları qeydə ala bilər
- ✅ Owner avans balansını görə bilər, amma withdraw edə bilməz (yalnız faktura ödənişinə gedir)
- ✅ Bulk payment maksimum 50 faktura (performance)

---

### 4️⃣ Finance Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** Ümumi kassa, balans idarəetməsi

**Əsas Entity-lər:**

```csharp
// CashAccount (Ümumi kassa)
public class CashAccount
{
    public Guid Id { get; set; }
    public string AccountName { get; set; }    // "28 May MTK Əsas Hesab"
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; }
}

// FinancialTransaction (Maliyyə əməliyyatı)
public class FinancialTransaction
{
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }  // Income (gəlir) / Expense (xərc)
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }

    // Income üçün
    public Guid? PaymentId { get; set; }

    // Expense üçün
    public Guid? ExpenseId { get; set; }
}
```

**Balans Hesablama:**
```
Gəlir:    Ödənişlər
Xərc:     Maaşlar + Təmirlər + Kommunal + Digər
Balans:   Gəlir - Xərc
```

---

### 5️⃣ Expenses Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** Xərc qeydiyyatı, kateqoriya idarəetməsi

```csharp
public class Expense
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }

    public Guid? BuildingId { get; set; }         // Hansı bina (opsional)
    public Guid? EmployeeId { get; set; }         // Əgər maaşdırsa
    public string ReceiptUrl { get; set; }        // Qəbz şəkli (Azure Blob)
}

public class ExpenseCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // "Maaşlar", "Təmir", "Kommunal"
    public string Color { get; set; }             // UI üçün (#FF5733)
}

public class Budget
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public decimal MonthlyLimit { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}
```

---

### 6️⃣ Employees Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** İşçi idarəetməsi, maaş ödənişi

```csharp
public class Employee
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }              // Identity module

    public string FullName { get; set; }
    public string Position { get; set; }          // "Təmizlikçi", "Təhlükəsizlik"
    public string Phone { get; set; }
    public decimal MonthlySalary { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
}

public class SalaryPayment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ExpenseId { get; set; }           // Expense kimi qeyd olunur

    public DateTime PaymentMonth { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
}
```

---

### 7️⃣ Maintenance Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** Şikayət idarəetməsi, təmir sorğuları

```csharp
public class Complaint
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public Guid ApartmentId { get; set; }
    public Guid? BuildingId { get; set; }
    public ComplaintCategory Category { get; set; }  // Lift, Water, Electricity
    public ComplaintStatus Status { get; set; }      // Open, InProgress, Resolved

    public Guid? AssignedEmployeeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class WorkOrder
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public WorkOrderType Type { get; set; }          // Preventive, Corrective
    public Guid? BuildingId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public decimal? EstimatedCost { get; set; }
}
```

**İş axını:**
```
Owner şikayət yaradır (Open)
  ↓
Manager görür, işçiyə təyin edir (Assigned)
  ↓
İşçi işə başlayır (InProgress)
  ↓
İşçi tamamlayır, foto əlavə edir (Resolved)
  ↓
Owner təsdiq edir (Closed)
```

---

### 8️⃣ Voting Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** Səsvermə sistemi (bütün sakinlər)

```csharp
public class Proposal
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal? EstimatedCost { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime VotingDeadline { get; set; }
    public ProposalStatus Status { get; set; }       // Active, Approved, Rejected

    public int TotalVotes { get; set; }
    public int YesVotes { get; set; }
    public int NoVotes { get; set; }
}

public class Vote
{
    public Guid Id { get; set; }
    public Guid ProposalId { get; set; }
    public Guid ApartmentId { get; set; }            // Hər mənzil 1 səs
    public VoteChoice Choice { get; set; }           // Yes, No, Abstain
    public DateTime VotedAt { get; set; }
}
```

---

### 9️⃣ Notifications Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** SMS, Email bildirişləri

```csharp
public class NotificationTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; }                 // "PaymentReminder", "InvoiceGenerated"
    public NotificationType Type { get; set; }       // SMS, Email, Push
    public string Template { get; set; }             // "Hörmətli {{Name}}, borcunuz {{Amount}} AZN"
}

public class NotificationHistory
{
    public Guid Id { get; set; }
    public string Recipient { get; set; }            // Phone / Email
    public NotificationType Type { get; set; }
    public string Message { get; set; }
    public NotificationStatus Status { get; set; }   // Sent, Failed
    public DateTime SentAt { get; set; }
}
```

**External Integrations:**
- bTaskım SMS gateway
- SMTP Email service

---

### 🔟 Documents Module 🚧 (PLANLAŞDIRILMIŞ)

**Məqsəd:** PDF generasiya, sənəd arxivi

```csharp
public class Document
{
    public Guid Id { get; set; }
    public DocumentType Type { get; set; }           // Invoice, Receipt, Report, Contract
    public string FileName { get; set; }
    public string FileUrl { get; set; }              // Azure Blob / Local storage
    public DateTime GeneratedAt { get; set; }
}
```

**PDF Generation:**
- QuestPDF library
- Ödəniş qəbzləri
- Aylıq/illik hesabatlar
- Müqavilələr

---

## 🎯 Killer Feature: Şəffaflıq Dashboardu

### Sakinlər üçün Dashboard

```
╔════════════════════════════════════════════╗
║  28 MAY MTK - MALİYYƏ ŞƏFFAFLIĞI          ║
╠════════════════════════════════════════════╣
║  [MART 2026]                               ║
║                                            ║
║  💰 Toplanan:      18,500 AZN             ║
║  💸 Xərclənən:     15,200 AZN             ║
║  📊 Qalıq:          3,300 AZN             ║
╚════════════════════════════════════════════╝

[XƏRC BÖLGÜSÜ - Pie Chart]
├─ Maaşlar: 9,000 AZN (59%)
├─ Təmir: 3,500 AZN (23%)
├─ Kommunal: 2,000 AZN (13%)
└─ Digər: 700 AZN (5%)

[BİNA ÜZRƏ BREAKDOWN]
┌────────┬──────────┬───────────┬─────────┐
│ Bina   │ Yığılan  │ Xərclənən │ Balans  │
├────────┼──────────┼───────────┼─────────┤
│ Blok 1 │ 6,200    │ 5,100     │ 1,100   │
│ Blok 2 │ 5,400    │ 4,800     │ 600     │
│ Blok 3 │ 6,900    │ 5,300     │ 1,600   │
└────────┴──────────┴───────────┴─────────┘

[SON XƏRCLƏR]
- 18.03 - Lift təmiri (Blok 1): 1,200 AZN
- 15.03 - Təmizlikçi maaşı: 800 AZN
- 10.03 - Elektrik ödənişi: 450 AZN
```

### Mənzil Sahibi Dashboard

```
[MƏNİM MƏNZİLİM]
Blok: 2
Mənzil: 18
Sahə: 85 m²
Qaraj: G-12 (qapalı)

[CARİ BORC]
Bu ay: 72.50 AZN
Toplam: 72.50 AZN

[ÖDƏNİŞ TARİXÇƏSİ]
15.02.2026 - 72.50 AZN ✅
15.01.2026 - 72.50 AZN ✅
15.12.2025 - 72.50 AZN ✅

[ŞİKAYƏTLƏRİM]
1. Lift işləmir (Açıq) - 12.03.2026
2. Soyuq su problemi (Həll olundu) - 05.03.2026
```

---

## 🔄 Əsas İş Axınları

### 1. Aylıq Faktura Generasiyası (⭐ Avans Auto-Apply)

```
Hangfire Cron Job (hər ayın 1-i, saat 00:00)
  ↓
Bütün aktiv mənzilləri tap (Buildings Module-dan)
  ↓
Hər mənzil üçün:
  - Tarif × m² = əsas məbləğ
  - Owner-in qarajları = əlavə məbləğ
  - Keçmiş borc əlavə et
  - Invoice yarat
  ↓
⭐ Owner-in OwnerAccount.CreditBalance > 0 yoxla
  ↓
Əgər avans varsa:
  - Virtual Payment yarat (Method = FromCredit)
  - InvoicePayment allocation
  - Invoice.PaidAmount += amount
  - OwnerAccount.CreditBalance -= amount
  - OwnerAccountTransaction (Debit) yarat
  - Əgər tam ödənilibsə → Invoice.Status = Paid
  ↓
Domain Event: InvoiceGenerated
  ↓
Integration Event: InvoiceGeneratedIntegrationEvent
  ↓
Notifications Module → SMS/Email göndər
  (Əgər avansdan ödənilibsə: "Avansınızdan avtomatik ödənilib")
```

### 2. Ödəniş Prosesi

```
Owner web-dən "Ödə" düyməsinə klikləyir
  ↓
Payment Gateway (Payriff) redirect
  ↓
Sakin bankda ödəyir
  ↓
Payriff Callback → ProcessPaymentCommand
  ↓
Payment entity yarat
Invoice.Status = Paid
Invoice.PaidAt = DateTime.UtcNow
  ↓
Domain Event: PaymentReceived
  ↓
Integration Events:
  - PaymentReceivedIntegrationEvent → Finance (balans artır)
  - GenerateReceiptIntegrationEvent → Documents (PDF qəbz)
  - SendPaymentConfirmationIntegrationEvent → Notifications
  ↓
FinancialTransaction.Type = Income
CashAccount.Balance += Amount
  ↓
PDF qəbz generasiya
Email/SMS təsdiq
```

### 3. Xərc Qeydiyyatı

```
Manager xərc daxil edir (Expense form)
  ↓
Command: CreateExpenseCommand
  ↓
Budget limitini yoxla
  - Limit aşırsa → Warning
  ↓
Qəbz şəklini upload et (Azure Blob)
  ↓
Expense entity yarat
  ↓
Domain Event: ExpenseCreated
  ↓
Integration Event → Finance
  ↓
FinancialTransaction.Type = Expense
CashAccount.Balance -= Amount
  ↓
Dashboard real-time yenilənir
```

### 4. Şikayət İş Axını

```
Owner şikayət yaradır
  ↓
Command: CreateComplaintCommand
  ↓
Complaint.Status = Open
  ↓
Integration Event → Notifications (Manager-ə bildiriş)
  ↓
Manager görür, işçiyə təyin edir
  ↓
Command: AssignComplaintCommand
Complaint.Status = Assigned
Complaint.AssignedEmployeeId = ...
  ↓
Integration Event → Notifications (İşçiyə bildiriş)
  ↓
İşçi işə başlayır
Complaint.Status = InProgress
  ↓
İşçi tamamlayır, foto əlavə edir
  ↓
Command: ResolveComplaintCommand
Complaint.Status = Resolved
Complaint.ResolvedAt = DateTime.UtcNow
  ↓
Integration Event → Notifications (Owner-ə bildiriş)
  ↓
Owner təsdiq edir
Command: CloseComplaintCommand
Complaint.Status = Closed
```

---

## 📊 Database Schema

### Shared Database Strategiyası

**Qərar:** Multi-tenancy YOXDUR
- Bir MTK üçün bir sistem
- Hər modul öz schema-sında
- Modullar arası əlaqə yalnız Integration Events vasitəsilə

```
PostgreSQL Database: mtk_db

Schemas:
  - identity.*          (Users, Roles, Groups, AuditLogs)
  - buildings.*         (Buildings, Apartments, Owners, Garages, OwnershipHistory)
  - billing.*           (Invoices, Payments, ServiceRates, InvoicePayments, OwnerAccounts, OwnerAccountTransactions)
  - finance.*           (CashAccount, Transactions)
  - expenses.*          (Expenses, Categories, Budget)
  - employees.*         (Employees, SalaryPayments)
  - maintenance.*       (Complaints, WorkOrders)
  - voting.*            (Proposals, Votes)
  - notifications.*     (Templates, History)
  - documents.*         (Documents, Files)
  - outbox.*            (OutboxMessages, OutboxMessageConsumers)
  - inbox.*             (InboxMessages, InboxMessageConsumers)
```

**⭐ Yeni Cədvəllər (Billing Schema):**

```sql
-- InvoicePayments (Junction Table - M:N)
CREATE TABLE billing.InvoicePayments (
    Id UUID PRIMARY KEY,
    PaymentId UUID NOT NULL REFERENCES billing.Payments(Id),
    InvoiceId UUID NOT NULL REFERENCES billing.Invoices(Id),
    AllocatedAmount DECIMAL(18,2) NOT NULL,
    AllocatedAt TIMESTAMP NOT NULL,
    CONSTRAINT CHK_AllocatedAmount_Positive CHECK (AllocatedAmount > 0)
);

-- OwnerAccounts (Avans Balance Tracking)
CREATE TABLE billing.OwnerAccounts (
    Id UUID PRIMARY KEY,
    OwnerId UUID NOT NULL UNIQUE,  -- One account per owner
    CreditBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    LastUpdated TIMESTAMP NOT NULL,
    CONSTRAINT CHK_CreditBalance_NonNegative CHECK (CreditBalance >= 0)
);

-- OwnerAccountTransactions (Avans Tarixçəsi)
CREATE TABLE billing.OwnerAccountTransactions (
    Id UUID PRIMARY KEY,
    OwnerAccountId UUID NOT NULL REFERENCES billing.OwnerAccounts(Id),
    Type VARCHAR(20) NOT NULL,  -- 'Credit' or 'Debit'
    Amount DECIMAL(18,2) NOT NULL,
    BalanceAfter DECIMAL(18,2) NOT NULL,
    Description VARCHAR(500) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL,
    PaymentId UUID NULL REFERENCES billing.Payments(Id),
    InvoiceId UUID NULL REFERENCES billing.Invoices(Id),
    CONSTRAINT CHK_Amount_Positive CHECK (Amount > 0)
);

-- Indexes
CREATE INDEX IX_InvoicePayments_PaymentId ON billing.InvoicePayments(PaymentId);
CREATE INDEX IX_InvoicePayments_InvoiceId ON billing.InvoicePayments(InvoiceId);
CREATE INDEX IX_OwnerAccounts_OwnerId ON billing.OwnerAccounts(OwnerId);
CREATE INDEX IX_OwnerAccountTransactions_OwnerAccountId ON billing.OwnerAccountTransactions(OwnerAccountId);
CREATE INDEX IX_OwnerAccountTransactions_CreatedAt ON billing.OwnerAccountTransactions(CreatedAt DESC)
```

---

## 🚀 İmplementation Roadmap

### Phase 1: Foundation ✅ (TAMAMLANMIŞ)
- [x] Common layer (Domain, Application, Infrastructure)
- [x] Identity module (User, Role, Keycloak)
- [x] API təməli
- [x] Swagger/OpenAPI
- [x] Outbox/Inbox pattern
- [x] MTK rolları seed data

### Phase 2: Core Business (4-5 həftə)
- [ ] Buildings module (Building, Apartment, Owner, Garage, OwnershipHistory)
- [ ] Ownership transfer command və business rules
- [ ] Billing module (Invoice, Payment, ServiceRate)
- [ ] Finance module (CashAccount, Transaction)
- [ ] Aylıq faktura generasiya job
- [ ] Payment gateway (Payriff test)
- [ ] Invoice calculation məntiqi
- [ ] Snapshot pattern (Invoice entity)

### Phase 3: Operations (3-4 həftə)
- [ ] Expenses module
- [ ] Employees module
- [ ] Maintenance module (Complaint, WorkOrder)
- [ ] File upload (qəbzlər, şəkillər)
- [ ] Şikayət iş axını

### Phase 4: Engagement (2-3 həftə)
- [ ] Voting module
- [ ] Notifications module (SMS/Email)
- [ ] bTaskım SMS integration
- [ ] Real-time notifications (SignalR - optional)

### Phase 5: Transparency & Reports (2-3 həftə)
- [ ] Documents module
- [ ] PDF generation (QuestPDF)
- [ ] Şəffaflıq dashboard API
- [ ] Financial reports
- [ ] Analytics endpoints

### Phase 6: Polish & Deploy (2 həftə)
- [ ] Admin panel UI (React/Blazor - future)
- [ ] Owner portal UI
- [ ] Unit tests
- [ ] Integration tests
- [ ] Performance testing
- [ ] Production deployment

---

## 🧪 Testing Strategy

```
Unit Tests:
  - Domain entities
  - Value objects
  - Domain events
  - Command handlers
  - Query handlers

Integration Tests:
  - API endpoints
  - Database operations
  - Keycloak integration
  - Payment gateway integration
  - Background jobs

E2E Tests:
  - Critical user flows
  - Ödəniş prosesi
  - Faktura generasiyası
```

---

## 🔐 Security

### Authentication
- Keycloak OAuth2 + PKCE
- JWT Bearer tokens
- Token refresh məntiqi

### Authorization
- Role-based (RBAC)
- Policy-based (Claims)
- Resource-based (Owner-ə məxsus məlumat)

### Data Protection
- HTTPS only
- Password hashing (Keycloak)
- Sensitive data encryption at rest
- SQL injection prevention (EF Core parameterized queries)
- CORS policy

### Audit
- Bütün əməliyyatlar audit log-a yazılır
- Kim, nə, nə zaman

---

## 📈 Performance Considerations

### Caching
- ServiceRate-lər (az dəyişir, çox oxunur) → Redis
- Building structure → Memory cache

### Database Indexing
- Apartment.OwnerId
- Invoice.ApartmentId, Invoice.Status, Invoice.BillingMonth
- Payment.TransactionId
- Expense.Date, Expense.CategoryId
- Complaint.Status, Complaint.CreatedAt

### Background Jobs
- Aylıq faktura generasiya (async, batch processing)
- Keycloak sync (retry məntiqi)
- Failed payment retry

### Query Optimization
- EF Core projections (DTO mapping)
- Pagination (offset/limit)
- CQRS read models (denormalized)

---

## 🛠️ Development Setup

### Prerequisites
```bash
- .NET 9.0 SDK
- PostgreSQL 16+
- Keycloak 26+
- Docker (optional - Keycloak, PostgreSQL containers)
- Visual Studio 2022 / Rider / VS Code
```

### Running Locally
```bash
# Clone repository
git clone [repo-url]
cd MTK

# Restore packages
dotnet restore

# Update database
dotnet ef database update \
  --project src/Modules/Identity/MTK.Modules.Identity.Infrastructure \
  --startup-project src/Apps/MTK.Api

# Run API
cd src/Apps/MTK.Api
dotnet run
```

### Environment Variables
```
ConnectionStrings__Database=Host=localhost;Database=mtk_db;Username=postgres;Password=***
Keycloak__Authority=http://localhost:8080/realms/mtk
Keycloak__ClientId=mtk-api
Keycloak__ClientSecret=***
Payriff__ApiKey=***
Payriff__Merchant=***
BTaskim__ApiKey=***
```

---

## 📚 Key Design Decisions

### 1. Niyə Multi-Tenancy YOXDUR?
**Qərar:** Sistem yalnız BİR MTK üçün nəzərdə tutulub
**Səbəb:**
- Sadə başlamaq
- 3 bina, ~125 mənzil (kiçik scope)
- Gələcəkdə genişlənmə varsa, ayrı deployment

### 2. Niyə Modular Monolith?
**Qərar:** Mikroservislər yox, modular monolit
**Səbəb:**
- Mikroservis kompleksliyi lazım deyil
- Deploy və idarəetmə sadədir
- Modulyarlıq qorunur (gələcək mikroservislərə keçid asan)
- Transaction boundary daxilində

### 3. Niyə Sayğac İdarəetməsi Yoxdur?
**Qərar:** Su/işıq/qaz sayğacları qeyd olunmur
**Səbəb:**
- Ümumi xərc bölgüsü (m² əsasında)
- Implementasiya kompleksliyi azalır
- İstifadəçi tələbində yoxdur (şimdilik)

### 4. Niyə Owner-Centric Model?
**Qərar:** Apartment və Garage Owner-ə bağlıdır
**Səbəb:**
- Bir sahibin çoxlu mənzili ola bilər
- Bir sahibin çoxlu qarajı ola bilər
- Ödəniş Owner bazasında hesablanır
- Real həyat ssenarisini əks etdirir

### 5. Niyə Ümumi Kassa?
**Qərar:** 3 bina üçün bir kassa
**Səbəb:**
- MTK bütün binaları idarə edir
- İşçilər ümumi (hər 3 binaya xidmət edir)
- Sadə maliyyə idarəetməsi

### 6. Sahiblik Transferi (Mənzil Satışı) - Debt Handling ⭐
**Qərar:** Köhnə sahibin borcu köhnə sahibə aiddir, yeni sahibə keçmir
**Səbəb:**
- Ədalətli: Borc yaradanı ödəməlidir
- Mübahisə azalır: Yeni sahibi təmiz başlayır
- MTK praktikası: Transfer yalnız borc 0 AZN olduqda
- Hüquqi aydınlıq: Borc Owner-ə bağlıdır, mənzilə yox

**Alternativ variantlar (rədd edildi):**
- ❌ **Variant B:** Borc mənzilə bağlı, yeni sahibə keçir
  - Problem: Alıcı ədalətsiz borca görə hesablama etməlidir
  - Problem: Satıcı borcdan qaça bilər
  - Problem: MTK-da uğursuz borc yığımı

**İmplementasiya:**
```csharp
// Business Rule
public class OwnerMustHaveNoDebtBeforeTransfer : IBusinessRule
{
    public bool IsBroken() => _outstandingDebt > 0;

    public Error Error => new Error(
        "Ownership.TransferNotAllowed",
        $"Transfer mümkün deyil. Köhnə sahibin {_outstandingDebt} AZN borcu var."
    );
}

// Transfer Validation
if (oldOwner.OutstandingDebt > 0)
    throw new DomainException("Transfer şərti: Borc 0 AZN olmalıdır");

// Sahiblik Tarixçəsi
OwnershipHistory.Create(apartmentId, oldOwnerId, startDate: ..., endDate: transferDate);
OwnershipHistory.Create(apartmentId, newOwnerId, startDate: transferDate, endDate: null);

// Invoice Snapshot
Invoice.OwnerId = oldOwnerId;  // Köhnə fakturalar köhnə sahibdə qalır
Invoice.OwnerName = "Kamran Həsənov";  // Snapshot
```

**Nəticə:**
- ✅ Transfer yalnız borc 0 AZN olduqda
- ✅ Yeni sahibi təmiz başlayır (0 AZN borc)
- ✅ Köhnə sahibin köhnə fakturaları qalır
- ✅ OwnershipHistory tam şəffaflıq verir
- ✅ Invoice entity snapshot pattern (immutable məlumatlar)

### 7. Avans Ödəniş və Partial Payment (Variant B - Complex Model) ⭐⭐⭐
**Qərar:** Junction table (InvoicePayment) + OwnerAccount (avans tracking) + Partial payment support
**Səbəb:**
- Real MTK ssenarisi: Sakin 3 ay qabağa ödəyir
- Nağd ödəniş dəstəyi: Mühasib/komandant qeydə alır
- Bulk payment: Bir ödəniş → çoxlu faktura
- Qismən ödəniş: Owner 100 AZN verir, 3 fakturadan 2-si ödənilir
- Avans avtomatik tətbiqi: Yeni faktura yaradılanda avtomatik avansdan çıxılır

**Alternativ variantlar (rədd edildi):**
- ❌ **Variant A (Sadə):** 1 Payment = 1 Invoice
  - Problem: Avans ödəniş mümkün deyil
  - Problem: Qismən ödəniş olmur
  - Problem: Bulk payment-də N payment record (duplicate TransactionId)

**Entity Strukturu:**
```csharp
// Payment → InvoicePayment ← Invoice (M:N)
Payment.TotalAmount = 240 AZN
  ├─ InvoicePayment: Invoice #1 → 80 AZN
  ├─ InvoicePayment: Invoice #2 → 80 AZN
  └─ Qalan: 80 AZN → OwnerAccount.CreditBalance (avans)

// Invoice partial tracking
Invoice.TotalAmount = 80 AZN
Invoice.PaidAmount = 50 AZN
Invoice.RemainingAmount = 30 AZN
Invoice.Status = PartiallyPaid

// OwnerAccount avans
OwnerAccount.CreditBalance = 160 AZN
  ├─ OwnerAccountTransaction (Credit): +160 AZN
  ├─ OwnerAccountTransaction (Debit):  -80 AZN (Aprel faktura)
  └─ Balance: 80 AZN
```

**Allocation Məntiqi (FIFO):**
```csharp
// Owner 150 AZN ödəyir, 3 faktura var (hər biri 80 AZN)
var remainingAmount = 150 AZN;
var unpaidInvoices = GetUnpaidInvoices(ownerId)
    .OrderBy(i => i.BillingMonth);  // FIFO

foreach (var invoice in unpaidInvoices)
{
    if (remainingAmount <= 0) break;

    var allocate = Math.Min(remainingAmount, invoice.RemainingAmount);

    InvoicePayment.Create(payment, invoice, allocate);
    invoice.PaidAmount += allocate;

    if (invoice.PaidAmount >= invoice.TotalAmount)
        invoice.Status = Paid;
    else
        invoice.Status = PartiallyPaid;

    remainingAmount -= allocate;
}

// Qalan pul → Avans
if (remainingAmount > 0)
    OwnerAccount.AddCredit(remainingAmount);
```

**Avtomatik Avans Tətbiqi (Faktura Generasiya):**
```csharp
// Hər ayın 1-i (Hangfire job)
GenerateMonthlyInvoices()
{
    foreach (var apartment in apartments)
    {
        var invoice = Invoice.Generate(...);

        // Owner-in avansı varsa avtomatik tətbiq et
        var ownerAccount = GetOwnerAccount(apartment.OwnerId);

        if (ownerAccount.CreditBalance > 0)
        {
            var amount = Math.Min(ownerAccount.CreditBalance, invoice.TotalAmount);

            // Virtual payment
            var payment = Payment.CreateFromCredit(owner, amount);
            InvoicePayment.Create(payment, invoice, amount);

            invoice.PaidAmount += amount;
            ownerAccount.DeductCredit(amount);

            if (invoice.PaidAmount >= invoice.TotalAmount)
                invoice.Status = Paid;  // ⭐ Avtomatik ödənilib!
        }
    }
}
```

**Nağd Ödəniş (Cash Payment):**
```csharp
// Mühasib sistemə daxil olur
RecordCashPayment(ownerId, invoiceIds, amount, recordedByUserId)
{
    var payment = Payment.CreateCash(
        owner,
        amount,
        recordedByUserId,
        "Leyla Məmmədova"  // Snapshot
    );

    payment.MarkAsSuccessful(DateTime.UtcNow);  // Dərhal təsdiq

    // Allocation (FIFO)
    AllocatePaymentToInvoices(payment, invoiceIds);

    // PDF qəbz print
    GenerateReceiptPdf(payment);
}
```

**Nəticə:**
- ✅ Avans ödəniş dəstəyi (3-6 ay qabağa)
- ✅ Avtomatik avans tətbiqi (yeni faktura generasiyada)
- ✅ Partial payment (qismən ödəniş)
- ✅ Bulk payment (bir ödəniş → çoxlu faktura)
- ✅ Nağd ödəniş tracking (kim, nə zaman qeydə aldı)
- ✅ OwnerAccount tarixçəsi (tam şəffaflıq)
- ✅ FIFO allocation (ən köhnə fakturadan)

**Mənfi:**
- ❌ Entity strukturu kompleksdir (junction table)
- ❌ Query-lər biraz çətin (JOIN-lər)
- ❌ Rollback məntiqi kompleks (transaction management)
- ⚠️ Performance: Bulk payment 50 faktura ilə məhdudlaşdırılmalı

---

## 🎓 Lessons Learned & Best Practices

### Domain-Driven Design
- Bounded Context-lər aydın təyin olunmalı
- Aggregate-lər kiçik saxlanmalı (performance)
- Domain Event-lər kommunikasiya üçün

### CQRS
- Command-lər immutable
- Query-lər projection ilə (DTO)
- Validation Command-də (FluentValidation)

### Event-Driven
- Integration Event-lər async (Outbox pattern)
- Idempotency (Inbox pattern)
- Retry məntiqi (Polly)

### Database
- Migration-lar versiya kontrol altında
- Seed data migration-da
- Index-lər query pattern-ə görə

---

## 📞 Support & Contacts

**Developer Team:**
- Backend Lead: [Name]
- DevOps: [Name]
- QA: [Name]

**Stakeholders:**
- Product Owner: [Name]
- MTK Representative: [Name]

---

## 📄 License

Proprietary - All rights reserved

---

**Son yeniləmə:** 21 Sentyabr 2026
**Versiya:** 1.0
**Status:** In Development 🚧
