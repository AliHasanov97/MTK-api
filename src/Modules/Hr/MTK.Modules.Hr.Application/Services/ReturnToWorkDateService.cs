using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;

namespace MTK.Modules.Hr.Application.Services;

public class ReturnToWorkDateService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly WorkingDayCalculatorService _workingDayCalculator;

    public ReturnToWorkDateService(
        IEmployeeRepository employeeRepository,
        ICalendarDayRepository calendarDayRepository,
        WorkingDayCalculatorService workingDayCalculator)
    {
        _employeeRepository = employeeRepository;
        _calendarDayRepository = calendarDayRepository;
        _workingDayCalculator = workingDayCalculator;
    }

    /// <summary>
    /// İşçinin işə qayıtma tarixini hesablayır.
    /// Employee tapılmazsa Error qaytarır.
    /// </summary>
    public async Task<Result<DateOnly>> CalculateAsync(
        Guid employeeId,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(
            employeeId,
            cancellationToken);

        if (employee == null)
            return Result.Failure<DateOnly>(EmployeeErrors.NotFound);

        return await CalculateAsync(employee, endDate, cancellationToken);
    }

    /// <summary>
    /// İşçinin işə qayıtma tarixini hesablayır (Employee artıq əldə edilib).
    /// </summary>
    public async Task<DateOnly> CalculateAsync(
        Employee employee,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var searchEndDate = endDate.AddDays(30);

        var calendarDays = await _calendarDayRepository.GetByDateRangeAsync(
            endDate,
            searchEndDate,
            cancellationToken);

        return _workingDayCalculator.CalculateReturnToWorkDate(
            endDate,
            employee.WorkingDays ?? WorkingDays.FiveDays,
            calendarDays);
    }

    /// <summary>
    /// İşçinin işə qayıtma tarixini hesablayır (nullable EndDate üçün).
    /// EndDate null olarsa, null qaytarır.
    /// </summary>
    public async Task<Result<DateOnly?>> CalculateIfEndDateExistsAsync(
        Guid employeeId,
        DateTimeOffset? endDate,
        CancellationToken cancellationToken = default)
    {
        if (!endDate.HasValue)
            return Result.Success<DateOnly?>(null);

        var result = await CalculateAsync(
            employeeId,
            DateOnly.FromDateTime(endDate.Value.UtcDateTime),
            cancellationToken);

        if (result.IsFailure)
            return Result.Failure<DateOnly?>(result.Error);

        return Result.Success<DateOnly?>(result.Value);
    }
}
