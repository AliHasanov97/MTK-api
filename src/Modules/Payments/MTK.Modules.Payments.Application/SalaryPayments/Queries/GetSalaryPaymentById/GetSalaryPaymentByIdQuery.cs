using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryPaymentById;

/// <summary>
/// Maaş ödənişini ID-yə görə gətir (Lines ilə birlikdə).
/// </summary>
public sealed record GetSalaryPaymentByIdQuery(Guid SalaryPaymentId) : IQuery<SalaryPaymentResponse>;
