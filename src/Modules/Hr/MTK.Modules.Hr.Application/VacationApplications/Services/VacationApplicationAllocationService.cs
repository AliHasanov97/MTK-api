using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using MTK.Modules.Hr.Domain.VacationApplications;


namespace MTK.Modules.Hr.Application.VacationApplications.Services;

public sealed class VacationApplicationAllocationService : IVacationApplicationAllocationService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly WorkingDayCalculatorService _workingDayCalculator;

    public VacationApplicationAllocationService(
        IEmployeeRepository employeeRepository,
        ICalendarDayRepository calendarDayRepository,
        WorkingDayCalculatorService workingDayCalculator)
    {
        _employeeRepository = employeeRepository;
        _calendarDayRepository = calendarDayRepository;
        _workingDayCalculator = workingDayCalculator;
    }

    public async Task<Result<(int RequestedDays, DateTimeOffset EndDate)>> CalculateDaysAndEndDateAsync(
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        int? requestedDays,
        CancellationToken cancellationToken = default)
    {
        var startDateOnly = DateOnly.FromDateTime(startDate.UtcDateTime);

        // İşçinin iş qrafikini əldə et (əvəzləmə bayramları üçün lazımdır)
        var employee = await _employeeRepository.GetByIdDefaultAsync(employeeId, cancellationToken);
        if (employee == null)
            return Result.Failure<(int, DateTimeOffset)>(
                new Error("Employee.NotFound", "İşçi tapılmadı"));

        var workingDays = employee.WorkingDays ?? WorkingDays.FiveDays;

        if (endDate.HasValue)
        {
            // Variant 1: EndDate verildi → məzuniyyət günlərini hesabla (həftəsonları sayılır, bayramlar çıxılır)
            var endDateOnly = DateOnly.FromDateTime(endDate.Value.UtcDateTime);

            if (endDateOnly < startDateOnly)
                return Result.Failure<(int, DateTimeOffset)>(
                    VacationApplicationErrors.InvalidRequestedDays);

            // CalendarDays-i əldə et (bayramlar)
            var calendarDays = await _calendarDayRepository.GetByDateRangeAsync(
                startDateOnly,
                endDateOnly,
                cancellationToken);

            // Məzuniyyət günlərini say (həftəsonları sayılır, yalnız bayramlar çıxılır)
            // İşçinin iş qrafikini nəzərə al - əvəzləmə bayramları fərqli ola bilər
            var calculatedDays = _workingDayCalculator.CountVacationDays(
                startDateOnly,
                endDateOnly,
                workingDays,
                calendarDays);

            if (calculatedDays <= 0)
                return Result.Failure<(int, DateTimeOffset)>(
                    VacationApplicationErrors.InvalidRequestedDays);

            return Result.Success((calculatedDays, endDate.Value));
        }

        if (requestedDays.HasValue)
        {
            // Variant 2: RequestedDays verildi → EndDate hesabla (yalnız bayramları atlayaraq)
            var calculatedDays = requestedDays.Value;

            if (calculatedDays <= 0)
                return Result.Failure<(int, DateTimeOffset)>(
                    VacationApplicationErrors.InvalidRequestedDays);

            // CalendarDays-i əldə et (geniş aralıq - bayramları da nəzərə al)
            var searchEndDate = startDateOnly.AddDays(calculatedDays + 60); // +60 gün buffer
            var calendarDays = await _calendarDayRepository.GetByDateRangeAsync(
                startDateOnly,
                searchEndDate,
                cancellationToken);

            // EndDate hesabla (yalnız bayramları atlayaraq, həftəsonları sayılır)
            // İşçinin iş qrafikini nəzərə al - əvəzləmə bayramları fərqli ola bilər
            var calculatedEndDate = _workingDayCalculator.CalculateVacationEndDate(
                startDateOnly,
                calculatedDays,
                workingDays,
                calendarDays);

            var endDateOffset = new DateTimeOffset(
                calculatedEndDate.Year,
                calculatedEndDate.Month,
                calculatedEndDate.Day,
                0, 0, 0,
                TimeSpan.Zero);

            return Result.Success((calculatedDays, endDateOffset));
        }

        // Heç biri verilməyib
        return Result.Failure<(int, DateTimeOffset)>(
            VacationApplicationErrors.NeitherEndDateNorRequestedDaysProvided);
    }
}
