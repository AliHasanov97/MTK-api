using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.CustomerCharges.Commands.CancelCustomerCharge;

internal sealed class CancelCustomerChargeCommandHandler : ICommandHandler<CancelCustomerChargeCommand>
{
    private readonly ICustomerChargeRepository _customerChargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelCustomerChargeCommandHandler(
        ICustomerChargeRepository customerChargeRepository,
        IUnitOfWork unitOfWork)
    {
        _customerChargeRepository = customerChargeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelCustomerChargeCommand request, CancellationToken cancellationToken)
    {
        var charge = await _customerChargeRepository.GetByIdAsync(request.ChargeId, cancellationToken);

        if (charge is null || charge.CustomerId is null)
        {
            return Result.Failure(new Error(
                "CustomerCharge.NotFound",
                $"Müştəri borcu tapılmadı: {request.ChargeId}"));
        }

        try
        {
            charge.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("CustomerCharge.CancelNotAllowed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
