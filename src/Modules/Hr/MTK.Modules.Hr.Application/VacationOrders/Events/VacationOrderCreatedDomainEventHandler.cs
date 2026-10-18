using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using MTK.Modules.Hr.Domain.VacationOrders.Events;
using MTK.Modules.Hr.IntegrationEvents.Payroll;
using MTK.Modules.Hr.IntegrationEvents.VacationPays;

namespace MTK.Modules.Hr.Application.VacationOrders.Events;

/// <summary>
/// Ödənişli məzuniyyət əmri.
///
/// YENİ Məntiq (Azərbaycan Əmək Məcəlləsi Maddə 145-146):
/// 1. VacationPayCreatedIntegrationEvent göndərilir - Payments modulunda ayrıca ödənilir
/// 2. Hər aydan həmin ayın məzuniyyət günləri maaşdan çıxılır (adjustment)
///
/// Məzuniyyət haqqı artıq maaşa adjustment kimi əlavə edilmir - ayrıca VacationPay entity-si yaradılır.
/// Bu, qanunvericiliyə uyğun orta əmək haqqı (12 ay) əsasında hesablamaya imkan verir.
/// </summary>
internal sealed class VacationOrderCreatedDomainEventHandler : DomainEventHandler<VacationOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly WorkHoursCalculatorService _workHoursCalculator;

    public VacationOrderCreatedDomainEventHandler(
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
        VacationOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithWorkSchedulesAsync(
            domainEvent.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return;
        }

        var start = DateOnly.FromDateTime(domainEvent.StartDate.UtcDateTime);
        var end = DateOnly.FromDateTime(domainEvent.EndDate.UtcDateTime);

        // 1. Payments moduluna məzuniyyət haqqı yaradılması tələbi göndər
        // Orta əmək haqqı və məbləğ Payments modulunda hesablanacaq
        var vacationPayEvent = new VacationPayCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OrderId,
            domainEvent.EmployeeId,
            employee.FullName,
            domainEvent.VacationDays,
            domainEvent.StartDate,
            domainEvent.EndDate);

        await _eventBus.PublishAsync(vacationPayEvent, cancellationToken);

        // 2. Hər ay üçün o ayın məzuniyyət günlərini maaşdan çıx (köhnə məntiq saxlanılır)
        await DeductVacationDaysPerMonth(domainEvent, employee, start, end, cancellationToken);
    }

    /// <summary>
    /// Hər ay üçün o ayın məzuniyyət günlərini maaşdan çıxır.
    /// Məzuniyyət haqqı artıq ayrıca ödənilir (VacationPay), buna görə yalnız çıxılma edilir.
    /// </summary>
    private async Task DeductVacationDaysPerMonth(
        VacationOrderCreatedDomainEvent domainEvent,
        Employee employee,
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken)
    {
        var current = start;

        while (current <= end)
        {
            var monthStart = new DateOnly(current.Year, current.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // Həmin aydakı məzuniyyət dövrünü müəyyənləşdir
            var periodStart = current > monthStart ? current : monthStart;
            var periodEnd = end < monthEnd ? end : monthEnd;

            // Həmin aydakı məzuniyyət saatlarını hesabla
            var periodCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(
                periodStart, periodEnd, cancellationToken);
            var vacationHoursInMonth = _workHoursCalculator.CalculateNormHours(
                employee, periodStart, periodEnd, periodCalendarDays);

            // Ayın normasını hesabla
            var monthCalendarDays = await _calendarDayRepository.GetByDateRangeAsync(
                monthStart, monthEnd, cancellationToken);
            var normHours = _workHoursCalculator.CalculateNormHours(
                employee, monthStart, monthEnd, monthCalendarDays);

            // Həmin aydan məzuniyyət saatlarını çıx
            var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                Guid.NewGuid(), // Hər ay üçün fərqli ID
                domainEvent.EmployeeId,
                PayrollAdjustmentReason.Vacation,
                hours: -vacationHoursInMonth,  // Mənfi - maaşdan çıxılma
                normHours: normHours,
                salaryMonth: current.Month,
                salaryYear: current.Year);

            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            // Növbəti aya keç
            current = monthEnd.AddDays(1);
        }
    }
}
