using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using MTK.Modules.Hr.Domain.UnexcusedAbsences.Events;
using MTK.Modules.Hr.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.Events;

/// <summary>
/// Üzrsüz işdən qalma. Hər əmr bir günü təmsil edir; həmin gün işçinin qrafikinə görə
/// işləməli olduğu saatlar maaşdan çıxılır. Düzəliş işarəli saat və ayın iş saatı norması ilə göndərilir.
/// </summary>
internal sealed class UnexcusedAbsenceCreatedDomainEventHandler
    : DomainEventHandler<UnexcusedAbsenceCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly WorkHoursCalculatorService _workHoursCalculator;

    public UnexcusedAbsenceCreatedDomainEventHandler(
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
        UnexcusedAbsenceCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithWorkSchedulesAsync(domainEvent.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return;
        }

        var date = DateOnly.FromDateTime(domainEvent.SetDate.UtcDateTime);

        var dayCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(date, date, cancellationToken);
        var absenceHours = _workHoursCalculator.CalculateNormHours(employee, date, date, dayCalendarDays);

        var monthStart = new DateOnly(date.Year, date.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var monthCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(monthStart, monthEnd, cancellationToken);
        var normHours = _workHoursCalculator.CalculateNormHours(employee, monthStart, monthEnd, monthCalendarDays);

        var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.UnexcusedAbsenceId,
            domainEvent.EmployeeId,
            PayrollAdjustmentReason.UnexcusedAbsence,
            hours: -absenceHours,
            normHours: normHours,
            salaryMonth: date.Month,
            salaryYear: date.Year);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
