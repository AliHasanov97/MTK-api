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
/// PaidAmount sahəsində izlənir. İdempotentlik həm "bu dövrdə borcu olan xidmətlər"
/// siyahısı (bir sorğu ilə), həm də bazadakı unikal index ilə qorunur (job təkrar
/// işləsə belə borc ikilənmir). Borcların yazılması və avansla bağlanması bir tranzaksiyadadır.
/// </summary>
internal sealed class VendorChargeGenerationService(
    IContractRepository contractRepository,
    IVendorChargeRepository vendorChargeRepository,
    IPaymentAllocationService paymentAllocationService,
    IUnitOfWork unitOfWork,
    ILogger<VendorChargeGenerationService> logger) : IVendorChargeGenerationService
{
    public async Task<int> GenerateScheduledChargesAsync(
        string period,
        bool regenerateCancelled = false,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting vendor charge generation for period {Period}", period);

        List<Contract> contracts = await contractRepository.ListActiveWithServicesAsync(cancellationToken);

        // Planlı iş ləğv edilmiş borcu "mövcud" sayır (təkrar yaratmır); əl ilə bərpa isə ləğv edilmişləri nəzərə almır.
        HashSet<Guid> billedServiceIds = await vendorChargeRepository.GetBilledServiceIdsForPeriodAsync(
            period, includeCancelled: !regenerateCancelled, cancellationToken);

        // Borcun tarixi dövrün ilk günüdür (sakin borcları ilə eyni); gec işləsə də dövrə uyğun qalır.
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

                Charge charge = Charge.ForServiceSchedule(contract, service, period, chargeDate);
                vendorChargeRepository.Add(charge);
                createdCharges.Add((contract, service, charge));
            }
        }

        if (createdCharges.Count > 0)
        {
            // Borcların yazılması və avansla bağlanması bir tranzaksiyadır: avans uğursuz olsa borclar da geri qaytarılır
            // və növbəti işləmədə təkrar cəhd olunur (əks halda borc "var" sayılıb avans ona heç vaxt bağlanmazdı).
            await using var transaction = await vendorChargeRepository.BeginTransactionAsync(cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var advanceResult = await paymentAllocationService.ApplyAdvanceToChargesAsync(
                createdCharges.Select(c => c.Charge).ToList(), cancellationToken);

            if (advanceResult.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError(
                    "Vendor advance settlement failed for period {Period} ({Code}: {Message}); generation rolled back",
                    period, advanceResult.Error.Code, advanceResult.Error.Message);

                throw new InvalidOperationException(
                    $"Tədarükçü avansı ilə bağlama uğursuz oldu, '{period}' dövrü üçün generasiya geri qaytarıldı ({advanceResult.Error.Code}: {advanceResult.Error.Message}).");
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
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
