namespace MTK.Modules.Payments.Domain.Rates;

public enum RateType
{
    PerSquareMeter,  // For apartments
    FixedGarage,     // For garages
    Manual           // Snapshot value for one-off/ad-hoc charges (no configured Rate behind them)
}