using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;

internal sealed class CreateChargeCommandHandler : ICommandHandler<CreateChargeCommand, Guid>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IOwnerBalanceRepository _ownerBalanceRepository;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly IPaymentAllocationService _paymentAllocationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateChargeCommandHandler(
        IChargeRepository chargeRepository,
        IOwnerBalanceRepository ownerBalanceRepository,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        IPaymentAllocationService paymentAllocationService,
        IUnitOfWork unitOfWork)
    {
        _chargeRepository = chargeRepository;
        _ownerBalanceRepository = ownerBalanceRepository;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _paymentAllocationService = paymentAllocationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateChargeCommand request, CancellationToken cancellationToken)
    {
        var ownership = await _propertyOwnershipRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        if (ownership is null || ownership.OwnerId != request.OwnerId)
        {
            return Result.Failure<Guid>(new Error(
                "Charge.PropertyOwnerMismatch",
                "Seçilmiş əmlak bu sahibə aid deyil"));
        }

        // Manual charges get their own period namespace so they never collide with the
        // "yyyy-MM" periods the monthly auto-generation uses for the same property.
        string period = request.Period ?? $"MANUAL-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..40];

        bool alreadyExists = await _chargeRepository.ChargeExistsForPeriodAsync(
            request.OwnerId, request.PropertyId, period, cancellationToken);

        if (alreadyExists)
        {
            return Result.Failure<Guid>(new Error(
                "Charge.PeriodAlreadyExists",
                $"Bu dövr üçün artıq haqq mövcuddur: {period}"));
        }

        var charge = Charge.Create(
            request.OwnerId,
            request.PropertyType,
            request.PropertyId,
            period,
            request.Amount,
            request.Amount,
            RateType.Manual,
            areaSquareMeters: null,
            description: request.Description);

        _chargeRepository.Add(charge);

        var ownerBalance = await _ownerBalanceRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

        if (ownerBalance is null)
        {
            ownerBalance = OwnerBalance.Create(request.OwnerId);
            _ownerBalanceRepository.Add(ownerBalance);
        }

        ownerBalance.AddCharge(request.Amount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // If this owner already has advance sitting on older payments, spend it on
        // this brand-new charge immediately instead of leaving it artificially unpaid.
        await _paymentAllocationService.SettleChargeFromAdvanceAsync(charge.Id, request.OwnerId, cancellationToken);

        return Result.Success(charge.Id);
    }
}
