using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Charges.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

/// <summary>
/// Aktiv müqavilələrin cədvəl üzrə xidmətlərindən borc yaradır.
///
/// Tədarükçü borcları da eyni <c>Charge</c> aqreqatındadır (PartyType == Vendor);
/// sakin generasiyasından fərqli olaraq burada OwnerBalance yoxdur — borclar öz
/// PaidAmount sahəsində izlənir. İdempotentlik həm ExistsForPeriodAsync yoxlaması,
/// həm də bazadakı unikal index ilə qorunur (job təkrar işləsə belə borc ikilənmir).
/// </summary>
internal sealed class VendorChargeGenerationService(
    IContractRepository contractRepository,
    IVendorChargeRepository vendorChargeRepository,
    IPaymentAllocationService paymentAllocationService,
    IUnitOfWork unitOfWork,
    ILogger<VendorChargeGenerationService> logger) : IVendorChargeGenerationService
{
    public async Task<int> GenerateScheduledChargesAsync(string period, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting vendor charge generation for period {Period}", period);

        List<Contract> contracts = await contractRepository.ListActiveWithServicesAsync(cancellationToken);

        var createdCharges = new List<(Contract Contract, ContractService Service, Charge Charge)>();

        foreach (Contract contract in contracts)
        {
            foreach (ContractService service in contract.Services)
            {
                if (!service.IsBillableFor(contract.StartDate, contract.EndDate, period))
                {
                    continue;
                }

                bool alreadyBilled = await vendorChargeRepository.ExistsForPeriodAsync(
                    service.Id, period, cancellationToken);

                if (alreadyBilled)
                {
                    continue;
                }

                var chargeDate = DateTimeOffset.UtcNow;

                Charge charge = Charge.ForServiceSchedule(contract, service, period, chargeDate);
                vendorChargeRepository.Add(charge);
                createdCharges.Add((contract, service, charge));
            }
        }

        // Add yalnız ChangeTracker-ə yazır — SaveChanges olmadan INSERT getmir
        // və job-ın scope-u bitəndə dəyişikliklər itkə düşür.
        if (createdCharges.Count > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Sakin generasiyası ilə eyni qayda: yeni yaranmış borcları tədarükçünün
            // əvvəlki ödənişlərindən qalan avansla bağla (varsa).
            var advanceResult = await paymentAllocationService.ApplyAdvanceToChargesAsync(
                createdCharges.Select(c => c.Charge).ToList(), cancellationToken);

            if (advanceResult.IsFailure)
            {
                logger.LogError(
                    "Vendor advance settlement failed for period {Period} ({Code}: {Message})",
                    period, advanceResult.Error.Code, advanceResult.Error.Message);
            }
            else
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        foreach (var (contract, service, charge) in createdCharges)
        {
            logger.LogInformation(
                "Created vendor charge {Amount} for contract {ContractNumber}, service {ServiceName}, period {Period}",
                charge.Amount, contract.Number, service.Name, period);
        }

        logger.LogInformation(
            "Vendor charge generation completed for period {Period}. Created {Count} charges",
            period, createdCharges.Count);

        return createdCharges.Count;
    }
}
