using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.OwnerBalances.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;

internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationService _paymentAllocationService;
    private readonly IOwnerBalanceService _ownerBalanceService;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationService paymentAllocationService,
        IOwnerBalanceService ownerBalanceService,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        IChargeRepository chargeRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationService = paymentAllocationService;
        _ownerBalanceService = ownerBalanceService;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _chargeRepository = chargeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        if (request.PropertyId.HasValue)
        {
            var ownership = await _propertyOwnershipRepository.GetByPropertyIdAsync(request.PropertyId.Value, cancellationToken);

            if (ownership is null || ownership.OwnerId != request.OwnerId)
            {
                return Result.Failure<Guid>(new Error(
                    "Payment.PropertyOwnerMismatch",
                    "Seçilmiş əmlak bu sahibə aid deyil"));
            }

            // Əmlaka hədəflənmiş ödəniş yalnız həmin əmlakın qalıq borcunu ödəyə
            // bilər — artıq (avans) yalnız sahib səviyyəli ödənişdə yaranır.
            var propertyDebt = (await _chargeRepository.GetUnpaidChargesAsync(
                    request.OwnerId,
                    request.PropertyId.Value,
                    cancellationToken))
                .Sum(c => c.OutstandingAmount);

            if (request.Amount > propertyDebt)
            {
                return Result.Failure<Guid>(new Error(
                    "Payment.ExceedsPropertyDebt",
                    $"Ödəniş əmlakın qalıq borcundan böyük ola bilməz (borc: {propertyDebt}). " +
                    "Avans kimi ödəniş üçün əmlakı seçməyin — ümumi sahib ödənişi edin."));
            }
        }

        // Ödəniş, onun paylanması və balansın yenilənməsi bir tranzaksiyadır: yarımçıq
        // vəziyyət (ödəniş var, paylanma yox) qala bilməz. Commit olunmasa, tranzaksiya
        // dispose olunanda avtomatik geri qaytarılır.
        await using var transaction = await _chargeRepository.BeginTransactionAsync(cancellationToken);

        var payment = Payment.Create(
            PartyType.Owner,
            request.OwnerId,
            request.Amount,
            request.PaymentMethod,
            DateTimeOffset.UtcNow,
            request.Reference,
            request.Notes,
            request.PropertyId,
            request.PropertyType);

        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Allocate payment to charges (scoped to the property when one was given)
        var allocationResult = await _paymentAllocationService.AllocatePaymentAsync(
            payment.Id,
            PartyType.Owner,
            request.OwnerId,
            request.Amount,
            request.PropertyId,
            cancellationToken);

        if (allocationResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(allocationResult.Error);
        }

        // Raising PaymentCompletedDomainEvent here is what posts the ledger entry:
        // PaymentCompletedDomainEventHandler writes the matching transaction.
        payment.MarkAsCompleted();
        // Allocation's charge.ApplyPayment mutations (and each allocation's own
        // RemainingDebtAfterPayment, set in-memory in PaymentAllocationService from
        // charge.OutstandingAmount) must be persisted before RecalculateAsync's
        // fresh queries below can see them — same two-phase-save reasoning as
        // CompanyBalanceService.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Balans aqreqatdan mütləq yenidən hesablanır — artırmalı yanaşma yoxdur.
        await _ownerBalanceService.RecalculateAsync([request.OwnerId], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
