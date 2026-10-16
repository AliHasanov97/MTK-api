using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.SalaryPayments;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.CreateSalaryPayment;

/// <summary>
/// Yeni maaş ödənişi yarat (Draft statusunda).
/// İşçinin əsas maaşı, bonusları və düzəlişləri avtomatik hesablanır.
/// </summary>
public sealed record CreateSalaryPaymentCommand(
    Guid EmployeeId,
    int SalaryMonth,
    int SalaryYear,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Notes = null) : ICommand<SalaryPaymentResponse>;
