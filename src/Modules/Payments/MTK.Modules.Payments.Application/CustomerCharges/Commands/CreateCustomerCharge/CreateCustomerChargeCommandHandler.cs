using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.CustomerCharges.Commands.CreateCustomerCharge;

internal sealed class CreateCustomerChargeCommandHandler : ICommandHandler<CreateCustomerChargeCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerChargeRepository _customerChargeRepository;
    private readonly IPaymentAllocationService _paymentAllocationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerChargeCommandHandler(
        ICustomerRepository customerRepository,
        ICustomerChargeRepository customerChargeRepository,
        IPaymentAllocationService paymentAllocationService,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _customerChargeRepository = customerChargeRepository;
        _paymentAllocationService = paymentAllocationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCustomerChargeCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<Guid>(new Error(
                "Customer.NotFound",
                $"Müştəri tapılmadı: {request.CustomerId}"));
        }

        if (!customer.IsActive)
        {
            return Result.Failure<Guid>(new Error(
                "Customer.Inactive",
                $"Dayandırılmış müştəriyə borc yazıla bilməz: {customer.Name}"));
        }

        // Borcun yazılması və avansla bağlanması bir tranzaksiyadır.
        await using var transaction = await _customerChargeRepository.BeginTransactionAsync(cancellationToken);

        var charge = Charge.ForCustomer(
            request.CustomerId,
            request.Description,
            request.Amount,
            DateTimeOffset.UtcNow,
            request.DueDate);

        _customerChargeRepository.Add(charge);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var advanceResult = await _paymentAllocationService.ApplyAdvanceToChargesAsync([charge], cancellationToken);
        if (advanceResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(advanceResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Success(charge.Id);
    }
}
