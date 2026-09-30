using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

internal sealed class CreateVendorPaymentCommandHandler : ICommandHandler<CreateVendorPaymentCommand, Guid>
{
    private readonly IVendorPaymentRepository _vendorPaymentRepository;
    private readonly IVendorChargeRepository _vendorChargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVendorPaymentCommandHandler(
        IVendorPaymentRepository vendorPaymentRepository,
        IVendorChargeRepository vendorChargeRepository,
        IUnitOfWork unitOfWork)
    {
        _vendorPaymentRepository = vendorPaymentRepository;
        _vendorChargeRepository = vendorChargeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateVendorPaymentCommand request, CancellationToken cancellationToken)
    {
        var charge = await _vendorChargeRepository.GetByIdAsync(request.VendorChargeId, cancellationToken);

        if (charge is null)
        {
            return Result.Failure<Guid>(new Error(
                "VendorCharge.NotFound",
                $"Tədarükçü borcu tapılmadı: {request.VendorChargeId}"));
        }

        if (charge.Status == Domain.VendorCharges.VendorChargeStatus.Cancelled)
        {
            return Result.Failure<Guid>(new Error(
                "VendorCharge.Cancelled",
                "Ləğv edilmiş borca ödəniş edilə bilməz"));
        }

        if (request.Amount > charge.OutstandingAmount)
        {
            return Result.Failure<Guid>(new Error(
                "VendorPayment.ExceedsOutstanding",
                $"Ödəniş qalıq borcdan böyükdür (qalıq: {charge.OutstandingAmount})"));
        }

        var payment = Domain.VendorPayments.VendorPayment.Create(
            charge.VendorId,
            charge.Id,
            request.Amount,
            request.PaymentMethod,
            request.PaymentDate,
            request.Reference,
            request.Notes);

        _vendorPaymentRepository.Add(payment);

        // Borc dərhal bağlanır (qismən də ola bilər) — ödəniş və borc eyni
        // tranzaksiyada yadda saxlanılır.
        charge.ApplyPayment(request.Amount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Ledger qeydi event-in handler-ində yazılır — pul hərəkəti borcun
        // bağlanması ilə bir yerdə qalsın deyə.
        payment.MarkAsCompleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
