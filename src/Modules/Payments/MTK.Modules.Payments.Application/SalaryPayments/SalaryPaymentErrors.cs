using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Application.SalaryPayments;

/// <summary>
/// SalaryPayment domain errors.
/// </summary>
public static class SalaryPaymentErrors
{
    public static readonly Error EmployeeNotFound = new(
        "SalaryPayment.EmployeeNotFound",
        "İşçi tapılmadı");

    public static readonly Error EmployeeInactive = new(
        "SalaryPayment.EmployeeInactive",
        "İşçi aktiv deyil");

    public static readonly Error PaymentAlreadyExists = new(
        "SalaryPayment.PaymentAlreadyExists",
        "Bu işçi üçün bu dövrdə artıq maaş ödənişi mövcuddur");

    public static readonly Error PaymentNotFound = new(
        "SalaryPayment.PaymentNotFound",
        "Maaş ödənişi tapılmadı");

    public static readonly Error PaymentAlreadyPaid = new(
        "SalaryPayment.PaymentAlreadyPaid",
        "Bu maaş ödənişi artıq ödənilib");

    public static readonly Error PaymentNotPaid = new(
        "SalaryPayment.PaymentNotPaid",
        "Bu maaş ödənişi hələ ödənilməmiş");

    public static readonly Error PaymentCannotBeDeleted = new(
        "SalaryPayment.PaymentCannotBeDeleted",
        "Yalnız Draft statusunda olan maaş ödənişləri silinə bilər");
}
