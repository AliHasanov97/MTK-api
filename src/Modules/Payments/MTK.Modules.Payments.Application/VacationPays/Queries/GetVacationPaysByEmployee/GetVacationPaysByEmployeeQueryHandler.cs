using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPaysByEmployee;

internal sealed class GetVacationPaysByEmployeeQueryHandler
    : IQueryHandler<GetVacationPaysByEmployeeQuery, IEnumerable<VacationPayResponse>>
{
    private readonly IVacationPayRepository _vacationPayRepository;

    public GetVacationPaysByEmployeeQueryHandler(IVacationPayRepository vacationPayRepository)
    {
        _vacationPayRepository = vacationPayRepository;
    }

    public async Task<Result<IEnumerable<VacationPayResponse>>> Handle(
        GetVacationPaysByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var vacationPays = await _vacationPayRepository.GetByEmployeeIdAsync(
            request.EmployeeId, cancellationToken);

        var response = vacationPays.Select(vp => new VacationPayResponse(
            vp.Id,
            vp.VacationOrderId,
            vp.EmployeeId,
            vp.EmployeeFullName,
            vp.AverageMonthlyWage,
            vp.AverageDailyWage,
            vp.VacationDays,
            vp.TotalAmount,
            vp.VacationStartDate,
            vp.VacationEndDate,
            vp.Status,
            vp.DueDate,
            vp.PaidAt,
            vp.PaymentMethod,
            vp.CreatedAt,
            vp.UpdatedAt));

        return Result.Success(response);
    }
}
