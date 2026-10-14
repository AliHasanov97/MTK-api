using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Charges.Services;

namespace MTK.Modules.Payments.Application.Charges.Commands.GenerateChargesForPeriod;

internal sealed class GenerateChargesForPeriodCommandHandler(
    IChargeGenerationService chargeGenerationService,
    IVendorChargeGenerationService vendorChargeGenerationService,
    ICustomerChargeGenerationService customerChargeGenerationService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<GenerateChargesForPeriodCommand, GenerateChargesForPeriodResponse>
{
    public async Task<Result<GenerateChargesForPeriodResponse>> Handle(
        GenerateChargesForPeriodCommand request,
        CancellationToken cancellationToken)
    {
        // Gələcək ay üçün borc yaratmaq mənasızdır (dövr başlamayıb)
        if (string.CompareOrdinal(request.Period, dateTimeProvider.UtcNow.AddDays(1).ToString("yyyy-MM")) > 0)
        {
            return Result.Failure<GenerateChargesForPeriodResponse>(
                new Error("Charge.FuturePeriod", "Hələ başlamamış dövr üçün borc yaratmaq olmaz"));
        }

        ChargeGenerationResult resident = await chargeGenerationService.GenerateMonthlyChargesAsync(
            request.Period, regenerateCancelled: true, cancellationToken);

        int vendorCreated = await vendorChargeGenerationService.GenerateScheduledChargesAsync(
            request.Period, regenerateCancelled: true, cancellationToken);

        int customerCreated = await customerChargeGenerationService.GenerateScheduledChargesAsync(
            request.Period, regenerateCancelled: true, cancellationToken);

        return new GenerateChargesForPeriodResponse(
            request.Period,
            resident.Created,
            resident.ApartmentsSkippedNoRate,
            resident.GaragesSkippedNoRate,
            vendorCreated,
            customerCreated);
    }
}
