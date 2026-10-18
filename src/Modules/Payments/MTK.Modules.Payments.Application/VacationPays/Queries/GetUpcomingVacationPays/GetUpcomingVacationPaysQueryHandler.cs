using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.GetUpcomingVacationPays;

internal sealed class GetUpcomingVacationPaysQueryHandler
    : IQueryHandler<GetUpcomingVacationPaysQuery, List<VacationPayResponse>>
{
    private readonly IVacationPayRepository _vacationPayRepository;

    public GetUpcomingVacationPaysQueryHandler(IVacationPayRepository vacationPayRepository)
    {
        _vacationPayRepository = vacationPayRepository;
    }

    public async Task<Result<List<VacationPayResponse>>> Handle(
        GetUpcomingVacationPaysQuery request,
        CancellationToken cancellationToken)
    {
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(request.DaysAhead);

        var vacationPays = await _vacationPayRepository.GetUpcomingAsync(
            fromDate, toDate, cancellationToken);

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
            vp.UpdatedAt)).ToList();

        return Result.Success(response);
    }
}
