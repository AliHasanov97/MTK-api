using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.SalaryPayments.Events;

namespace MTK.Modules.Payments.Domain.SalaryPayments;

/// <summary>
/// Maaş ödənişi (manual, işçi-başına).
/// Aggregate root - SalaryPaymentLine child entity-lərinə malikdir.
/// </summary>
public sealed class SalaryPayment : SearchableEntity
{
    private readonly List<SalaryPaymentLine> _lines = new();

    private SalaryPayment() : base() { }

    public Guid EmployeeId { get; private set; }

    /// <summary>Snapshot - işçinin adı dəyişsə belə, tarixçə düzgün qalır.</summary>
    public string EmployeeFullName { get; private set; } = string.Empty;

    /// <summary>Maaş ayı (1-12).</summary>
    public int SalaryMonth { get; private set; }

    public int SalaryYear { get; private set; }

    /// <summary>Əsas maaş.</summary>
    public decimal BaseSalary { get; private set; }

    /// <summary>Bonusların cəmi (denormalized).</summary>
    public decimal TotalBonuses { get; private set; }

    /// <summary>Düzəlişlərin cəmi - işarəli (denormalized).</summary>
    public decimal TotalAdjustments { get; private set; }

    /// <summary>Ümumi maaş (denormalized).</summary>
    public decimal GrossSalary { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }
    public DateTimeOffset PaymentDate { get; private set; }
    public SalaryPaymentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<SalaryPaymentLine> Lines => _lines.AsReadOnly();

    /// <summary>
    /// Yeni maaş ödənişi yarat (Draft statusunda).
    /// </summary>
    public static SalaryPayment Create(
        Guid employeeId,
        string employeeFullName,
        int salaryMonth,
        int salaryYear,
        decimal baseSalary,
        DateTimeOffset paymentDate,
        PaymentMethod paymentMethod,
        string? notes = null)
    {
        // Validation
        if (salaryMonth < 1 || salaryMonth > 12)
        {
            throw new ArgumentException("Maaş ayı 1-12 arasında olmalıdır", nameof(salaryMonth));
        }

        if (baseSalary < 0)
        {
            throw new ArgumentException("Əsas maaş mənfi ola bilməz", nameof(baseSalary));
        }

        if (string.IsNullOrWhiteSpace(employeeFullName))
        {
            throw new ArgumentException("İşçi adı boş ola bilməz", nameof(employeeFullName));
        }

        var payment = new SalaryPayment
        {
            EmployeeId = employeeId,
            EmployeeFullName = employeeFullName.Trim(),
            SalaryMonth = salaryMonth,
            SalaryYear = salaryYear,
            BaseSalary = baseSalary,
            PaymentDate = paymentDate,
            PaymentMethod = paymentMethod,
            Status = SalaryPaymentStatus.Draft,
            Notes = notes,
            TotalBonuses = 0,
            TotalAdjustments = 0,
            GrossSalary = baseSalary
        };

        payment.SetCreatedAt();
        return payment;
    }

    /// <summary>
    /// Ödənişə line əlavə et (bonus, adjustment və s.).
    /// Yalnız Draft statusunda mümkündür.
    /// </summary>
    public SalaryPaymentLine AddLine(
        SalaryPaymentLineType lineType,
        string description,
        decimal amount,
        Guid? sourceId = null)
    {
        if (Status != SalaryPaymentStatus.Draft)
        {
            throw new InvalidOperationException("Yalnız Draft statusunda line əlavə edilə bilər");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Təsvir boş ola bilməz", nameof(description));
        }

        var line = SalaryPaymentLine.Create(Id, lineType, description, amount, sourceId);
        _lines.Add(line);
        RecalculateTotals();
        SetUpdatedAt();

        return line;
    }

    /// <summary>
    /// Line sil.
    /// Yalnız Draft statusunda mümkündür.
    /// </summary>
    public void RemoveLine(Guid lineId)
    {
        if (Status != SalaryPaymentStatus.Draft)
        {
            throw new InvalidOperationException("Yalnız Draft statusunda line silinə bilər");
        }

        _lines.RemoveAll(l => l.Id == lineId);
        RecalculateTotals();
        SetUpdatedAt();
    }

    /// <summary>
    /// Cəmləri yenidən hesabla.
    /// </summary>
    private void RecalculateTotals()
    {
        TotalBonuses = _lines
            .Where(l => l.LineType == SalaryPaymentLineType.Bonus)
            .Sum(l => l.Amount);

        TotalAdjustments = _lines
            .Where(l => l.LineType == SalaryPaymentLineType.Adjustment)
            .Sum(l => l.Amount);

        GrossSalary = BaseSalary + TotalBonuses + TotalAdjustments;
    }

    /// <summary>
    /// Ödənişi Paid statusuna keçir və domain event raise edir.
    /// Transaction-ların yaradılması üçün event handler tətbiq olunur.
    /// </summary>
    public void MarkAsPaid()
    {
        if (Status != SalaryPaymentStatus.Draft)
        {
            throw new InvalidOperationException("Yalnız Draft statusundan Paid-ə keçə bilər");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Ən azı bir line olmalıdır");
        }

        Status = SalaryPaymentStatus.Paid;
        SetUpdatedAt();

        RaiseDomainEvent(new SalaryPaymentPaidDomainEvent(
            Id,
            EmployeeId,
            SalaryMonth,
            SalaryYear,
            GrossSalary));
    }
}
