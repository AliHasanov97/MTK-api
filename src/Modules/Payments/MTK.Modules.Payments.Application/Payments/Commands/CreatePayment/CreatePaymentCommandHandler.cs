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
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationService paymentAllocationService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationService = paymentAllocationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.Create(
            request.OwnerId,
            request.Amount,
            request.PaymentMethod,
            request.PaymentDate,
            request.Reference,
            request.Notes);

        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Allocate payment to charges
        await _paymentAllocationService.AllocatePaymentAsync(
            payment.Id,
            request.OwnerId,
            request.Amount,
            cancellationToken);

        payment.MarkAsCompleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
