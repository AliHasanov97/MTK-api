using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.PayrollTaxBrackets;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets.Commands.CreatePayrollTaxBracket;

/// <summary>
/// Yeni dərəcə pilləsi əlavə edir. Eyni RateType+Party+LowerBound+UpperBound üçün hələ bağlanmamış
/// (EffectiveTo == null) köhnə pillə varsa, bu, yeni pillənin EffectiveFrom-u ilə avtomatik bağlanır —
/// əks halda PayrollCalculator hər ikisini tətbiq edib ikiqat hesablayardı (bax: handler).
/// </summary>
public sealed record CreatePayrollTaxBracketCommand(
    PayrollRateType RateType,
    PayrollParty Party,
    decimal LowerBound,
    decimal? UpperBound,
    decimal Rate,
    DateTimeOffset EffectiveFrom,
    string? Description) : ICommand<PayrollTaxBracketResponse>;
