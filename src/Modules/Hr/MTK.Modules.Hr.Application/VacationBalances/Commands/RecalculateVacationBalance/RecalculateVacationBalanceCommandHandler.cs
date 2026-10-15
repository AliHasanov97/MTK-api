using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationBalances.Commands.RecalculateVacationBalance;

internal sealed class RecalculateVacationBalanceCommandHandler
    : ICommandHandler<RecalculateVacationBalanceCommand, VacationBalanceResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IVacationBalanceService _vacationBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public RecalculateVacationBalanceCommandHandler(
        IEmployeeRepository employeeRepository,
        IVacationBalanceService vacationBalanceService,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _vacationBalanceService = vacationBalanceService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VacationBalanceResponse>> Handle(
        RecalculateVacationBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<VacationBalanceResponse>(EmployeeErrors.NotFound);
        }

        // Cari iş ili üçün sətir yoxdursa yaradır (artıq canlı məlumatla düzgün yaranır);
        // varsa, aşağıda əlavə günləri bir daha, həmin canlı məlumatla üzərinə yazırıq.
        var balanceResult = await _vacationBalanceService.EnsureCurrentAsync(request.EmployeeId, null, cancellationToken);
        if (balanceResult.IsFailure)
        {
            return Result.Failure<VacationBalanceResponse>(balanceResult.Error);
        }

        var balance = balanceResult.Value;
        var workYearStartOffset = new DateTimeOffset(balance.WorkYearStart.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var additionalDays = VacationEntitlementCalculator.Calculate(employee, workYearStartOffset);

        balance.RecalculateAdditionalDays(additionalDays);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(VacationBalanceResponse.FromDomain(balance));
    }
}
