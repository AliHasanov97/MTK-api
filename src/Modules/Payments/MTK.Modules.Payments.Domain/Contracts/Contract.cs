using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Contracts.Events;

namespace MTK.Modules.Payments.Domain.Contracts;

/// <summary>
/// Tədarükçü ilə bağlanan müqavilə (Aggregate Root).
///
/// Şərtlər yalnız bu aggregate daxilindən idarə olunur:
/// <list type="bullet">
/// <item><see cref="ContractService"/> — dövri xidmət; cədvəl üzrə avtomatik borc yaradır.</item>
/// </list>
/// </summary>
public sealed class Contract : SearchableEntity
{
    private readonly List<ContractService> _services = new();

    private Contract() : base() { }

    /// <summary>Müqavilə nömrəsi, məs. "2026/045".</summary>
    public string Number { get; private set; } = string.Empty;

    public Guid VendorId { get; private set; }

    // Xarici modul istinadları: FK yox, yalnız Guid (Charge.OwnerId / PropertyOwnership
    // konvensiyası). Company entity-si gələndə ya Guid saxlanılır, ya da read model sync olunur.
    public Guid? CreatedByUserId { get; private set; }

    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }

    public ContractStatus Status { get; private set; }

    /// <summary>ISO 4217 kodu. Default "AZN".</summary>
    public string Currency { get; private set; } = "AZN";

    public string? Note { get; private set; }

    public IReadOnlyCollection<ContractService> Services => _services.AsReadOnly();

    /// <summary>Aylıq borc yaradan aktiv xidmətlər.</summary>
    public IEnumerable<ContractService> MonthlyServices =>
        _services.Where(s => s.IsActive && s.BillingPeriod == BillingPeriod.Monthly);

    /// <summary>Aylıq yükün cəmi (hesablanan, saxlanılmır).</summary>
    public decimal MonthlyAmount => MonthlyServices.Sum(s => s.PeriodAmount);

    /// <summary>Müddətin bitib-bitmədiyi — status-dan asılı olmayan real vəziyyət.</summary>
    public bool IsExpired => DateTimeOffset.UtcNow.Date > EndDate.Date;

    public bool IsActive => Status == ContractStatus.Active && !IsExpired;

    public static Contract Create(
        string number,
        Guid vendorId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid? createdByUserId = null,
        string? note = null,
        string currency = "AZN")
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Müqavilə nömrəsi mütləqdir", nameof(number));
        if (endDate.Date < startDate.Date)
            throw new ArgumentException("Müqavilənin bitmə tarixi başlanğıcdan əvvəl ola bilməz", nameof(endDate));

        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            Number = number.Trim(),
            VendorId = vendorId,
            CreatedByUserId = createdByUserId,
            StartDate = startDate,
            EndDate = endDate,
            Status = ContractStatus.Draft,
            Currency = currency,
            Note = note
        };

        contract.SetCreatedAt();
        contract.RaiseDomainEvent(new ContractCreatedDomainEvent(contract.Id, vendorId, contract.Number));

        return contract;
    }

    public void UpdateDates(DateTimeOffset startDate, DateTimeOffset endDate, string? note = null)
    {
        if (Status == ContractStatus.Terminated)
            throw new InvalidOperationException("Ləğv edilmiş müqavilə dəyişdirilə bilməz");
        if (endDate.Date < startDate.Date)
            throw new ArgumentException("Müqavilənin bitmə tarixi başlanğıcdan əvvəl ola bilməz", nameof(endDate));

        StartDate = startDate;
        EndDate = endDate;
        Note = note ?? Note;
        SetUpdatedAt();
    }

    // ------------------------------------------------------------------
    // Xidmətlər — yalnız Draft mərhələsində dəyişdirilir. Təsdiqlənmiş
    // qiyməti sonradan "sakitcə" dəyişmək auditi mənasız edər.
    // ------------------------------------------------------------------

    public ContractService AddService(
        string name,
        decimal unitPrice,
        BillingPeriod billingPeriod = BillingPeriod.Monthly,
        decimal quantity = 1,
        string unit = "ay",
        string? description = null,
        DateTimeOffset? serviceStartDate = null,
        DateTimeOffset? serviceEndDate = null,
        int? paymentTermDays = null)
    {
        EnsureEditable();

        var service = ContractService.Create(
            Id, name, unitPrice, billingPeriod, quantity, unit, description,
            serviceStartDate, serviceEndDate, paymentTermDays);

        _services.Add(service);
        SetUpdatedAt();

        return service;
    }

    public void UpdateService(
        Guid serviceId,
        string name,
        decimal unitPrice,
        BillingPeriod billingPeriod,
        decimal quantity,
        string unit,
        string? description = null,
        DateTimeOffset? serviceStartDate = null,
        DateTimeOffset? serviceEndDate = null,
        int? paymentTermDays = null)
    {
        EnsureEditable();

        Service(serviceId).Update(
            name, unitPrice, billingPeriod, quantity, unit, description,
            serviceStartDate, serviceEndDate, paymentTermDays);

        SetUpdatedAt();
    }

    public void RemoveService(Guid serviceId)
    {
        EnsureEditable();

        _services.Remove(Service(serviceId));
        SetUpdatedAt();
    }

    /// <summary>
    /// Tək xidməti dayandırır — müqavilə aktiv qalır, həmin xidmətdən artıq
    /// borc yaranmır. Draft məhdudiyyəti yoxdur: dayandırma qanuni əməliyyatdır.
    /// </summary>
    public void DeactivateService(Guid serviceId)
    {
        Service(serviceId).Deactivate();
        SetUpdatedAt();
    }

    public void ActivateService(Guid serviceId)
    {
        Service(serviceId).Activate();
        SetUpdatedAt();
    }

    // ------------------------------------------------------------------
    // Status keçidləri
    // ------------------------------------------------------------------

    public void Activate()
    {
        if (Status == ContractStatus.Active)
            return;

        if (_services.Count == 0)
            throw new InvalidOperationException(
                "Müqavilə aktivləşməzdən əvvəl ən azı bir xidmət olmalıdır");

        Status = ContractStatus.Active;
        SetUpdatedAt();

        RaiseDomainEvent(new ContractActivatedDomainEvent(Id, VendorId, Number));
    }

    public void Suspend(string? note = null)
    {
        if (Status != ContractStatus.Active)
            throw new InvalidOperationException("Yalnız aktiv müqavilə dayandırıla bilər");

        Status = ContractStatus.Suspended;
        Note = note ?? Note;
        SetUpdatedAt();
    }

    public void Terminate(DateTimeOffset terminatedOn, string? note = null)
    {
        if (Status == ContractStatus.Terminated)
            return;

        Status = ContractStatus.Terminated;
        EndDate = terminatedOn;
        Note = note ?? Note;
        SetUpdatedAt();

        RaiseDomainEvent(new ContractTerminatedDomainEvent(Id, VendorId, Number, terminatedOn));
    }

    /// <summary>
    /// Aktiv müqavilə silinmir — əvvəlcə <see cref="Terminate"/> çağırılmalıdır.
    /// </summary>
    public void Delete()
    {
        if (Status == ContractStatus.Active)
            throw new InvalidOperationException("Aktiv müqavilə ləğv edilmədən silinə bilməz");

        SetDeletedAt();
    }

    private ContractService Service(Guid serviceId)
    {
        return _services.FirstOrDefault(s => s.Id == serviceId)
            ?? throw new InvalidOperationException($"Xidmət tapılmadı: {serviceId}");
    }

    private void EnsureEditable()
    {
        if (Status != ContractStatus.Draft)
            throw new InvalidOperationException("Xidmətlər yalnız Draft mərhələsində dəyişdirilə bilər");
    }
}
