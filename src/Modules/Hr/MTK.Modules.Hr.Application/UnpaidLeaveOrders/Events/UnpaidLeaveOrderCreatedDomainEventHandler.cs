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
/// saatlar (bayram/qeyri-iş günləri çıxılmaqla) maaşdan çıxılır.
///
/// Hər ay üçün ayrı-ayrı adjustment yaradılır (çox aylıq məzuniyyət üçün).
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

        // Hər ay üçün ayrı-ayrı adjustment yarat
        var current = start;

        while (current <= end)
        {
            var monthStart = new DateOnly(current.Year, current.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // Həmin aydakı ödənişsiz məzuniyyət dövrünü müəyyənləşdir
            var periodStart = current > monthStart ? current : monthStart;
            var periodEnd = end < monthEnd ? end : monthEnd;

            // Həmin aydakı ödənişsiz məzuniyyət saatlarını hesabla
            var periodCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(
                periodStart, periodEnd, cancellationToken);
            var leaveHoursInMonth = _workHoursCalculator.CalculateNormHours(
                employee, periodStart, periodEnd, periodCalendarDays);

            // Ayın normasını hesabla
            var monthCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(
                monthStart, monthEnd, cancellationToken);
            var normHours = _workHoursCalculator.CalculateNormHours(
                employee, monthStart, monthEnd, monthCalendarDays);

            // Həmin aydan ödənişsiz məzuniyyət saatlarını çıx
            var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                current == start ? domainEvent.OrderId : Guid.NewGuid(), // İlk ay əmrin ID-si, digərləri fərqli
                domainEvent.EmployeeId,
                PayrollAdjustmentReason.UnpaidLeave,
                hours: -leaveHoursInMonth,
                normHours: normHours,
                salaryMonth: current.Month,
                salaryYear: current.Year);

            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            // Növbəti aya keç
            current = monthEnd.AddDays(1);
        }
    }
}
