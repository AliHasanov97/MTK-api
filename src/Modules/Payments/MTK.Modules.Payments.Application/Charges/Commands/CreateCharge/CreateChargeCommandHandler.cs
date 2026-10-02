using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.OwnerBalances.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;

internal sealed class CreateChargeCommandHandler : ICommandHandler<CreateChargeCommand, Guid>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly IPaymentAllocationService _paymentAllocationService;
    private readonly IOwnerBalanceService _ownerBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateChargeCommandHandler(
        IChargeRepository chargeRepository,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        IPaymentAllocationService paymentAllocationService,
        IOwnerBalanceService ownerBalanceService,
        IUnitOfWork unitOfWork)
    {
        _chargeRepository = chargeRepository;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _paymentAllocationService = paymentAllocationService;
        _ownerBalanceService = ownerBalanceService;
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
            request.PropertyId, period, cancellationToken);

        if (alreadyExists)
        {
            return Result.Failure<Guid>(new Error(
                "Charge.PeriodAlreadyExists",
                $"Bu dövr üçün artıq haqq mövcuddur: {period}"));
        }

        // Borcun yaranması, avansla bağlanması və balansın yenilənməsi bir
        // tranzaksiyadır — yarımçıq vəziyyət qala bilməz.
        await using var transaction = await _chargeRepository.BeginTransactionAsync(cancellationToken);

        var charge = Charge.Create(
            request.OwnerId,
            request.PropertyType,
            request.PropertyId,
            period,
            // Dövr verilməyibsə borc indi yaranır; verilibsə də manual borc öz
            // yaranma anı ilə yaşlanır (Period sıralama üçün istifadə olunmur).
            DateTimeOffset.UtcNow,
            request.Amount,
            request.Amount,
            RateType.Manual,
            areaSquareMeters: null,
            description: request.Description);

        _chargeRepository.Add(charge);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Sahibin əvvəlki ödənişlərindən qalan avans varsa, borc dərhal ondan bağlanır.
        // Nəticə udulmur: uğursuzluq çağırana (və API-yə) qaytarılır.
        var advanceResult = await _paymentAllocationService.ApplyAdvanceToChargesAsync([charge], cancellationToken);
        if (advanceResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(advanceResult.Error);
        }

        await _ownerBalanceService.RecalculateAsync([request.OwnerId], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Result.Success(charge.Id);
    }
}
