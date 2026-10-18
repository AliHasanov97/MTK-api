using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.VacationPays;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;

public sealed record VacationPayResponse(
    Guid Id,
    Guid VacationOrderId,
    Guid EmployeeId,
    string EmployeeFullName,
    decimal AverageMonthlyWage,
    decimal AverageDailyWage,
    int VacationDays,
    decimal TotalAmount,
    DateOnly VacationStartDate,
    DateOnly VacationEndDate,
    VacationPayStatus Status,
    DateOnly DueDate,
    DateTimeOffset? PaidAt,
    PaymentMethod? PaymentMethod,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
