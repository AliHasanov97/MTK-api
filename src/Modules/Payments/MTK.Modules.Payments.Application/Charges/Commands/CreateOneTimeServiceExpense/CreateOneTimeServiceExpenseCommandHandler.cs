using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateOneTimeServiceExpense;

internal sealed class CreateOneTimeServiceExpenseCommandHandler
    : ICommandHandler<CreateOneTimeServiceExpenseCommand, Guid>
{
    private readonly IContractRepository _contractRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOneTimeServiceExpenseCommandHandler(
        IContractRepository contractRepository,
        IChargeRepository chargeRepository,
        IPaymentRepository paymentRepository,
        IPaymentAllocationRepository paymentAllocationRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _chargeRepository = chargeRepository;
        _paymentRepository = paymentRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateOneTimeServiceExpenseCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetWithServicesAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure<Guid>(new Error("Contract.NotFound", "Müqavilə tapılmadı"));
        }

        if (!contract.IsActive)
        {
            return Result.Failure<Guid>(new Error("Contract.NotActive", "Müqavilə aktiv deyil"));
        }

        var service = contract.Services.FirstOrDefault(s => s.Id == request.ContractServiceId);

        if (service is null || !service.IsActive)
        {
            return Result.Failure<Guid>(new Error("ContractService.NotFound", "Xidmət tapılmadı"));
        }

        if (service.BillingPeriod != BillingPeriod.OneTime)
        {
            return Result.Failure<Guid>(new Error(
                "ContractService.NotOneTime",
                "Yalnız birdəfəlik (OneTime) xidmətlər üçün bu yolla xərc daxil edilə bilər"));
        }

        // Borcun yaranması, ona qarşı ödənişin tamamlanması bir tranzaksiyadır —
        // yarımçıq vəziyyət (borc var, ödəniş yox) qala bilməz.
        await using var transaction = await _chargeRepository.BeginTransactionAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var charge = Charge.ForOneTimeService(contract, service, request.Amount, now);
        _chargeRepository.Add(charge);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var payment = Payment.CreateForVendor(
            contract.VendorId,
            request.Amount,
            request.PaymentMethod,
            now,
            request.Notes);
        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // FIFO-ya ehtiyac yoxdur: admin konkret bu xidmətə görə ödəyib, ödəniş
        // birbaşa yaradılmış borca tətbiq olunur (tədarükçünün başqa açıq
        // borcları toxunulmaz qalır).
        charge.ApplyPayment(request.Amount);
        _paymentAllocationRepository.Add(PaymentAllocation.Create(
            payment.Id, charge.Id, request.Amount, charge.OutstandingAmount, isFromAdvance: false));

        // Ledger qeydi və balans yeniləməsi PaymentCompletedDomainEvent handler-ində
        // (outbox vasitəsilə) baş verir — tədarükçü ödənişinin hər yerdə eyni yolla getməsi üçün.
        payment.MarkAsCompleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
