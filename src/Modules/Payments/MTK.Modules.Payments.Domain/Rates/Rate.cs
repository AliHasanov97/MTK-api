using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Rates.Events;

namespace MTK.Modules.Payments.Domain.Rates;

public sealed class Rate : SearchableEntity
{
    private Rate() : base() { }

    public RateType RateType { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveTo { get; private set; }
    public string? Description { get; private set; }

    public static Rate Create(RateType rateType, decimal amount, DateTimeOffset effectiveFrom, string? description)
    {
        var rate = new Rate
        {
            RateType = rateType,
            Amount = amount,
            EffectiveFrom = effectiveFrom,
            Description = description
        };
        rate.SetCreatedAt();
        rate.RaiseDomainEvent(new RateCreatedDomainEvent(rate.Id, rateType, amount));
        return rate;
    }

    public void Update(decimal amount, string? description)
    {
        Amount = amount;
        Description = description;
        SetUpdatedAt();
    }

    public void Deactivate(DateTimeOffset effectiveTo)
    {
        EffectiveTo = effectiveTo;
        SetUpdatedAt();
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}