namespace MTK.Modules.Payments.Application.Charges.Services;

public interface IChargeGenerationService
{
    Task<int> GenerateMonthlyChargesAsync(string period, CancellationToken cancellationToken = default);
}