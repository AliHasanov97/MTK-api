using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets;

public static class PayrollTaxBracketErrors
{
    public static readonly Error NotFound = new(
        "PayrollTaxBracket.NotFound",
        "Dərəcə pilləsi tapılmadı");

    public static readonly Error InvalidBounds = new(
        "PayrollTaxBracket.InvalidBounds",
        "Yuxarı həd aşağı həddən böyük olmalıdır");

    public static readonly Error AlreadyDeactivated = new(
        "PayrollTaxBracket.AlreadyDeactivated",
        "Bu dərəcə pilləsi artıq bağlanıb");
}
