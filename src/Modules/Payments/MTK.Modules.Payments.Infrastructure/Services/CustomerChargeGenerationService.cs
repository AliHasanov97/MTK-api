using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Charges.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

/// <summary>
/// Aktiv müştəri müqavilələrinin cədvəl üzrə xidmətlərindən borc yaradır (müştəri bizə borcludur).
/// <see cref="VendorChargeGenerationService"/>-in müştəri tərəfli güzgüsüdür: idempotentlik "bu dövrdə
/// borcu olan xidmətlər" siyahısı və bazadakı <c>(ContractServiceId, Period)</c> unikal indeksi ilə qorunur,
/// borcların yazılması və avansla bağlanması bir tranzaksiyadadır.
/// </summary>
internal sealed class CustomerChargeGenerationService(
    IContractRepository contractRepository,
    ICustomerChargeRepository customerChargeRepository,
    IPaymentAllocationService paymentAllocationService,
    IUnitOfWork unitOfWork,
    ILogger<CustomerChargeGenerationService> logger) : ICustomerChargeGenerationService
{
    public async Task<int> GenerateScheduledChargesAsync(
        string period,
        bool regenerateCancelled = false,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting customer charge generation for period {Period}", period);

        List<Contract> contracts = (await contractRepository.ListActiveWithServicesAsync(cancellationToken))
            .Where(c => c.CustomerId.HasValue)
            .ToList();

        HashSet<Guid> billedServiceIds = await customerChargeRepository.GetBilledServiceIdsForPeriodAsync(
            period, includeCancelled: !regenerateCancelled, cancellationToken);

        DateTimeOffset chargeDate = ChargePeriod.FirstDay(period);

        var createdCharges = new List<(Contract Contract, ContractService Service, Charge Charge)>();

        foreach (Contract contract in contracts)
        {
            foreach (ContractService service in contract.Services)
            {
                if (!service.IsBillableFor(contract.StartDate, contract.EndDate, period))
                {
                    continue;
                }

                if (billedServiceIds.Contains(service.Id))
                {
                    continue;
                }

                if (service.PeriodAmount <= 0)
                {
                    logger.LogWarning(
                        "Skipping customer service {ServiceName} of contract {ContractNumber}: non-positive amount {Amount}",
                        service.Name, contract.Number, service.PeriodAmount);
                    continue;
                }

                Charge charge = Charge.ForCustomerServiceSchedule(contract, service, period, chargeDate);
                customerChargeRepository.Add(charge);
                createdCharges.Add((contract, service, charge));
            }
        }

        if (createdCharges.Count > 0)
        {
            await using var transaction = await customerChargeRepository.BeginTransactionAsync(cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var advanceResult = await paymentAllocationService.ApplyAdvanceToChargesAsync(
                createdCharges.Select(c => c.Charge).ToList(), cancellationToken);

            if (advanceResult.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError(
                    "Customer advance settlement failed for period {Period} ({Code}: {Message}); generation rolled back",
                    period, advanceResult.Error.Code, advanceResult.Error.Message);

                throw new InvalidOperationException(
                    $"Müştəri avansı ilə bağlama uğursuz oldu, '{period}' dövrü üçün generasiya geri qaytarıldı ({advanceResult.Error.Code}: {advanceResult.Error.Message}).");
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        logger.LogInformation(
            "Customer charge generation completed for period {Period}. Created {Count} charges",
            period, createdCharges.Count);

        return createdCharges.Count;
    }
}
