using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Vendor ödənişlərini seed edir
/// </summary>
public sealed class VendorPaymentSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<VendorPaymentSeeder> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IVendorChargeRepository _vendorChargeRepository;

    public VendorPaymentSeeder(
        ISender sender,
        ILogger<VendorPaymentSeeder> logger,
        IPaymentRepository paymentRepository,
        IVendorChargeRepository vendorChargeRepository)
    {
        _sender = sender;
        _logger = logger;
        _paymentRepository = paymentRepository;
        _vendorChargeRepository = vendorChargeRepository;
    }

    public async Task SeedAsync()
    {
        if (!VendorSeeder.VendorIds.Any())
        {
            _logger.LogWarning("  ⚠️  Vendor-lər mövcud deyil. Əvvəlcə VendorSeeder işlətməlisiniz.");
            return;
        }

        // Əvvəlcə mövcud ödənişləri yoxla
        var existingPayments = await _paymentRepository.ListAsync();
        var vendorPayments = existingPayments.Where(p => p.VendorId.HasValue).ToList();

        if (vendorPayments.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} vendor ödənişi mövcuddur", vendorPayments.Count);
            return;
        }

        var faker = new Faker();
        var paymentCount = 0;

        // Hər vendor üçün 70-90% ehtimalla ödəniş yaradaq
        foreach (var vendorId in VendorSeeder.VendorIds)
        {
            // 70-90% şans
            if (faker.Random.Int(1, 100) > 90)
                continue;

            // Bu vendor üçün unpaid chargeları tap
            var unpaidCharges = await _vendorChargeRepository.GetUnpaidChargesAsync(vendorId);

            if (!unpaidCharges.Any())
                continue;

            // Ümumi borcu hesabla
            var totalDebt = unpaidCharges.Sum(c => c.OutstandingAmount);

            if (totalDebt <= 0)
                continue;

            // Random ödəniş məbləği (borcun 50-100%-i)
            var paymentPercentage = faker.Random.Decimal(0.5m, 1.0m);
            var paymentAmount = Math.Round(totalDebt * paymentPercentage, 2);

            // Random payment method (vendor-lərə əsasən bank transferi)
            var paymentMethods = new[]
            {
                PaymentMethod.BankTransfer,
                PaymentMethod.BankTransfer, // daha yüksək ehtimal
                PaymentMethod.BankTransfer,
                PaymentMethod.Cash
            };
            var paymentMethod = faker.PickRandom(paymentMethods);

            var command = new CreateVendorPaymentCommand(
                vendorId,
                paymentAmount,
                paymentMethod,
                faker.Lorem.Sentence(3)); // Random qeyd

            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                paymentCount++;

                if (paymentCount % 5 == 0)
                {
                    _logger.LogInformation("  ✅ {Count} vendor ödənişi yaradıldı...", paymentCount);
                }
            }
            else
            {
                _logger.LogError("  ❌ Vendor ödənişi yaradıla bilmədi: {Error}", result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} vendor ödənişi yaradıldı", paymentCount);

        // Ödəniş statistikası
        var allVendorCharges = new List<MTK.Modules.Payments.Domain.Charges.Charge>();
        foreach (var vendorId in VendorSeeder.VendorIds)
        {
            var charges = await _vendorChargeRepository.ListByVendorAsync(vendorId);
            allVendorCharges.AddRange(charges);
        }

        var paidCharges = allVendorCharges.Count(c => c.Status == MTK.Modules.Payments.Domain.Charges.ChargeStatus.Paid);
        var partiallyPaid = allVendorCharges.Count(c => c.Status == MTK.Modules.Payments.Domain.Charges.ChargeStatus.PartiallyPaid);

        _logger.LogInformation("  📊 Vendor borc statusu: {Paid} ödənildi, {Partial} qismən, {Unpaid} ödənilməyib",
            paidCharges, partiallyPaid, allVendorCharges.Count - paidCharges - partiallyPaid);
    }
}