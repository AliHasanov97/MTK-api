using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders.Events;
using MTK.Modules.Hr.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.Events;

/// <summary>
/// Ödənişsiz məzuniyyət əmri. İşçinin qrafikinə görə məzuniyyət günlərində işləməli olduğu
/// saatlar (bayram/qeyri-iş günləri çıxılmaqla) maaşdan çıxılır. Düzəliş işarəli saat ilə
/// və həmin ayın iş saatı norması ilə birlikdə göndərilir (saatlıq dərəcə hesablanması üçün).
/// </summary>
internal sealed class UnpaidLeaveOrderCreatedDomainEventHandler
    : DomainEventHandler<UnpaidLeaveOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly WorkHoursCalculatorService _workHoursCalculator;

    public UnpaidLeaveOrderCreatedDomainEventHandler(
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
        UnpaidLeaveOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithWorkSchedulesAsync(domainEvent.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return;
        }

        var start = DateOnly.FromDateTime(domainEvent.StartDate.UtcDateTime);
        var end = DateOnly.FromDateTime(domainEvent.EndDate.UtcDateTime);

        var leaveCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(start, end, cancellationToken);
        var leaveHours = _workHoursCalculator.CalculateNormHours(employee, start, end, leaveCalendarDays);

        var monthStart = new DateOnly(start.Year, start.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var monthCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(monthStart, monthEnd, cancellationToken);
        var normHours = _workHoursCalculator.CalculateNormHours(employee, monthStart, monthEnd, monthCalendarDays);

        var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OrderId,
            domainEvent.EmployeeId,
            PayrollAdjustmentReason.UnpaidLeave,
            hours: -leaveHours,
            normHours: normHours,
            salaryMonth: start.Month,
            salaryYear: start.Year);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
