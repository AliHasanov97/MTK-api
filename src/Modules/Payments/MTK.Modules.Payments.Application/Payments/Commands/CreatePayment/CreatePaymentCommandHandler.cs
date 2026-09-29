using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;

internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationService _paymentAllocationService;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationService paymentAllocationService,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationService = paymentAllocationService;
        _propertyOwnershipRepository = propertyOwnershipRepository;
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
        }

        var payment = Payment.Create(
            request.OwnerId,
            request.Amount,
            request.PaymentMethod,
            request.PaymentDate,
            request.Reference,
            request.Notes,
            request.PropertyId,
            request.PropertyType);

        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Allocate payment to charges (scoped to the property when one was given)
        await _paymentAllocationService.AllocatePaymentAsync(
            payment.Id,
            request.OwnerId,
            request.Amount,
            request.PropertyId,
            cancellationToken);

        // Raising PaymentCompletedDomainEvent here is what posts the ledger entry:
        // PaymentCompletedDomainEventHandler writes the matching transaction.
        payment.MarkAsCompleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
