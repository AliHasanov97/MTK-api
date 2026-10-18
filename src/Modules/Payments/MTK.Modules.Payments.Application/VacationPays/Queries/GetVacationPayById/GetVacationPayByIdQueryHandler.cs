using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;

internal sealed class GetVacationPayByIdQueryHandler
    : IQueryHandler<GetVacationPayByIdQuery, VacationPayResponse>
{
    private readonly IVacationPayRepository _vacationPayRepository;

    public GetVacationPayByIdQueryHandler(IVacationPayRepository vacationPayRepository)
    {
        _vacationPayRepository = vacationPayRepository;
    }

    public async Task<Result<VacationPayResponse>> Handle(
        GetVacationPayByIdQuery request,
        CancellationToken cancellationToken)
    {
        var vacationPay = await _vacationPayRepository.GetByIdAsync(
            request.VacationPayId, cancellationToken);

        if (vacationPay is null)
        {
            return Result.Failure<VacationPayResponse>(VacationPayErrors.NotFound);
        }

        var response = new VacationPayResponse(
            vacationPay.Id,
            vacationPay.VacationOrderId,
            vacationPay.EmployeeId,
            vacationPay.EmployeeFullName,
            vacationPay.AverageMonthlyWage,
            vacationPay.AverageDailyWage,
            vacationPay.VacationDays,
            vacationPay.TotalAmount,
            vacationPay.VacationStartDate,
            vacationPay.VacationEndDate,
            vacationPay.Status,
            vacationPay.DueDate,
            vacationPay.PaidAt,
            vacationPay.PaymentMethod,
            vacationPay.CreatedAt,
            vacationPay.UpdatedAt);

        return Result.Success(response);
    }
}
