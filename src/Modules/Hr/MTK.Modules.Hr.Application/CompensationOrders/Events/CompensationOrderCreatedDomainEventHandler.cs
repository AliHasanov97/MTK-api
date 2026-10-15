using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.CompensationOrders.Events;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using MTK.Modules.Hr.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Application.CompensationOrders.Events;

/// <summary>
/// Məzuniyyət kompensasiyası əmri — istifadə olunmamış məzuniyyət günlərinin pul əvəzi.
/// İşçiyə ödəniləcək məbləğdir; günlər standart günlük saatla saata çevrilir və cari ayın
/// iş saatı norması ilə birlikdə göndərilir (pul = baza maaş × saat / norma).
/// </summary>
internal sealed class CompensationOrderCreatedDomainEventHandler
    : DomainEventHandler<CompensationOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly WorkHoursCalculatorService _workHoursCalculator;

    public CompensationOrderCreatedDomainEventHandler(
        IEventBus eventBus,
        IEmployeeRepository employeeRepository,
        ICalendarDayRepository calendarDayRepository,
        WorkHoursCalculatorService workHoursCalculator)
    {
        _eventBus = eventBus;
        _employeeRepository = employeeRepository;
        _calendarDayRepository = calendarDayRepository;
        _workHoursCalculator = workHoursCalculator;
    }

    public override async Task Handle(
        CompensationOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithWorkSchedulesAsync(domainEvent.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return;
        }

        // Kompensasiya cari ayın əmək haqqı ilə birlikdə ödənilir (əmrdə tarix yoxdur).
        var now = DateTime.UtcNow;
        var monthStart = new DateOnly(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var monthCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(monthStart, monthEnd, cancellationToken);
        var normHours = _workHoursCalculator.CalculateNormHours(employee, monthStart, monthEnd, monthCalendarDays);

        // Günlər standart günlük saatla saata çevrilir (məzuniyyət günləri tam iş günü sayılır).
        var compensationHours = domainEvent.CompensatedDays * WorkHoursCalculatorService.DefaultDailyHours;

        var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
            Guid.NewGuid(),
            now,
            domainEvent.OrderId,
            domainEvent.EmployeeId,
            PayrollAdjustmentReason.VacationCompensation,
            hours: compensationHours,
            normHours: normHours,
            salaryMonth: now.Month,
            salaryYear: now.Year);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
