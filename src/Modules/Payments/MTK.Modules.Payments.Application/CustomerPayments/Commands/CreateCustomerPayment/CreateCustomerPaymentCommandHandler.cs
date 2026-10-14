using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.CustomerPayments.Commands.CreateCustomerPayment;

internal sealed class CreateCustomerPaymentCommandHandler : ICommandHandler<CreateCustomerPaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentAllocationService _paymentAllocationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        ICustomerRepository customerRepository,
        IPaymentAllocationService paymentAllocationService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _paymentAllocationService = paymentAllocationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCustomerPaymentCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdDefaultAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<Guid>(new Error(
                "Customer.NotFound",
                $"Müştəri tapılmadı: {request.CustomerId}"));
        }

        // Ödəniş, onun paylanması və ledger qeydi bir tranzaksiyadır.
        await using var transaction = await _paymentRepository.BeginTransactionAsync(cancellationToken);

        var payment = Domain.Payments.Payment.CreateForCustomer(
            request.CustomerId,
            request.Amount,
            request.PaymentMethod,
            DateTimeOffset.UtcNow,
            request.Notes);

        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var allocationResult = await _paymentAllocationService.AllocatePaymentAsync(
            payment.Id,
            PartyType.Customer,
            request.CustomerId,
            request.Amount,
            propertyId: null,
            cancellationToken);

        if (allocationResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(allocationResult.Error);
        }

        // Ledger qeydi event-in handler-ində yazılır.
        payment.MarkAsCompleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
