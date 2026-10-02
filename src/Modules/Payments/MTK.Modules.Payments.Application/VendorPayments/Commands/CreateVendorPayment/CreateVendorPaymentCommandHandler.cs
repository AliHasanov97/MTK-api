using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

internal sealed class CreateVendorPaymentCommandHandler : ICommandHandler<CreateVendorPaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IPaymentAllocationService _paymentAllocationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVendorPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IVendorRepository vendorRepository,
        IChargeRepository chargeRepository,
        IPaymentAllocationService paymentAllocationService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _vendorRepository = vendorRepository;
        _chargeRepository = chargeRepository;
        _paymentAllocationService = paymentAllocationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateVendorPaymentCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdDefaultAsync(request.VendorId, cancellationToken);

        if (vendor is null)
        {
            return Result.Failure<Guid>(new Error(
                "Vendor.NotFound",
                $"Tədarükçü tapılmadı: {request.VendorId}"));
        }

        // Sakin ödənişindən fərqli olaraq tədarükçüyə avans icazə verilmir —
        // ödəniş yalnız mövcud açıq borcu ödəyə bilər, artığı rədd olunur.
        var outstandingDebt = (await _chargeRepository.GetUnpaidChargesByPartyAsync(
                PartyType.Vendor, request.VendorId, cancellationToken))
            .Sum(c => c.OutstandingAmount);

        if (request.Amount > outstandingDebt)
        {
            return Result.Failure<Guid>(new Error(
                "VendorPayment.ExceedsDebt",
                $"Ödəniş tədarükçünün qalıq borcundan böyük ola bilməz (borc: {outstandingDebt})."));
        }

        // Ödəniş, onun paylanması və ledger qeydi bir tranzaksiyadır: yarımçıq
        // vəziyyət (ödəniş var, paylanma yox) qala bilməz.
        await using var transaction = await _paymentRepository.BeginTransactionAsync(cancellationToken);

        var payment = Domain.Payments.Payment.Create(
            PartyType.Vendor,
            request.VendorId,
            request.Amount,
            request.PaymentMethod,
            DateTimeOffset.UtcNow,
            request.Reference,
            request.Notes);

        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Sakin ödənişi ilə eyni FIFO/avans mexanizmi — tədarükçünün bütün açıq
        // borclarına ən köhnədən paylanır.
        var allocationResult = await _paymentAllocationService.AllocatePaymentAsync(
            payment.Id,
            PartyType.Vendor,
            request.VendorId,
            request.Amount,
            propertyId: null,
            cancellationToken);

        if (allocationResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(allocationResult.Error);
        }

        // Ledger qeydi event-in handler-ində yazılır — pul hərəkəti borcun
        // bağlanması ilə bir yerdə qalsın deyə.
        payment.MarkAsCompleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
