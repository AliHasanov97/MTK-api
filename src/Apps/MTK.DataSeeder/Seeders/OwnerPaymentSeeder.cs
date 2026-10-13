using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Sakin ödənişlərini seed edir
/// </summary>
public sealed class OwnerPaymentSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<OwnerPaymentSeeder> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IChargeRepository _chargeRepository;

    public OwnerPaymentSeeder(
        ISender sender,
        ILogger<OwnerPaymentSeeder> logger,
        IPaymentRepository paymentRepository,
        IChargeRepository chargeRepository)
    {
        _sender = sender;
        _logger = logger;
        _paymentRepository = paymentRepository;
        _chargeRepository = chargeRepository;
    }

    public async Task SeedAsync()
    {
        if (!OwnerSeeder.OwnerIds.Any())
        {
            _logger.LogWarning("  ⚠️  Sakinlər mövcud deyil. Əvvəlcə OwnerSeeder işlətməlisiniz.");
            return;
        }

        // Əvvəlcə mövcud ödənişləri yoxla
        var existingPayments = await _paymentRepository.ListAsync();
        var ownerPayments = existingPayments.Where(p => p.OwnerId.HasValue).ToList();

        if (ownerPayments.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} sakin ödənişi mövcuddur", ownerPayments.Count);
            return;
        }

        var faker = new Faker();
        var paymentCount = 0;

        // Hər sakin üçün 60-80% ehtimalla ödəniş yaradaq
        foreach (var ownerId in OwnerSeeder.OwnerIds)
        {
            // 60-80% şans
            if (faker.Random.Int(1, 100) > 80)
                continue;

            // Bu sakin üçün unpaid chargeları tap
            var unpaidCharges = await _chargeRepository.GetUnpaidChargesAsync(ownerId);

            if (!unpaidCharges.Any())
                continue;

            // Ümumi borcu hesabla
            var totalDebt = unpaidCharges.Sum(c => c.OutstandingAmount);

            if (totalDebt <= 0)
                continue;

            // Random ödəniş məbləği (borcun 40-100%-i)
            var paymentPercentage = faker.Random.Decimal(0.4m, 1.0m);
            var paymentAmount = Math.Round(totalDebt * paymentPercentage, 2);

            // Random payment method
            var paymentMethods = new[] { PaymentMethod.Cash, PaymentMethod.BankTransfer, PaymentMethod.Card };
            var paymentMethod = faker.PickRandom(paymentMethods);

            var command = new CreatePaymentCommand(
                ownerId,
                paymentAmount,
                paymentMethod,
                faker.Lorem.Sentence(3)); // Random qeyd

            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                paymentCount++;

                if (paymentCount % 20 == 0)
                {
                    _logger.LogInformation("  ✅ {Count} sakin ödənişi yaradıldı...", paymentCount);
                }
            }
            else
            {
                _logger.LogError("  ❌ Sakin ödənişi yaradıla bilmədi: {Error}", result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} sakin ödənişi yaradıldı", paymentCount);

        // Ödəniş statistikası
        var allCharges = await _chargeRepository.ListAsync();
        var ownerCharges = allCharges.Where(c => c.OwnerId.HasValue).ToList();
        var paidCharges = ownerCharges.Count(c => c.Status == MTK.Modules.Payments.Domain.Charges.ChargeStatus.Paid);
        var partiallyPaid = ownerCharges.Count(c => c.Status == MTK.Modules.Payments.Domain.Charges.ChargeStatus.PartiallyPaid);

        _logger.LogInformation("  📊 Borc statusu: {Paid} ödənildi, {Partial} qismən, {Unpaid} ödənilməyib",
            paidCharges, partiallyPaid, ownerCharges.Count - paidCharges - partiallyPaid);
    }
}