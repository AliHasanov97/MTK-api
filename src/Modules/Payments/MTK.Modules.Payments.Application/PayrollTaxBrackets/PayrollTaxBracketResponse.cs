using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets;

public sealed record PayrollTaxBracketResponse(
    Guid Id,
    PayrollRateType RateType,
    PayrollParty Party,
    decimal LowerBound,
    decimal? UpperBound,
    decimal Rate,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveTo,
    string? Description)
{
    public static PayrollTaxBracketResponse FromDomain(PayrollTaxBracket bracket) => new(
        bracket.Id,
        bracket.RateType,
        bracket.Party,
        bracket.LowerBound,
        bracket.UpperBound,
        bracket.Rate,
        bracket.EffectiveFrom,
        bracket.EffectiveTo,
        bracket.Description);
}
