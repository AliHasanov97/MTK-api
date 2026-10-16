using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryCalculationPreview;

/// <summary>
/// İşçinin maaş hesablamasını preview et (yaratmadan əvvəl).
/// </summary>
public sealed record GetSalaryCalculationPreviewQuery(
    Guid EmployeeId,
    int SalaryMonth,
    int SalaryYear) : IQuery<SalaryCalculationPreviewResponse>;
