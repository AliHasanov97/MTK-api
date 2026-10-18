using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.VacationPays.Events;

namespace MTK.Modules.Payments.Domain.VacationPays;

/// <summary>
/// Məzuniyyət haqqı (Azərbaycan Əmək Məcəlləsinə uyğun hesablanmış).
/// HR modulundan VacationOrder yaradılanda avtomatik yaranır.
/// Məzuniyyətdən ən azı 3 gün əvvəl ödənilməlidir.
/// </summary>
public sealed class VacationPay : Entity
{
    private VacationPay() : base() { }

    /// <summary>
    /// HR modulundakı VacationOrder-in ID-si (idempotency key).
    /// </summary>
    public Guid VacationOrderId { get; private set; }

    /// <summary>
    /// İşçinin ID-si.
    /// </summary>
    public Guid EmployeeId { get; private set; }

    /// <summary>
    /// İşçinin adı (snapshot - dəyişməz).
    /// </summary>
    public string EmployeeFullName { get; private set; } = string.Empty;

    /// <summary>
    /// Orta aylıq əmək haqqı (son 12 ayın ortası).
    /// </summary>
    public decimal AverageMonthlyWage { get; private set; }

    /// <summary>
    /// Orta günlük əmək haqqı (orta aylıq ÷ 30.4).
    /// </summary>
    public decimal AverageDailyWage { get; private set; }

    /// <summary>
    /// Məzuniyyət günlərinin sayı (təqvim günləri).
    /// </summary>
    public int VacationDays { get; private set; }

    /// <summary>
    /// Ümumi məzuniyyət haqqı (orta günlük × günlər).
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Məzuniyyətin başlanğıc tarixi.
    /// </summary>
    public DateOnly VacationStartDate { get; private set; }

    /// <summary>
    /// Məzuniyyətin bitmə tarixi.
    /// </summary>
    public DateOnly VacationEndDate { get; private set; }

    /// <summary>
    /// Ödəniş statusu.
    /// </summary>
    public VacationPayStatus Status { get; private set; }

    /// <summary>
    /// Ödənilməli olan tarix (məzuniyyət başlanğıcından 3 gün əvvəl).
    /// Azərbaycan Əmək Məcəlləsinə (Maddə 146) uyğun.
    /// </summary>
    public DateOnly DueDate { get; private set; }

    /// <summary>
    /// Ödəniş tarixi (ödənildikdə doldurulur).
    /// </summary>
    public DateTimeOffset? PaidAt { get; private set; }

    /// <summary>
    /// Ödəniş üsulu (ödənildikdə doldurulur).
    /// </summary>
    public PaymentMethod? PaymentMethod { get; private set; }

    /// <summary>
    /// Məzuniyyət haqqı yarat.
    /// </summary>
    public static VacationPay Create(
        Guid vacationOrderId,
        Guid employeeId,
        string employeeFullName,
        decimal averageMonthlyWage,
        decimal averageDailyWage,
        int vacationDays,
        decimal totalAmount,
        DateOnly vacationStartDate,
        DateOnly vacationEndDate,
        DateOnly dueDate)
    {
        if (string.IsNullOrWhiteSpace(employeeFullName))
        {
            throw new ArgumentException("İşçi adı boş ola bilməz", nameof(employeeFullName));
        }

        if (averageMonthlyWage < 0)
        {
            throw new ArgumentException("Orta aylıq əmək haqqı mənfi ola bilməz", nameof(averageMonthlyWage));
        }

        if (averageDailyWage < 0)
        {
            throw new ArgumentException("Orta günlük əmək haqqı mənfi ola bilməz", nameof(averageDailyWage));
        }

        if (vacationDays <= 0)
        {
            throw new ArgumentException("Məzuniyyət günləri 0-dan böyük olmalıdır", nameof(vacationDays));
        }

        if (totalAmount < 0)
        {
            throw new ArgumentException("Ümumi məbləğ mənfi ola bilməz", nameof(totalAmount));
        }

        if (vacationEndDate < vacationStartDate)
        {
            throw new ArgumentException("Bitmə tarixi başlanğıcdan əvvəl ola bilməz", nameof(vacationEndDate));
        }

        var vacationPay = new VacationPay
        {
            VacationOrderId = vacationOrderId,
            EmployeeId = employeeId,
            EmployeeFullName = employeeFullName.Trim(),
            AverageMonthlyWage = averageMonthlyWage,
            AverageDailyWage = averageDailyWage,
            VacationDays = vacationDays,
            TotalAmount = totalAmount,
            VacationStartDate = vacationStartDate,
            VacationEndDate = vacationEndDate,
            Status = VacationPayStatus.Pending,
            DueDate = dueDate
        };

        vacationPay.SetCreatedAt();

        // Domain event - transaction yaradılması üçün
        vacationPay.RaiseDomainEvent(new VacationPayCreatedDomainEvent(vacationPay.Id));

        return vacationPay;
    }

    /// <summary>
    /// Məzuniyyət haqqını ödənilmiş kimi işarələ.
    /// Transaction domain event handler tərəfindən yaradılacaq.
    /// </summary>
    public void MarkAsPaid(PaymentMethod paymentMethod, DateTimeOffset paidAt)
    {
        if (Status == VacationPayStatus.Paid)
        {
            throw new InvalidOperationException($"Məzuniyyət haqqı artıq ödənilib ({PaidAt:yyyy-MM-dd HH:mm})");
        }

        Status = VacationPayStatus.Paid;
        PaymentMethod = paymentMethod;
        PaidAt = paidAt;
        SetUpdatedAt();

        // Domain event - transaction yaradılması üçün
        RaiseDomainEvent(new VacationPayPaidDomainEvent(
            Id,
            EmployeeId,
            TotalAmount,
            paymentMethod));
    }
}
