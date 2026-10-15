using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationBalances.Events;

/// <summary>
/// İş tarixçəsi (əvvəlki iş yeri) əlavə/yenilənəndə/silinəndə ümumi iş stajı dəyişə bilər, bu da
/// Maddə 115 staj bonusuna (5/10/15 il → +2/+4/+6 gün) təsir edir. VacationBalance sətri "lazy" —
/// yalnız bir dəfə yaradılıb sonra snapshot kimi saxlanıldığı üçün (bax: VacationBalanceService),
/// staj dəyişəndə CARİ (bu gün daxil olan) iş ilinin balansı özbaşına yenilənmirdi. Bu handler
/// məhz bunu düzəldir: hər EmployeeWorkHistoryChangedDomainEvent-də cari balansı tapıb əlavə
/// günləri yenidən hesablayır.
///
/// Qəsdən yalnız CARİ iş ilinə toxunur — bağlanmış (keçmiş) iş illəri və hələ yaradılmamış gələcək
/// iş illəri dəyişməz qalır: keçmiş illər artıq "bağlanmış" uçotdur, gələcək il isə öz vaxtında,
/// canlı məlumatla (bax: VacationBalanceService.EnsureCurrentAsync) düzgün yaradılacaq.
/// </summary>
internal sealed class EmployeeWorkHistoryChangedDomainEventHandler : DomainEventHandler<EmployeeWorkHistoryChangedDomainEvent>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IVacationBalanceRepository _vacationBalanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeWorkHistoryChangedDomainEventHandler(
        IEmployeeRepository employeeRepository,
        IVacationBalanceRepository vacationBalanceRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _vacationBalanceRepository = vacationBalanceRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        EmployeeWorkHistoryChangedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(domainEvent.EmployeeId, cancellationToken);
        if (employee is null)
        {
            // İşçi silinib ola bilər (nadir yarış vəziyyəti) — yenidən hesablamağa ehtiyac yoxdur.
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var balance = await _vacationBalanceRepository.GetCurrentAsync(domainEvent.EmployeeId, today, cancellationToken);
        if (balance is null)
        {
            // Cari iş ili üçün balans sətri hələ yaradılmayıb — növbəti dəfə lazım olanda
            // (ərizə/əmr/sorğu) canlı məlumatla onsuz da düzgün yaradılacaq.
            return;
        }

        var workYearStartOffset = new DateTimeOffset(balance.WorkYearStart.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var additionalDays = VacationEntitlementCalculator.Calculate(employee, workYearStartOffset);

        balance.RecalculateAdditionalDays(additionalDays);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
