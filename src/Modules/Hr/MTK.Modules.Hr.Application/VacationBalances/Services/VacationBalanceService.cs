using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationBalances.Services;

internal sealed class VacationBalanceService : IVacationBalanceService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IVacationBalanceRepository _vacationBalanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VacationBalanceService(
        IEmployeeRepository employeeRepository,
        IVacationBalanceRepository vacationBalanceRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _vacationBalanceRepository = vacationBalanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VacationBalance>> EnsureCurrentAsync(
        Guid employeeId,
        DateTimeOffset? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<VacationBalance>(EmployeeErrors.NotFound);
        }

        var referenceDate = asOfDate ?? DateTimeOffset.UtcNow;
        var asOfDateOnly = DateOnly.FromDateTime(referenceDate.UtcDateTime);

        var existing = await _vacationBalanceRepository.GetCurrentAsync(employeeId, asOfDateOnly, cancellationToken);
        if (existing is not null)
        {
            return Result.Success(existing);
        }

        var hireDate = DateOnly.FromDateTime(employee.StartWorkDate.UtcDateTime);
        var workYearStart = ResolveWorkYearStart(hireDate, asOfDateOnly);

        // Əvvəlki iş ilinin qalığı — limitsiz keçir (CarriedOverDays).
        var previous = await _vacationBalanceRepository.GetMostRecentBeforeAsync(employeeId, workYearStart, cancellationToken);
        var carriedOverDays = Math.Max(0, previous?.RemainingDays ?? 0);

        var workYearStartOffset = new DateTimeOffset(workYearStart.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var breakdown = VacationEntitlementCalculator.Calculate(employee, workYearStartOffset);

        var balance = VacationBalance.Create(employeeId, workYearStart, employee.VacationDays, breakdown, carriedOverDays);

        _vacationBalanceRepository.Add(balance);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(balance);
    }

    /// <summary>İşçinin işə başlama tarixinin ildönümünə görə, asOfDate-i əhatə edən iş ilinin başlanğıcı.</summary>
    private static DateOnly ResolveWorkYearStart(DateOnly hireDate, DateOnly asOfDate)
    {
        if (asOfDate < hireDate)
        {
            return hireDate;
        }

        var yearsSinceHire = asOfDate.Year - hireDate.Year;
        var candidate = hireDate.AddYears(yearsSinceHire);

        if (candidate > asOfDate)
        {
            candidate = candidate.AddYears(-1);
        }

        return candidate;
    }
}
