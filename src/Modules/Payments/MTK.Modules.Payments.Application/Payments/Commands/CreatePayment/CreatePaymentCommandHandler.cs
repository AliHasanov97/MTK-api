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
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationService paymentAllocationService,
        IOwnerBalanceService ownerBalanceService,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        IChargeRepository chargeRepository,
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationService = paymentAllocationService;
        _ownerBalanceService = ownerBalanceService;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _chargeRepository = chargeRepository;
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        if (request.ApartmentId.HasValue && request.GarageId.HasValue)
        {
            return Result.Failure<Guid>(new Error(
                "Payment.PropertyAmbiguous",
                "Ödəniş eyni anda həm mənzilə, həm qaraja hədəflənə bilməz"));
        }

        Guid? propertyId = request.ApartmentId ?? request.GarageId;

        // Advance/general payments (no property) skip the ownership check below —
        // this is the only place that would otherwise catch a bogus OwnerId before a
        // Payment gets created against it.
        var owner = await _ownerRepository.GetByIdDefaultAsync(request.OwnerId, cancellationToken);
        if (owner is null)
        {
            return Result.Failure<Guid>(new Error(
                "Owner.NotFound",
                $"Sahib tapılmadı: {request.OwnerId}"));
        }

        if (propertyId.HasValue)
        {
            var ownership = await _propertyOwnershipRepository.GetByPropertyIdAsync(propertyId.Value, cancellationToken);

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
                    propertyId.Value,
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

        var payment = Payment.CreateForOwner(
            request.OwnerId,
            request.Amount,
            request.PaymentMethod,
            DateTimeOffset.UtcNow,
            request.Notes,
            request.ApartmentId,
            request.GarageId);

        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Allocate payment to charges (scoped to the property when one was given)
        var allocationResult = await _paymentAllocationService.AllocatePaymentAsync(
            payment.Id,
            PartyType.Owner,
            request.OwnerId,
            request.Amount,
            propertyId,
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
