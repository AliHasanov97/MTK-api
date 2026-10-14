using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Bir dərəcə pilləsi (bracket), məs. "DSMF, işəgötürən payı, 200-8000 AZN aralığı → 15%".
/// <see cref="Rates.Rate"/> ilə eyni fəlsəfə (EffectiveFrom/EffectiveTo ilə versiyalı) —
/// qanun dəyişəndə deploy etmədən, sadəcə yeni sətir əlavə etməklə yenilənir.
///
/// Hesablama <see cref="PayrollCalculator"/>-da mütərəqqi (progressive) tətbiq olunur:
/// məbləğin <see cref="LowerBound"/>-dan <see cref="UpperBound"/>-a qədər olan hissəsinə
/// <see cref="Rate"/> tətbiq edilir. <see cref="UpperBound"/> = null sonsuzluq deməkdir.
/// </summary>
public sealed class PayrollTaxBracket : Entity
{
    private PayrollTaxBracket() : base() { }

    public PayrollRateType RateType { get; private set; }
    public PayrollParty Party { get; private set; }

    public decimal LowerBound { get; private set; }
    public decimal? UpperBound { get; private set; }

    /// <summary>Faiz, məs. 0.22 = 22%.</summary>
    public decimal Rate { get; private set; }

    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveTo { get; private set; }

    public string? Description { get; private set; }

    public static PayrollTaxBracket Create(
        PayrollRateType rateType,
        PayrollParty party,
        decimal lowerBound,
        decimal? upperBound,
        decimal rate,
        DateTimeOffset effectiveFrom,
        string? description = null)
    {
        if (upperBound is not null && upperBound <= lowerBound)
        {
            throw new ArgumentException("UpperBound LowerBound-dan böyük olmalıdır", nameof(upperBound));
        }

        // Id qəsdən təyin edilmir — EF onu özü generasiya edir (bax: Rate.Create).
        var bracket = new PayrollTaxBracket
        {
            RateType = rateType,
            Party = party,
            LowerBound = lowerBound,
            UpperBound = upperBound,
            Rate = rate,
            EffectiveFrom = effectiveFrom,
            Description = description
        };

        bracket.SetCreatedAt();
        return bracket;
    }

    public void Update(decimal rate, string? description)
    {
        Rate = rate;
        Description = description;
        SetUpdatedAt();
    }

    public void Deactivate(DateTimeOffset effectiveTo)
    {
        EffectiveTo = effectiveTo;
        SetUpdatedAt();
    }
}
