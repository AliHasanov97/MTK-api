using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Application.VacationPays;

public static class VacationPayErrors
{
    public static readonly Error NotFound = new(
        "VacationPay.NotFound",
        "Məzuniyyət haqqı tapılmadı");

    public static readonly Error AlreadyPaid = new(
        "VacationPay.AlreadyPaid",
        "Məzuniyyət haqqı artıq ödənilib");
}
